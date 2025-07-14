using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Chuck
{
    [DataContract]
    public class ChuckState : IEquatable<ChuckState>
    {
        public ChuckState()
        {
        }

        public ChuckState(Dictionary<Length, MaterialPresence> waferPresences)
        {
            WaferClampStates = new Dictionary<Length, bool>();
            WaferPresences = waferPresences;
            PinLiftUp = false;
            PinLiftDown = true;
        }

        public ChuckState(Dictionary<Length, bool> waferClampStates, Dictionary<Length, MaterialPresence> waferPresences)
        {
            WaferClampStates = waferClampStates;
            WaferPresences = waferPresences;
            PinLiftUp = false;
            PinLiftDown = true;
        }

        public ChuckState(Dictionary<Length, bool> waferClampStates, Dictionary<Length, MaterialPresence> waferPresences, bool pinLiftUp, bool pinLiftDown)
        {
            WaferClampStates = waferClampStates;
            WaferPresences = waferPresences;
            PinLiftUp = pinLiftUp;
            PinLiftDown = pinLiftDown;
        }

        [DataMember]
        public Dictionary<Length, bool> WaferClampStates { get; set; }

        [DataMember]
        public Dictionary<Length, MaterialPresence> WaferPresences { get; set; }

        [DataMember]
        public bool PinLiftUp { get; set; }

        [DataMember]
        public bool PinLiftDown { get; set; }
        
        public override bool Equals(object obj) => Equals(obj as ChuckState);

        public bool Equals(ChuckState other)
        {
            return ((WaferClampStates == other.WaferClampStates) && (WaferPresences == other.WaferPresences) && (PinLiftUp == other.PinLiftUp) && (PinLiftDown == other.PinLiftDown));
        }

        public static bool operator ==(ChuckState lChuckState, ChuckState rChuckState)
        {
            if (ReferenceEquals(lChuckState, null) && ReferenceEquals(rChuckState, null))
            {
                return true;
            }

            if (ReferenceEquals(lChuckState, null) || ReferenceEquals(rChuckState, null))
            {
                return false;
            }
            return lChuckState.Equals(rChuckState);
        }

        public static bool operator !=(ChuckState lChuckState, ChuckState rChuckState)
        {
            if (ReferenceEquals(lChuckState, null) && ReferenceEquals(rChuckState, null))
            {
                return false;
            }

            if (ReferenceEquals(lChuckState, null) || ReferenceEquals(rChuckState, null))
            {
                return true;
            }
            return !lChuckState.Equals(rChuckState);
        }

        public void CopyTo(ChuckState copyState)
        {
            copyState.WaferClampStates = WaferClampStates;
            copyState.WaferPresences = WaferPresences;
            copyState.PinLiftDown = PinLiftDown;
            copyState.PinLiftUp = PinLiftUp;
        }

        public override int GetHashCode() => (WaferClampStates, WaferPresences, PinLiftUp, PinLiftDown).GetHashCode();
    }
}
