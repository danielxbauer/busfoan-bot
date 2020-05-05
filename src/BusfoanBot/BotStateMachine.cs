using System;
using BusfoanBot.Models;
using Discord.WebSocket;
using Statecharts.NET.Language;
using Statecharts.NET.Model;
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

        public static NamedDataEventFactory<SocketMessage> StartGame
            => Define.EventWithData<SocketMessage>(nameof(StartGame));

        public static NamedDataEventFactory<SocketMessage> NextQuestion
            => Define.EventWithData<SocketMessage>(nameof(NextQuestion));

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

        static void StartQuestions(BotContext context, SocketMessage message)
        {
            context.SendMessage("LOS GEHTSS!");
        }

        static void LogNotEnoughPlayers(BotContext context, SocketMessage message)
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
                                .WithActions<BotContext>(
                                    Log("AddPlayer"), 
                                    Run<BotContext, SocketMessage>(AddPlayer)),
                            On(LeavePlayer)
                                .TransitionTo.Self
                                .WithActions<BotContext>(Run<BotContext, SocketMessage>(RemovePlayer)),
                            On(StartGame).If<BotContext>(c => c.AreEnoughPlayers)
                                .TransitionTo.Sibling("asking")
                                .WithActions(Run<BotContext, SocketMessage>(StartQuestions)),
                            On(StartGame)
                                .TransitionTo.Self
                                .WithActions<BotContext>(Run<BotContext, SocketMessage>(LogNotEnoughPlayers))),
                        "asking"
                            .WithTransitions(
                                On(NextQuestion).If<BotContext>(c => c.AreQuestionsLeft)
                                    .TransitionTo.Child("question"),
                                On(NextQuestion)
                                    .TransitionTo.Sibling("final"))
                            .AsCompound()
                            .WithInitialState("question")
                            .WithStates(
                                "question"
                                    ////.WithTransitions(
                                    ////    Immediately.If<BotContext>(IsQuestionLeft)
                                    ////        .TransitionTo.Child("player")
                                    ////        .WithActions(Run<BotContext, SocketMessage>(SelectNextQuestion)),
                                    ////    Immediately
                                    ////        .TransitionTo.Sibling("NO_MORE_QUESTION"))
                                    .AsCompound()
                                    .WithInitialState("player")
                                    .WithStates(
                                        "player")),
                        //Immediately.If(/* question left */)
                        //    .TransitionTo.Sibling("xyz")
                        //    .WithActions(/* select next question */),
                        // Immediately.TransitionTo.Sibling("EXIT")
                        "final".AsFinal()));

        private static void SelectNextQuestion(BotContext context, SocketMessage message)
        {
            Question activeQuestion = context.Questions.Peek();
            message.Channel.SendMessageAsync(activeQuestion.Text);
        }
    }
}
