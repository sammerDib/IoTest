using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule;

namespace UnitySC.ToolControl.ProcessModules.Devices.ProcessModule.ToolControlProcessModule
{
    [Device(IsAbstract = true)]
    public interface IToolControlProcessModule : IDriveableProcessModule
    {
        [Status]
        string ProcessModuleName { get; }

        void SetProcessModuleName(string processModuleName);
    }
}
