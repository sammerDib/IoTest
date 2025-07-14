using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Rorze.Devices.LoadPort.LayingPlanLoadPort;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.Conditions;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Conditions;

namespace UnitySC.EFEM.Rorze.Devices.Robot.MapperRR75x
{
    // Implement interface IMaterialLocationContainer if the device has material locations
    [Device]
    public interface IMapperRR75x : IRR75x
    {
        #region Status

        [Status]
        bool RobotPositionReverted { get; }

        #endregion

        #region Commands

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsIdle))]
        void MapLocation(IMaterialLocationContainer location);

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsIdle))]
        void MapTransferLocation(TransferLocation location);

        #endregion

        #region Methods

        void EnqueueMapping(ILayingPlanLoadPort loadPort);

        #endregion
    }
}
