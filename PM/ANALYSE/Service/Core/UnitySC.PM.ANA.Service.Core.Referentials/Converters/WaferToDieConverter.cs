using System;

using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Tools;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.Referentials.Converters
{
    public class WaferToDieConverter : IReferentialConverter<XYZTopZBottomPosition>
    {
        private DieReferentialSettings _dieReferentialSettings;
        private ILogger _logger;
        public bool IsEnabled { get; set; } = true;

        public WaferToDieConverter(DieReferentialSettings settings)
        {
            _logger = ClassLocator.Default.GetInstance<ILogger>();
            _dieReferentialSettings = settings;
            _logger.Debug($"WaferToDieConverter Constructor : {settings?.ToString() ?? "null"}");
        }

        public XYZTopZBottomPosition Convert(XYZTopZBottomPosition xyzPosition)
        {
            var position = xyzPosition.Clone() as XYZTopZBottomPosition;

            if (IsEnabled && xyzPosition.Referential.Tag == ReferentialTag.Wafer)
            {
                if (_dieReferentialSettings is null)
                {
                    throw new Exception($"Die referential settings must provided to convert from wafer to die referential.");
                }

                var dieWidth = _dieReferentialSettings.DieGridDimensions.DieWidth;
                var dieHeight = _dieReferentialSettings.DieGridDimensions.DieHeight;
                var streetWidth = _dieReferentialSettings.DieGridDimensions.StreetWidth;
                var streetHeight = _dieReferentialSettings.DieGridDimensions.StreetHeight;
                var dieGridAngle = _dieReferentialSettings.DieGridAngle;
                var dieGridTopLeft = _dieReferentialSettings.DieGridTopLeft;

                var positionWithoutAngle = position.Clone() as XYZTopZBottomPosition;
                MathTools.ApplyAntiClockwiseRotation(-dieGridAngle, positionWithoutAngle, dieGridTopLeft);

                //0.001 (tiny distance) allows you to find the right die index when you are in the die position (0,0)
                var distFromTopLeftDieGridX = Math.Abs(dieGridTopLeft.X - positionWithoutAngle.X) + 0.001;
                var distFromTopLeftDieGridY = Math.Abs(dieGridTopLeft.Y - positionWithoutAngle.Y) - 0.001;
                int dieColumnId = (int)Math.Truncate((distFromTopLeftDieGridX + (streetWidth.Millimeters / 2)) / (dieWidth.Millimeters + streetWidth.Millimeters));
                int dieRowId = (int)Math.Truncate((distFromTopLeftDieGridY + (streetHeight.Millimeters / 2)) / (dieHeight.Millimeters + streetHeight.Millimeters));

                var dieOriginPosition = new XYZTopZBottomPosition(new WaferReferential(), dieGridTopLeft.X, dieGridTopLeft.Y, 0, 0);
                dieOriginPosition.X += dieColumnId * (dieWidth.Millimeters + streetWidth.Millimeters);
                dieOriginPosition.Y -= dieHeight.Millimeters + dieRowId * (dieHeight.Millimeters + streetHeight.Millimeters);
                MathTools.ApplyAntiClockwiseRotation(dieGridAngle, dieOriginPosition, dieGridTopLeft);

                position.Referential = new DieReferential(column: dieColumnId, line: dieRowId);
                position.X = position.X - dieOriginPosition.X;
                position.Y = position.Y - dieOriginPosition.Y;

                //System.Diagnostics.Debug.WriteLine($"Wafer To Die : [{xyzPosition.X} {xyzPosition.Y}] => [{position.X} {position.Y}] -- ({dieColumnId} | {dieRowId})");
            }
            return position;
        }

        public bool Accept(ReferentialTag from, ReferentialTag to)
        {
            return from == ReferentialTag.Wafer && to == ReferentialTag.Die;
        }

        public void UpdateSettings(DieReferentialSettings settings)
        {
            _logger.Debug($"WaferToDieConverter UpdateSettings : {settings?.ToString() ?? "null"}");
            _dieReferentialSettings = settings;
        }
    }
}
