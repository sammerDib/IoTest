using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface
{
    [DataContract]
    public class LiseHFInputParams : ILiseHFInputParams
    {
        public LiseHFInputParams()
        {
        }

        #region Properties

        [DataMember]
        public Length DepthTarget { get; set; }

        [DataMember]
        public LengthTolerance DepthTolerance { get; set; }

        [DataMember]
        public string ObjectiveId { get; set; }

        [DataMember]
        public int NbMeasuresAverage { get; set; }

        [DataMember]
        public bool IsLowIlluminationPower { get; set; }

        [DataMember]
        public double IntegrationTimems { get; set; } // from int to double [02/09/2024]
        
        [DataMember]
        public double IntensityFactor { get; set; }

        [DataMember]
        public double Threshold { get; set; }

        [DataMember]
        public double ThresholdPeak { get; set; }

        [DataMember]
        public CalibrationFrequency CalibrationFreq { get; set; }

        [DataMember]
        public string ReportPath { get; set; }

        [DataMember]
        public bool ReportSignals { get; set; }

        [DataMember]
        public bool ReportOutputs { get; set; }

        [DataMember]
        public List<LayerWithToleranceSettings> PhysicalLayers { get; set; }

        [DataMember]
        public bool SaveFFTSignal { get; set; }

        #endregion Properties
    }

    public static class LiseHFInputParamsFactory
    {
        public static LiseHFInputParams FromLiseHFSettings(LiseHFSettings liseHFsettings)
        {
            return new LiseHFInputParams()
            {
                NbMeasuresAverage = liseHFsettings.NbMeasuresAverage,
                IsLowIlluminationPower = liseHFsettings.IsLowIlluminationPower,
                IntensityFactor = liseHFsettings.IntensityFactor,
                Threshold = liseHFsettings.Threshold,
                ThresholdPeak = liseHFsettings.ThresholdPeak,
                SaveFFTSignal = liseHFsettings.SaveFFTSignal,
                CalibrationFreq = liseHFsettings.CalibrationFreq,
                ObjectiveId = liseHFsettings.ProbeObjectiveContext.ObjectiveId,
                PhysicalLayers = liseHFsettings.Layers,
            };
        }
    }
}
