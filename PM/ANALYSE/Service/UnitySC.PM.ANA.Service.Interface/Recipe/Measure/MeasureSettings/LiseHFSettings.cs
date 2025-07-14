using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings
{
    public class LiseHFSettings : ProbeWithObjectiveContextSettings
    {
        [DataMember]
        public bool IsLowIlluminationPower { get; set; }

        [DataMember]
        public double IntegrationTimems { get; set; } // move from int to double [02/09/2024]

        // comment géré ici le changement de version dans les fichiers serializé ?
        [DataMember]
        public double IntensityFactor { get; set; } = double.NaN;  // [0.01, Inf[

        [DataMember]
        public int NbMeasuresAverage { get; set; }

        [DataMember]
        public double Threshold { get; set; }

        [DataMember]
        public double ThresholdPeak { get; set; }

        [DataMember]
        public bool SaveFFTSignal { get; set; }

        [DataMember]
        public CalibrationFrequency CalibrationFreq { get; set; }

        [DataMember]
        public List<LayerWithToleranceSettings> Layers { get; set; }

        public override bool Equals(object obj)
        {
             return obj is LiseHFSettings settings &&
                   ProbeId == settings.ProbeId &&
                   IsLowIlluminationPower == settings.IsLowIlluminationPower &&
                   IntegrationTimems == settings.IntegrationTimems &&
                   IntensityFactor == settings.IntensityFactor &&
                   NbMeasuresAverage == settings.NbMeasuresAverage &&
                   Threshold == settings.Threshold &&
                   ThresholdPeak ==  settings.ThresholdPeak &&
                   SaveFFTSignal == settings.SaveFFTSignal &&
                   CalibrationFreq == settings.CalibrationFreq &&
                   EqualityComparer<ObjectiveContext>.Default.Equals(ProbeObjectiveContext, settings.ProbeObjectiveContext)&&
                   Layers.SequenceEqual(settings.Layers);
        }

        public override int GetHashCode()
        {
            // dois t'on inclure ici ThresholdPeak & SaveFFTSignal
            return (ProbeId, IsLowIlluminationPower, IntegrationTimems, IntensityFactor, NbMeasuresAverage, Threshold, CalibrationFreq, ProbeObjectiveContext, Layers, ThresholdPeak, SaveFFTSignal).GetHashCode();
        }

        public override ProbeSettings Clone()
        {
            var settings = new LiseHFSettings();
            settings.ProbeId = ProbeId;
            settings.IsLowIlluminationPower = IsLowIlluminationPower;
            settings.IntegrationTimems = IntegrationTimems;
            settings.IntensityFactor= IntensityFactor;
            settings.NbMeasuresAverage = NbMeasuresAverage;
            settings.Threshold = Threshold;
            settings.ThresholdPeak = ThresholdPeak;
            settings.SaveFFTSignal = SaveFFTSignal;
            settings.CalibrationFreq = CalibrationFreq;
            settings.ProbeObjectiveContext = ProbeObjectiveContext;
            settings.Layers = new List<LayerWithToleranceSettings>();
            foreach (var layer in Layers)
            {
                settings.Layers.Add(layer.Clone());
            }
            return settings;
        }
    }
}
