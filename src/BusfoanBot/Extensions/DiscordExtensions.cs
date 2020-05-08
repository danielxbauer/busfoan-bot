using System.Collections.Generic;
using Discord;

namespace BusfoanBot.Extensions
{
    public static class DiscordExtensions
    {
        public static EmbedBuilder AddFields(this EmbedBuilder builder, IEnumerable<EmbedFieldBuilder> fieldBuilders)
        {
            foreach(var fieldBuilder in fieldBuilders) 
            {
                builder.AddField(fieldBuilder);
            }

            return builder;
        }
    }
}
