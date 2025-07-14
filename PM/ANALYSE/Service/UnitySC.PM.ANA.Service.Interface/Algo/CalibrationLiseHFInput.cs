using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude(typeof(ANAContextBase))]
    public class CalibrationLiseHFInput : IANAInputFlow
    {
        public CalibrationLiseHFInput()
        { }

        public CalibrationLiseHFInput(string probeID)
        {
            ProbeID = probeID;
         }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (ProbeID is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The probe ID is missing.");
            }

   
            return validity;
        }

        [DataMember]
        public ANAContextBase InitialContext { get; set; }

        [DataMember]
        public string ProbeID { get; set; }

        [DataMember]
        public LiseHFInputParams InputParams { get; set;}
  
    }
}
