using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

using Agileo.Common.Configuration;
using Agileo.EquipmentModeling;
using Agileo.ModelingFramework;
using Agileo.SemiDefinitions;

using UnitsNet;

using UnitySC.Equipment.Abstractions.Devices.Aligner.Configuration;
using UnitySC.Equipment.Abstractions.Devices.Aligner.Enums;
using UnitySC.Equipment.Abstractions.Devices.Aligner.Resources;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Material;

using GenericDeviceMessages =
    UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Resources.Messages;

namespace UnitySC.Equipment.Abstractions.Devices.Aligner
{
    public partial class Aligner : IExtendedConfigurableDevice
    {
        #region Properties

        public WaferLocation Location { get; protected set; }

        #endregion Properties

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

        #region private

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

        #endregion

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
                    LoadConfiguration();
                    break;
                case SetupPhase.SettingUp:
                    MaterialLocations.Add(Location);
                    Location.PropertyChanged += Location_PropertyChanged;
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

        #region Aligner Commands

        protected abstract void InternalAlign(Angle target, AlignType alignType);

        protected abstract void InternalCentering();

        protected abstract void InternalPrepareTransfer(
            EffectorType effector,
            SampleDimension dimension,
            MaterialType materialType);

        protected abstract void InternalSetDateAndTime();

        protected abstract void InternalClamp();

        protected abstract void InternalUnclamp();

        protected abstract void InternalMoveZAxis(bool isBottom);

        #endregion Aligner Commands

        #region IMaterialLocationContainer

        public OneToManyComposition<MaterialLocation> MaterialLocations { get; protected set; }

        public SampleDimension GetMaterialDimension(byte slot = 1)
        {
            return Location.Substrate?.MaterialDimension ?? SampleDimension.NoDimension;
        }

        public MaterialType GetMaterialType(byte slot = 1)
        {
            return Location.Wafer?.MaterialType ?? MaterialType.SiliconWithNotch;
        }

        public virtual bool IsReadyForTransfer(
            EffectorType effector,
            out List<string> errorMessages,
            Agileo.EquipmentModeling.Material armMaterial = null,
            byte slot = 1)
        {
            errorMessages = new List<string>();

            // Check we're not already busy with something else
            if (State is OperatingModes.Executing or OperatingModes.Initialization)
            {
                errorMessages.Add(GenericDeviceMessages.AlreadyBusy);
            }

            // Check that slot exists
            if (slot != 1)
            {
                errorMessages.Add(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Messages.AlignerHaveOnlyOneSlot,
                        slot));
            }

            // If there is a material on arm, check it can fit into the aligner
            if (armMaterial != null)
            {
                if (Location.Material != null)
                {
                    errorMessages.Add(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Messages.AlignerAlreadyHaveMaterial,
                            slot));
                }
            }
            else
            {
                // If there is no material on arm, check aligner can provide one
                if (Location.Material == null)
                {
                    errorMessages.Add(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Messages.AlignerHaveNoMaterial,
                            slot));
                }
            }

            //Check wafer presence has no conflict
            if (SubstrateDetectionError)
            {
                errorMessages.Add(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Messages.SubstrateDetectionError,
                        slot));
            }

            return errorMessages.Count == 0;
        }

        #endregion IMaterialLocationContainer

        #region IConfigurableDevice

        public IConfigManager ConfigManager { get; protected set; }

        /// <summary>
        /// Gets the device current configuration (<see cref="IConfigManager.Current" />).
        /// </summary>
        public AlignerConfiguration Configuration
            => ConfigManager.Current.Cast<AlignerConfiguration>();

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
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
