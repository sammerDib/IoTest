using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Referentials.Interface
{
    [DataContract]
    public abstract class ReferentialSettingsBase
    {
        [DataMember]
        public readonly ReferentialTag Tag;

        public ReferentialSettingsBase(ReferentialTag tag)
        {
            Tag = tag;
        }
    }
}
