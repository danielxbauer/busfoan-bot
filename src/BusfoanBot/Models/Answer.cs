using System;
using Discord;

namespace BusfoanBot.Models
{
    public sealed class Answer
    {
        public Answer(IEmote emote, Func<Card, bool> isCorrect)
        {
            Emote = emote ?? throw new ArgumentNullException(nameof(emote));
            IsCorrect = isCorrect ?? throw new ArgumentNullException(nameof(isCorrect));
        }

        public IEmote Emote { get; }
        public Func<Card, bool> IsCorrect { get; }
    }
}
