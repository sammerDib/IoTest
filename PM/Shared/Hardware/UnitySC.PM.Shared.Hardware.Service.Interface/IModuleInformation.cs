namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    public enum ModulePositions
    {
        Up = 0,
        Down,
    }

    public interface IModuleInformation
    {
        string ModuleID { get; set; }

        string ModuleName { get; set; }

        ModulePositions ModulePosition { get; set; } // UP or DOWN (TOP or BOTTOM)
    }
}
