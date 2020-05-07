using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Discord;

namespace BusfoanBot.Models
{
    public class Question
    {
        public Question(string text, IEnumerable<Answer> answers)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Answers = ImmutableList.CreateRange<Answer>(answers);
        }

        public string Text { get; }
        public ImmutableList<Answer> Answers { get; }

        public static Question Create(string text, params Answer[] answers)
            => new Question(text, answers);

        public bool IsCorrectAnswer(IEmote emote, ImmutableList<Card> lastCards, Card card)
            => Answers.Where(a => a.Emote.Name == emote.Name).First().IsCorrect(lastCards, card);
    }
}
