using System;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;

namespace UnitySC.Equipment.Abstractions
{
    public interface ILayingPlanLoadPortIos
    {
        #region Properties

        #region Load Port 1

        bool PlacementSensorALoadPort1 { get; }

        bool PlacementSensorBLoadPort1 { get; }

        bool PlacementSensorCLoadPort1 { get; }

        bool PlacementSensorDLoadPort1 { get; }

        bool WaferProtrudeSensor1LoadPort1 { get; }

        bool WaferProtrudeSensor2LoadPort1 { get; }

        bool WaferProtrudeSensor3LoadPort1 { get; }

        #endregion

        #region Load Port 2

        bool PlacementSensorALoadPort2 { get; }

        bool PlacementSensorBLoadPort2 { get; }

        bool PlacementSensorCLoadPort2 { get; }

        bool PlacementSensorDLoadPort2 { get; }

        bool WaferProtrudeSensor1LoadPort2 { get; }

        bool WaferProtrudeSensor2LoadPort2 { get; }

        bool WaferProtrudeSensor3LoadPort2 { get; }

        #endregion

        #region Load Port 3

        bool PlacementSensorALoadPort3 { get; }

        bool PlacementSensorBLoadPort3 { get; }

        bool PlacementSensorCLoadPort3 { get; }

        bool PlacementSensorDLoadPort3 { get; }

        bool WaferProtrudeSensor1LoadPort3 { get; }

        bool WaferProtrudeSensor2LoadPort3 { get; }

        bool WaferProtrudeSensor3LoadPort3 { get; }

        #endregion

        #region Load Port 4

        bool PlacementSensorALoadPort4 { get; }

        bool PlacementSensorBLoadPort4 { get; }

        bool PlacementSensorCLoadPort4 { get; }

        bool PlacementSensorDLoadPort4 { get; }

        bool WaferProtrudeSensor1LoadPort4 { get; }

        bool WaferProtrudeSensor2LoadPort4 { get; }

        bool WaferProtrudeSensor3LoadPort4 { get; }

        #endregion

        bool IsCommunicationStarted { get; }

        bool IsCommunicating { get; }

        OperatingModes State { get; }

        #endregion

        #region Methods

        void StartCommunication();

        void StopCommunication();

        #endregion

        #region Events

        event EventHandler<StatusChangedEventArgs> StatusValueChanged;

        #endregion
    }
}
