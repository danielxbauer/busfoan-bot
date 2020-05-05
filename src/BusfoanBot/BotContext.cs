using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BusfoanBot.Models;
using Statecharts.NET.Interfaces;
using Statecharts.NET.XState;

namespace BusfoanBot
{
    public class BotContext : IContext<BotContext>, IXStateSerializable
    {
        public ImmutableList<Player> AllPlayers { get; private set; }
        public ImmutableStack<Question> Questions { get; }
        public ImmutableStack<Player> Players { get; }

        public ObjectValue AsJSObject()
            => new ObjectValue(Enumerable.Empty<JSProperty>());

        public BotContext CopyDeep() => this;

        public bool Equals([AllowNull] BotContext other) 
            => true; // TODO: not needed

        public BotContext(
            IEnumerable<Question> questions)
        {
            AllPlayers = ImmutableList<Player>.Empty;
            Questions = ImmutableStack.CreateRange(questions.Reverse());
            Players = ImmutableStack<Player>.Empty;
        }

        public bool Join(Player player)
        {
            bool alreadyAdded = this.AllPlayers.Any(p => p.Id == player.Id);
            if (!alreadyAdded)
                AllPlayers = AllPlayers.Add(player);

            return alreadyAdded;
        }

        public bool AreQuestionsLeft => !Questions.IsEmpty;
        public bool AreEnoughPlayers => AllPlayers.Count >= 1; // TODO: check >= 2 && <= 12
    }
}
