using System;
using System.Collections.Immutable;

namespace Busfoan.Domain
{
    public sealed class Answer
    {
        public Answer(string emote, Func<ImmutableList<Card>, Card, bool> isCorrect)
        {
            Emote = emote ?? throw new ArgumentNullException(nameof(emote));
            IsCorrect = isCorrect ?? throw new ArgumentNullException(nameof(isCorrect));
        }

        public string Emote { get; }
        public Func<ImmutableList<Card>, Card, bool> IsCorrect { get; }
    }
}
