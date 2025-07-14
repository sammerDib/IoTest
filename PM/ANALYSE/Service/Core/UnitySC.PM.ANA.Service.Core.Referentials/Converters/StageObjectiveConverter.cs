using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Core.Referentials.Converters
{
    public class StageObjectiveConverter : IReferentialConverter
    {
        public ReferentialTag ReferentialTag1 => ReferentialTag.Stage;

        public ReferentialTag ReferentialTag2 => ReferentialTag.Objective;

        public bool IsEnabled { get; set; } = true;

        public PositionBase Convert(PositionBase positionBase, ReferentialBase referentialTo)
        {
            XYZTopZBottomPosition position = positionBase.Clone() as XYZTopZBottomPosition;
            if (position != null && IsEnabled)
            {
                if (positionBase.Referential.Tag == ReferentialTag1 && referentialTo.Tag == ReferentialTag2)
                {
                    StageToObjective(position);
                }
                else if (positionBase.Referential.Tag == ReferentialTag2 && referentialTo.Tag == ReferentialTag1)
                {
                    StageToWafer(position);
                }
                else
                {
                    throw new InvalidOperationException("Bad referential in WaferStage converter");
                }
            }
            position.Referential = referentialTo;
            return position;
        }

        private void StageToWafer(XYZTopZBottomPosition position, WaferReferentialSettings waferReferentialSettings)
        {
            ApplyRotation(-waferReferentialSettings.WaferAngle, position);
            position.X -= (waferReferentialSettings.ShiftX.Millimeters);
            position.Y -= (waferReferentialSettings.ShiftY.Millimeters);
            var currentObjectiveCalibUp = GetObjectiveCalibration(ModulePositions.Up);
            if (!(waferReferentialSettings.ZTopFocus is null) && !(currentObjectiveCalibUp is null))
                position.ZTop -= (waferReferentialSettings.ZTopFocus - currentObjectiveCalibUp.ZOffsetWithMainObjective).Millimeters;
            var currentObjectiveCalibDown = GetObjectiveCalibration(ModulePositions.Down);
            if (!(currentObjectiveCalibDown?.AutoFocus is null)) // Bottom focus never change
                position.ZBottom -= currentObjectiveCalibDown.AutoFocus.ZFocusPosition.Millimeters;
        }

        private void WaferToStage(XYZTopZBottomPosition position, WaferReferentialSettings waferReferentialSettings)
        {
            position.X += (waferReferentialSettings.ShiftX.Millimeters);
            position.Y += (waferReferentialSettings.ShiftY.Millimeters);
            ApplyRotation(waferReferentialSettings.WaferAngle, position);
            var currentObjectiveCalibUp = GetObjectiveCalibration(ModulePositions.Up);
            if (!(waferReferentialSettings.ZTopFocus is null) && !(currentObjectiveCalibUp is null))
                position.ZTop += (waferReferentialSettings.ZTopFocus - currentObjectiveCalibUp.ZOffsetWithMainObjective).Millimeters;
            var currentObjectiveCalibDown = GetObjectiveCalibration(ModulePositions.Down);
            if (!(currentObjectiveCalibDown?.AutoFocus is null)) // Bottom focus never change
                position.ZBottom += currentObjectiveCalibDown.AutoFocus.ZFocusPosition.Millimeters;
        }
    }
}
