using System.Collections.Generic;

using Agileo.StateMachine;

using UnitySC.Equipment.Abstractions.Vendor.Material;

namespace UnitySC.Equipment.Devices.Controller.Activities
{
    internal partial class Clear
    {
        private class RobotDone : Event
        {
        }

        private class StatusReceived : Event
        {
            private IEnumerable<SubstrateLocation> _equipmentSubstrateLocations;
            public IEnumerable<SubstrateLocation> EquipmentSubstrateLocations
            {
                get { return _equipmentSubstrateLocations; }
            }

            /// <summary>
            /// Initializes a new instance of <see cref="StatusReceived"/> event containing all Equipment Substrate Locations except robot's locations.
            /// </summary>
            /// <param name="substrateLocations"></param>
			public StatusReceived(IEnumerable<SubstrateLocation> substrateLocations)
            {
                _equipmentSubstrateLocations = substrateLocations;
            }
        }

        private class PmDone : Event
        {
        }
    }
}
