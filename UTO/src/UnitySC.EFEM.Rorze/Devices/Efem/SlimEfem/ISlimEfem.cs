using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.Efem;

namespace UnitySC.EFEM.Rorze.Devices.Efem.SlimEfem
{
    /// <summary>
    /// Implementation of an <see cref="IEfem"/> device working with (being a facade of) Beckhoff EK9000 device.
    /// </summary>
    [Device]
    public interface ISlimEfem : IEfem
    {
    }
}
