using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface
{
    [DataContract]
    [KnownType(typeof(ProbeSample))]
    public class SingleLiseInputParams : ILiseInputParams
    {
        public SingleLiseInputParams(ProbeSample probeSample)
        {
            ProbeSample = probeSample;
        }

        public SingleLiseInputParams(ProbeSample probeSample, double gain, double qualityThreshold, double detectionThreshold, int nbMeasure)
        {
            ProbeSample = probeSample;
            Gain = gain;
            QualityThreshold = qualityThreshold;
            DetectionThreshold = detectionThreshold;
            NbMeasuresAverage = nbMeasure;
        }

        public SingleLiseInputParams(SingleLiseInputParams inputParams)
        {
            ProbeSample = inputParams.ProbeSample;
            Gain = inputParams.Gain;
            QualityThreshold = inputParams.QualityThreshold;
            DetectionThreshold = inputParams.DetectionThreshold;
            NbMeasuresAverage = inputParams.NbMeasuresAverage;
        }

        #region Properties

        [DataMember]
        public double Gain { get; set; }

        [DataMember]
        public double QualityThreshold { get; set; }

        [DataMember]
        public double DetectionThreshold { get; set; }

        [DataMember]
        public int NbMeasuresAverage { get; set; }

        [DataMember]
        public IProbeSample ProbeSample { get; set; }

        #endregion Properties
    }
}
