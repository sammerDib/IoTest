using System;
using System.Collections.Generic;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Referentials.Converters;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.Referentials
{
    public class AnaReferentialManager : ReferentialManagerBase<XYZTopZBottomPosition>
    {                
        private readonly AnaHardwareManager _hardwareManager;
        private readonly CalibrationManager _calibrationManager;        

        public AnaReferentialManager() : base()
        {            
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();

            _referentialConverters = new List<IReferentialConverter<XYZTopZBottomPosition>>()
            {
                new MotorToStageConverter(_hardwareManager, _calibrationManager),
                new StageToMotorConverter(_hardwareManager, _calibrationManager),
                new StageToWaferConverter(_hardwareManager, _calibrationManager, null),
                new WaferToStageConverter(_hardwareManager, _calibrationManager, null),
                new DieToWaferConverter(null),
                new WaferToDieConverter(null)
            };
        }
        
        /// <summary>
        /// Get referential settings used by converter
        /// </summary>
        public override ReferentialSettingsBase GetSettings(ReferentialTag referentialTag)
        {            
            switch (referentialTag)
            {
                case ReferentialTag.Wafer:
                    return (GetConverter(ReferentialTag.Wafer, ReferentialTag.Stage) as WaferToStageConverter)?.GetSettings();

                case ReferentialTag.Die:
                    return (GetConverter(ReferentialTag.Die, ReferentialTag.Wafer) as DieToWaferConverter)?.GetSettings();

                case ReferentialTag.Stage:
                    return (GetConverter(ReferentialTag.Stage, ReferentialTag.Motor) as StageToMotorConverter)?.GetSettings();

                default:
                    throw new Exception($"Cannot get {referentialTag} settings into referential manager.");
            }
        }

        /// <summary>
        /// Set referential settings used by converter
        /// </summary>
        public override void SetSettings(ReferentialSettingsBase settings)
        {
            if (settings == null)
            {
                return;
            }
            switch (settings.Tag)
            {
                case ReferentialTag.Wafer:
                    (GetConverter(ReferentialTag.Wafer, ReferentialTag.Stage) as WaferToStageConverter).UpdateSettings(settings as WaferReferentialSettings);
                    (GetConverter(ReferentialTag.Stage, ReferentialTag.Wafer) as StageToWaferConverter).UpdateSettings(settings as WaferReferentialSettings);
                    break;

                case ReferentialTag.Die:
                    _logger.Debug($"ReferentialManager : SetSettings {settings.Tag}, {settings.GetType()}");
                    (GetConverter(ReferentialTag.Die, ReferentialTag.Wafer) as DieToWaferConverter).UpdateSettings(settings as DieReferentialSettings);
                    (GetConverter(ReferentialTag.Wafer, ReferentialTag.Die) as WaferToDieConverter).UpdateSettings(settings as DieReferentialSettings);
                    break;

                case ReferentialTag.Stage:
                    _logger.Debug($"ReferentialManager : SetSettings {settings.Tag}, {settings.GetType()}");
                    (GetConverter(ReferentialTag.Motor, ReferentialTag.Stage) as MotorToStageConverter).UpdateSettings(settings as StageReferentialSettings);
                    (GetConverter(ReferentialTag.Stage, ReferentialTag.Motor) as StageToMotorConverter).UpdateSettings(settings as StageReferentialSettings);
                    break;

                default:
                    throw new Exception($"Cannot set {settings.Tag} settings into referential manager.");
            }
        }

        /// <summary>
        /// Delete referential settings used by converter
        /// </summary>
        public override void DeleteSettings(ReferentialTag referentialTag)
        {
            _logger.Debug($"ReferentialManager : DeleteSettings {referentialTag}");

            switch (referentialTag)
            {
                case ReferentialTag.Wafer:
                    (GetConverter(ReferentialTag.Wafer, ReferentialTag.Stage) as WaferToStageConverter).UpdateSettings(null);
                    (GetConverter(ReferentialTag.Stage, ReferentialTag.Wafer) as StageToWaferConverter).UpdateSettings(null);
                    break;

                case ReferentialTag.Die:
                    (GetConverter(ReferentialTag.Die, ReferentialTag.Wafer) as DieToWaferConverter).UpdateSettings(null);
                    (GetConverter(ReferentialTag.Wafer, ReferentialTag.Die) as WaferToDieConverter).UpdateSettings(null);
                    break;

                default:
                    throw new Exception($"Cannot delete {referentialTag} settings into referential manager.");
            }
        }       
    }
}
