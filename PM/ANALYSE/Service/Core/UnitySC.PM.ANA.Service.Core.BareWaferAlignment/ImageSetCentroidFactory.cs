using System.Collections.Generic;

using UnitySCSharedAlgosOpenCVWrapper;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Units;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.PM.ANA.Hardware;

namespace UnitySC.PM.ANA.Service.Core.BareWaferAlignment
{
    /// <summary>
    ///  Given a Wafer type, this factory will create a list of image centroids suited for alignment detection.
    /// </summary>
    public class ImageSetCentroidFactory
    {
        public static List<BareWaferAlignmentImageData> GetImageDataListFor(WaferDimensionalCharacteristic waferType)
        {

            var chuckCenterOffset = GetChuckCenterPosition(waferType.Diameter);

            if (waferType.WaferShape == WaferShape.Notch)
            {
                return GetNotchImageDatas(waferType, chuckCenterOffset);
            }
            else
            {
                throw new UnsupportedWaferException($"Wafer of type {WaferShape.Notch.ToString()} and diameter {waferType.Diameter} is not yet supported.");
            }
        }

        private static XYPosition GetChuckCenterPosition(Length waferDiameter)
        {

            var hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            var slotConfig = hardwareManager?.Chuck?.Configuration.GetSubstrateSlotConfigByWafer(waferDiameter);
            return slotConfig?.PositionChuckCenter?.ToXYPosition() ?? new XYPosition(new StageReferential(), 0.0, 0.0);
        }


        private static List<BareWaferAlignmentImageData> GetNotchImageDatas(WaferDimensionalCharacteristic waferType, XYPosition chuckCenterOffset)
        {
            double waferRadius_um = waferType.Diameter.Micrometers / 2.0;
            
            double zeroWaferInStageX_um = chuckCenterOffset.X * 1000.0; 
            double zeroWaferInStageY_um = chuckCenterOffset.Y * 1000.0;

            var notchImage = new BareWaferAlignmentImageData
            {
                Centroid = { X = zeroWaferInStageX_um, Y = -waferRadius_um + zeroWaferInStageY_um },
                ExpectedShape = WaferEdgeShape.NOTCH,
                EdgePosition = EdgePosition.BOTTOM,
                StitchColumns = 2,
                StitchRows = 2
            };

            var topImage =
                new BareWaferAlignmentImageData
                {
                    Centroid = { X = zeroWaferInStageX_um, Y = waferRadius_um + zeroWaferInStageY_um },
                    ExpectedShape = WaferEdgeShape.EDGE,
                    EdgePosition = EdgePosition.TOP
                };

            var rightImage =
                new BareWaferAlignmentImageData
                {
                    Centroid = { X = waferRadius_um + zeroWaferInStageX_um, Y = zeroWaferInStageY_um },
                    ExpectedShape = WaferEdgeShape.EDGE,
                    EdgePosition = EdgePosition.RIGHT
                };

            var leftImage =
                new BareWaferAlignmentImageData
                {
                    Centroid = { X = -waferRadius_um + zeroWaferInStageX_um, Y = zeroWaferInStageY_um },
                    ExpectedShape = WaferEdgeShape.EDGE,
                    EdgePosition = EdgePosition.LEFT,
                };

            return new List<BareWaferAlignmentImageData>(4) { leftImage, topImage, rightImage, notchImage };
        }
    }
}
