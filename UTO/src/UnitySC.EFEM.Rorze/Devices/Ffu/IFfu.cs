using Agileo.EquipmentModeling;

namespace UnitySC.EFEM.Rorze.Devices.Ffu
{
    /// <summary>
    /// Implementation of an <see cref="IFfu"/> device working with (being a facade of) Rorze RC550 Dio0 device.
    /// </summary>
    [Device]
    public interface IFfu : UnitySC.Equipment.Abstractions.Devices.Ffu.IFfu
    {
    }
}
