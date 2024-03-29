﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Busfoan.Domain;
using Discord;
using Discord.WebSocket;
using Statecharts.NET.Interfaces;
using Statecharts.NET.XState;

namespace Busfoan.Bot.Discord
{
    public delegate EmbedBuilder MessageBuilder(EmbedBuilder builder);

    public class BotContext : DiscordContext, IContext<BotContext>, IXStateSerializable
    {
        public BotContext(
            ISocketMessageChannel channel,
            IEnumerable<Question> questions)
            : base(channel)
        {
            AllPlayers = ImmutableList<Player>.Empty;
            Questions = ImmutableStack.CreateRange(questions.Reverse());
            Players = ImmutableStack<Player>.Empty;
            Cards = ImmutableStack<Card>.Empty;
            PlayerCards = new Dictionary<ulong, ImmutableList<Card>>();
        }

        public ImmutableStack<Card> Cards { get; set; }
        public IDictionary<ulong, ImmutableList<Card>> PlayerCards { get; }

        public ImmutableStack<Question> Questions { get; private set; }
        public Question ActiveQuestion { get; private set; }

        public ImmutableList<Player> AllPlayers { get; private set; }
        public ImmutableStack<Player> Players { get; private set; }
        public Player ActivePlayer { get; private set; }

        public Pyramid Pyramid { get; set; }

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

        public bool AreQuestionsLeft => !Questions.IsEmpty;
        public bool AreEnoughPlayers => AllPlayers.Count >= 1; // TODO: check >= 2 && <= 12
        public bool ArePlayersLeft => !Players.IsEmpty;

        public void SelectNextQuestion()
        {
            ActiveQuestion = Questions.Peek();
            Questions = Questions.Pop();
            Players = ImmutableStack.CreateRange(AllPlayers);
        }

        public void SelectNextPlayer()
        {
            ActivePlayer = Players.Peek();
            Players = Players.Pop();
        }

        public Card Draw()
        {
            Card card = Cards.Peek();
            Cards = Cards.Pop();
            return card;
        }

        public ImmutableList<Card> Draw(ulong player)
        {
            Card card = Draw();

            if (!PlayerCards.ContainsKey(player))
                PlayerCards.Add(player, ImmutableList.CreateRange(new[] { card }));
            else
                PlayerCards[player] = PlayerCards[player].Add(card);

            return PlayerCards[player];
        }

        public void RevealCard(ulong player)
        {
            Card card = PlayerCards[player].Last();
            card.IsRevealed = true;
        }
    }
}
