using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Referentials.Interface
{
    [DataContract]
    public class AnaPosition : XYZTopZBottomPosition, IEquatable<AnaPosition>
    {
        [DataMember]
        public List<ZPiezoPosition> ZPiezoPositions { get; set; }

        private AnaPosition() : base(null)
        {
            // Required by XML serialization
        }

        public AnaPosition(ReferentialBase referential) : base(referential)
        {
            ZPiezoPositions = new List<ZPiezoPosition>();
        }

        public AnaPosition(ReferentialBase referential, double x, double y, double zTop, double zBottom, List<ZPiezoPosition> zPiezoPositions) : base(referential, x, y, zTop, zBottom)
        {
            ZPiezoPositions = zPiezoPositions;
        }

        /// <summary>
        /// Returns the piezo position of the axis with the ID given in parameter, or null if the
        /// axis is not found.
        /// </summary>
        /// <param name="axisID">The axis ID</param>
        public ZPiezoPosition GetPiezoPosition(string axisID)
        {
            return ZPiezoPositions.FirstOrDefault(piezoPosition => piezoPosition.AxisID == axisID);
        }

        /// <summary>
        /// Set not NaN values X and Y given in parameter as current property values.
        /// </summary>
        public void Merge(XYPosition xyPosition)
        {
            if (Referential != xyPosition.Referential)
            {
                throw new Exception($"Cannot merge positions from referential {xyPosition.Referential} to {Referential}");
            }
            X = double.IsNaN(xyPosition.X) ? X : xyPosition.X;
            Y = double.IsNaN(xyPosition.Y) ? Y : xyPosition.Y;
        }

        /// <summary>
        /// Set not NaN values X, Y, ZTop and ZBottom given in parameter as current property values.
        /// </summary>
        public void Merge(XYZTopZBottomPosition xyZTopZBottomPosition)
        {
            if (Referential != xyZTopZBottomPosition.Referential)
            {
                throw new Exception($"Cannot merge positions from referential {xyZTopZBottomPosition.Referential} to {Referential}");
            }
            Merge(new XYPosition(xyZTopZBottomPosition.Referential, xyZTopZBottomPosition.X, xyZTopZBottomPosition.Y));

            ZTop = double.IsNaN(xyZTopZBottomPosition.ZTop) ? ZTop : xyZTopZBottomPosition.ZTop;
            ZBottom = double.IsNaN(xyZTopZBottomPosition.ZBottom) ? ZBottom : xyZTopZBottomPosition.ZBottom;
        }

        /// <summary>
        /// Set not NaN values ZTop given in parameter as current property values.
        /// </summary>
        public void Merge(ZTopPosition zTopPosition)
        {
            if (Referential != zTopPosition.Referential)
            {
                throw new Exception($"Cannot merge positions from referential {zTopPosition.Referential} to {Referential}");
            }
            ZTop = double.IsNaN(zTopPosition.Position) ? ZTop : zTopPosition.Position;
        }

        /// <summary>
        /// Set not NaN values ZBottom given in parameter as current property values.
        /// </summary>
        public void Merge(ZBottomPosition zBottomPosition)
        {
            if (Referential != zBottomPosition.Referential)
            {
                throw new Exception($"Cannot merge positions from referential {zBottomPosition.Referential} to {Referential}");
            }
            ZBottom = double.IsNaN(zBottomPosition.Position) ? ZBottom : zBottomPosition.Position;
        }

        /// <summary>
        /// Set not NaN values X, Y, ZTop, ZBottom and ZPiezoPositions given in parameter as current
        /// property values.
        /// </summary>
        public void Merge(AnaPosition anaPosition)
        {
            if (Referential != anaPosition.Referential)
            {
                throw new Exception($"Cannot merge positions from referential {anaPosition.Referential} to {Referential}");
            }
            Merge(new XYZTopZBottomPosition(anaPosition.Referential, anaPosition.X, anaPosition.Y, anaPosition.ZTop, anaPosition.ZBottom));

            foreach (var piezoPosition in ZPiezoPositions)
            {
                var anaPiezoPosition = anaPosition.GetPiezoPosition(piezoPosition.AxisID);
                if (!(anaPiezoPosition is null))
                    piezoPosition.Position = double.IsNaN(anaPiezoPosition.Position) ? piezoPosition.Position : anaPiezoPosition.Position;
            }
        }

        /// <summary>
        /// Add a new ZPiezoPosition in the list of piezo positions. If the position already exists
        /// (i.e. has the same AxisID property), the position value is updated to the one given in
        /// parameter, except if NaN.
        /// </summary>
        public void AddOrUpdateZPiezoPosition(ZPiezoPosition zPiezoPosition)
        {
            if (Referential != zPiezoPosition.Referential)
            {
                throw new Exception($"Cannot merge positions from referential {zPiezoPosition.Referential} to {Referential}");
            }

            if (zPiezoPosition is null || zPiezoPosition.PiezoPosition is null)
            {
                throw new Exception($"Invalid ZPiezoPosition: a position value should be specified.");
            }

            if (zPiezoPosition.Position.IsNaN())
            {
                return; // no need to update
            }

            var piezoPosition = ZPiezoPositions.FirstOrDefault(p => p.AxisID == zPiezoPosition.AxisID);
            if (piezoPosition is null)
            {
                ZPiezoPositions.Add(zPiezoPosition);
            }
            else
            {
                piezoPosition.Position = zPiezoPosition.Position;
            }
        }

        public override bool Equals(object other)
        {
            return (other is AnaPosition otherAnaPosition) ? Equals(otherAnaPosition) : false;
        }

        public bool Equals(AnaPosition otherAnaPosition)
        {
            if (otherAnaPosition is null) return false;

            bool hasSameReferential = Referential == otherAnaPosition.Referential;
            bool hasSameX = X == otherAnaPosition.X;
            bool hasSameY = Y == otherAnaPosition.Y;
            bool hasSameZTop = ZTop == otherAnaPosition.ZTop;
            bool hasSameZBottom = ZBottom == otherAnaPosition.ZBottom;
            bool hasSamePiezoPositions = PiezoPositionsAreEqual(ZPiezoPositions, otherAnaPosition.ZPiezoPositions);

            return hasSameReferential && hasSameX && hasSameY && hasSameZTop && hasSameZBottom && hasSamePiezoPositions;
        }

        private bool PiezoPositionsAreEqual(List<ZPiezoPosition> lPositions, List<ZPiezoPosition> rPositions)
        {
            bool leftAreIncludedInRight = OneWayInclusion(lPositions, rPositions);
            bool rightAreIncludedInLeft = OneWayInclusion(rPositions, lPositions);

            return leftAreIncludedInRight && rightAreIncludedInLeft;
        }

        /// <summary>
        /// Determines if the left positions' list given in parameter is included in right one.
        /// </summary>
        private bool OneWayInclusion(List<ZPiezoPosition> lPositions, List<ZPiezoPosition> rPositions)
        {
            return lPositions.All(lPosition =>
            {
                var rPosition = rPositions.FirstOrDefault(p => p.AxisID == lPosition.AxisID);

                bool hasCorrespondingAxisId = !(rPosition is null);
                bool areEqualPositions = rPosition?.Equals(lPosition) ?? false;

                return hasCorrespondingAxisId && areEqualPositions;
            });
        }

        public static bool operator ==(AnaPosition lPosition, AnaPosition rPosition)
        {
            if (lPosition is null && rPosition is null)
            {
                return true;
            }

            if (lPosition is null || rPosition is null)
            {
                return false;
            }

            return lPosition.Equals(rPosition);
        }

        public static bool operator !=(AnaPosition lAxesPosition, AnaPosition rAxesPosition)
        {
            return !(lAxesPosition == rAxesPosition);
        }

        public override object Clone()
        {
            var clonePosition = new AnaPosition(Referential, X, Y, ZTop, ZBottom, new List<ZPiezoPosition>());
            foreach (var piezoPosition in ZPiezoPositions)
            {
                clonePosition.ZPiezoPositions.Add((ZPiezoPosition)piezoPosition.Clone());
            }
            return clonePosition;
        }

        public override string ToString()
        {
            var anaPositionAsString = new StringBuilder(base.ToString());
            anaPositionAsString.AppendLine($"\tZPiezoPositions =");

            foreach (var piezoPosition in ZPiezoPositions)
            {
                anaPositionAsString.AppendLine($"\t\tFor piezo axis {piezoPosition.AxisID}, Position = {piezoPosition.Position}");
            }

            return anaPositionAsString.ToString();
        }

        public override int GetHashCode()
        {
            int piezosHash = ZPiezoPositions.Aggregate(0, (total, next) => (total, next.GetHashCode()).GetHashCode());
            return (base.GetHashCode(), piezosHash).GetHashCode();
        }
        public override bool Near(PositionBase otherPosition, Length tolerance = null)
        {
            if (otherPosition is AnaPosition otherAnaPosition)
            {
                if (tolerance == null)
                {
                    tolerance = 0.001.Millimeters();
                }
                bool isNearX = X.Near(otherAnaPosition.X, tolerance.Millimeters);
                bool isNearY = Y.Near(otherAnaPosition.Y, tolerance.Millimeters);
                bool isNearZTop = ZTop.Near(otherAnaPosition.ZTop, tolerance.Millimeters);
                bool isNearZBottom = ZBottom.Near(otherAnaPosition.ZBottom, tolerance.Millimeters);

                bool isNearPiezoPositions = ZPiezoPositions != null && otherAnaPosition.ZPiezoPositions != null &&
                                            ZPiezoPositions.Count == otherAnaPosition.ZPiezoPositions.Count;

                if (isNearPiezoPositions)
                {
                    for (int i = 0; i < ZPiezoPositions.Count; i++)
                    {
                        if (!ZPiezoPositions[i].Near(otherAnaPosition.ZPiezoPositions[i], tolerance))
                        {
                            isNearPiezoPositions = false;
                            break;
                        }
                    }
                }
                else if (ZPiezoPositions != null || otherAnaPosition.ZPiezoPositions != null)
                {
                    isNearPiezoPositions = false;
                }

                return isNearX && isNearY && isNearZTop && isNearZBottom && isNearPiezoPositions;
            }
            return false;
        }
    }
}
