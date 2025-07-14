using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.LightTower.Enums;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.Conditions;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Conditions;

using LightState = Agileo.GUI.Services.LightTower.LightState;

namespace UnitySC.Equipment.Abstractions.Devices.LightTower
{
    [Device(IsAbstract = true)]
    public interface ILightTower : IUnityCommunicatingDevice
    {
        #region Statuses

        [Status(Documentation = "Status of the green light")]
        LightState GreenLight { get; }

        [Status(Documentation = "Status of the orange light")]
        LightState OrangeLight { get; }

        [Status(Documentation = "Status of the blue light")]
        LightState BlueLight { get; }

        [Status(Documentation = "Status of the red light")]
        LightState RedLight { get; }

        [Status(Documentation = "Status of the Buzzer")]
        BuzzerState BuzzerState { get; }

        [Status(Documentation = "Status of the general Signal tower state")]
        LightTowerState SignalTowerState { get; }

        #endregion Statuses

        #region Commands

        [Command(Documentation = "Change Light Tower's Green color")]
        [Pre(Type = typeof(IsCommunicating))]
        void DefineGreenLightMode(LightState state);

        [Command(Documentation = "Change Light Tower's Orange color")]
        [Pre(Type = typeof(IsCommunicating))]
        void DefineOrangeLightMode(LightState state);

        [Command(Documentation = "Change Light Tower's Blue color")]
        [Pre(Type = typeof(IsCommunicating))]
        void DefineBlueLightMode(LightState state);

        [Command(Documentation = "Change Light Tower's Red color")]
        [Pre(Type = typeof(IsCommunicating))]
        void DefineRedLightMode(LightState state);

        [Command(Documentation = "Change Buzzer State")]
        [Pre(Type = typeof(IsCommunicating))]
        void DefineBuzzerMode(BuzzerState state);

        [Command(Documentation = "Change Light Tower's state and associated colors")]
        [Pre(Type = typeof(IsCommunicating))]
        void DefineState(LightTowerState state);

        [Command(Documentation = "Send current date and time to hardware in order to synchronize logs.")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsNotBusy))]
        void SetDateAndTime();

        #endregion Commands
    }
}
