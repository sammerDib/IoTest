using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface
{
    [DataContract]
    public class ProbeLiseConfig : ProbeSingleConfigBase, IProbeLiseConfig
    {
        public ProbeLiseConfig()
        {
        }

        [DataMember]
        public int ConfigLise1 { get; set; }

        [DataMember]
        public int ConfigLise2 { get; set; }

        [DataMember]
        public string DeviceType { get; set; }

        [DataMember]
        public string SerialNumber { get; set; }

        [DataMember]
        public float ProbeRange { get; set; } //(µm)

        [DataMember]
        public float MinimumGain { get; set; }

        [DataMember]
        public float MaximumGain { get; set; }

        [DataMember]
        public float GainStep { get; set; }

        [DataMember]
        public float AutoGainStep { get; set; }

        [DataMember]
        public int Frequency { get; set; } //(Hz)

        [DataMember]
        public double CalibWavelength { get; set; } //calibrated wavelength of reference laser minus 1530nm (nm).

        [DataMember]
        public Length DiscriminationDistanceInTheAir { get; set; } //distance between two interfaces (peak on signal) in the air below which we lose precision on the location of these interfaces (peak on signal)

        [DataMember]
        public float SaturationValue { get; set; } //threshold value from which the amplitude of the peaks on the signal saturates (flattening of the top of the peaks)

        [DataMember]
        public Length ComparisonTol { get; set; } //in µm tolerance between peak from go and back.

        [DataMember]
        public int Lag { get; set; } //number of values at the start of the Lise signal used for the algorithm calibration (must not contain peaks)

        [DataMember]
        public int DetectionCoef { get; set; } //multiplying coefficient of the statistic "Mean + standard deviation" above which the values are considered as peaks

        [DataMember]
        public int PeakInfluence { get; set; } //influence of the peaks detected in the calculation of the "mean" and "standard deviation" statistics

        [DataMember]
        public Length AcceptanceThreshold { get; set; } //acceptance threshold when fitting a layer thickness at the peaks on the Lise signal
    }
}
