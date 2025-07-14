using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Tools;

namespace UnitySC.PM.EME.Service.Core.Referentials.Converters
{
    public class StageToWaferConverter : IReferentialConverter<XYZPosition>
    {
        private WaferReferentialSettings Settings { get; set; }
        public bool IsEnabled { get; set; } = true;

        public StageToWaferConverter(WaferReferentialSettings settings)
        {
            Settings = settings;
        }

        public XYZPosition Convert(XYZPosition xyzPosition)
        {
            var position = xyzPosition.Clone() as XYZPosition;

            if (IsEnabled && Settings != null && position.Referential.Tag == ReferentialTag.Stage)
            {
                position.X -= Settings.ShiftX.Millimeters;
                position.Y -= Settings.ShiftY.Millimeters;
                MathTools.ApplyAntiClockwiseRotation(-Settings.WaferAngle, position, new XYPosition(position.Referential, 0, 0));
            }

            position.Referential = new WaferReferential();
            return position;
        }
        public bool Accept(ReferentialTag from, ReferentialTag to)
        {
            return from == ReferentialTag.Stage && to == ReferentialTag.Wafer;
        }
        public void UpdateSettings(WaferReferentialSettings settings)
        {
            Settings = settings;
        }
    }
}
