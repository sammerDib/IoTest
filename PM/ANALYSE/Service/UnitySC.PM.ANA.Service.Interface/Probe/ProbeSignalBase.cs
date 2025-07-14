using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface
{
    [DebuggerDisplay("RawValues = {RawValues}, SelectedPeaks = {SelectedPeaks}, DiscardedPeaks = {DiscardedPeaks}")]
    [DataContract]
    public class ProbeSignalBase : IProbeSignal
    {
        public ProbeSignalBase()
        {
            RawValues = new List<double>();
        }

        [DataMember]
        public string ProbeID { get; set; }

        [DataMember]
        public List<double> RawValues { get; set; }

 

    }
}
