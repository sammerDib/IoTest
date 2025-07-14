using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Warp
{
    public class WarpResultSettings
    {
        [DataMember]
        public bool IsSurfaceWarp { get; set; }

        [DataMember]
        public Length WarpMax { get; set; }
    }
}
