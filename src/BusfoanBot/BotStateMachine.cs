using System;
using BusfoanBot.Extensions;
using BusfoanBot.Models;
using Discord.WebSocket;
using Statecharts.NET.Language;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;
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

        public static NamedEvent NextQuestion
            => Define.Event(nameof(NextQuestion));

        public static NamedDataEventFactory<SocketMessage> NextPlayer
            => Define.EventWithData<SocketMessage>(nameof(NextPlayer));
    }

    public static class BotStateMachine
    {
        static void WelcomeMessage(BotContext context)
        {
            context.SendMessage("Seas leil. Wer wü mit busfoan? (!einsteigen)");
        }

        static void AddPlayer(BotContext context, SocketMessage message)
        {
            var player = new Player(message.Author.Id, message.Author.Username);
            bool alreadyAdded = context.Join(player);

            string reply = !alreadyAdded
                ? $"{player.Name} is eingestiegen. San scho {context.AllPlayers.Count} leit im bus!"
                : $"{player.Name} is schon im bus. San scho {context.AllPlayers.Count} leit im bus!";

            context.SendMessage(reply);
        }

        static void RemovePlayer(BotContext context, SocketMessage message)
        {
            bool removedSuccess = context.Kick(message.Author.Id);

            string reply = !removedSuccess
                ? $"{message.Author.Username} is ausgestiegen. San nu {context.AllPlayers.Count} leit im bus!"
                : $"{message.Author.Username} woit aussteigen, aber irgendwos is schiefgaunga. Probiers numoi pls";

            context.SendMessage(reply);
        }

        static void LogGameStartMessage(BotContext context)
        {
            context.SendMessage("LOS GEHTSS!");
        }

        static void LogNotEnoughPlayers(BotContext context)
        {
            context.SendMessage("Nu ned gnuag leit");
        }

        public static BotContext GetInitialContext(ISocketMessageChannel channel) 
            => new BotContext(channel, new[]
            {
                new Question("Rot oder schwarz"),
                new Question("Drunter, Drüber oder Grenze?")
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
                                .WithActions<BotContext>(Run<BotContext>(WelcomeMessage))),
                        "wait-for-players".WithTransitions(
                            //Ignore("EXIT"),
                            //After(20.Seconds()).TransitionTo // Timeout for not answering..
                            On(JoinPlayer)
                                .TransitionTo.Self
                                .WithAction<BotContext, SocketMessage>(AddPlayer),
                            On(LeavePlayer)
                                .TransitionTo.Self
                                .WithAction<BotContext, SocketMessage>(RemovePlayer),
                            On(StartGame).If<BotContext>(c => c.AreEnoughPlayers)
                                .TransitionTo.Sibling("asking"),
                            On(StartGame)
                                .TransitionTo.Self
                                .WithActions<BotContext>(Run<BotContext>(LogNotEnoughPlayers))),
                        "asking"
                            .WithEntryActions<BotContext>(Run<BotContext>(LogGameStartMessage))
                            .WithTransitions(
                                On(NextQuestion).If<BotContext>(c => c.AreQuestionsLeft)
                                    .TransitionTo.Child("question"),
                                On(NextQuestion)
                                    .TransitionTo.Sibling("final"))
                            .AsCompound()
                            .WithInitialState("question")
                            .WithStates(
                                "question".WithTransitions(
                                    Immediately.If<BotContext>(c => c.AreQuestionsLeft)
                                        .TransitionTo.Sibling("player")
                                        .WithAction(c => c.SelectNextQuestion()),
                                    Immediately.TransitionTo.Absolute("busfoan", "final")
                                        /*.TransitionTo.Self.WithActions(Send(NextQuestion))*/),
                                "player".WithTransitions(
                                    Immediately.If<BotContext>(c => c.ArePlayersLeft)
                                        .TransitionTo.Sibling("waiting")
                                        .WithActions(
                                            Run<BotContext>(c => c.SelectNextPlayer()),
                                            Run<BotContext>(AskQuestion)),
                                    Immediately
                                        .TransitionTo.Sibling("question")
                                        .WithActions(Log("NO MORE PLAYER"))),
                                "waiting".WithTransitions(
                                    On("CHECK").TransitionTo.Sibling("player"))),
                        "final".AsFinal()));

        private static void AskQuestion(BotContext context)
        {
            context.Channel.SendMessageAsync($"{context.ActivePlayer.Name}: {context.ActiveQuestion.Text}");
        }
    }
}
