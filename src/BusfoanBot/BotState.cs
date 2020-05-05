using System;
using System.Linq;
using System.Threading.Tasks;
using BusfoanBot.Models;
using Discord;
using Discord.WebSocket;

namespace BusfoanBot
{
    public class BotState
    {
        private readonly BotContext state;

        public BotState()
        {
            this.state = new BotContext();
            state.Questions.AddRange(new[]
            {
                new Question("Rot oder schwarz?")
            });
        }

        public async Task HandleMessage(SocketMessage message)
        {
            Log(message.Content);

            if (message.Content.StartsWith("!busfoan"))
            {
                await message.Channel.SendMessageAsync("jawoi");
            }
        }

        public async Task SendAsync(string command, SocketMessage message)
        {
            switch (command)
            {
                case "START": await ShowWelcomeMessage(message); break;
                case "JOIN": await Join(message); break;
                case "START-GAME": await StartGame(message); break;
            }
        }

        private async Task ShowWelcomeMessage(SocketMessage message)
        {
            await message.Channel.SendMessageAsync("Gscheid busfoan! Wer will einsteigen? (Tippe !einsteigen)");
        }

        private async Task Join(SocketMessage message)
        {
            var player = new Player(message.Author.Username);
            this.state.Players.Add(player);

            await message.Channel.SendMessageAsync($"{player.Name} is eingestiegen. San scho {state.Players.Count} leit im bus!");
        }

        private async Task StartGame(SocketMessage message)
        {
            await message.Channel.SendMessageAsync($"Da bus startet mit {state.Players.Count} leiwaunde leit.");
            await message.Channel.SendMessageAsync("test",
                embed: new EmbedBuilder()
                .WithTitle("Abfoat")
                .WithDescription("seas, seas")
                .WithColor(Color.Blue)
                .Build());
            await NextQuestion(message);
        }

        private async Task NextQuestion(SocketMessage message)
        {
            state.SelectNextQuestion();
            await message.Channel.SendMessageAsync(state.ActiveQuestion.Text);
        }

        private void Log(string message)
        {
            Log(new LogMessage(LogSeverity.Info, "Bot", message));
        }

        public Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }
    }
}
