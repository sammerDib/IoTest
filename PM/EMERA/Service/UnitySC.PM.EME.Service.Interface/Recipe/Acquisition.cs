using System;
using System.Runtime.Serialization;

using UnitySC.Shared.Data.Enum.Module;

namespace UnitySC.PM.EME.Service.Interface.Recipe
{
    [DataContract]
    public class Acquisition : IEquatable<Acquisition>
    {
        public Acquisition()
        {            
        }
        
        [DataMember]
        public string Name { get; set; }

        [DataMember] 
        public EMEFilter Filter { get; set; }

        [DataMember]
        public double ExposureTime { get; set; }

        [DataMember]
        public string LightDeviceId { get; set; }
        public bool Equals(Acquisition other)
        {
            return !(other is null) &&
                other.Name == Name &&
                other.Filter== Filter &&
                other.ExposureTime == ExposureTime &&
                other.LightDeviceId == LightDeviceId;
        }
        public static bool operator ==(Acquisition lAcquisition, Acquisition rAcquisition)
        {
            if (lAcquisition is null)
            {
                return rAcquisition is null;
            }
            return lAcquisition.Equals(rAcquisition);
        }

        public static bool operator !=(Acquisition lAcquisition, Acquisition rAcquisition)
        {
            if (lAcquisition is null)
            {
                return !(rAcquisition is null);
            }
            return !lAcquisition.Equals(rAcquisition);
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as Acquisition);
        }
        public override int GetHashCode() => (Name, Filter, ExposureTime, LightDeviceId).GetHashCode();

    }
}
