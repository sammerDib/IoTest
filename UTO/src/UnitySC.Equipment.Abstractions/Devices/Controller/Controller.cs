using System.Collections.ObjectModel;

using Agileo.Common.Configuration;
using Agileo.EquipmentModeling;
using Agileo.ModelingFramework;

using UnitySC.Equipment.Abstractions.Devices.Controller.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;

namespace UnitySC.Equipment.Abstractions.Devices.Controller
{
    public partial class Controller : IExtendedConfigurableDevice
    {
        #region Fields

        protected ZeroToOneReference<MaterialManager> _materialManagerReference;

        #endregion Fields

        #region Properties

        // Used by ServiceMode to create the panel view.
        // ReSharper disable once UnusedMember.Global
        public MaterialManager MaterialManager => _materialManagerReference.Value;

        #endregion Properties

        #region Setup

        private void InstanceInitialization()
        {
            _materialManagerReference =
                ReferenceFactory.ZeroToOneReference<MaterialManager>("MaterialManager", this);
            _materialManagerReference.SetValue(new MaterialManager("MaterialManager", this));
        }

        #endregion

        #region Abstract Methods

        public abstract ReadOnlyCollection<MaterialLocation> PickSafeLocations(
            Device device,
            DeviceCommand command,
            Parameter parameter);

        #endregion

        #region IConfigurableDevice

        public IConfigManager ConfigManager { get; protected set; }

        /// <summary>
        /// Gets the device current configuration (<see cref="IConfigManager.Current" />).
        /// </summary>
        public ControllerConfiguration Configuration
            => ConfigManager.Current.Cast<ControllerConfiguration>();

        /// <inheritdoc />
        public abstract string RelativeConfigurationDir { get; }

        /// <inheritdoc />
        public abstract void LoadConfiguration(string deviceConfigRootPath = "");

        /// <inheritdoc />
        public void SetExecutionMode(ExecutionMode executionMode)
        {
            ExecutionMode = executionMode;
        }

        #endregion IConfigurableDevice
    }
}
