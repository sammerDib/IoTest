using Agileo.EquipmentModeling;

using UnitsNet;
using UnitsNet.Units;

using UnitySC.Equipment.Abstractions.Devices.Ffu.Enum;

namespace UnitySC.Equipment.Abstractions.Devices.Ffu
{
    public partial class Ffu
    {
        protected virtual void InternalSimulateSetDateAndTime(Tempomat tempomat)
            => tempomat.Sleep(Duration.FromSeconds(2));

        protected virtual void InternalSimulateSetFfuSpeed(
            double setPoint,
            FfuSpeedUnit unit,
            Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromSeconds(2));
            FanSpeed = RotationalSpeed.From(setPoint, RotationalSpeedUnit.RevolutionPerMinute);
            if (FanSpeed.RevolutionsPerMinute < Configuration.LowSpeedThreshold)
            {
                SetAlarm("FFU_011");
            }
        }
    }
}
