using System;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Rorze.Devices.LoadPort.LayingPlanLoadPort;

using Duration = UnitsNet.Duration;

namespace UnitySC.EFEM.Rorze.Devices.Robot.MapperRR75x
{
    public partial class MapperRR75x
    {
        protected virtual void InternalSimulateMapLocation(
            IMaterialLocationContainer location,
            Tempomat tempomat)
        {
            if (location is not LayingPlanLoadPort loadPort)
            {
                throw new InvalidOperationException(
                    $"Specified location must be of type {nameof(LayingPlanLoadPort)}");
            }

            var locationId = RegisteredLocations[loadPort.Name];
            InternalSimulateGoToTransferLocation(locationId, RobotArm.Arm1, 1, tempomat);
            tempomat.Sleep(Duration.FromSeconds(2));
            var slotsState = loadPort.GetSimulatedMapping();
            loadPort.SetMapping(slotsState);
            DequeueMapping();
        }

        protected virtual void InternalSimulateMapTransferLocation(
            TransferLocation location,
            Tempomat tempomat)
        {
            var foundDeviceName = RegisteredLocations.FirstOrDefault(x => x.Value == location);
            var loadPort = this.GetEquipment()
                .AllDevices<LayingPlanLoadPort>()
                .FirstOrDefault(lp => lp.Name == foundDeviceName.Key);
            if (loadPort == null)
            {
                throw new InvalidOperationException(
                    $"Specified location must be of type {nameof(LayingPlanLoadPort)}");
            }

            InternalSimulateGoToTransferLocation(location, RobotArm.Arm1, 1, tempomat);
            tempomat.Sleep(Duration.FromSeconds(2));
            var slotsState = loadPort.GetSimulatedMapping();
            loadPort.SetMapping(slotsState);
            DequeueMapping();
        }
    }
}
