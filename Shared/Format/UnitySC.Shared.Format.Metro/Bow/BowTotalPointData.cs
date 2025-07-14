using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

/// <summary>
/// BowTotalPointData is a dummy mesaure point data
/// Measure Warp contains several sub measure points data which are represented by WarpPointData
/// In order to display bow result during recipe run, we use BowTotalPointData, which will contain global measure result
/// Finally, this "dummy" point is used in MeasureBow.FinalizeMetroResults to build BowWaferResults
/// And it is removed from BowResult
/// </summary>
namespace UnitySC.Shared.Format.Metro.Bow
{
    public class BowTotalPointData : MeasurePointDataResultBase
    {
        [DataMember]
        public Length Bow { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()} Bow : {Bow}";
        }
    }
}
