using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.ProcessModule;

namespace UnitySC.EfemController.ProcessModules.Devices.ProcessModule.EfemControllerProcessModule
{
    /// <summary>
    /// Implementation of an <see cref="IProcessModule" /> device working with (being a facade of) Rorze
    /// RC530 Dio2 device.
    /// </summary>
    [Device]
    public interface IEfemControllerProcessModule : IProcessModule
    {
    }
}
