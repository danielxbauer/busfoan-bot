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

        public static ContextDataUnguardedWithActions<TContext, TEventData> WithRunAction<TContext, TEventData>(
            this DataUnguardedWithTarget<TEventData> builder,
            System.Action<TContext, TEventData> action)
            where TContext : IContext<TContext>
        {
            return builder.WithActions<TContext>(Run<TContext, TEventData>(action));
        }
    }
}
