using System;
using System.Collections.Generic;

using UnitySC.PM.EME.Service.Core.Calibration;
using UnitySC.PM.EME.Service.Core.Referentials.Converters;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Service.Core.Referentials
{
    public class EmeReferentialManager : ReferentialManagerBase<XYZPosition>
    {
        private readonly CalibrationManager _calibrationManager;
        public EmeReferentialManager()
        {
            _calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
            _referentialConverters = new List<IReferentialConverter<XYZPosition>>
            {
                new MotorToStageConverter(_calibrationManager),
                new StageToMotorConverter(_calibrationManager),
                new StageToWaferConverter(null),
                new WaferToStageConverter(null),
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
                    return (GetConverter(ReferentialTag.Wafer, ReferentialTag.Stage) as WaferToStageConverter)?.Settings;
                case ReferentialTag.Stage:
                    return (GetConverter(ReferentialTag.Stage, ReferentialTag.Motor) as StageToMotorConverter)?.Settings;
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

                default:
                    throw new Exception($"Cannot delete {referentialTag} settings into referential manager.");
            }
        }
    }
}
