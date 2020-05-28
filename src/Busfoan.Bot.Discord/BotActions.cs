using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Busfoan.Bot.Discord.Models;
using Busfoan.Domain;
using Busfoan.Graphic;
using Discord;
using Discord.WebSocket;
using Statecharts.NET.Utilities;

namespace Busfoan.Bot.Discord
{
    public static class BotActions
    {
        public static IImageProcessor imageProcessor; // TODO:

        private static Embed GetWelcomeMessage(BotContext context)
        {
            var playerNames = context.AllPlayers.Select(p => $"\t*{p.Name}");

            var message = "Seas leidl.\n" +
                           "I bins da bus!\n" +
                           "\n" +
                          $"{Emotes.Check} zum einsteigen\n" +
                          $"{Emotes.Bus} zum starten\n" +
                          $"{Emotes.CrossMark} zum beenden\n" +
                          $"\n" +
                          $"{playerNames.Count()} san dabei:\n" +
                          $"{string.Join('\n', playerNames)}";

            return new EmbedBuilder()
                .WithDescription(message)
                .WithColor(Color.Gold)
                .Build();
        }

        public static async Task WelcomeMessage(BotContext context)
        {
            var message = GetWelcomeMessage(context);
            await context.SendReactableMessage(message, Emotes.Check, Emotes.Bus, Emotes.CrossMark);
        }

        public static async Task UpdateWelcomeMessage(BotContext context)
        {
            var message = GetWelcomeMessage(context);
            await context.UpdateMessage(context.LastReactableMessage, message);
        }

        public static async Task ExitMessage(BotContext context)
        {
            await context.SendMessage("So des woas. Bis boid!");
        }

        public static async Task Canceled(BotContext context)
        {
            await context.SendMessage(b => b
                .WithColor(Color.Red)
                .WithDescription($"{Emotes.CrossMark} Bus gecancelt {Emotes.CrossMark}"));
        }

        public static Task DeleteLastReactableMessage(BotContext context)
            => DeleteLastReactableMessage(context, null);
        public static async Task DeleteLastReactableMessage(BotContext context, SocketReaction reaction)
        {
            if (context.LastReactableMessage == null) return;
            await context.DeleteMessage(context.LastReactableMessage);
        }

        public static async Task AddPlayer(BotContext context, SocketReaction reaction)
        {
            IUser author = reaction.User.GetValueOrDefault();
            if (author == null) return;

            context.Join(new Player(author.Id, author.Username));
            await UpdateWelcomeMessage(context);
        }

        public static async Task RemovePlayer(BotContext context, SocketReaction reaction)
        {
            IUser author = reaction.User.GetValueOrDefault();
            if (author == null) return;

            context.Kick(author.Id);
            await UpdateWelcomeMessage(context);
        }

        public static async Task LogGameStartMessage(BotContext context)
        {
            await context.SendMessage("LOS GEHTSS!");
        }

        public static async Task LogNotEnoughPlayers(BotContext context, SocketReaction reaction)
        {
            await context.SendMessage("Nu ned gnuag leit");
        }

        public static void ShuffleCards(BotContext context)
        {
            // TODO: put into context
            var random = new Random();
            var shuffledCards = GenerateCards().OrderBy(x => random.Next());
            context.Cards = ImmutableStack.CreateRange(shuffledCards);
            context.Pyramid = new Pyramid(4, context.Draw);
        }

        public static IEnumerable<Card> GenerateCards()
        {
            return new[] { CardSymbol.Club, CardSymbol.Spade, CardSymbol.Diamond, CardSymbol.Heart }
                .SelectMany(symbol => GenerateCards(symbol));
        }
        private static IEnumerable<Card> GenerateCards(CardSymbol symbol)
        {
            return "2 3 4 5 6 7 8 9 10 J Q K A"
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select((type, index) => new Card(type, symbol, index + 2));
        }

        public static void SelectNextQuestion(BotContext context)
            => context.SelectNextQuestion();

        public static void SelectNextPlayer(BotContext context)
            => context.SelectNextPlayer();

        public static async Task AskQuestion(BotContext context)
        {
            var cards = context.Draw(context.ActivePlayer.Id);
            
            var message = new EmbedBuilder()
                .WithAuthor(context.ActivePlayer.Name)
                .WithDescription(context.ActiveQuestion.Text)
                .WithColor(Color.Orange)
                .Build();

            var cardImage = imageProcessor.GenerateCardImage(cards);
            await context.SendReactableFile(cardImage, message,
                context.ActiveQuestion.Answers.Select(a => new Emoji(a.Emote)).AsEnumerable());
        }

        // TODO: check if this works?
        public static async Task RevealCard(BotContext context)
        {
            if (context.ActivePlayer == null) return;

            await context.SendMessage(b => b
                .WithColor(Color.Red)
                .WithAuthor(context.ActivePlayer.Name)
                .WithDescription($"{Emotes.CrossMark} Sauf ans {Emotes.BeerClinking}"));
        }
        public static async Task RevealCardForUser(BotContext context, SocketReaction reaction)
        {
            IUser player = reaction.User.GetValueOrDefault();
            if (player == null) return;

            var cards = context.PlayerCards.GetValue(player.Id, ImmutableList<Card>.Empty);
            context.RevealCard(player.Id);

            // TODO: test what happens if an exception is thrown here (e.g. wrong file path)
            bool isCorrect = context.ActiveQuestion.IsCorrectAnswer(reaction.Emote.Name, cards);

            //var cards = context.PlayerCards.GetValue(context.ActivePlayer.Id, ImmutableList<Card>.Empty);
            var message = new EmbedBuilder()
                .WithColor(isCorrect ? Color.Green : Color.Red)
                .WithAuthor(context.ActivePlayer.Name)
                .WithDescription(isCorrect
                    ? $"{Emotes.Check} Verteil ans {Emotes.BeerClinking}"
                    : $"{Emotes.CrossMark} Sauf ans {Emotes.BeerClinking}")
                .Build();

            ////context.DeleteMessage(context.LastReactableMessage);

            var cardImage = imageProcessor.GenerateCardImage(cards);
            await context.SendFile(cardImage, message);
        }

        public static async Task LogPyramidStartMessage(BotContext context)
        {
            await context.SendMessage("LOS GEHTSS!");
        }

        public static async Task ShowPyramid(BotContext context)
        {
            //await context.SendMessage("PYRAMID");
            var image = imageProcessor.GeneratePyramidImage(context.Pyramid);
            await context.SendFile(image);
        }

        public static async Task RevealPyramidCard(BotContext context)
        {
            context.Pyramid.RevealCard();
            await context.SendMessage("REVEAL PYRAMID CARD");
        }
    }
}
