using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude(typeof(ANAContextBase))]
    public class AutoAlignInput : IANAInputFlow
    {
        // For serialization
        public AutoAlignInput()
        {
        }

        public AutoAlignInput(WaferDimensionalCharacteristic waferDimensionalCharacteristic)
        {
            Wafer = waferDimensionalCharacteristic;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (Wafer is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The wafer dimensional characteristic is missing.");
            }

            return validity;
        }

        [DataMember]
        public ANAContextBase InitialContext { get; set; }

        [DataMember]
        public WaferDimensionalCharacteristic Wafer { get; set; }
    }
}
