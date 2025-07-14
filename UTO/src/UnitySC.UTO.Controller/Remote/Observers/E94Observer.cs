using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.Semi.Gem.Abstractions.E30;
using Agileo.Semi.Gem300.Abstractions.E40;
using Agileo.Semi.Gem300.Abstractions.E87;
using Agileo.Semi.Gem300.Abstractions.E94;

using UnitySC.Equipment.Abstractions.Devices.Controller;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Shared.Tools.Collection;
using UnitySC.UTO.Controller.Remote.Services;

using Action = Agileo.Semi.Gem300.Abstractions.E94.Action;

namespace UnitySC.UTO.Controller.Remote.Observers
{
    internal class E94Observer : E30StandardSupport
    {
        #region Fields

        private readonly IE87Standard _e87Standard;
        private readonly IE94Standard _e94Standard;
        private Equipment.Devices.Controller.Controller _controller;

        #endregion

        #region Constructor

        public E94Observer(
            IE87Standard e87Standard,
            IE94Standard e94Standard,
            IE30Standard e30Standard,
            ILogger logger)
            : base(e30Standard, logger)
        {
            _e87Standard = e87Standard;
            _e94Standard = e94Standard;
        }

        #endregion

        #region override

        public override void OnSetup(Agileo.EquipmentModeling.Equipment equipment)
        {
            base.OnSetup(equipment);

            _e94Standard.ControlJobChanged += E94Std_ControlJobChanged;

            _controller = Equipment.AllOfType<Equipment.Devices.Controller.Controller>().First();
            _controller.StatusValueChanged += Controller_StatusValueChanged;
           
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _e94Standard.ControlJobChanged -= E94Std_ControlJobChanged;
                _controller.StatusValueChanged -= Controller_StatusValueChanged;
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Event Handlers

        private void E94Std_ControlJobChanged(object sender, ControlJobStateChangedEventArgs e)
        {
            if (e.NewState is State.EXECUTING
                && e.PreviousState is State.SELECTED or State.WAITINGFORSTART)
            {
                E30Standard.RemoteControlServices.NotifyProcessingStarted();
            }

            if (e.NewState is State.COMPLETED)
            {
                E30Standard.RemoteControlServices.NotifyProcessingCompleted();

                var isFinishedNormally =
                    e.ControlJob.ProcessJobStatusList.All(
                        elem => elem.State == JobState.PROCESSCOMPLETE);

                FinishCarrierAccessing(e.ControlJob, isFinishedNormally);
            }
        }

        private void Controller_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            if (e.Status.Name != nameof(IController.State)
                || (OperatingModes)e.NewValue is not OperatingModes.Maintenance and not OperatingModes.Engineering)
            {
                return;
            }

            foreach (var controlJob in _e94Standard.ControlJobs.Where(cj => cj.State is not State.QUEUED and not State.COMPLETED).ToList())
            {
                _e94Standard.StandardServices.Abort(controlJob.ObjID, Action.RemoveJobs);
            }
        }

        #endregion

        #region Private Methods

        private void FinishCarrierAccessing(IControlJob controlJob, bool isFinishedNormally)
        {
            // Iterates over each materials (carriers) of completed control job
            // If the material is used by another job do nothing.
            // If the material is IN ACCESS the accessing status is updated, otherwise the carrier is
            // cancelled.

            var carrierIds = new HashSet<string>();

            // Get all carriers linked to control job
            if (!controlJob.CarrierInputSpecifications.IsEmpty())
            {
                carrierIds = new HashSet<string>(controlJob.CarrierInputSpecifications);
            }
            else
            {
                foreach(var pjSpec in controlJob.ProcessingControlSpecifications)
                {
                    var pj = App.UtoInstance.GemController.E40Std.GetProcessJob(pjSpec.PRJobID);
                    if (pj != null)
                    {
                        foreach(var carrierId in pj.CarrierIDSlotsAssociation.Select(job=>job.CarrierID))
                        {
                            carrierIds.Add(carrierId);
                        }
                    }
                }
            }

            foreach (var specification in controlJob.MaterialOutSpecifications)
            {
                carrierIds.Add(specification.SourceMap.CarrierID);
                carrierIds.Add(specification.DestinationMap.CarrierID);
            }

            foreach (var carrierID in carrierIds)
            {
                var carrier = _e87Standard.GetCarrierById(carrierID);
                if (carrier is null)
                {
                    continue;
                }

                if (GetControlJobsAssociatedToCarrier(carrierID)
                    .Any(cj => cj.ObjID != controlJob.ObjID && cj.State != State.COMPLETED))
                {
                    continue;
                }

                if (carrier.CarrierAccessingStatus is AccessingStatus.InAccess)
                {
                    try
                    {
                        _e87Standard.IntegrationServices.NotifyCarrierAccessingHasBeenFinished(
                            carrierID,
                            isFinishedNormally);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, "Error occurred in E87 services.");
                    }
                }
                else
                {
                    _e87Standard.StandardServices.CancelCarrier(carrierID, carrier.PortID);
                }
            }
        }

        private IEnumerable<IControlJob> GetControlJobsAssociatedToCarrier(string carrierId)
        {
            return _e94Standard.ControlJobs
                .Where(
                    cj => cj.CarrierInputSpecifications.Contains(carrierId)
                          || cj.MaterialOutSpecifications.Any(
                              s => s.SourceMap.CarrierID.Contains(carrierId)
                                   || s.DestinationMap.CarrierID.Contains(carrierId)));
        }

        #endregion

        #region Public

        public void ClearControlJobs()
        {
            foreach (var controlJob in _e94Standard.ControlJobs.Where(cj=>cj.State != State.COMPLETED).ToList())
            {
                _e94Standard.StandardServices.Abort(controlJob.ObjID, Action.RemoveJobs);
            }
        }

        #endregion
    }
}
