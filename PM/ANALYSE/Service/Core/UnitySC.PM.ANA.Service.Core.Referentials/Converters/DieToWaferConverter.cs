using System;

using UnitySC.PM.ANA.Service.Interface.Referential;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.Referentials.Converters
{
    public class DieToWaferConverter : IReferentialConverter<XYZTopZBottomPosition>
    {
        private DieReferentialSettings _dieReferentialSettings;
        private ILogger _logger;

        public bool IsEnabled { get; set; } = true;

        public DieToWaferConverter(DieReferentialSettings settings)
        {
            _logger = ClassLocator.Default.GetInstance<ILogger>();
            _dieReferentialSettings = settings;
            _logger.Debug($"DieToWaferConverter Constructor : {settings?.ToString()??"null"}");
        }

        public XYZTopZBottomPosition Convert(XYZTopZBottomPosition xyzPosition)
        {
            XYZTopZBottomPosition position = xyzPosition.Clone() as XYZTopZBottomPosition;

            if (IsEnabled && xyzPosition.Referential is DieReferential dieReferential)
            {
                if (_dieReferentialSettings is null)
                {
                    throw new Exception($"Die referential settings must be provided to convert from die to wafer referential.");
                }

                position=ReferentialConvertersHelper.ConvertDiePositionToWafer(xyzPosition, dieReferential, _dieReferentialSettings);

                //System.Diagnostics.Debug.WriteLine($"Die To Wafer : {dieReferential.DieLine} | {dieReferential.DieColumn}) -- [{xyzPosition.X} {xyzPosition.Y}] => [{position.X} {position.Y}]");

            }

            return position;
        }

      
        public bool Accept(ReferentialTag from, ReferentialTag to)
        {
            return from == ReferentialTag.Die && to == ReferentialTag.Wafer;
        }

        public void UpdateSettings(DieReferentialSettings settings)
        {
            _logger.Debug($"DieToWaferConverter UpdateSettings : {settings?.ToString() ?? "null"}");
            _dieReferentialSettings = settings;
        }

        public DieReferentialSettings GetSettings()
        {
            return _dieReferentialSettings;
        }
    }
}
