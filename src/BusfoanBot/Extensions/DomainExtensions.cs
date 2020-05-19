using System;
using System.Collections.Immutable;
using BusfoanBot.Domain;
using Discord;

namespace BusfoanBot.Extensions
{
    internal static class DomainExtensions
    {
        public static Answer AsAnswer(this IEmote emote, Func<ImmutableList<Card>, Card, bool> isCorrect)
            => new Answer(emote.Name, isCorrect);
    }
}
