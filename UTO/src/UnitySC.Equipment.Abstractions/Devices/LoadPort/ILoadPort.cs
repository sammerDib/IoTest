using System.Collections.Generic;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.LoadPort.Conditions;
using UnitySC.Equipment.Abstractions.Devices.LoadPort.Configuration;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.Conditions;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Conditions;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort
{
    [Device(IsAbstract = true)]
    public interface ILoadPort : IUnityCommunicatingDevice, IExtendedMaterialLocationContainer
    {
        #region Commands

        [Command]
        [Pre(Type = typeof(IsInService))]
        [Pre(Type = typeof(IsCommunicating))]
        void GetStatuses();

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsInService))]
        [Pre(Type = typeof(IsIdle))]
        [Pre(Type = typeof(IsCarrierCorrectlyPlaced))]
        void Clamp();

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsInService))]
        [Pre(Type = typeof(IsIdle))]
        [Pre(Type = typeof(IsInUnclampPosition))]
        void Unclamp();

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsInService))]
        [Pre(Type = typeof(IsIdle))]
        [Pre(Type = typeof(IsCarrierCorrectlyPlaced))]
        void Dock();

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsInService))]
        [Pre(Type = typeof(IsIdle))]
        void Undock();

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsInService))]
        [Pre(Type = typeof(IsIdle))]
        [Pre(Type = typeof(IsCarrierCorrectlyPlaced))]
        void Open(bool performMapping);

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsInService))]
        [Pre(Type = typeof(IsIdle))]
        [Pre(Type = typeof(IsCarrierCorrectlyPlaced))]
        void PrepareForTransfer();

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsInService))]
        [Pre(Type = typeof(IsIdle))]
        void PostTransfer();

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsInService))]
        [Pre(Type = typeof(IsIdle))]
        void Close();

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsInService))]
        [Pre(Type = typeof(IsIdle))]
        [Pre(Type = typeof(IsMappingSupported))]
        [Pre(Type = typeof(IsCarrierCorrectlyPlaced))]
        void Map();

        [Command(Category = "Identification")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsInService))]
        [Pre(Type = typeof(IsNotBusy))]
        [Pre(Type = typeof(IsCarrierIdSupported))]
        [Pre(Type = typeof(IsCarrierCorrectlyPlaced))]
        void ReadCarrierId();

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsInService))]
        [Pre(Type = typeof(IsIdle))]
        [Pre(Type = typeof(IsCarrierCorrectlyPlaced))]
        void ReleaseCarrier();

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsNotBusy))]
        void SetLight(LoadPortLightRoleType role, LightState lightState);

        [Command(Documentation = "Send current date and time to hardware in order to synchronize logs.")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsNotBusy))]
        void SetDateAndTime();

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsInService))]
        [Pre(Type = typeof(IsNotBusy))]
        [Pre(Type = typeof(IsCarrierCorrectlyPlacedOrAbsent))]
        [Pre(Type = typeof(IsE84Enabled))]
        void SetAccessMode(LoadingType accessMode);

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsInService))]
        [Pre(Type = typeof(IsNotBusy))]
        [Pre(Type = typeof(IsE84Enabled))]
        void RequestLoad();

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsInService))]
        [Pre(Type = typeof(IsNotBusy))]
        [Pre(Type = typeof(IsCarrierCorrectlyPlaced))]
        [Pre(Type = typeof(IsE84Enabled))]
        void RequestUnload();

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsInService))]
        [Pre(Type = typeof(IsNotBusy))]
        [Pre(Type = typeof(IsCarrierCorrectlyPlaced))]
        [Pre(Type = typeof(IsCarrierUndocked))]
        [Pre(Type = typeof(IsManualCarrierTypeEnabled))]
        void SetCarrierType(uint carrierType);

        #endregion Commands

        #region Statuses

        [Status]
        CassettePresence CarrierPresence { get; }

        [Status]
        LoadPortState PhysicalState { get; }

        [Status]
        LoadingType AccessMode { get; }

        [Status]
        bool IsInService { get; }

        [Status(Category = "Sensors status")]
        bool IsClamped { get; }

        [Status(Category = "Sensors status")]
        bool IsDocked { get; }

        [Status(Category = "Sensors status")]
        bool IsDoorOpen { get; }

        [Status(Category = "Sensors status")]
        bool IsHandOffButtonPressed { get; }

        [Status(Category = "Indicators status")]
        LightState LoadLightState { get; }

        [Status(Category = "Indicators status")]
        LightState UnloadLightState { get; }

        [Status(Category = "Indicators status")]
        LightState ManualModeLightState { get; }

        [Status(Category = "Indicators status")]
        LightState AutoModeLightState { get; }

        [Status(Category = "Indicators status")]
        LightState ReservedLightState { get; }

        [Status(Category = "Indicators status")]
        LightState ErrorLightState { get; }

        [Status(Category = "Indicators status")]
        LightState HandOffLightState { get; }

        [Status]
        uint CarrierTypeNumber { get; }

        [Status]
        string CarrierTypeName { get; }

        [Status]
        string CarrierTypeDescription { get; }

        List<string> AvailableCarrierTypes { get; }

        [Status]
        uint CarrierTypeIndex { get; }

        [Status]
        string CarrierProfileName { get; }

        #endregion Statuses

        #region E84

        [Status(Category = "E84")]
        bool I_VALID { get; }

        [Status(Category = "E84")]
        bool I_CS_0 { get; }

        [Status(Category = "E84")]
        bool I_CS_1 { get; }

        [Status(Category = "E84")]
        bool I_TR_REQ { get; }

        [Status(Category = "E84")]
        bool I_BUSY { get; }

        [Status(Category = "E84")]
        bool I_COMPT { get; }

        [Status(Category = "E84")]
        bool I_CONT { get; }

        [Status(Category = "E84")]
        bool O_L_REQ { get; }

        [Status(Category = "E84")]
        bool O_U_REQ { get; }

        [Status(Category = "E84")]
        bool O_READY { get; }

        [Status(Category = "E84")]
        bool O_HO_AVBL { get; }

        [Status(Category = "E84")]
        bool O_ES { get; }

        [Status(Category = "E84")]
        bool E84TransferInProgress { get; }

        [Status(Category = "E84")]
        E84Errors CurrentE84Error { get; }

        [Command(Category = "E84")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsInService))]
        [Pre(Type = typeof(IsNotBusy))]
        void EnableE84();

        [Command(Category = "E84")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsInService))]
        [Pre(Type = typeof(IsNotBusy))]
        void DisableE84();

        [Command(Category = "E84")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsInService))]
        [Pre(Type = typeof(IsNotBusy))]
        void ManageEsSignal(bool isActive);

        [Command(Category = "E84", Documentation = "TP in seconds")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsInService))]
        [Pre(Type = typeof(IsNotBusy))]
        void SetE84Timeouts(int tp1, int tp2, int tp3, int tp4, int tp5);

        bool NeedsInitAfterE84Error();

        #endregion E84
    }
}
