using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Hardware.Service.Interface.IOComponent;

namespace UnitySC.PM.Shared.Hardware.Service.Interface
{

    [XmlInclude(typeof(ACSControllerConfig))]
    [KnownType(typeof(ACSControllerConfig))]
    [XmlInclude(typeof(NICouplerControllerConfig))]
    [KnownType(typeof(NICouplerControllerConfig))]
    [DataContract]
    public class IOControllerConfig : ControllerConfig
    {
        [DataMember]
        public string ControllerAddress;

        [DataMember]
        [XmlArray]
        public List<IO> IOList = new List<IO>();

        public Dictionary<string, Input> GetInputs()
        {
            return IOList.Where(x => x is Input).ToDictionary(x => x.CommandName ?? x.Name, x => (Input)x);
        }
        public Dictionary<string, Output> GetOutputs()
        {
            return IOList.Where(x => x is Output).ToDictionary(x => x.CommandName ?? x.Name, x => (Output)x);
        }

    }
}
