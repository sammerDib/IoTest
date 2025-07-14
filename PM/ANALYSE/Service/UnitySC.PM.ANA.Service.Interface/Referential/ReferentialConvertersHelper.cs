using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Interface.Referential
{
    public static class ReferentialConvertersHelper
    {

        public static XYZTopZBottomPosition ConvertDiePositionToWafer(XYZTopZBottomPosition position, DieReferential dieReferential, DieReferentialSettings dieReferentialSettings)
        {
            var dieWidth = dieReferentialSettings.DieGridDimensions.DieWidth;
            var dieHeight = dieReferentialSettings.DieGridDimensions.DieHeight;
            var streetWidth = dieReferentialSettings.DieGridDimensions.StreetWidth;
            var streetHeight = dieReferentialSettings.DieGridDimensions.StreetHeight;
            var dieGridAngle = dieReferentialSettings.DieGridAngle;
            var dieGridTopLeft = dieReferentialSettings.DieGridTopLeft;

            var dieOriginPosition = new XYZTopZBottomPosition(new WaferReferential(), dieGridTopLeft.X, dieGridTopLeft.Y, 0, 0);
            dieOriginPosition.X += dieReferential.DieColumn * (dieWidth.Millimeters + streetWidth.Millimeters);
            dieOriginPosition.Y -= dieReferential.DieLine * (dieHeight.Millimeters + streetHeight.Millimeters) + dieHeight.Millimeters;
            MathTools.ApplyAntiClockwiseRotation(dieGridAngle, dieOriginPosition, dieGridTopLeft);

            XYZTopZBottomPosition waferPosition = position.Clone() as XYZTopZBottomPosition;
            waferPosition.Referential = new WaferReferential();
            waferPosition.X = dieOriginPosition.X + position.X;
            waferPosition.Y = dieOriginPosition.Y + position.Y;

            return waferPosition;
        }
    }
}
