using Agileo.Common.Configuration;
using Agileo.EquipmentModeling;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices
{
    public interface IConfigurableDevice
    {
        /// <summary>
        /// Relative path to the directory where the configuration will be loaded.
        /// </summary>
        string RelativeConfigurationDir { get; }

        /// <summary>
        /// Loads the device configuration.
        /// </summary>
        /// <param name="deviceConfigRootPath">The path to root folder containing Devices configuration.</param>
        void LoadConfiguration(string deviceConfigRootPath = "");

        /// <summary>
        /// Sets the device execution mode (real or simulated).
        /// </summary>
        /// <param name="executionMode">The execution mode of the device.</param>
        void SetExecutionMode(ExecutionMode executionMode);
    }

    public interface IConfigurableDevice<out T> : IConfigurableDevice
        where T : IConfiguration
    {
        T CreateDefaultConfiguration();
    }
}
