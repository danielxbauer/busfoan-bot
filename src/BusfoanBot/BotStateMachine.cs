using System;
using System.Linq;
using System.Threading.Tasks;
using BusfoanBot.Extensions;
using BusfoanBot.Models;
using Discord.WebSocket;
using Statecharts.NET.Language;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities.Time;
using static BusfoanBot.BotActions;
using static BusfoanBot.BotEvents;
using static Statecharts.NET.Language.Keywords;

namespace BusfoanBot
{
    public static class BotStateMachine
    {
        public static BotContext GetInitialContext(ISocketMessageChannel channel)
            => new BotContext(channel, new[]
            {
                Question.Create($"{Emotes.ThumbsUp} Rot oder {Emotes.Grin} Schwarz?",
                    new Answer(Emotes.ThumbsUp, (_, card) => card.IsRed),
                    new Answer(Emotes.Grin, (_, card) => card.IsBlack)),
                Question.Create($"{Emotes.ThumbsUp} Drunter, {Emotes.Check} Drüber oder {Emotes.Grin} Grenze?",
                    new Answer(Emotes.ThumbsUp, (lastCards, card) => card.Value < lastCards.ElementAt(0).Value),
                    new Answer(Emotes.Check, (lastCards, card) => card.Value > lastCards.ElementAt(0).Value),
                    new Answer(Emotes.Grin, (lastCards, card) => card.Value == lastCards.ElementAt(0).Value)),
                Question.Create($"{Emotes.ThumbsUp} Außen, {Emotes.Check} Innen oder {Emotes.Grin} Grenze?",
                    new Answer(Emotes.ThumbsUp, (lastCards, card) => card.Value > lastCards.OrderBy(c => c.Value).ElementAt(0).Value
                                                                  && card.Value < lastCards.OrderBy(c => c.Value).ElementAt(1).Value),
                    new Answer(Emotes.Check, (lastCards, card) => card.Value > lastCards.OrderBy(c => c.Value).ElementAt(0).Value
                                                               || card.Value > lastCards.OrderBy(c => c.Value).ElementAt(1).Value),
                    new Answer(Emotes.Grin, (lastCards, card) => card.Value == lastCards.OrderBy(c => c.Value).ElementAt(0).Value
                                                               ||card.Value == lastCards.OrderBy(c => c.Value).ElementAt(1).Value)),
                Question.Create($"{Emotes.Club} Club, {Emotes.Spade} Spade, {Emotes.Diamond} Diamond oder {Emotes.Heart} Herz?",
                    new Answer(Emotes.Club, (_, card) => card.Symbol == CardSymbol.Club),
                    new Answer(Emotes.Spade, (_, card) => card.Symbol == CardSymbol.Spade),
                    new Answer(Emotes.Diamond, (_, card) => card.Symbol == CardSymbol.Diamond),
                    new Answer(Emotes.Heart, (_, card) => card.Symbol == CardSymbol.Heart))
            });

        public static StatechartDefinition<BotContext> Behaviour(BotContext botContext) => Define.Statechart
            .WithInitialContext(botContext)
            .WithRootState(
                "busfoan"
                    .WithEntryActions(Log("Bot started"))
                    .AsCompound()
                    .WithInitialState("idle")
                    .WithStates(
                        "idle".WithTransitions(
                            On(WakeUp)
                                .TransitionTo.Sibling("wait-for-players")
                                .WithActions<BotContext>(Async(WelcomeMessage))),
                        "wait-for-players"
                            .WithTransitions(
                                On(ReactionAdded).IfReactedWith(Emotes.Check)
                                    .TransitionTo.Self
                                    .WithActions(AsyncReaction(AddPlayer)),
                                On(ReactionRemoved).IfReactedWith(Emotes.Check)
                                    .TransitionTo.Self
                                    .WithActions(AsyncReaction(RemovePlayer)),
                                On(ReactionAdded).IfReactedWith(Emotes.Bus, (context, _) => context.AreEnoughPlayers)
                                    .TransitionTo.Sibling("cleanup-wait-for-players"),
                                On(ReactionAdded).IfReactedWith(Emotes.Bus)
                                    .TransitionTo.Self
                                    .WithActions(AsyncReaction(LogNotEnoughPlayers))),
                        "cleanup-wait-for-players"
                            .WithEntryActions<BotContext>(
                                Log("CLEANUP"),
                                Assign<BotContext>(c => c.LastReactableMessage = null))
                            .WithTransitions(Immediately.TransitionTo.Sibling("asking")),
                        "asking"
                            .WithEntryActions<BotContext>(
                                Async(LogGameStartMessage),
                                Run<BotContext>(ShuffleCards))
                            .AsCompound()
                            .WithInitialState("question")
                            .WithStates(
                                "question".WithTransitions(
                                    Immediately.If<BotContext>(c => c.AreQuestionsLeft)
                                        .TransitionTo.Sibling("player")
                                        .WithActions(Run<BotContext>(SelectNextQuestion)),
                                    Immediately
                                        .TransitionTo.Absolute("busfoan", "final")),
                                "player".WithTransitions(
                                    Immediately.If<BotContext>(c => c.ArePlayersLeft)
                                        .TransitionTo.Sibling("waiting")
                                        .WithActions(
                                            Run<BotContext>(SelectNextPlayer),
                                            Async(AskQuestion)),
                                    Immediately
                                        .TransitionTo.Sibling("question")
                                        .WithActions(Log("NO MORE PLAYER"))),
                                "waiting".WithTransitions(
                                    On(ReactionAdded).IfReactable()
                                        .TransitionTo.Sibling("checking")
                                        .WithActions(AsyncReaction(RevealCardForUser)),
                                    After(180.Seconds())
                                        .TransitionTo.Sibling("checking")
                                        .WithActions<BotContext>(Async(RevealCard))),
                                "checking"
                                    .WithEntryActions<BotContext>(
                                        Log("CLEANUP"),
                                        Assign<BotContext>(c => c.LastReactableMessage = null))
                                    .WithTransitions(
                                        After(1.Seconds()).TransitionTo.Sibling("player"))),
                        "final"
                            .WithEntryActions<BotContext>(Async(ExitMessage))
                            .AsFinal()));

        private static SideEffectActionDefinition<BotContext, SocketReaction> AsyncReaction(Func<BotContext, SocketReaction, Task> action)
            => Run<BotContext, SocketReaction>((context, message) => Task.WaitAll(action(context, message)));

        private static SideEffectActionDefinition<BotContext, TData> Async<TData>(Func<BotContext, TData, Task> action)
            => Run<BotContext, TData>((context, message) => Task.WaitAll(action(context, message)));

        private static SideEffectActionDefinition<BotContext> Async(Func<BotContext, Task> action)
            => Run<BotContext>(context => Task.WaitAll(action(context)));
    }
}
