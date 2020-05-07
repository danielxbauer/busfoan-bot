﻿using System;
using System.Linq;
using System.Threading.Tasks;
using BusfoanBot.Extensions;
using BusfoanBot.Models;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Statecharts.NET.Interfaces;
using Statecharts.NET.Language;
using Statecharts.NET.Language.Builders.StateNode;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;
using Statecharts.NET.Utilities.Time;
using static BusfoanBot.BotStateMachineEvents;
using static BusfoanBot.BotStateMachineActions;
using static Statecharts.NET.Language.Keywords;
using System.Runtime.CompilerServices;

namespace BusfoanBot
{
    public static class BotStateMachineEvents
    {
        public static NamedEvent WakeUp
            => Define.Event(nameof(WakeUp));

        public static NamedDataEventFactory<SocketMessage> NextPlayer
            => Define.EventWithData<SocketMessage>(nameof(NextPlayer));

        public static NamedDataEventFactory<SocketMessage> CheckCard
            => Define.EventWithData<SocketMessage>(nameof(CheckCard));

        public static NamedDataEventFactory<SocketReaction> ReactionAdded
            => Define.EventWithData<SocketReaction>(nameof(ReactionAdded));

        public static NamedDataEventFactory<SocketReaction> ReactionRemoved
            => Define.EventWithData<SocketReaction>(nameof(ReactionRemoved));

        // Currently not possible because there is no way to tell if it triggert some reaction
        ////public static Func<IEmote, NamedDataEventFactory<SocketReaction>> ReactionAdded
        ////    => emote => Define.EventWithData<SocketReaction>(emote.Name);
    }

    public static class BotStateMachineActions
    {
        ////public static Task WelcomeMessage(BotContext context) => WelcomeMessage(context, null);

        private static Embed GetWelcomeMessage(BotContext context)
        {
            var playerNames = context.AllPlayers.Select(p => $"\t*{p.Name}");

            var message = "Seas leidl.\n" +
                           "I bins da bus!\n" +
                           "\n" +
                          $"{Emotes.Check} zum einsteigen\n" +
                          $"{Emotes.Bus} zum starten\n" +
                          $"\n" +
                          $"{playerNames.Count()} san dabei:\n" +
                          $"{string.Join('\n', playerNames)}";

            return new EmbedBuilder()
                .WithDescription(message)
                .WithColor(Color.Orange)
                .Build(); ;
        }

        public static async Task WelcomeMessage(BotContext context)
        {
            var message = GetWelcomeMessage(context);
            await context.SendReactableMessage(message, Emotes.Check, Emotes.Bus);
        }

        public static async Task UpdateWelcomeMessage(BotContext context)
        {
            var message = GetWelcomeMessage(context);
            await context.UpdateMessage(context.LastReactableMessage, message);
        }

        public static async Task AddPlayer(BotContext context, IUser author)
        {
            context.Join(new Player(author.Id, author.Username));
            await UpdateWelcomeMessage(context);
        }

        public static async Task RemovePlayer(BotContext context, IUser author)
        {
            context.Kick(author.Id);
            await UpdateWelcomeMessage(context);
        }
    }

    public static class BotStateMachine
    {
        // TODO: put actions somewhere else?!
        static async Task LogGameStartMessage(BotContext context)
        {
            await context.SendMessage("LOS GEHTSS!");
        }

        static async Task LogNotEnoughPlayers(BotContext context, SocketReaction reaction)
        {
            await context.SendMessage("Nu ned gnuag leit");
        }

        public static BotContext GetInitialContext(ISocketMessageChannel channel)
            => new BotContext(channel, new[]
            {
                Question.Create($"Rot {Emotes.ThumbsUp} oder schwarz {Emotes.Grin} ?",
                    new Answer(Emotes.ThumbsUp, card => card.IsRed), // TODO: also lastcard into func?!
                    new Answer(Emotes.Grin, card => card.IsBlack)),
                Question.Create("Drunter, Drüber oder Grenze?",
                    new Answer(Emotes.ThumbsUp, card => card.IsRed)) // TODO
            });

