using System.Diagnostics;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    [DebuggerDisplay("Position = {Position}, Speed = {Speed}, Accel = {Accel}")]
    [DataContract]
    public class AxisMove
    {
        public AxisMove(double position, double speed, double accel)
        {
            Position = position;
            Speed = speed;
            Accel = accel;
        }
        [DataMember]
        public double Position { get; set; }

        [DataMember]
        public double Speed { get; set; }

        [DataMember]
        public double Accel { get; set; }


    }
}
