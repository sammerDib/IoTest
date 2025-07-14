using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.EME.Service.Interface.Context;
using UnitySC.PM.EME.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude(typeof(EMEContextBase))]
    public class AutoFocusCameraInput : IEMEInputFlow
    {
        public AutoFocusCameraInput()
        {
        }

        
        
        public AutoFocusCameraInput(ScanRangeType rangeType, ScanRangeWithStep scanRange = null) : this(null, rangeType, scanRange)
        {
        }
        

        public AutoFocusCameraInput(EMEContextBase context, ScanRangeType rangeType, ScanRangeWithStep scanRange = null)
        {
            InitialContext = context;
            RangeType = rangeType;
            ScanRangeConfigured = scanRange;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (RangeType == ScanRangeType.Configured)
            {
                if (ScanRangeConfigured is null)
                {
                    validity.IsValid = false;
                    validity.Message.Add($"The scan range configured must be provided when range type  is set to 'Configured'.");
                }
                else
                {
                    validity.ComposeWith(ScanRangeConfigured.CheckInputValidity());
                }
            }

            return validity;
        }

        
        [DataMember]
        public EMEContextBase InitialContext { get; set; }

        [DataMember]
        public ScanRangeWithStep ScanRangeConfigured { get; set; }

        [DataMember]
        public ScanRangeType RangeType { get; set; }
    }
}
