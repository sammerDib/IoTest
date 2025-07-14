using System;
using System.Linq;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.ProcessModule.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;

namespace UnitySC.EfemController.ProcessModules.Devices.ProcessModule.EfemControllerProcessModule
{
    public partial class EfemControllerProcessModule
        : IConfigurableDevice<ProcessModuleConfiguration>
    {
        #region Fields

        private IProcessModuleIos IoModule { get; set; }

        #endregion

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
                    break;
                case SetupPhase.SettingUp:
                    var ioModule = this.GetTopDeviceContainer()
                        .AllDevices()
                        .FirstOrDefault(d => d is IProcessModuleIos);
                    if (ioModule is not IProcessModuleIos processModuleIos)
                    {
                        throw new InvalidOperationException(
                            $"Mandatory device of type {nameof(IProcessModuleIos)} is not found in equipment model tree.");
                    }

                    IoModule = processModuleIos;
                    IoModule.StatusValueChanged += IoModule_StatusValueChanged;
                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion Setup

        #region Configuration

        public new ProcessModuleConfiguration Configuration
            => base.Configuration.Cast<ProcessModuleConfiguration>();

        public ProcessModuleConfiguration CreateDefaultConfiguration()
        {
            return new ProcessModuleConfiguration();
        }

        public override string RelativeConfigurationDir
            => $"./Devices/{nameof(Equipment.Abstractions.Devices.ProcessModule.ProcessModule)}/{nameof(EfemControllerProcessModule)}/Resources";

        public override void LoadConfiguration(string deviceConfigRootPath = "")
        {
            ConfigManager ??= this.LoadDeviceConfiguration(
                deviceConfigRootPath,
                Logger,
                InstanceId);
        }

        #endregion

        #region ICommunicatingDevice Commands

        protected override void InternalStartCommunication()
        {
            IoModule.StartCommunication();
        }

        protected override void InternalStopCommunication()
        {
            IoModule.StopCommunication();
        }

        #endregion ICommunicatingDevice Commands

        #region Override

        public override void CheckSubstrateDetectionError(bool reset = false)
        {
            //No detection on this module
            SubstrateDetectionError = false;
        }

        #endregion

        #region Private

        private void RefreshInterlock()
        {
            switch (InstanceId)
            {
                case 1:
                    IsDoorOpen = IoModule.I_PM1_DoorOpened;
                    IsReadyToLoadUnload = IoModule.I_PM1_ReadyToLoadUnload;
                    break;
                case 2:
                    IsDoorOpen = IoModule.I_PM2_DoorOpened;
                    IsReadyToLoadUnload = IoModule.I_PM2_ReadyToLoadUnload;
                    break;
            }
        }

        #endregion

        #region Event Handlers

        private void IoModule_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status.Name)
            {
                case nameof(IProcessModuleIos.I_PM1_DoorOpened):
                case nameof(IProcessModuleIos.I_PM2_DoorOpened):
                case nameof(IProcessModuleIos.I_PM1_ReadyToLoadUnload):
                case nameof(IProcessModuleIos.I_PM2_ReadyToLoadUnload):
                    RefreshInterlock();
                    break;
                case nameof(IProcessModuleIos.IsCommunicating):
                    IsCommunicating = IoModule.IsCommunicating;
                    if (!IoModule.IsCommunicating)
                    {
                        ResetInterlocksStatuses();
                    }
                    else
                    {
                        RefreshInterlock();
                    }

                    break;
                case nameof(IProcessModuleIos.IsCommunicationStarted):
                    IsCommunicationStarted = IoModule.IsCommunicationStarted;
                    break;
            }
        }

        #endregion Event Handlers

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (IoModule != null)
                {
                    IoModule.StatusValueChanged -= IoModule_StatusValueChanged;
                    IoModule = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
