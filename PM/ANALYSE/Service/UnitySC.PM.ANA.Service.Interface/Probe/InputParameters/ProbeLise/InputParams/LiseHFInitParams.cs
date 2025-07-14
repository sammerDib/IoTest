using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PM.ANA.Service.Interface
{
    [DataContract]
    [KnownType(typeof(ProbeSample))]
    public class  LiseHFInitParams : ILiseInputParams
    {
        #region Properties
        [DataMember]
        public IProbeSample ProbeSample { get; set; }

        [DataMember]
        public int AttenuationSliderID { get; set; }

        [DataMember]
        public double IntegrationTime_ms { get; set; }

        [DataMember]
        public int NbAverage { get; set; }

        [DataMember]
        public double Threshold_ValidSignal { get; set; }

        [DataMember]
        public double Threshold_Peak { get; set; }

        [DataMember]
        public double TSVDiameter { get; set; }

        [DataMember]
        public bool IsCalibrationAcquisition { get; set; }

        [DataMember]
        public bool IsDarkAcquisition { get; set; }

        [DataMember]
        public bool NeedSignalAnalysis { get; set; }

        [DataMember]
        public bool NeedDisplayOnly { get; set; }

        #endregion

        public LiseHFInitParams()
        {
            
        }
    }
}
