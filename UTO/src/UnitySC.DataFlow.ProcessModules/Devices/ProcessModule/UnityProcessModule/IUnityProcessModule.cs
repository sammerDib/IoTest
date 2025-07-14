using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule;

namespace UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.UnityProcessModule
{
    [Device(IsAbstract = true)]
    public interface IUnityProcessModule : IDriveableProcessModule
    {
    }
}
