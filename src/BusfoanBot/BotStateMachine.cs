using System;
using BusfoanBot.Models;
using Discord.WebSocket;
using Statecharts.NET.Language;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities.Time;
using static Statecharts.NET.Language.Keywords;

namespace BusfoanBot
{
    public static class BotStateMachine
    {
        static void WelcomeMessage()
        {
            Console.WriteLine("Welcome Message");
        }

        static void AddPlayer(BotContext context, SocketMessage message)
        {
            var player = new Player(message.Author.Username);
            context.Players.Add(player);

            message.Channel.SendMessageAsync($"{player.Name} is eingestiegen. San scho {context.Players.Count} leit im bus!");
        }

        static void RemovePlayer(BotContext context, SocketMessage message)
        {
            // TODO
        }

        static void StartQuestions(BotContext context, SocketMessage message)
        {
            message.Channel.SendMessageAsync("LOS GEHTSS!");
        }

        static void LogNotEnoughPlayers(BotContext context, SocketMessage message)
        {
            message.Channel.SendMessageAsync("Nu ned gnuag leit");
        }

        static bool AreEnoughPlayers(BotContext context)
            => context.Players.Count >= 2;

        public static NamedDataEventFactory<SocketMessage> StartGame
            => Define.EventWithData<SocketMessage>("START-GAME");

        public static NamedDataEventFactory<SocketMessage> JoinPlayer 
            => Define.EventWithData<SocketMessage>("JOIN");

        public static NamedDataEventFactory<SocketMessage> LeavePlayer
            => Define.EventWithData<SocketMessage>("LEAVE");

        public static readonly StatechartDefinition<BotContext> Behaviour = Define.Statechart
            .WithInitialContext(new BotContext())
            .WithRootState(
                "busfoan"
                    .WithEntryActions(Run(() => Console.WriteLine("NOW THIS WORKS AS WELL 🎉")))
                    .AsCompound()
                    .WithInitialState("idle")
                    .WithStates(
                        "idle".WithTransitions(
                            On("START")
                                .TransitionTo.Sibling("wait-for-players")
                                .WithActions(Run(WelcomeMessage))),
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
                            On(StartGame).If<BotContext>(AreEnoughPlayers)
                                .TransitionTo.Sibling("question")
                                .WithActions(Run<BotContext, SocketMessage>(StartQuestions)),
                            On(StartGame)
                                .TransitionTo.Self
                                .WithActions<BotContext>(Run<BotContext, SocketMessage>(LogNotEnoughPlayers))),
                        "question"
                            .WithEntryActions(Run(() => Console.WriteLine("start questions"))),
                        //Immediately.If(/* question left */)
                        //    .TransitionTo.Sibling("xyz")
                        //    .WithActions(/* select next question */),
                        // Immediately.TransitionTo.Sibling("EXIT")
                        "final".AsFinal()));
    }
}
