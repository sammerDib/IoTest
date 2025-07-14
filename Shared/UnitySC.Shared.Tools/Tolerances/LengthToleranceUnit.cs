using System;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Tools.Tolerances
{
    public enum LengthToleranceUnit
    {
        Percentage,
        Micrometer,
        Nanometer,
        Millimeter
    }

    public static class LengthToleranceUnitExtensions
    {
        public static LengthUnit ToLengthUnit(this LengthToleranceUnit toleranceUnit)
        {
            switch (toleranceUnit)
            {
                case LengthToleranceUnit.Percentage:
                    return LengthUnit.Undefined;

                case LengthToleranceUnit.Millimeter:
                    return LengthUnit.Millimeter;

                case LengthToleranceUnit.Micrometer:
                    return LengthUnit.Micrometer;

                case LengthToleranceUnit.Nanometer:
                    return LengthUnit.Nanometer;

                default: throw new ArgumentOutOfRangeException(nameof(toleranceUnit), toleranceUnit, null);
            }
        }
    }
}
