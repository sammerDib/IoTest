using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure
{
    public class MeasureWarpConfiguration : MeasureConfigurationBase
    {
        // TODO Warp : need to be updated when presets type for Warp measure will be established

        [DataMember]
        public Length DefaultReferencePlanePointsDistanceFromWaferEdge { get; set; }

        [DataMember]
        public List<Angle> DefaultReferencePlanePointsAngularPositions { get; set; }

        /// <summary>
        /// The angle to apply to the default reference plane points when they are unreachable.
        /// This situation may happen for example when acquiring from below on an open chuck and
        /// the support pins are right where the default points should be.
        /// </summary>
        [DataMember]
        public Angle ReferencePlanePointsRotationWhenDefaultUnreachable { get; set; }

        /// <summary>
        /// Time (in ms) to wait after releasing the wafer from the clamp to let it go back to its
        /// unconstrained shape.
        /// Note: releasing the wafer is only performed on non-open chucks, when the bow measure is
        /// already not very precise.
        /// </summary>
        [DataMember]
        public int ReleaseWaferTimeoutMilliseconds { get; set; }

        /// <summary>
        /// Factor used to check if total thickness measured with dual lise is consistent
        /// If measure point is on chuck for bottom lise, then we measure wafer thickness + chuck thickness
        /// So we need to identify this case in order to flag this measure point
        /// </summary>
        [DataMember]
        public double DualLiseTotalThicknessValidityFactor { get; set; }

        /// <summary>
        /// The number of acquisition to perform to have one single lise signal.
        /// These acquisitions' air gap and thicknesses measures are averaged to 
        /// get a more accurate result.
        /// </summary>
        [DataMember]
        public int NbAveragingLise { get; set; } = 16;
    }
}
