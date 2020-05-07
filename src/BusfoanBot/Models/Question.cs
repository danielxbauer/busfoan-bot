using System;
using System.Collections.Generic;
using System.Collections.Immutable;

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
    }
}
