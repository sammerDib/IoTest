using System;
using System.Runtime.Serialization;

using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Context
{
    [DataContract]
    [ApplyAfter(typeof(ObjectiveContext))]
    public class PositionContext : ANAContextBase
    {
    }

    [DataContract]
    public class AnaPositionContext : PositionContext
    {
        // Needed for XML serialization
        public AnaPositionContext()
        {
        }

        public AnaPositionContext(AnaPosition position)
        {
            Position = position;
        }

        [DataMember]
        public AnaPosition Position { get; set; }
    }

    [DataContract]
    public class XYPositionContext : PositionContext
    {
        // Needed for XML serialization
        public XYPositionContext()
        {
        }

        public XYPositionContext(XYPosition position)
        {
            Position = position;
        }

        [DataMember]
        public XYPosition Position { get; set; }
    }

    [DataContract]
    public class XYZTopZBottomPositionContext : PositionContext
    {
        // Needed for XML serialization
        public XYZTopZBottomPositionContext()
        {
        }

        public XYZTopZBottomPositionContext(XYZTopZBottomPosition position)
        {
            Position = position;
        }

        [DataMember]
        public XYZTopZBottomPosition Position { get; set; }
    }

    public static class PositionContextFactory
    {
        public static PositionContext Build(PositionBase position)
        {
            switch (position)
            {
                case AnaPosition anaPosition:
                    return new AnaPositionContext(anaPosition);

                case XYZTopZBottomPosition xyZTopZBottomPosition:
                    return new XYZTopZBottomPositionContext(xyZTopZBottomPosition);

                case XYPosition xyPosition:
                    return new XYPositionContext(xyPosition);

                default:
                    throw new ArgumentException($"Unknown position type \"{position.GetType()}\" to create a PositionContext from.");
            }
        }
    }

    public static class PositionContextExtension
    {
        public static PositionBase GetPosition(this PositionContext context)
        {
            switch (context)
            {
                case AnaPositionContext anaPosition:
                    return anaPosition.Position;

                case XYPositionContext xyPosition:
                    return xyPosition.Position;

                case XYZTopZBottomPositionContext xyZBottomPosition:
                    return xyZBottomPosition.Position;
            }

            throw new ArgumentException($"Unknown position context type \"{context.GetType()}\" to extract position from.");
        }
    }
}
