using System.Linq;
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
		private static DiscordSocketClient client;
		private static readonly string token = "NzA2NDk3MjMwMDA1MjA3MDQx.Xq7pFw.mGgt38aCNVHmcZ4NxR4xXwqDMgY";

		private static RunningStatechart<BotContext> statechart;

		public static async Task Main(string[] args)
        {
			var parsedStatechart = Parser.Parse(Behaviour) as ExecutableStatechart<BotContext>;
			statechart = Interpreter.Interpret(parsedStatechart);
			statechart.OnMacroStep += step =>
			{
				Log(new LogMessage(LogSeverity.Info, "StateChart", string.Join(", ", statechart.NextEvents)));
			};

			string viz = Behaviour.AsXStateVisualizerV4Definition();
			
			client = new DiscordSocketClient();
			client.Log += Log;
			client.MessageReceived += MessageReceived;

			await client.LoginAsync(TokenType.Bot, token);
			await client.StartAsync();

			await statechart.Start();
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
				statechart.Send(new NamedEvent("START"));

			if (message.Content.StartsWith("!einsteigen"))				
				statechart.Send(JoinPlayer(message));

			if (message.Content.StartsWith("!aussteigen"))
				statechart.Send(LeavePlayer(message));

			if (message.Content.StartsWith("!abfoat"))
			{
				statechart.Send(StartGame(message));
				statechart.Send(NextQuestion(message));
			}

			// statechart.NextEvents ...
			// statechart.OnMarcoStep => Show next events

			return Task.CompletedTask;
		}
	}
}
