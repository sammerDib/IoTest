using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.Common.Configuration;
using Agileo.EquipmentModeling;

using UnitsNet;

using UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;

namespace UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader
{
    public partial class SubstrateIdReader : IExtendedConfigurableDevice
    {
        #region Properties

        public List<RecipeModel> Recipes { get; } = new();
        public ReaderPositioner.ReaderPositioner Positioner { get; private set; }

        #endregion Properties

        #region Setup

        private void InstanceInitialization()
        {
            // Default configure the instance.
            // Call made from the constructor.
        }

        public override void SetUp(SetupPhase phase)
        {
            base.SetUp(phase);
            switch (phase)
            {
                case SetupPhase.AboutToSetup:
                    LoadConfiguration();
                    break;
                case SetupPhase.SettingUp:
                    Positioner = this.GetEquipment()
                        .AllDevices<ReaderPositioner.ReaderPositioner>()
                        .FirstOrDefault();
                    DeviceType.AllCommands().First(x => x.Name == nameof(Initialize)).Timeout =
                        Duration.FromSeconds(Configuration.InitializationTimeout);
                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion Setup

        #region SubstrateId Reader Commands

        protected override void InternalInitialize(bool mustForceInit)
        {
            base.InternalInitialize(mustForceInit);
            Positioner?.Initialize(mustForceInit);
            InternalRequestRecipes();
        }

        protected abstract void InternalRequestRecipes();

        protected abstract void InternalRead(string recipeName);

        protected abstract void InternalGetImage(string imagePath);

        #endregion SubstrateId Reader Commands

        #region Overrides

        protected override void HandleCommandExecutionStateChanged(CommandExecutionEventArgs e)
        {
            if (e.Execution.Context.Command.Name == nameof(ISubstrateIdReader.Read))
            {
                switch (e.NewState)
                {
                    case ExecutionState.Failed:
                        //Do not switch in maintenance mode if read command failed
                        SetState(OperatingModes.Idle);
                        return;
                }
            }

            base.HandleCommandExecutionStateChanged(e);
        }

        #endregion

        #region IConfigurableDevice

        public IConfigManager ConfigManager { get; protected set; }

        /// <summary>
        /// Gets the device current configuration (<see cref="IConfigManager.Current" />).
        /// </summary>
        public SubstrateIdReaderConfiguration Configuration
            => ConfigManager.Current.Cast<SubstrateIdReaderConfiguration>();

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
