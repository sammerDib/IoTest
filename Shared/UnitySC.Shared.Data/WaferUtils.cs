using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.Geometry;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Data
{
    public static class WaferUtils
    {
        public static IWaferShape CreateWaferShapeFromWaferCharacteristics(WaferDimensionalCharacteristic waferCharacteristics, Length edgeExclusion)
        {
            var waferCenter = new PointUnits(0.Millimeters(), 0.Millimeters());

            switch (waferCharacteristics.WaferShape)
            {
                case WaferShape.Sample:
                    {
                        return new SampleWafer(waferCenter, waferCharacteristics.SampleHeight, waferCharacteristics.SampleWidth, edgeExclusion);
                    }

                case WaferShape.Flat:
                    {
                        return new WaferWithFlats(waferCenter, waferCharacteristics.Diameter / 2, edgeExclusion, waferCharacteristics.Flats);
                    }

                case WaferShape.Notch:
                    {
                        return new WaferWithNotch(waferCenter, waferCharacteristics.Diameter / 2, edgeExclusion, waferCharacteristics.Notch);
                    }

                default:
                    {
                        return new CircularWafer(waferCenter, waferCharacteristics.Diameter / 2, edgeExclusion);
                    }
            }
        }
    }
}
