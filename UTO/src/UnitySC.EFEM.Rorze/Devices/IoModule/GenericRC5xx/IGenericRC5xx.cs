using Agileo.EquipmentModeling;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.Conditions;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx
{
    /// <summary>
    /// Define all commands and statuses common to all RC5xx IO devices (eg: RC530 and RC550).
    /// </summary>
    [Device(IsAbstract = true)]
    public interface IGenericRC5xx : IUnityCommunicatingDevice
    {
        #region Statuses

        [Status]
        OperationMode OperationMode { get; }

        [Status]
        CommandProcessing CommandProcessing { get; }

        [Status]
        string IoModuleInError { get; }

        [Status]
        string ErrorCode { get; }

        #endregion Statuses

        #region Commands

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        void SetOutputSignal(SignalData signalData);

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        void SetDateAndTime();

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        void GetStatuses();

        #endregion Commands
    }
}
