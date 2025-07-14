using System;
using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class CenteredRegionOfInterest : IEquatable<CenteredRegionOfInterest>
    {
        [DataMember]
        public Length Width { get; set; }

        [DataMember]
        public Length Height { get; set; }

        [DataMember]
        public Length OffsetX { get; set; } = 0.Millimeters();

        [DataMember]
        public Length OffsetY { get; set; } = 0.Millimeters();

        public CenteredRegionOfInterest()
        {
        }

        public CenteredRegionOfInterest(Length width, Length height, Length offsetX = null, Length offsetY = null)
        {
            Width = width;
            Height = height;

            if (!(offsetX is null))
                OffsetX = offsetX;

            if (!(offsetY is null))
                OffsetY = offsetY;
        }

        public bool Equals(CenteredRegionOfInterest other)
        {
            return !(other is null) &&
                other.Width == Width &&
                other.Height == Height &&
                other.OffsetX == OffsetX &&
                other.OffsetY == OffsetY;
        }
    }
}
