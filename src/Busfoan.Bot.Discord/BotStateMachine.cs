using System;
using System.Linq;
using System.Threading.Tasks;
using Busfoan.Bot.Discord.Extensions;
using Busfoan.Bot.Discord.Models;
using Busfoan.Domain;
using Discord.WebSocket;
using Statecharts.NET.Language;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities.Time;
using static Busfoan.Bot.Discord.BotActions;
using static Busfoan.Bot.Discord.BotEvents;
using static Statecharts.NET.Language.Keywords;

namespace Busfoan.Bot.Discord
{
    public static class BotStateMachine
    {
        public static BotContext GetInitialContext(ISocketMessageChannel channel)
            => new BotContext(channel, new[]
            {
                Question.Create($"{Emotes.ThumbsUp} Rot oder {Emotes.Grin} Schwarz?",
                    Emotes.ThumbsUp.AsAnswer(cards => cards.First().IsRed),
                    Emotes.Grin.AsAnswer(cards => cards.First().IsBlack)),
                Question.Create($"{Emotes.ThumbsUp} Drunter, {Emotes.Check} Drüber oder {Emotes.Grin} Grenze?",
                    Emotes.ThumbsUp.AsAnswer(cards => cards.Last().Value < cards.First().Value),
                    Emotes.Check.AsAnswer(cards => cards.Last().Value > cards.First().Value),
                    Emotes.Grin.AsAnswer(cards => cards.Last().Value == cards.First().Value)),
                Question.Create($"{Emotes.ThumbsUp} Außen, {Emotes.Check} Innen oder {Emotes.Grin} Grenze?",
                    Emotes.ThumbsUp.AsAnswer(cards =>
                    {
                        var card = cards.Last();
                        var lastCards = cards.RemoveAt(cards.Count - 1).OrderBy(c => c.Value);
                        return card.Value > lastCards.ElementAt(0).Value
                            && card.Value < lastCards.ElementAt(1).Value;
                    }),
                    Emotes.Check.AsAnswer(cards =>
                    {
                        var card = cards.Last();
                        var lastCards = cards.RemoveAt(cards.Count - 1).OrderBy(c => c.Value);
                        return card.Value > lastCards.ElementAt(0).Value
                            || card.Value > lastCards.ElementAt(1).Value;
                    }),
                    Emotes.Grin.AsAnswer(cards =>
                    {
                        var card = cards.Last();
                        var lastCards = cards.RemoveAt(cards.Count - 1).OrderBy(c => c.Value);
                        return card.Value == lastCards.ElementAt(0).Value
                            || card.Value == lastCards.ElementAt(1).Value;
                    })),
                Question.Create($"{Emotes.Club} Club, {Emotes.Spade} Spade, {Emotes.Diamond} Diamond oder {Emotes.Heart} Herz?",
                    Emotes.Club.AsAnswer(cards => cards.Last().Symbol == CardSymbol.Club),
                    Emotes.Spade.AsAnswer(cards => cards.Last().Symbol == CardSymbol.Spade),
                    Emotes.Diamond.AsAnswer(cards => cards.Last().Symbol == CardSymbol.Diamond),
                    Emotes.Heart.AsAnswer(cards => cards.Last().Symbol == CardSymbol.Heart))
            });

        public static StatechartDefinition<BotContext> Behaviour(BotContext botContext, BotOptions options) => Define.Statechart
            .WithInitialContext(botContext)
            .WithRootState(
                "busfoan"
                    .WithEntryActions(Log("Bot started"))
                    .WithTransitions(
                        On(Cancel)
                            .TransitionTo.Child("canceled"))
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
                                    .WithActions(AsyncReaction(LogNotEnoughPlayers)),
                                On(ReactionAdded).IfReactedWith(Emotes.CrossMark)
                                    .TransitionTo.Self.WithActions(
                                        Send(Cancel),
                                        Async(DeleteLastReactableMessage))),
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
                                        ////.TransitionTo.Absolute("busfoan", "final")),
                                        .TransitionTo.Absolute("busfoan", "pyramid")),
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
                                    After(options.AnswerTimeout)
                                        .TransitionTo.Sibling("checking")
                                        .WithActions<BotContext>(Async(RevealCard))),
                                "checking"
                                    .WithEntryActions<BotContext>(
                                        Log("CLEANUP"),
                                        Assign<BotContext>(c => c.LastReactableMessage = null))
                                    .WithTransitions(
                                        After(1.Seconds()).TransitionTo.Sibling("player"))),
                        "pyramid"
                            .WithEntryActions<BotContext>(
                                Async(LogPyramidStartMessage))
                            .AsCompound()
                            .WithInitialState("show")
                            .WithStates(
                                "show".WithTransitions(
                                    Immediately.If<BotContext>(c => c.Pyramid.CardsLeftToReveal)
                                        .TransitionTo.Sibling("waiting")
                                        .WithActions(Async(ShowPyramid)),
                                    Immediately
                                        .TransitionTo.Absolute("busfoan", "final")),
                                "waiting".WithTransitions(
                                    After(1.Seconds()).TransitionTo.Sibling("checking")),
                                "checking".WithTransitions(
                                    After(1.Seconds())
                                        .TransitionTo.Sibling("show")
                                        .WithActions<BotContext>(Async(RevealPyramidCard)))),
                        "canceled"
                            .WithEntryActions<BotContext>(
                                Async(Canceled),
                                Async(options.OnDone))
                            .AsFinal(),
                        "final"
                            .WithEntryActions<BotContext>(
                                Async(ExitMessage),
                                Async(options.OnDone))
                            .AsFinal()));

        private static SideEffectActionDefinition<BotContext, SocketReaction> AsyncReaction(Func<BotContext, SocketReaction, Task> action)
            => Run<BotContext, SocketReaction>((context, message) => Task.WaitAll(action(context, message)));

        private static SideEffectActionDefinition<BotContext, TData> Async<TData>(Func<BotContext, TData, Task> action)
            => Run<BotContext, TData>((context, message) => Task.WaitAll(action(context, message)));

        private static SideEffectActionDefinition<BotContext> Async(Func<BotContext, Task> action)
            => Run<BotContext>(context => Task.WaitAll(action(context)));
    }
}
