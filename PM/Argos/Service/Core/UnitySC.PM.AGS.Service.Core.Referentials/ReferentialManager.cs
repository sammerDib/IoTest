using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;


namespace UnitySC.PM.AGS.Service.Core.Referentials
{
    public class ReferentialManager : IReferentialManager
    {
        private readonly ILogger<ReferentialManager> _logger;
        private readonly HardwareManager _hardwareManager;
        private List<IReferentialConverter<ArgosPosition>> _referentialConverters;

        public ReferentialManager()
        {
            _logger = ClassLocator.Default.GetInstance<ILogger<ReferentialManager>>();
            _hardwareManager = ClassLocator.Default.GetInstance<HardwareManager>();

            _referentialConverters = new List<IReferentialConverter<ArgosPosition>>()
            {
            };
        }

        public PositionBase ConvertTo(PositionBase positionToConvert, ReferentialTag referentialTo)
        {
            return positionToConvert;
        }

        public void SetSettings(ReferentialSettingsBase settings)
        {
            _logger.Error("No implementation of settings in Argos Referential manager");
        }

        public ReferentialSettingsBase GetSettings(ReferentialTag referentialTag)
        {
            _logger.Error("No implementation of settings in Argos Referential manager");
            return null;
        }

        public void DeleteSettings(ReferentialTag referentialTag)
        {
            _logger.Error("No implementation of settings in Argos Referential manager");
        }
        private IReferentialConverter<ArgosPosition> GetConverter(ReferentialTag from, ReferentialTag to)
        {
            foreach (var referential in _referentialConverters)
            {
                if (referential.Accept(from, to))
                {
                    return referential;
                }
            }

            _logger.Error($"No converter exist between {from} and {to}");
            throw new InvalidOperationException($"No converter path exist between {from} and {to}");
        }

        public void DisableReferentialConverter(ReferentialTag from, ReferentialTag to)
        {
            GetConverter(from, to).IsEnabled = false;
        }

        public void EnableReferentialConverter(ReferentialTag from, ReferentialTag to)
        {
            GetConverter(from, to).IsEnabled = true;
        }
    }
}
