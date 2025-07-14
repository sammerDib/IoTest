using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.ProbeLiseHF
{
    [DebuggerDisplay("RawValues = {RawValues}")]
    [DataContract]
    public class ProbeLiseHFSignal : ProbeSignalBase, ICloneable
    {
        public ProbeLiseHFSignal()
        {
            RawValues = new List<double>();
        }

        public ProbeLiseHFSignal(List<double> rawValues, List<double> XValues, int satLevel)
        {
            RawValues = new List<double>(rawValues);
            StepX = (XValues.Count<2) ? 0.0 : (XValues[XValues.Count-1] - XValues[0])/(XValues.Count - 1)*1000;
            SaturationLevel = satLevel;
        }

        // Distance between two raw values (nm)
        [DataMember]
        public double StepX { get; set; }

        [DataMember]
        public int SaturationLevel { get; set; }

        [DataMember]
        public double Threshold { get; set; }

        [DataMember]
        public double ThresholdPeak { get; set; }

        [DataMember]
        public double Quality { get; set; }

        [DataMember]
        public string Message { get; set; }

        public object Clone()
        {
            return new ProbeLiseHFSignal()
            {
                RawValues = new List<double>(RawValues),
                ProbeID = ProbeID,
                StepX = StepX,
                SaturationLevel = SaturationLevel,
                Threshold = Threshold,
                ThresholdPeak = ThresholdPeak,
                Quality = Quality,
                Message = Message
            };
        }
    }
}
