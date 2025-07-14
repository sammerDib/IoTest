using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise
{
    [DataContract]
    public class LiseResult : IProbeThicknessesResult
    {
        public LiseResult()
        {
            LayersThickness = new List<ProbeThicknessMeasure>();
            Quality = 0;
        }

        #region Properties

        [DataMember]
        public List<ProbeThicknessMeasure> LayersThickness { get; set; }

        [DataMember]
        public double Quality { get; set; }

        [DataMember]
        public DateTime Timestamp { get; set; }

        [DataMember]
        public string Message { get; set; }

        #endregion Properties
    }
}
