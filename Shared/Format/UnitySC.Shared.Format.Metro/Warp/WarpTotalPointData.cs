using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Warp
{
    /// <summary>
    /// WarpTotalPointData is a dummy mesaure point data
    /// Measure Warp contains several sub measure points data which are represented by WarpPointData
    /// In order to display warp result during recipe run, we use WarpTotalPointData, which will contain global measure results (Warp + TTV)
    /// Finally, this "dummy" point is used in MeasureWarp.FinalizeMetroResults to build WarpWaferResults & TTVWaferResults
    /// And it is removed from WarpResult
    /// </summary>
    public class WarpTotalPointData : MeasurePointDataResultBase
    {
        [DataMember]
        public Length Warp { get; set; }
        
        [DataMember]
        public Length TTV { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()} Warp : {Warp}";
        }
    }
}
