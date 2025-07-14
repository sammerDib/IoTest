using System.Collections.Generic;

using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.Semi.Gem.Abstractions.E30;
using Agileo.Semi.Gem300.Abstractions.E87;
using Agileo.SemiDefinitions;
using UnitySC.UTO.Controller.Remote.Constants;
using UnitySC.UTO.Controller.Remote.Services;

using LoadPort = UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort;

namespace UnitySC.UTO.Controller.Remote.Observers
{
    internal class MaterialMovementObserver : E30StandardSupport
    {
        #region Constructors
        public MaterialMovementObserver(IE30Standard e30Standard, ILogger logger)
            : base(e30Standard, logger)
        {
        }

        #endregion Constructors

        #region IInstanciableDevice Support

        public override void OnSetup(Agileo.EquipmentModeling.Equipment equipment)
        {
            base.OnSetup(equipment);

            if (App.ControllerInstance.GemController.IsSetupDone)
            {
                foreach (var loadPort in Equipment.AllOfType<LoadPort>())
                {
                    loadPort.StatusValueChanged += LoadPort_StatusValueChanged;
                }
            }
            else
            {
                Logger.Warning("From GEM part - Unable to subscribe on EventHandler in Material Observer");
            }
        }

        #endregion IInstanciableDevice Support

        #region Event Handlers

        private void LoadPort_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            if (sender is not LoadPort loadPort)
            {
                return;
            }

            switch (e.Status.Name)
            {
                case nameof(LoadPort.CarrierPresence):
                    switch (loadPort.CarrierPresence)
                    {
                        case CassettePresence.Correctly:
                            E30Standard.MaterialMovementServices.NotifyMaterialReceived(new[]
                            {
                                new VariableUpdate(WellknownNames.DVs.MaterialID, string.Empty),
                                new VariableUpdate(E87WellknownNames.DVs.PortID, (byte)loadPort.InstanceId)
                            });

                            //Send event
                            if ((CassettePresence)e.OldValue is not CassettePresence.Correctly)
                            {
                                E30Standard.DataServices.SendEvent(CEIDs.CustomEvents.CarrierPlacementSensorOn,
                                    new List<VariableUpdate>()
                                    {
                                        new(E87WellknownNames.DVs.PortID, (byte)loadPort.InstanceId)
                                    });
                            }
                            break;
                        case CassettePresence.Absent:
                            E30Standard.MaterialMovementServices.NotifyMaterialRemoved(
                                new List<VariableUpdate>());
                            break;
                        case CassettePresence.PresentNoPlacement:
                            //Send event
                            if ((CassettePresence)e.OldValue is not CassettePresence.Correctly)
                            {
                                E30Standard.DataServices.SendEvent(
                                    CEIDs.CustomEvents.CarrierPresenceSensorOn,
                                    new List<VariableUpdate>()
                                    {
                                        new(E87WellknownNames.DVs.PortID, (byte)loadPort.InstanceId)
                                    });
                            }
                            break;
                    }
                    break;
            }
        }

        #endregion Event Handlers

        #region Send Collection Event Methods

        #endregion Send Collection Event Methods

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing && App.ControllerInstance.GemController.IsSetupDone)
            {
                // dispose equipment StatusChanged event here
            }
            base.Dispose(disposing);
        }

        #endregion Dispose
    }
}
