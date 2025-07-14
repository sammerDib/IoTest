using System;
using System.Collections.Generic;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Referentials.Interface.Positions
{
    public class AxisPositionsList : PositionBase
    {
        public override object Clone()
        {
            return new AxisPositionsList(PositionsList);
        }

        public AxisPositionsList()
        {
            PositionsList = new List<PositionItem>();
        }

        private AxisPositionsList(List<PositionItem> itemList)
        {
            PositionsList = itemList;
        }

        public List<PositionItem> PositionsList;

        public void Add(PositionItem positionItem)
        {
            PositionsList.Add(positionItem);
        }

        public List<PositionItem> GetPositionItems()
        {
            return PositionsList;
        }
        public override bool Near(PositionBase otherPosition, Length tolerance = null)
        {
            if (otherPosition is AxisPositionsList otherList)
            {
                if (tolerance == null)
                {
                    tolerance = 0.001.Millimeters();
                }
                if (PositionsList.Count != otherList.PositionsList.Count)
                {
                    return false;
                }

                for (int i = 0; i < PositionsList.Count; i++)
                {
                    if (!PositionsList[i].Near(otherList.PositionsList[i], tolerance))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }

    public class LinearAxisPositionItem : PositionItem
    {
        private string _axisId;
        private Length _position;

        public LinearAxisPositionItem(string axId, Length length)
        {
            _axisId = axId;
            _position = length;
        }
        public override bool Near(PositionItem other, Length tolerance = null)
        {
            if (other is LinearAxisPositionItem otherLinear)
            {
                if (tolerance == null)
                {
                    tolerance = 0.001.Millimeters();
                }
                return _axisId == otherLinear._axisId && _position.Near(otherLinear._position, tolerance);
            }
            return false;
        }
    }

    public class RotationalAxisPositionItem : PositionItem
    {
        private string _axisId;
        private Angle _position;

        public RotationalAxisPositionItem(string axId, Angle angle)
        {
            _axisId = axId;
            _position = angle;
        }
        public override bool Near(PositionItem other, Length tolerance)
        {
            if (other is RotationalAxisPositionItem otherRotational)
            {
                if (tolerance == null)
                {
                    tolerance = 0.001.Millimeters();
                }
                double angleToleranceInDegrees = tolerance.GetValueAs(LengthUnit.Meter) * 360.0; // Exemple de conversion
                return _axisId == otherRotational._axisId && Math.Abs(_position.Degrees - otherRotational._position.Degrees) <= angleToleranceInDegrees;
            }

            return false;
        }
    }

    public abstract class PositionItem
    {
        public abstract bool Near(PositionItem other, Length tolerance = null);
    }
}
