using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.ProbeLise
{
    [DebuggerDisplay("RawValues = {RawValues}, SelectedPeaks = {SelectedPeaks}, DiscardedPeaks = {DiscardedPeaks}")]
    [DataContract]
    public class ProbeLiseSignal : ProbeSignalBase, ICloneable
    {
        public ProbeLiseSignal()
        {
            RawValues = new List<double>();
            ReferencePeaks = new List<ProbePoint>();
            SelectedPeaks = new List<ProbePoint>();
            DiscardedPeaks = new List<ProbePoint>();
            Means = new List<double>();
            StdDev = new List<double>();
        }

  
        // Distance between two raw values (nm)
        [DataMember]
        public float StepX { get; set; }

        [DataMember]
        public float SaturationValue { get; set; }

        [DataMember]
        public List<ProbePoint> ReferencePeaks { get; set; }

        [DataMember]
        public List<ProbePoint> SelectedPeaks { get; set; }

        [DataMember]
        public List<ProbePoint> DiscardedPeaks { get; set; }

        [DataMember]
        public List<double> Means { get; set; }

        [DataMember]
        public List<double> StdDev { get; set; }

        public object Clone()
        {
            return new ProbeLiseSignal()
            {
                RawValues = RawValues,
                ProbeID = ProbeID,
                StepX = StepX,
                SaturationValue = SaturationValue,
                ReferencePeaks = ReferencePeaks,
                SelectedPeaks = SelectedPeaks,
                DiscardedPeaks = DiscardedPeaks,
                Means = Means,
                StdDev = StdDev
            };
        }
    }
}
