using System;

namespace UnitySC.PM.Shared.Referentials.Interface
{
    public static class ConversionExtensions
    {
        /// <summary>
        /// This extension method converts a position into a XYPosition.
        /// </summary>
        /// <exception cref="Exception">If the given position cannot be converted.</exception>
        public static XYPosition ToXYPosition(this PositionBase positionBaseFrom)
        {
            switch (positionBaseFrom)
            {
                case XYZTopZBottomPosition xyZTopZBottomPosition:
                    {
                        return new XYPosition(xyZTopZBottomPosition.Referential, xyZTopZBottomPosition.X, xyZTopZBottomPosition.Y);
                    }
                case XYPosition xyPosition:
                    {
                        return xyPosition;
                    }
                default:
                    throw new Exception($"The type {positionBaseFrom.GetType().Name} can not be converted to XYPosition");
            }
        }

        /// <summary>
        /// This extension method converts a position into a XYZPosition.
        /// </summary>
        /// <exception cref="Exception">If the given position cannot be converted.</exception>
        public static XYZPosition ToXYZPosition(this PositionBase positionBaseFrom)
        {
            switch (positionBaseFrom)
            {
                case XYZPosition xyzPosition:
                    {
                        // We create a new XYZTopZBottomPosition because it could be a AnaPosition
                        return new XYZPosition(xyzPosition.Referential, xyzPosition.X, xyzPosition.Y, xyzPosition.Z);
                    }
                case XYPosition xyPosition:
                    {
                        return new XYZPosition(xyPosition.Referential, xyPosition.X, xyPosition.Y, double.NaN);
                    }
                case XPosition xPosition:
                    {
                        return new XYZPosition(xPosition.Referential, xPosition.Position, double.NaN, double.NaN);
                    }
                default:
                    throw new Exception($"The type {positionBaseFrom.GetType().Name} can not be converted to XYZPosition");
            }
        }
        /// <summary>
        /// This extension method converts a position into a XYZTopZBottomPosition.
        /// </summary>
        /// <exception cref="Exception">If the given position cannot be converted.</exception>
        public static XYZTopZBottomPosition ToXYZTopZBottomPosition(this PositionBase positionBaseFrom)
        {
            switch (positionBaseFrom)
            {
                case XYZTopZBottomPosition xyZTopZBottomPosition:
                    {
                        // We create a new XYZTopZBottomPosition because it could be a AnaPosition
                        return new XYZTopZBottomPosition(xyZTopZBottomPosition.Referential, xyZTopZBottomPosition.X, xyZTopZBottomPosition.Y, xyZTopZBottomPosition.ZTop, xyZTopZBottomPosition.ZBottom);
                    }
                case XYPosition xyPosition:
                    {
                        return new XYZTopZBottomPosition(xyPosition.Referential, xyPosition.X, xyPosition.Y, double.NaN, double.NaN);
                    }
                case XPosition xPosition:
                    {
                        return new XYZTopZBottomPosition(xPosition.Referential, xPosition.Position, double.NaN, double.NaN, double.NaN);
                    }
                default:
                    throw new Exception($"The type {positionBaseFrom.GetType().Name} can not be converted to XYZTopZBottomPosition");
            }
        }

        // <summary>
        /// This extension method converts a position into a XTPosition.
        /// </summary>
        /// <exception cref="Exception">If the given position cannot be converted.</exception>
        public static XTPosition ToXTPosition(this PositionBase positionBaseFrom)
        {
            switch (positionBaseFrom)
            {
                case XTPosition xtPosition:
                    {
                        return xtPosition;
                    }
                case XPosition xPosition:
                    {
                        return new XTPosition(xPosition.Referential, xPosition.Position, double.NaN);
                    }
                case TPosition tPosition:
                    {
                        return new XTPosition(tPosition.Referential, double.NaN, tPosition.Position);
                    }
                default:
                    throw new Exception($"The type {positionBaseFrom.GetType().Name} can not be converted to XTPosition");
            }
        }

        // <summary>
        /// This extension method converts a position into a Position (derived from OneAxisPosition.
        /// </summary>
        /// <exception cref="Exception">If the given position cannot be converted.</exception>
        public static double ToOnePosition(this PositionBase positionBaseFrom)
        {
            switch (positionBaseFrom)
            {
                case OneAxisPosition oneAxisPosition:
                    {
                        return oneAxisPosition.Position;
                    }
                default:
                    throw new Exception($"The type {positionBaseFrom.GetType().Name} can not be converted to OneAxisPosition.Position");
            }
        }
    }
}
