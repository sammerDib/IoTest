using Agileo.Common.Logging;
using Agileo.Semi.Gem.Abstractions.E30;
using Agileo.Semi.Gem.Abstractions.E30.Capabilities;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Devices.Controller;
using UnitySC.UTO.Controller.Remote.Constants;
using UnitySC.UTO.Controller.Remote.Services;

namespace UnitySC.UTO.Controller.Remote.Observers
{
    internal class ProcessingSmObserver : E30StandardSupport
    {
        #region Fields

        private IRemoteControlCapability RemoteControlServices => App.ControllerInstance.GemController.E30Std.RemoteControlServices;

        private string CurrentHostSmState => RemoteControlServices?.ProcessState;

        private string PreviousHostSmState => RemoteControlServices?.PreviousProcessState;

        #endregion

        #region Constructor

        public ProcessingSmObserver(IE30Standard e30Standard, ILogger logger) : base(e30Standard, logger)
        {
        }

        #endregion

        #region IInstanciableDevice Support

        public override void OnSetup(Agileo.EquipmentModeling.Equipment equipment)
        {
            base.OnSetup(equipment);

            App.ControllerInstance.ControllerEquipmentManager.Controller.StatusValueChanged +=
                Controller_StatusValueChanged;

            /* Subscribe on Host Processing SM state changed */
            RemoteControlServices.ProcessStateChanged += RemoteControlServices_ProcessStateChanged;
        }

        private void RemoteControlServices_ProcessStateChanged(object sender, ProcessStateChangedEventArgs e)
        {
            LogStateMachineTransition();
        }

        #endregion

        #region Event Handlers

        private void Controller_StatusValueChanged(object sender, Agileo.EquipmentModeling.StatusChangedEventArgs e)
        {
            if (e.Status.Name == nameof(IController.State))
            {
                E30Standard.RemoteControlServices.NotifyProcessingStateChanged(
                    App.ControllerInstance.ControllerEquipmentManager.Controller.State);

                switch (PreviousHostSmState)
                {
                    case nameof(OperatingModes.Maintenance):
                        switch (CurrentHostSmState)
                        {
                            case nameof(OperatingModes.Initialization):
                                E30Standard.DataServices.SendEvent(CEIDs.PSMEvents.MaintenanceToInitialization);
                                break;
                        }
                        break;

                    case nameof(OperatingModes.Initialization):
                        switch (CurrentHostSmState)
                        {
                            case nameof(OperatingModes.Maintenance):
                                E30Standard.DataServices.SendEvent(CEIDs.PSMEvents.InitializationToMaintenance);
                                break;

                            case nameof(OperatingModes.Idle):
                                E30Standard.DataServices.SendEvent(CEIDs.PSMEvents.InitializationToIdle);
                                break;
                        }
                        break;

                    case nameof(OperatingModes.Idle):
                        switch (CurrentHostSmState)
                        {
                            case nameof(OperatingModes.Maintenance):
                                E30Standard.DataServices.SendEvent(CEIDs.PSMEvents.IdleToMaintenance);
                                break;

                            case nameof(OperatingModes.Executing):
                                E30Standard.DataServices.SendEvent(CEIDs.PSMEvents.IdleToExecuting);
                                break;
                        }
                        break;

                    case nameof(OperatingModes.Executing):
                        switch (CurrentHostSmState)
                        {
                            case nameof(OperatingModes.Maintenance):
                                E30Standard.DataServices.SendEvent(CEIDs.PSMEvents.ExecutingToMaintenance);
                                break;

                            case nameof(OperatingModes.Idle):
                                E30Standard.DataServices.SendEvent(CEIDs.PSMEvents.ExecutingToIdle);
                                break;
                        }
                        break;
                }
            }
        }

        #endregion

        #region Private Methods

        private void LogStateMachineTransition()
        {
            Logger.Debug($"Host SM   - {PreviousHostSmState} --> {CurrentHostSmState}");
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                App.ControllerInstance.ControllerEquipmentManager.Controller.StatusValueChanged +=
                    Controller_StatusValueChanged;

                if (App.ControllerInstance.GemController.IsSetupDone)
                {
                    RemoteControlServices.ProcessStateChanged -= RemoteControlServices_ProcessStateChanged;
                }
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
