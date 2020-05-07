using System;
using System.Collections.Immutable;
using Discord;

namespace BusfoanBot.Models
{
    public sealed class Answer
    {
        public Answer(IEmote emote, Func<ImmutableList<Card>, Card, bool> isCorrect)
        {
            Emote = emote ?? throw new ArgumentNullException(nameof(emote));
            IsCorrect = isCorrect ?? throw new ArgumentNullException(nameof(isCorrect));
        }

        public IEmote Emote { get; }
        public Func<ImmutableList<Card>, Card, bool> IsCorrect { get; }
    }
}
