using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Busfoan.Domain
{
    public class Question
    {
        public Question(string text, IEnumerable<Answer> answers)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Answers = ImmutableList.CreateRange(answers);
        }

        public string Text { get; }
        public ImmutableList<Answer> Answers { get; }

        public static Question Create(string text, params Answer[] answers)
            => new Question(text, answers);

        public bool IsCorrectAnswer(string emote, ImmutableList<Card> lastCards, Card card)
            => Answers.Where(a => a.Emote == emote).First().IsCorrect(lastCards, card);
    }
}
