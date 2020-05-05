using System;

namespace BusfoanBot.Models
{
    public class Player
    {
        public Player(string name, string discriminator)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Discriminator = discriminator ?? throw new ArgumentNullException(nameof(discriminator));
        }

        public string Id => $"{Name}#{Discriminator}";
        public string Name { get; }
        public string Discriminator { get; }
    }
}
