using System;
using Discord;
using Discord.WebSocket;
using Statecharts.NET.Interfaces;
using Statecharts.NET.Language;
using Statecharts.NET.Language.Builders.StateNode;
using Statecharts.NET.Language.Builders.Transition;
using Statecharts.NET.Model;
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

        public static ContextDataGuarded<BotContext, SocketReaction> IfReactedWith(
            this WithNamedDataEvent<SocketReaction> @event,
            IEmote emote,
            Func<BotContext, SocketReaction, bool> andConition = null)
        {
            return @event.If<BotContext>((context, reaction) =>
            {
                return context.LastReactableMessage != null
                    && context.LastReactableMessage.Id == reaction.MessageId
                    && reaction.Emote.Name == emote.Name
                    && (andConition?.Invoke(context, reaction) ?? true);
            });
        }

        // Currently not possible because TransitionTo.Self triggers EXIT/ENTER again :(
        ////public static CompoundWithStates ReactableWithTransitions(
        ////    this string state,
        ////    TransitionDefinition transition,
        ////    params TransitionDefinition[] transitions)
        ////{
        ////    return state
        ////        .WithTransitions(transition, transitions)
        ////        .AsCompound()
        ////        .WithInitialState("start")
        ////        .WithStates(
        ////            "start"
        ////                .WithEntryActions(Log("REACTION____START"))
        ////                .WithTransitions(Immediately.TransitionTo.Sibling("waiting-to-end")),
        ////            "waiting-to-end"
        ////                .WithExitActions<BotContext>(
        ////                    Log("RACTION____END"),
        ////                    Assign<BotContext>(context =>
        ////                    {
        ////                        context.LastBotMessage = null;
        ////                    })
        ////                    ));
        ////}
    }
}
