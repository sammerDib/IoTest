using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client.Proxy.Axes.Models
{
    public static class PositionExtension
    {
        // checks if the two positions are close
        public static bool Near(this Position position, Position position2, double epsilon, bool ignoreSpeed = true)
        {
            if (position.X.Near(position2.X, epsilon) &&
                position.Y.Near(position2.Y, epsilon) &&
                position.ZTop.Near(position2.ZTop, epsilon) &&
                position.ZBottom.Near(position2.ZBottom, epsilon) &&
                ((position.Speed == position2.Speed) || ignoreSpeed))
                return true;

            return false;
        }
    }
}
