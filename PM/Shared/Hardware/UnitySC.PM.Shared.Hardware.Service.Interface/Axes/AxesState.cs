using System;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    [DataContract]
    public class AxesState : IEquatable<AxesState>
    {
        public AxesState()
        {
        }

        public AxesState(bool allAxisEnabled, bool oneAxisIsMoving, bool isLanded = false)
        {
            AllAxisEnabled = allAxisEnabled;
            OneAxisIsMoving = oneAxisIsMoving;
            Landed = isLanded;
        }

        [DataMember]
        public bool AllAxisEnabled { get; set; }

        [DataMember]
        public bool OneAxisIsMoving { get; set; }

        [DataMember]
        public bool Landed { get; set; }

        public override bool Equals(object obj) => Equals(obj as AxesState);

        public bool Equals(AxesState other)
        {
            return ((other != null) && (AllAxisEnabled == other.AllAxisEnabled) && (OneAxisIsMoving == other.OneAxisIsMoving) && (Landed == other.Landed));
        }

        public static bool operator ==(AxesState lAxesState, AxesState rAxesState)
        {
            if (lAxesState is null)
            {
                return rAxesState is null;
            }

            return lAxesState.Equals(rAxesState);
        }

        public static bool operator !=(AxesState lAxesState, AxesState rAxesState)
        {
            if (lAxesState is null)
            {
                return !(rAxesState is null);
            }

            return !lAxesState.Equals(rAxesState);
        }

        public void CopyTo(AxesState copyState)
        {
            copyState.AllAxisEnabled = AllAxisEnabled;
            copyState.OneAxisIsMoving = OneAxisIsMoving;
            copyState.Landed = Landed;
        }

        public override int GetHashCode() => (AllAxisEnabled, OneAxisIsMoving, Landed).GetHashCode();
    }
}
