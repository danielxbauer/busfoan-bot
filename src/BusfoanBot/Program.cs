using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Statecharts.NET;
using Statecharts.NET.Model;
using Task = System.Threading.Tasks.Task;
using static BusfoanBot.BotStateMachine;
using Statecharts.NET.XState;

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
			string viz = Behaviour.AsXStateVisualizerV4Definition();
			
			client = new DiscordSocketClient();
			client.Log += new BotState().Log;
			client.MessageReceived += MessageReceived;

			await client.LoginAsync(TokenType.Bot, token);
			await client.StartAsync();

			await statechart.Start();
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
				statechart.Send(StartGame(message));

			// statechart.NextEvents ...
			// statechart.OnMarcoStep => Show next events

			return Task.CompletedTask;
		}
	}
}
