using System;

namespace BusfoanBot.Models
{
    public class Player
    {
        public Player(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }
    }
}
