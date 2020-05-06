using System.Collections.Generic;
using Discord;
using Discord.WebSocket;
using Statecharts.NET;
using Statecharts.NET.Model;
using Statecharts.NET.XState;
using static BusfoanBot.BotStateMachine;
using static BusfoanBot.BotStateMachineEvents;
using Task = System.Threading.Tasks.Task;

namespace BusfoanBot
{
	class Program
    {
		private static readonly string token = "NzA2NDk3MjMwMDA1MjA3MDQx.Xq7pFw.mGgt38aCNVHmcZ4NxR4xXwqDMgY";
		
		private static DiscordSocketClient client;
		private static IDictionary<ulong, RunningStatechart<BotContext>> statecharts
			= new Dictionary<ulong, RunningStatechart<BotContext>>();

		public static async Task Main()
        {
			client = new DiscordSocketClient();
			client.Log += Log;
			client.MessageReceived += MessageReceived;

			await client.LoginAsync(TokenType.Bot, token);
			await client.StartAsync();

			// Block this task until the program is closed.
			await Task.Delay(-1);
		}

		private static Task Log(LogMessage message)
		{
			System.Console.WriteLine(message.ToString());
			return Task.CompletedTask;
		}

		private static Task MessageReceived(SocketMessage message)
		{
			if (message.Author.IsBot) return Task.CompletedTask;

			if (message.Content.StartsWith("!busfoan"))
				GetStatechartIn(message.Channel).Send(WakeUp);

			if (message.Content.StartsWith("!einsteigen"))
				GetStatechartIn(message.Channel).Send(JoinPlayer(message));

			if (message.Content.StartsWith("!aussteigen"))
				GetStatechartIn(message.Channel).Send(LeavePlayer(message));

			if (message.Content.StartsWith("!abfoat"))
				GetStatechartIn(message.Channel).Send(StartGame);

			if (message.Content.StartsWith("!1"))
				GetStatechartIn(message.Channel).Send(CheckCard(message));

			return Task.CompletedTask;
		}

		private static RunningStatechart<BotContext> GetStatechartIn(ISocketMessageChannel channel)
		{
			if (statecharts.ContainsKey(channel.Id)) return statecharts[channel.Id];

			BotContext initialContext =	GetInitialContext(channel);
			StatechartDefinition<BotContext> behaviour = Behaviour(initialContext);

			var parsedStatechart = Parser.Parse(behaviour) as ExecutableStatechart<BotContext>;
			var statechart = Interpreter.Interpret(parsedStatechart);
			statechart.OnMacroStep += step =>
			{
				Log(new LogMessage(LogSeverity.Info, "StateChart", string.Join(", ", statechart.NextEvents)));
			};

			string viz = behaviour.AsXStateVisualizerV4Definition();

			statecharts.Add(channel.Id, statechart);
			statechart.RunAsync();
			return statechart;
		}
	}
}
