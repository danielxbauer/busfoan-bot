using System;

namespace Busfoan.Domain
{
    public class Player
    {
        public Player(ulong id, string name)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public ulong Id { get; }
        public string Name { get; }
    }
}
