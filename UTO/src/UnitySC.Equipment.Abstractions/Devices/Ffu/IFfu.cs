using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.Ffu.Conditions;

using UnitsNet;
using UnitsNet.Units;

using UnitySC.Equipment.Abstractions.Devices.Ffu.Enum;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.Conditions;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Conditions;

namespace UnitySC.Equipment.Abstractions.Devices.Ffu
{
    [Device(IsAbstract = true)]
    public interface IFfu : IUnityCommunicatingDevice
    {
        #region Statuses

        [Status]
        [Unit(RotationalSpeedUnit.RevolutionPerMinute)]
        RotationalSpeed FanSpeed { get; }

        [Status(IsLoggingActivated = false)]
        [Unit(PressureUnit.Millipascal)]
        Pressure DifferentialPressure { get; }

        [Status]
        bool HasAlarm { get; }

        #endregion Statuses

        #region Commands

        [Command(Documentation = "Set 0 to stop FFU")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsFfuSpeedValid))]
        void SetFfuSpeed(double setPoint, FfuSpeedUnit unit);

        [Command(Documentation = "Send current date and time to hardware in order to synchronize logs.")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsNotBusy))]
        void SetDateAndTime();

        #endregion Commands

        #region Public

        public string IsFfuSpeedValid(double setPoint, FfuSpeedUnit unit);

        #endregion
    }
}
