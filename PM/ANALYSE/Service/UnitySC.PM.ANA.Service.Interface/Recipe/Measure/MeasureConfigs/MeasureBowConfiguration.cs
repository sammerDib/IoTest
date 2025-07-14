using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure
{
    [DataContract]
    public class MeasureBowConfiguration : MeasureConfigurationBase
    {
        /// <summary>
        /// The maximum angle between the reference plane and the horizontal plane.
        /// <para>
        /// Normally, the reference plane should be very close to be horizontal. If the angle of the
        /// reference plane is above this threshold, the measure will return an error status so that
        /// the user is notified that the bow result is suspicious.
        /// </para>
        /// <para>
        /// In the SEMI norm, the worst case scenario would be that the pillars are at 6.48mm from
        /// the edge of the wafer, and have a height difference of 0.25mm. For a 150mm wafer it
        /// represents an angle of 0.105° = arcsin(0.25/(150 - 2*6.48)).
        /// </para>
        /// </summary>
        [DataMember]
        public Angle MaxReferencePlaneAngle { get; set; }
        
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
        /// The maximum difference in distance between each consecutive reference plane point.
        /// The reference plane points should form a polygon as close as possible to a regular polygon.
        /// </summary>
        [DataMember]
        public Length MaxReferencePlanePointsEdgeDeviationFromRegularPolygon { get; set; }

        /// <summary>
        /// Time (in ms) to wait after releasing the wafer from the clamp to let it go back to its
        /// unconstrained shape.
        /// Note: releasing the wafer is only performed on non-open chucks, when the bow measure is
        /// already not very precise.
        /// </summary>
        [DataMember]
        public int ReleaseWaferTimeoutMilliseconds { get; set; }

        /// <summary>
        /// The number of acquisition to perform to have one single lise signal.
        /// These acquisitions' air gap and thicknesses measures are averaged to 
        /// get a more accurate result.
        /// </summary>
        [DataMember]
        public int NbAveragingLise { get; set; } = 16;
    }
}