        public static StatechartDefinition<BotContext> Behaviour(BotContext botContext) => Define.Statechart
            .WithInitialContext(botContext)
            .WithRootState(
                "busfoan"
                    .WithEntryActions(Run(() => Console.WriteLine("NOW THIS WORKS AS WELL 🎉")))
                    .AsCompound()
                    .WithInitialState("idle")
                    .WithStates(
                        "idle".WithTransitions(
                            On(WakeUp)
                                .TransitionTo.Sibling("wait-for-players")
                                .WithActions<BotContext>(RunIn(WelcomeMessage))),
                        "wait-for-players"
                            .WithTransitions(
                                On(ReactionAdded).IfReactedWith(Emotes.Check)
                                    .TransitionTo.Self
                                    .WithActions(RunIn<SocketReaction>((c, r) => AddPlayer(c, r.User.Value))),
                                On(ReactionRemoved).IfReactedWith(Emotes.Check)
                                    .TransitionTo.Self
                                    .WithActions(RunIn<SocketReaction>((c, r) => RemovePlayer(c, r.User.Value))),
                                On(ReactionAdded).IfReactedWith(Emotes.Bus, (c, r) => c.AreEnoughPlayers)
                                    .TransitionTo.Sibling("cleanup-wait-for-players"),
                                On(ReactionAdded).IfReactedWith(Emotes.Bus)
                                    .TransitionTo.Self
                                    .WithActions(RunIn<SocketReaction>(LogNotEnoughPlayers))),
                        "cleanup-wait-for-players"
                            .WithEntryActions<BotContext>(
                                Log("CLEANUP"),
                                Assign<BotContext>(c => c.LastReactableMessage = null))
                            .WithTransitions(Immediately.TransitionTo.Sibling("asking")),
                        "asking"
                            .WithEntryActions<BotContext>(
                                RunIn(LogGameStartMessage),
                                Run<BotContext>(context => context.ShuffleCards()))
                            .AsCompound()
                            .WithInitialState("question")
                            .WithStates(
                                "question".WithTransitions(
                                    Immediately.If<BotContext>(c => c.AreQuestionsLeft)
                                        .TransitionTo.Sibling("player")
                                        .WithAction(c => c.SelectNextQuestion()),
                                    Immediately
                                        .TransitionTo.Absolute("busfoan", "final")),
                                "player".WithTransitions(
                                    Immediately.If<BotContext>(c => c.ArePlayersLeft)
                                        .TransitionTo.Sibling("waiting")
                                        .WithActions(
                                            Run<BotContext>(c => c.SelectNextPlayer()),
                                            Run<BotContext>(c => Task.WaitAll(AskQuestion(c)))),
                                    Immediately
                                        .TransitionTo.Sibling("question")
                                        .WithActions(Log("NO MORE PLAYER"))),
                                "waiting".WithTransitions(
                                    On(CheckCard)
                                        .TransitionTo.Sibling("checking")
                                        .WithActions<BotContext>(RunIn<SocketMessage>((context, message) => context.RevealCardFor(message.Author.Id))),
                                    After(30.Seconds())
                                        .TransitionTo.Sibling("checking")
                                        .WithActions<BotContext>(RunIn(context => context.RevealCard()))),
                                "checking".WithTransitions(
                                    After(1.Seconds())
                                        .TransitionTo.Sibling("player"))),
                        "final"
                            .WithEntryActions<BotContext>(RunIn(c => c.SendMessage("So des woas. Bis boid!")))
                            .AsFinal()));

        private static SideEffectActionDefinition<BotContext, TData> RunIn<TData>(Func<BotContext, TData, Task> action)
            => Run<BotContext, TData>((context, message) => Task.WaitAll(action(context, message)));

        private static SideEffectActionDefinition<BotContext> RunIn(Func<BotContext, Task> action)
            => Run<BotContext>(context => Task.WaitAll(action(context)));

        private static async Task AskQuestion(BotContext context)
        {
            var message = new EmbedBuilder()
                .WithDescription($"{context.ActivePlayer.Name}: {context.ActiveQuestion.Text}")
                .WithColor(Color.Orange)
                .Build();

            await context.SendReactableMessage(message, context.ActiveQuestion.Answers.Select(a => a.Emote).AsEnumerable());
        }
    }
}
