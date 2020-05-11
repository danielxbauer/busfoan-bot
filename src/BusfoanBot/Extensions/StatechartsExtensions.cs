using System;
using Discord;
using Discord.WebSocket;
using Statecharts.NET.Interfaces;
using Statecharts.NET.Language.Builders.Transition;
using static Statecharts.NET.Language.Keywords;

namespace BusfoanBot.Extensions
{
    public static class StatechartsExtensions
    {
        public static ContextGuardedWithActions<TContext> WithAction<TContext>(
            this ContextGuardedWithTarget<TContext> builder,
            System.Action<TContext> action)
            where TContext : IContext<TContext>
        {
            return builder.WithActions(Run<TContext>(action));
        }

        public static ContextDataGuarded<BotContext, SocketReaction> IfReactable(
            this WithNamedDataEvent<SocketReaction> @event,
            Func<BotContext, SocketReaction, bool> andConition = null)
        {
            return @event.If<BotContext>((context, reaction) =>
            {
                return context.LastReactableMessage != null
                    && context.LastReactableMessage.Id == reaction.MessageId
                    && (andConition?.Invoke(context, reaction) ?? true);
            });
        }

        public static ContextDataGuarded<BotContext, SocketReaction> IfReactedWith(
            this WithNamedDataEvent<SocketReaction> @event,
            IEmote emote,
            Func<BotContext, SocketReaction, bool> andConition = null)
        {
            return @event.IfReactable((context, reaction) =>
            {
                return reaction.Emote.Name == emote.Name
                    && (andConition?.Invoke(context, reaction) ?? true);
            });
        }
    }
}
