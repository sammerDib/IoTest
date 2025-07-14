using Agileo.EquipmentModeling;

namespace UnitySC.EFEM.Rorze.Devices.LightTower
{
    /// <summary>
    /// Implementation of an <see cref="ILightTower"/> device working with (being a facade of) Rorze RC530 Dio1 device.
    /// </summary>
    [Device]
    public interface ILightTower : UnitySC.Equipment.Abstractions.Devices.LightTower.ILightTower
    {
    }
}
