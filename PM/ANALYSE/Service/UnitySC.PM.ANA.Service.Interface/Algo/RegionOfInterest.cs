using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class RegionOfInterest : IEquatable<RegionOfInterest>
    {
        [DataMember]
        public Length X { get; set; }
        [DataMember]
        public Length Y { get; set; }

        [DataMember]
        public Length Width { get; set; }

        [DataMember]
        public Length Height { get; set; }

        [DataMember]
        public Length OffsetX { get; set; } = 0.Millimeters();

        [DataMember]
        public Length OffsetY { get; set; } = 0.Millimeters();

        public RegionOfInterest()
        {
        }
        public RegionOfInterest(Length x, Length y, Length width, Length height, Length offsetX = null, Length offsetY = null)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;

            if (!(offsetX is null))
                OffsetX = offsetX;

            if (!(offsetY is null))
                OffsetY = offsetY;
        }

        public bool Equals(RegionOfInterest other)
        {
            return !(other is null) &&
                other.X == X && 
                other.Y == Y && 
                other.Width == Width &&
                other.Height == Height &&
                other.OffsetX == OffsetX &&
                other.OffsetY == OffsetY;
        }
    }
}
