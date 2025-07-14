using System;
using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    [Serializable]
    [DataContract]
    public class PiezoAxisConfig : MotorizedAxisConfig
    {
        /// <summary>
        /// Minimum and maximum position values allowed by the controller.
        /// Those values should take precedence over all others returned by the controlller (if more restrictive).
        /// </summary>
        [DataMember]
        public override Length PositionMin { get; set; }

        [DataMember]
        public override Length PositionMax { get; set; }

        /// <summary>
        /// Park position specifies the piezo position after axis initalization or when calling the Park() method.
        /// Its value must be consitent with the above Min/Max position values.
        /// </summary>
        [DataMember]
        public override Length PositionPark { get; set; }
    }
}
