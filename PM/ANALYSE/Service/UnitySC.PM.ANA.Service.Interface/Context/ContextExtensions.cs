using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Context
{
    public static class ContextExtensions
    {
        public static ANAContextBase AppendWith(this ANAContextBase context, ANAContextBase otherContext)
        {
            if (context is null)
                return otherContext;

            if (otherContext is null)
                return context;

            return new ContextsList(context, otherContext);
        }

        public static ANAContextBase AppendWithPositionContext(this ANAContextBase context, PositionBase position)
        {
            var positionContext = position is null ? null : PositionContextFactory.Build(position);
            return context.AppendWith(positionContext);
        }
    }
}
