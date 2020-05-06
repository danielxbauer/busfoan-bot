using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Discord;

namespace BusfoanBot.Models
{
    public class Question
    {
        public Question(string text, IEnumerable<IEmote> emotes)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Emotes = ImmutableList.CreateRange<IEmote>(emotes);
        }

        public string Text { get; }
        public ImmutableList<IEmote> Emotes { get; }

        public static Question Create(string text, params IEmote[] emotes)
            => new Question(text, emotes);
    }
}
