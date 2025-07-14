using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude(typeof(ANAContextBase))]
    public class AFLiseInput : IANAInputFlow
    {
        public AFLiseInput()
        {
        }

        public AFLiseInput(string probeID, double gain = double.NaN, ScanRange zPosScanRange = null)
            : this(null, probeID, gain, zPosScanRange)
        {
        }

        public AFLiseInput(ANAContextBase context, string probeID, double gain = double.NaN,
           ScanRange zPosScanRange = null)
        {
            InitialContext = context;
            ProbeID = probeID;
            Gain = gain;
            ZPosScanRange = zPosScanRange;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (ProbeID is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"the probe ID is missing.");
            }

            if (!(ZPosScanRange is null))
            {
                validity.ComposeWith(ZPosScanRange.CheckInputValidity());
            }

            return validity;
        }

        [DataMember]
        public ANAContextBase InitialContext { get; set; }

        [DataMember]
        public string ProbeID { get; set; }

        [DataMember]
        public ScanRange ZPosScanRange { get; set; }

        [DataMember]
        public double Gain { get; set; }
    }
}
