using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using BusfoanBot.Models;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Statecharts.NET.Interfaces;
using Statecharts.NET.XState;

namespace BusfoanBot
{
    public class BotContext : IContext<BotContext>, IXStateSerializable
    {
        public BotContext(
            ISocketMessageChannel channel,
            IEnumerable<Question> questions)
        {
            Channel = channel ?? throw new ArgumentNullException(nameof(channel));
            AllPlayers = ImmutableList<Player>.Empty;
            Questions = ImmutableStack.CreateRange(questions.Reverse());
            Players = ImmutableStack<Player>.Empty;
            Cards = ImmutableStack<Card>.Empty;
            PlayerCards = new Dictionary<ulong, ImmutableList<Card>>();
        }

        public ISocketMessageChannel Channel { get; }
        public RestUserMessage LastReactableMessage { get; set; }

        public ImmutableStack<Card> Cards { get; set; }
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

        public async Task<RestUserMessage> SendMessage(string message)
        {
            return await Channel.SendMessageAsync(message);
        }
        public async Task<RestUserMessage> SendMessage(Func<EmbedBuilder, EmbedBuilder> builder)
        {
            var embed = builder(new EmbedBuilder()).Build();
            return await Channel.SendMessageAsync(null, embed: embed);
        }

        public async Task<RestUserMessage> SendReactableMessage(Embed message, params IEmote[] emotes)
            => await this.SendReactableMessage(message, emotes.AsEnumerable());
        public async Task<RestUserMessage> SendReactableMessage(Embed message, IEnumerable<IEmote> emotes)
        {
            LastReactableMessage = await Channel.SendMessageAsync(null, embed: message);
            await LastReactableMessage.AddReactionsAsync(emotes.ToArray());
            return LastReactableMessage;
        }

        public async Task UpdateMessage(RestUserMessage message, Embed content)
        {
            await message.ModifyAsync(prop =>
            {
                prop.Embed = content;
            });
        }
        public async Task DeleteMessage(RestUserMessage message)
        {
            await message.DeleteAsync();
        }

        public async Task SendFile(string filePath)
        {
            await Channel.SendFileAsync(filePath);
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

        public Card RevealCard(ulong player)
        {
            Card card = Cards.Peek();
            Cards = Cards.Pop();

            if (!PlayerCards.ContainsKey(player))
                PlayerCards.Add(player, ImmutableList.CreateRange(new[] { card }));
            else
                PlayerCards[player] = PlayerCards[player].Add(card);

            return card;
        }
    }
}
