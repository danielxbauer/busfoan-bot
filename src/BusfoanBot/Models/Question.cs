using System;

namespace BusfoanBot.Models
{
    public class Question
    {
        public Question(string text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        public string Text { get; }
    }
}
