using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Tools;

namespace UnitySC.PM.EME.Service.Core.Referentials.Converters
{
    public class WaferToStageConverter : IReferentialConverter<XYZPosition>
    {
        public WaferReferentialSettings Settings { get; private set; }
        public bool IsEnabled { get; set; } = true;

        public WaferToStageConverter(WaferReferentialSettings settings)
        {
            Settings = settings;
        }

        public XYZPosition Convert(XYZPosition xyzPosition)
        {
            var position = xyzPosition.Clone() as XYZPosition;

            if (IsEnabled && xyzPosition.Referential.Tag == ReferentialTag.Wafer && Settings != null)
            {
                MathTools.ApplyAntiClockwiseRotation(Settings.WaferAngle, position, new XYPosition(position.Referential, 0, 0));

                position.X += Settings.ShiftX.Millimeters;
                position.Y += Settings.ShiftY.Millimeters;
            }

            position.Referential = new StageReferential();
            return position;
        }
        public bool Accept(ReferentialTag from, ReferentialTag to)
        {
            return from == ReferentialTag.Wafer && to == ReferentialTag.Stage;
        }
        public void UpdateSettings(WaferReferentialSettings settings)
        {
            Settings = settings;
        }
    }
}
