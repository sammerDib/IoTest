using System.Runtime.Serialization;

namespace UnitySC.PM.AGS.Service.Interface.Flow
{
    [DataContract]
    public class AcquisitionConfiguration
    {
        [DataMember] public string Name;
    }
}
