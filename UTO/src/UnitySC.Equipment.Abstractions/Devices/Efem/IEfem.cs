using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.Efem.Enums;

using UnitsNet;

using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice;
using UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader.Enums;

namespace UnitySC.Equipment.Abstractions.Devices.Efem
{
    [Device(IsAbstract = true)]
    public interface IEfem : IUnityCommunicatingDevice
    {
        #region Statuses

        [Status]
        OperationMode OperationMode { get; }

        [Status]
        RobotStatus RobotStatus { get; }

        [Status]
        Ratio RobotSpeed { get; }

        [Status]
        LoadPortStatus LoadPortStatus1 { get; }

        [Status]
        bool IsLoadPort1CarrierPresent { get; }

        [Status]
        LoadPortStatus LoadPortStatus2 { get; }

        [Status]
        bool IsLoadPort2CarrierPresent { get; }

        [Status]
        LoadPortStatus LoadPortStatus3 { get; }

        [Status]
        bool IsLoadPort3CarrierPresent { get; }

        [Status]
        LoadPortStatus LoadPortStatus4 { get; }

        [Status]
        bool IsLoadPort4CarrierPresent { get; }

        [Status]
        AlignerStatus AlignerStatus { get; }

        [Status]
        bool IsAlignerCarrierPresent { get; }

        [Status(Documentation = "0=Open, 1=Failed")]
        bool SafetyDoorSensor { get; }

        [Status(Documentation = "0=Normal, 1=Failed")]
        bool VacuumSensor { get; }

        [Status(Documentation = "0=Failed, 1=Normal")]
        bool AirSensor { get; }

        [Status(Documentation = "0=Normal, 1=Failed")]
        bool FfuAlarm { get; }

        [Status(Documentation = "0=Failed, 1=Normal")]
        bool IonizerAirState { get; }

        [Status(Documentation = "0=Normal, 1=Failed")]
        bool IonizerAlarm { get; }

        [Status(Documentation = "1=Free, 0=Obstructed")]
        bool LightCurtainBeam { get; }

        [Status(Documentation = "0=Normal, 1=Interlocked")]
        bool Interlock { get; }

        #endregion Statuses

        #region public

        (DevicePosition,int) GetPosition(Device device);

        bool CanReleaseRobot();

        string ReadSubstrateId(ReaderSide readerSide, string frontRecipeName, string backRecipeName);

        #endregion
    }
}
