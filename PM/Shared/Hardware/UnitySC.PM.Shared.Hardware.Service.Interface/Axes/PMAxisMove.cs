using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    [DataContract]
    public class PMAxisMove
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="axisId"></param>
        /// <param name="position"></param>
        /// <param name="speed">Optional parameter, if not provided value will be null</param>
        /// <param name="accel">Optional parameter, if not provided value will be null</param>
        public PMAxisMove(string axisId, Length position, Speed speed = null, Acceleration accel = null)
        {
            AxisId = axisId;
            Position = position;
            Speed = speed;
            Accel = accel;
        }

        public override string ToString()
        {
            return $"AxisId = {AxisId}, Position = {Position}, Speed = {Speed.MillimetersPerSecond}, Accel = {Accel.MillimetersPerSecondSquared}";
        }

        //FIXME Could be replaced by an enum of all allowed axisIds 
        [DataMember] public string AxisId { get; set; }

        [DataMember] public Length Position { get; set; }

        [DataMember] public Speed Speed { get; set; }

        [DataMember] public Acceleration Accel { get; set; }
    }

    public class RotationalAxisMove : PMAxisMove
    {
        public RotationalAxisMove(string axisId, Angle anglePosition, Speed speed = null, Acceleration accel = null) : base(axisId, null, speed, accel)
        {
            AnglePosition = anglePosition;
        }

        [DataMember] public Angle AnglePosition { get; set; }
    }
}
