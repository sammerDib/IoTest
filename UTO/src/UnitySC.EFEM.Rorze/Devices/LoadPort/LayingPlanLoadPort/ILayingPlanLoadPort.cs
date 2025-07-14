using System.Collections.ObjectModel;
using Agileo.EquipmentModeling;

using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.LayingPlanLoadPort
{
    // Implement interface IMaterialLocationContainer if the device has material locations
    [Device]
    public interface ILayingPlanLoadPort : ILoadPort
    {
        #region Statuses

        [Status]
        bool PlacementSensorA { get; }

        [Status]
        bool PlacementSensorB { get; }

        [Status]
        bool PlacementSensorC { get; }

        [Status]
        bool PlacementSensorD { get; }

        [Status]
        bool WaferProtrudeSensor1 { get; }

        [Status]
        bool WaferProtrudeSensor2 { get; }

        [Status]
        bool WaferProtrudeSensor3 { get; }

        [Status]
        bool MappingRequested { get; }

        #endregion

        #region Public Methods

        void SetMapping(Collection<RR75xSlotState> slotsState);

        #endregion
    }
}
