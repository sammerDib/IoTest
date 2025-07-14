using System;

using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.ANA.Service.Interface
{
    [Serializable]
    public class ProbeModuleSettings
    {
        /// <summary>Probe position (UP or DOWN)</summary>
        public ModulePositions Position { get; set; }
    }
}