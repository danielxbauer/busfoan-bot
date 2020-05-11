using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace BusfoanBot
{
    public abstract class DiscordContext
    {
        public ISocketMessageChannel Channel { get; }
        public RestUserMessage LastReactableMessage { get; set; }

        protected DiscordContext(ISocketMessageChannel channel)
        {
            Channel = channel ?? throw new ArgumentNullException(nameof(channel));
        }

        public async Task<RestUserMessage> SendMessage(string message)
        {
            return await Channel.SendMessageAsync(message);
        }
        public async Task<RestUserMessage> SendMessage(MessageBuilder builder)
        {
            var embed = builder(new EmbedBuilder()).Build();
            return await Channel.SendMessageAsync(null, embed: embed);
        }

        public async Task<RestUserMessage> SendReactableMessage(Embed message, params IEmote[] emotes)
            => await this.SendReactableMessage(message, emotes.AsEnumerable());
        public async Task<RestUserMessage> SendReactableMessage(Embed message, IEnumerable<IEmote> emotes)
        {
            LastReactableMessage = await Channel.SendMessageAsync(null, embed: message);
            await LastReactableMessage.AddReactionsAsync(emotes.ToArray());
            return LastReactableMessage;
        }

        public async Task UpdateMessage(RestUserMessage message, Embed content)
        {
            await message.ModifyAsync(prop =>
            {
                prop.Embed = content;
            });
        }
        public async Task DeleteMessage(RestUserMessage message)
        {
            await message.DeleteAsync();
        }

        public async Task SendFile(string filePath)
        {
            await Channel.SendFileAsync(filePath);
        }
        public async Task SendFile(string filePath, MessageBuilder builder)
        {
            var embed = builder(new EmbedBuilder()).Build();
            await Channel.SendFileAsync(filePath, embed: embed);
        }
    }
}
