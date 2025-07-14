using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.ANA.Service.Interface
{
    public interface IDualLiseInputParams : ILiseInputParams
    {
        SingleLiseInputParams ProbeUpParams { get; set; }
        SingleLiseInputParams ProbeDownParams { get; set; }

        // Contains the ID of the top probe or the bottom probe
        string CurrentProbeAcquisition { get; set; }

        ModulePositions CurrentProbeModule { get; set; }
    }
}
