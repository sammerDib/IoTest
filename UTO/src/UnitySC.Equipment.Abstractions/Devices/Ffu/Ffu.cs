using Agileo.Common.Configuration;
using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.Ffu.Configuration;
using UnitySC.Equipment.Abstractions.Devices.Ffu.Enum;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;

namespace UnitySC.Equipment.Abstractions.Devices.Ffu
{
    public partial class Ffu : IExtendedConfigurableDevice
    {
        #region Public

        public abstract string IsFfuSpeedValid(double setPoint, FfuSpeedUnit unit);

        #endregion

        #region Setup

        private void InstanceInitialization()
        {
            // Default configure the instance.
            // Call made from the constructor.
        }

        #endregion Setup

        #region IConfigurableDevice

        public IConfigManager ConfigManager { get; protected set; }

        /// <summary>
        /// Gets the device current configuration (<see cref="IConfigManager.Current" />).
        /// </summary>
        public FfuConfiguration Configuration => ConfigManager.Current.Cast<FfuConfiguration>();

        /// <inheritdoc />
        public abstract string RelativeConfigurationDir { get; }

        /// <inheritdoc />
        public abstract void LoadConfiguration(string deviceConfigRootPath = "");

        /// <inheritdoc />
        public void SetExecutionMode(ExecutionMode executionMode) => ExecutionMode = executionMode;

        public FfuConfiguration CreateDefaultConfiguration() => new();

        #endregion IConfigurableDevice

        #region FFU Commands

        protected abstract void InternalSetDateAndTime();

        protected abstract void InternalSetFfuSpeed(double setPoint, FfuSpeedUnit unit);

        #endregion FFU Commands
    }
}
