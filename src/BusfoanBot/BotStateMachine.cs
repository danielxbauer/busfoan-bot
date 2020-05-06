using System;
using System.Linq;
using System.Threading.Tasks;
using BusfoanBot.Extensions;
using BusfoanBot.Models;
using Discord;
using Discord.WebSocket;
using Statecharts.NET.Language;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;
using Statecharts.NET.Utilities.Time;
using static BusfoanBot.BotStateMachineEvents;
using static Statecharts.NET.Language.Keywords;

namespace BusfoanBot
{
    public static class BotStateMachineEvents
    {
        public static NamedEvent WakeUp
            => Define.Event(nameof(WakeUp));

        public static NamedDataEventFactory<SocketMessage> JoinPlayer
            => Define.EventWithData<SocketMessage>(nameof(JoinPlayer));

        public static NamedDataEventFactory<SocketMessage> LeavePlayer
            => Define.EventWithData<SocketMessage>(nameof(LeavePlayer));

        public static NamedEvent StartGame
            => Define.Event(nameof(StartGame));

        public static NamedDataEventFactory<SocketMessage> NextPlayer
            => Define.EventWithData<SocketMessage>(nameof(NextPlayer));

        public static NamedDataEventFactory<SocketMessage> CheckCard
            => Define.EventWithData<SocketMessage>(nameof(CheckCard));

        public static NamedDataEventFactory<SocketReaction> ThumbUp
            => Define.EventWithData<SocketReaction>(nameof(ThumbUp));
    }

    public static class BotStateMachine
    {
        static async Task WelcomeMessage(BotContext context)
        {
            await context.SendReactableMessage("Seas leil. Wer wü mit busfoan? (!einsteigen)",
                Emotes.ThumbsUp);
        }

        static async Task AddPlayer(BotContext context, IUser author)
        {
            var player = new Player(author.Id, author.Username);
            bool alreadyAdded = context.Join(player);

            string reply = !alreadyAdded
                ? $"{player.Name} is eingestiegen. San scho {context.AllPlayers.Count} leit im bus!"
                : $"{player.Name} is schon im bus. San scho {context.AllPlayers.Count} leit im bus!";

            await context.SendMessage(reply);
        }

        static async Task RemovePlayer(BotContext context, SocketMessage message)
        {
            bool removedSuccess = context.Kick(message.Author.Id);

            string reply = !removedSuccess
                ? $"{message.Author.Username} is ausgestiegen. San nu {context.AllPlayers.Count} leit im bus!"
                : $"{message.Author.Username} woit aussteigen, aber irgendwos is schiefgaunga. Probiers numoi pls";

            await context.SendMessage(reply);
        }

        // TODO: put actions somewhere else?!
        static async Task LogGameStartMessage(BotContext context)
        {
            await context.SendMessage("LOS GEHTSS!");
        }

        static async Task LogNotEnoughPlayers(BotContext context)
        {
            await context.SendMessage("Nu ned gnuag leit");
        }

        public static BotContext GetInitialContext(ISocketMessageChannel channel) 
            => new BotContext(channel, new[]
            {
                Question.Create($"Rot {Emotes.ThumbsUp} oder schwarz {Emotes.Grin} ?", Emotes.ThumbsUp, Emotes.Grin),
                Question.Create("Drunter, Drüber oder Grenze?", Emotes.ThumbsUp)
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
                                .WithActions<BotContext>(Run<BotContext>(c => Task.WaitAll(WelcomeMessage(c))))),
                        "wait-for-players".WithTransitions(
                            //Ignore("EXIT"),
                            //After(20.Seconds()).TransitionTo // Timeout for not answering..
                            On(JoinPlayer)
                                .TransitionTo.Self
                                .WithActions<BotContext>(RunIn<SocketMessage>((c, m) => AddPlayer(c, m.Author))),
                            // TODO: check if removed!!
                            On(ThumbUp).If<BotContext>((context, reaction) => context.LastBotMessage != null 
                                && context.LastBotMessage.Id == reaction.MessageId 
                                && !reaction.User.Value.IsBot)
                                .TransitionTo.Self
                                .WithActions(RunIn<SocketReaction>((c, r) => AddPlayer(c, r.User.Value))),
                            On(LeavePlayer)
                                .TransitionTo.Self
                                .WithActions<BotContext>(RunIn<SocketMessage>(RemovePlayer)),
                            On(StartGame).If<BotContext>(c => c.AreEnoughPlayers)
                                .TransitionTo.Sibling("asking"),
                            On(StartGame)
                                .TransitionTo.Self
                                .WithActions<BotContext>(RunIn(LogNotEnoughPlayers))),
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

        private static SideEffectAction<BotContext, TData> RunIn<TData>(Func<BotContext, TData, Task> action)
            => Run<BotContext, TData>((context, message) => Task.WaitAll(action(context, message)));
                
        private static SideEffectAction<BotContext> RunIn(Func<BotContext, Task> action)
            => Run<BotContext>(context => Task.WaitAll(action(context)));

        private static async Task AskQuestion(BotContext context)
        {
            await context.SendReactableMessage($"{context.ActivePlayer.Name}: {context.ActiveQuestion.Text}",
                context.ActiveQuestion.Emotes.AsEnumerable());
        }
    }
}
