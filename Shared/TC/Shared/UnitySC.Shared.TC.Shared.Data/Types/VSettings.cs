using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.Shared.TC.Shared.Data
{
    [DataContract]
    public class VSettings<TVariableType>
    {
        [DataMember]
        public List<TVariableType> VariableList = new List<TVariableType>();

        public VSettings()
        {
        }
    }
}
