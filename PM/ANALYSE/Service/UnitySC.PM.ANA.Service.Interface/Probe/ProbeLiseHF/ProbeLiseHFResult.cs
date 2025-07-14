using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Probe.ProbeLiseHF
{
    [DataContract]
    public class ProbeLiseHFResult : IProbeThicknessesResult
    {
        public ProbeLiseHFResult()
        {
            LayersThickness = new List<ProbeThicknessMeasure>();
        }

        #region Properties

        [DataMember]
        public List<ProbeThicknessMeasure> LayersThickness { get; set; }

        [DataMember]
        public DateTime Timestamp { get; set; }

        [DataMember]
        public double Quality { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public byte[] FFTSignal { get; set; }  // FFT signal memoy of 2*float *N (X0 Y0 X1 Y1 .. XN YN)

        #endregion Properties
    }
}
