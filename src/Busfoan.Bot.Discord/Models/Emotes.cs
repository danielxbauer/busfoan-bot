using Discord;

namespace Busfoan.Bot.Discord.Models
{
    public static class Emotes
    {
        public static IEmote ThumbsUp => new Emoji("👍");
        public static IEmote Grin => new Emoji("😀");
        public static IEmote Check => new Emoji("✅");
        public static IEmote Bus => new Emoji("🚌");
        public static IEmote Club => new Emoji("♣️");
        public static IEmote Heart => new Emoji("❤️");
        public static IEmote Spade => new Emoji("♠️");
        public static IEmote Diamond => new Emoji("♦️");
        public static IEmote BeerClinking => new Emoji("🍻");
        public static IEmote CrossMark => new Emoji("❌");
    }
}
