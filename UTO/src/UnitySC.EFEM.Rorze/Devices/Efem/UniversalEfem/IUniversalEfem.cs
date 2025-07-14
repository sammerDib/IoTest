using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.Efem;

namespace UnitySC.EFEM.Rorze.Devices.Efem.UniversalEfem
{
    /// <summary>
    /// Implementation of an <see cref="IEfem"/> device working with (being a facade of) Rorze RC530 Dio1 device.
    /// </summary>
    [Device]
    public interface IUniversalEfem : IEfem
    {
    }
}
