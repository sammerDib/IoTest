using System;

using UnitySCSharedAlgosOpenCVWrapper;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.ANA.Service.Core.BareWaferAlignment
{
    /// <summary>
    ///
    /// Contains conversion methods used for data types that exist in C# and C++
    ///
    /// </summary>
    public static class TypeConverter
    {
        public static WaferEdgePositions ConvertEdgePosition(EdgePosition algoEdgePosition)
        {
            switch (algoEdgePosition)
            {
                case EdgePosition.LEFT:
                    return WaferEdgePositions.Left;

                case EdgePosition.BOTTOM:
                    return WaferEdgePositions.Bottom;

                case EdgePosition.RIGHT:
                    return WaferEdgePositions.Right;

                case EdgePosition.TOP:
                    return WaferEdgePositions.Top;
            }

            throw new ArgumentException($"Unknown edge position " + algoEdgePosition);
        }

        public static WaferType ConvertWaferShape(WaferShape shape)
        {
            switch (shape)
            {
                case WaferShape.Notch:
                    return WaferType.NOTCH;

                case WaferShape.NonFlat:
                    throw new UnsupportedWaferException("Wafer of shape " + shape + " is not yet supported.");

                case WaferShape.Flat:
                    throw new UnsupportedWaferException("Wafer of shape " + shape + " is not yet supported.");

                case WaferShape.Sample:
                    throw new UnsupportedWaferException("Wafer of shape " + shape + " is not yet supported.");

                default:
                    throw new UnsupportedWaferException("Wafer of shape " + shape + " is not yet supported.");
            }
        }
    }
}
