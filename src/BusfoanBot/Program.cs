using System.Collections.Generic;
using Discord;
using Discord.WebSocket;
using Statecharts.NET;
using Statecharts.NET.Model;
using Statecharts.NET.XState;
using static BusfoanBot.BotStateMachine;
using static BusfoanBot.BotEvents;
using Task = System.Threading.Tasks.Task;
using BusfoanBot.Models;

namespace BusfoanBot
{
	class Program
    {
		private static readonly string token = "NzA2NDk3MjMwMDA1MjA3MDQx.Xq7pFw.mGgt38aCNVHmcZ4NxR4xXwqDMgY";
		
		private static DiscordSocketClient client;
		private static ImageCache imageCache;
		private static IDictionary<ulong, RunningStatechart<BotContext>> statecharts
			= new Dictionary<ulong, RunningStatechart<BotContext>>();

		public static async Task Main()
        {
			imageCache = new ImageCache();

			client = new DiscordSocketClient();
			client.Log += Log;
			client.MessageReceived += MessageReceived;
            client.ReactionAdded += (_, __, reaction) => ReactionAdded(reaction);
			client.ReactionRemoved += (_, __, reaction) => ReactionRemoved(reaction);

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

			if (message.Content.StartsWith("!aufhern"))
				GetStatechartIn(message.Channel).Send(Cancel);

			return Task.CompletedTask;
		}

		private static Task ReactionAdded(SocketReaction reaction)
		{
			if (!statecharts.ContainsKey(reaction.Channel.Id)) return Task.CompletedTask;
			if (!reaction.User.IsSpecified || reaction.User.Value.IsBot) return Task.CompletedTask;

			var statechart = GetStatechartIn(reaction.Channel);
			statechart.Send(BotEvents.ReactionAdded(reaction));

			return Task.CompletedTask;
		}

		private static Task ReactionRemoved(SocketReaction reaction)
		{
			if (!statecharts.ContainsKey(reaction.Channel.Id)) return Task.CompletedTask;
			if (!reaction.User.IsSpecified || reaction.User.Value.IsBot) return Task.CompletedTask;

			var statechart = GetStatechartIn(reaction.Channel);
			statechart.Send(BotEvents.ReactionRemoved(reaction));

			return Task.CompletedTask;
		}

		private static RunningStatechart<BotContext> GetStatechartIn(ISocketMessageChannel channel)
		{
			if (statecharts.ContainsKey(channel.Id)) return statecharts[channel.Id];

			BotContext initialContext =	GetInitialContext(channel, imageCache);
			var options = new BotOptions
			{
				OnDone = context => CleanupStatechart(context.Channel)
			};

			StatechartDefinition<BotContext> behaviour = Behaviour(initialContext, options);

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

		private static Task CleanupStatechart(ISocketMessageChannel channel)
        {
			if (!statecharts.ContainsKey(channel.Id)) return Task.CompletedTask;

			// TODO: How to stop running statechart?
			statecharts.Remove(channel.Id);

			return Task.CompletedTask;
        }
	}
}
