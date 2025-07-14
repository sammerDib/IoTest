using System;

using Agileo.Drivers;

namespace UnitySC.Equipment.Abstractions.Drivers.Common
{
    public interface IExtendedDeviceDriver : IDeviceDriver
    {
        /// <summary>
        /// Occurs when command is interrupted on driver.
        /// </summary>
        event EventHandler CommandInterrupted;
    }
}
