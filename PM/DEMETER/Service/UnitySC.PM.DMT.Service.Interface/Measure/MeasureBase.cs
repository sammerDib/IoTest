using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.PM.DMT.Service.Interface.AutoExposure;
using UnitySC.PM.DMT.Shared;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.Enum.Module;

namespace UnitySC.PM.DMT.Service.Interface.Measure
{
    [DataContract]
    [KnownType(typeof(BrightFieldMeasure))]
    [KnownType(typeof(DeflectometryMeasure))]
    [KnownType(typeof(HighAngleDarkFieldMeasure))]
    [KnownType(typeof(BackLightMeasure))]
    public abstract class MeasureBase
    {
        /// <summary>
        ///     La ROI servant pour le calcul automatique de l'exposure time
        /// </summary>
        [DataMember] public ROI ROI = new ROI();

        public abstract string MeasureName { get; }

        public abstract MeasureType MeasureType { get; }

        [DataMember] public Side Side { get; set; }

        /// <summary>
        ///     True if the execution of this measure is required
        /// </summary>
        [DataMember]
        public bool IsEnabled { get; set; }

        /// <summary>
        ///     Define when the exposure time trigger is set for a lot
        /// </summary>
        [DataMember]
        public AutoExposureTimeTrigger AutoExposureTimeTrigger { get; set; }

        /// <summary>
        ///     Exposure time during acquisition.
        ///     This is an abstract value, that must will be corrected using the calibration to get the actual value in millisecond.
        /// </summary>
        [DataMember]
        public double ExposureTimeMs { get; set; } = 80;

        // Gray level target for auto exposure
        [DataMember] public int AutoExposureTargetSaturation { get; set; } = 220;

        // Gray level tolerance for the auto exposure target
        [DataMember] public int AutoExposureSaturationTolerance { get; set; } = 10;

        // Initial Exposure Time for auto exposure
        [DataMember] public double AutoExposureInitialExposureTimeMs { get; set; } = 20;

        // Ratio of pixels above the AutoExposureTargetSaturation
        [DataMember] public double AutoExposureRatioSaturated { get; set; } = 30;

        public abstract List<ResultType> GetOutputTypes();

        protected void SafeAddOutputTo(List<ResultType> outputs, DMTResult dmtResType)
        {
            try
            {
                var resType = dmtResType.ToResultType(); // throw ArgumentException if cannot convert
                outputs.Add(resType);
            }
            catch (ArgumentException argex)
            {
                // this result should not be sent to adc or save in database
                // skip adding in outputs 
            }
        }

    }
}
