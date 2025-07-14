using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Spectrometer
{
    [DebuggerDisplay("RawValues = {RawValues}")]
    [DataContract]
    public class SpectroSignal : ICloneable
    {
        public SpectroSignal()
        {
            RawValues = new List<double>();
            Wave = new List<double>();
        }

        [DataMember]
        public List<double> RawValues { get; set; }

        [DataMember]
        public List<double> Wave { get; set; }

        public object Clone()
        {
            return new SpectroSignal()
            {
                RawValues = new List<double>(RawValues),
                Wave = new List<double>(Wave)
            };
        }
    }
}
