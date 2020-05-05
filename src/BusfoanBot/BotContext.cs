using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BusfoanBot.Models;
using Statecharts.NET.Interfaces;
using Statecharts.NET.XState;

namespace BusfoanBot
{
    public class BotContext : IContext<BotContext>, IXStateSerializable
    {
        public List<Question> Questions { get; } = new List<Question>();
        public Question ActiveQuestion { get; private set; }
        public int ActiveQuestionIndex { get; private set; } = -1;

        public List<Player> Players { get; } = new List<Player>();
        public Player ActivePlayer { get; }

        public ObjectValue AsJSObject()
            => new ObjectValue(Enumerable.Empty<JSProperty>());

        public BotContext CopyDeep() => this;

        public bool Equals([AllowNull] BotContext other) 
            => true; // TODO: not needed

        public void SelectNextQuestion()
        {
            ActiveQuestionIndex++;
            ActiveQuestion = ActiveQuestionIndex < Questions.Count
                ? Questions[ActiveQuestionIndex]
                : Questions.First();
        }
    }
}
