using Discord.WebSocket;
using Statecharts.NET.Language;
using Statecharts.NET.Model;

namespace Busfoan.Bot.Discord
{
    public static class BotEvents
    {
        public static NamedEvent WakeUp
            => Define.Event(nameof(WakeUp));

        public static NamedEvent Cancel
            => Define.Event(nameof(Cancel));

        public static NamedDataEventFactory<SocketMessage> NextPlayer
            => Define.EventWithData<SocketMessage>(nameof(NextPlayer));

        public static NamedDataEventFactory<SocketReaction> ReactionAdded
            => Define.EventWithData<SocketReaction>(nameof(ReactionAdded));

        public static NamedDataEventFactory<SocketReaction> ReactionRemoved
            => Define.EventWithData<SocketReaction>(nameof(ReactionRemoved));
    }
}
