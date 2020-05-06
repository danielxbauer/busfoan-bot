using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BusfoanBot.Models;
using Discord.WebSocket;
using Statecharts.NET.Interfaces;
using Statecharts.NET.XState;

namespace BusfoanBot
{
    public class BotContext : IContext<BotContext>, IXStateSerializable
    {
        private readonly Random random;

        public BotContext(
            ISocketMessageChannel channel,
            IEnumerable<Question> questions)
        {
            random = new Random();

            Channel = channel ?? throw new ArgumentNullException(nameof(channel));
            AllPlayers = ImmutableList<Player>.Empty;
            Questions = ImmutableStack.CreateRange(questions.Reverse());
            Players = ImmutableStack<Player>.Empty;
            Cards = ImmutableStack<Card>.Empty;
            PlayerCards = new Dictionary<ulong, ImmutableList<Card>>();
        }

        public ISocketMessageChannel Channel { get; }
        public ImmutableStack<Card> Cards { get; private set; }
        public IDictionary<ulong, ImmutableList<Card>> PlayerCards { get; }

        public ImmutableStack<Question> Questions { get; private set; }
        public Question ActiveQuestion { get; private set; }

        public ImmutableList<Player> AllPlayers { get; private set; }
        public ImmutableStack<Player> Players { get; private set; }
        public Player ActivePlayer { get; private set; }

        public ObjectValue AsJSObject() => new ObjectValue(Enumerable.Empty<JSProperty>());
        public BotContext CopyDeep() => this;

        public bool Join(Player player)
        {
            bool alreadyAdded = AllPlayers.Any(p => p.Id == player.Id);
            if (!alreadyAdded)
                AllPlayers = AllPlayers.Add(player);

            return alreadyAdded;
        }

        internal bool Kick(ulong id)
        {
            int count = AllPlayers.Count;
            AllPlayers = AllPlayers.RemoveAll(p => p.Id == id);
            return AllPlayers.Count - count > 0;
        }

        public void SendMessage(string message)
            => Channel.SendMessageAsync(message);

        public bool AreQuestionsLeft => !Questions.IsEmpty;
        public bool AreEnoughPlayers => AllPlayers.Count >= 1; // TODO: check >= 2 && <= 12
        public bool ArePlayersLeft => !Players.IsEmpty;

        public void ShuffleCards()
        {
            var shuffledCards = GenerateCards().OrderBy(x => random.Next());
            Cards = ImmutableStack.CreateRange<Card>(shuffledCards);
        }

        private IEnumerable<Card> GenerateCards()
        {
            return new[] { CardSymbol.Club, CardSymbol.Spade, CardSymbol.Diamond, CardSymbol.Heart }
                .SelectMany(symbol => GenerateCards(symbol));
        }

        private IEnumerable<Card> GenerateCards(CardSymbol symbol)
        {
            return "2 3 4 5 6 7 8 9 10 J Q K A"
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(type => new Card(type, symbol));
        }

        internal void SelectNextQuestion()
        {
            ActiveQuestion = Questions.Peek();
            Questions = Questions.Pop();
            Players = ImmutableStack.CreateRange(AllPlayers);
        }

        internal void SelectNextPlayer()
        {
            ActivePlayer = Players.Peek();
            Players = Players.Pop();
        }

        public void RevealCard()
        {
            if (ActivePlayer != null)
            {
                Card card = RevealCard(ActivePlayer.Id);
                SendMessage($"Sorry zlaung gwoat! Karte is: {card}");
            }
        } 

        public void RevealCardFor(ulong player)
        {
            Card card = RevealCard(player);
            SendMessage($"Karte is: {card}");
        }

        private Card RevealCard(ulong player)
        {
            Card card = Cards.Peek();
            Cards = Cards.Pop();

            if (!PlayerCards.ContainsKey(player))
                PlayerCards.Add(player, ImmutableList.CreateRange(new[] { card }));
            else
                PlayerCards[player].Add(card);

            return card;
        }
    }
}
