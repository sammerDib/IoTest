using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

using Agileo.Common.Configuration;
using Agileo.EquipmentModeling;
using Agileo.ModelingFramework;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.ProcessModule.Conditions;
using UnitySC.Equipment.Abstractions.Devices.ProcessModule.Configuration;
using UnitySC.Equipment.Abstractions.Devices.ProcessModule.Resources;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice;
using UnitySC.Equipment.Abstractions.Vendor.Material;

namespace UnitySC.Equipment.Abstractions.Devices.ProcessModule
{
    public partial class ProcessModule : IExtendedConfigurableDevice
    {
        #region Properties

        public WaferLocation Location { get; protected set; }

        #endregion Properties

        #region Setup

        private void InstanceInitialization()
        {
            Location = new WaferLocation($"{Name} substrate location");
            MaterialLocations =
                ReferenceFactory.OneToManyComposition<MaterialLocation>(
                    nameof(MaterialLocations),
                    this);
        }

        public override void SetUp(SetupPhase phase)
        {
            base.SetUp(phase);
            switch (phase)
            {
                case SetupPhase.AboutToSetup:
                    StatusValueChanged += ProcessModule_StatusValueChanged;
                    DeviceType.AddPrecondition(
                        nameof(IGenericDevice.Initialize),
                        new IsInService(),
                        Logger);
                    DeviceType.AddPrecondition(
                        nameof(IUnityCommunicatingDevice.StartCommunication),
                        new IsInService(),
                        Logger);
                    DeviceType.AddPrecondition(
                        nameof(IUnityCommunicatingDevice.StartCommunication),
                        new IsInService(),
                        Logger);
                    break;
                case SetupPhase.SettingUp:
                    MaterialLocations.Add(Location);
                    Location.PropertyChanged += Location_PropertyChanged;
                    ConfigManager.CurrentChanged += ConfigManager_CurrentChanged;
                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion Setup

        #region IMaterialLocationContainer

        public OneToManyComposition<MaterialLocation> MaterialLocations { get; protected set; }

        public SampleDimension GetMaterialDimension(byte slot = 1)
        {
            return Location.Substrate?.MaterialDimension ?? SampleDimension.NoDimension;
        }

        public virtual bool IsReadyForTransfer(
            EffectorType effector,
            out List<string> errorMessages,
            Agileo.EquipmentModeling.Material armMaterial = null,
            byte slot = 1)
        {
            errorMessages = new List<string>();

            // Check that slot exists
            if (slot != 1)
            {
                errorMessages.Add(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Messages.ProcessModuleHaveOnlyOneSlot,
                        slot));
            }

            if (!IsDoorOpen)
            {
                errorMessages.Add(Messages.DoorClosed);
            }

            if (!IsReadyToLoadUnload)
            {
                errorMessages.Add(Messages.NotReadyToLoadUnload);
            }

            return errorMessages.Count == 0;
        }

        #endregion IMaterialLocationContainer

        #region IConfigurableDevice

        public IConfigManager ConfigManager { get; protected set; }

        /// <summary>
        /// Gets the device current configuration (<see cref="IConfigManager.Current" />).
        /// </summary>
        public ProcessModuleConfiguration Configuration
            => ConfigManager.Current.Cast<ProcessModuleConfiguration>();

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

        #region event handlers

        private void ProcessModule_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            if (e.Status.Name == nameof(IsOutOfService) && IsOutOfService)
            {
                SetState(OperatingModes.Maintenance);
            }
        }

        private void Location_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Location.Material))
            {
                if (Location.Substrate != null)
                {
                    WaferDimension = Location.Substrate.MaterialDimension;
                    SimplifiedWaferId = Location.Substrate.SimplifiedName;
                    WaferStatus = Location.Substrate.Status;
                    Location.Substrate.PropertyChanged += Substrate_PropertyChanged;
                }
                else
                {
                    WaferDimension = SampleDimension.NoDimension;
                    SimplifiedWaferId = string.Empty;
                    WaferStatus = WaferStatus.None;
                }
            }
        }

        private void Substrate_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Substrate.Status))
            {
                WaferStatus = Location.Substrate?.Status ?? WaferStatus.None;
            }
        }

        private void ConfigManager_CurrentChanged(object sender, ConfigurationChangedEventArgs e)
        {
            if (e.NewConfiguration is not ProcessModuleConfiguration configuration)
            {
                return;
            }

            IsOutOfService = configuration.IsOutOfService;
        }

        #endregion

        #region protected

        protected virtual void ResetInterlocksStatuses()
        {
            IsDoorOpen = false;
            IsReadyToLoadUnload = false;
        }

        #endregion

        #region Public

        public virtual void CheckSubstrateDetectionError(bool reset = false)
        {
            if ((WaferPresence == WaferPresence.Present && Location.Wafer != null)
                || (WaferPresence == WaferPresence.Absent && Location.Wafer == null)
                || reset)
            {
                SubstrateDetectionError = false;
            }
            else
            {
                SubstrateDetectionError = true;
            }
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Location.PropertyChanged -= Location_PropertyChanged;
                if (Location.Substrate != null)
                {
                    Location.Substrate.PropertyChanged -= Substrate_PropertyChanged;
                }

                StatusValueChanged -= ProcessModule_StatusValueChanged;
                ConfigManager.CurrentChanged -= ConfigManager_CurrentChanged;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
