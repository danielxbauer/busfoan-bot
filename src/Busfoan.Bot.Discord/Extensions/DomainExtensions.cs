using System;
using System.Collections.Immutable;
using Busfoan.Domain;
using Discord;

namespace Busfoan.Bot.Discord.Extensions
{
    internal static class DomainExtensions
    {
        public static Answer AsAnswer(this IEmote emote, Func<ImmutableList<Card>, Card, bool> isCorrect)
            => new Answer(emote.Name, isCorrect);
    }
}
