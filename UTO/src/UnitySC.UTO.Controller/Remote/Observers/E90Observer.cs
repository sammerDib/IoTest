using System;
using System.Linq;

using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.Semi.Gem.Abstractions.E30;
using Agileo.Semi.Gem300.Abstractions.E40;
using Agileo.Semi.Gem300.Abstractions.E87;
using Agileo.Semi.Gem300.Abstractions.E90;

using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.JobDefinition;
using UnitySC.Equipment.Abstractions.Vendor.Material;
using UnitySC.Equipment.Devices.Controller.Activities.WaferFlow.Enum;
using UnitySC.UTO.Controller.Counters;
using UnitySC.UTO.Controller.Remote.Constants;
using UnitySC.UTO.Controller.Remote.Services;

using Carrier = UnitySC.Equipment.Abstractions.Material.Carrier;
using Substrate = UnitySC.Equipment.Abstractions.Vendor.Material.Substrate;

namespace UnitySC.UTO.Controller.Remote.Observers
{
    internal class E90Observer : E30StandardSupport
    {
        #region Fields

        private readonly IE90Standard _e90Standard;
        private readonly IE87Standard _e87Standard;
        private Equipment.Devices.Controller.Controller _controller;

        #endregion

        #region Constructor

        public E90Observer(IE90Standard e90Standard, IE87Standard e87Standard, IE30Standard e30Standard, ILogger logger)
            : base(e30Standard, logger)
        {
            _e90Standard = e90Standard;
            _e87Standard = e87Standard;
        }

        #endregion

        #region Overrides

        public override void OnSetup(Agileo.EquipmentModeling.Equipment equipment)
        {
            base.OnSetup(equipment);

            _controller = Equipment.AllOfType<Equipment.Devices.Controller.Controller>().First();
            _controller.MaterialManager.MaterialMoved += MaterialManager_MaterialMoved;
            _controller.JobStatusChanged += Controller_JobStatusChanged;
            _controller.SubstrateIdReadingHasBeenFinished += Controller_SubstrateIdReadingHasBeenFinished;
            _e90Standard.SubstrateInstantiated += E90Standard_SubstrateInstantiated;
            _e90Standard.SubstrateReadingStateChanged += E90Standard_SubstrateReadingStateChanged;
            _e87Standard.TransferStateChanged += E87Standard_TransferStateChanged;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _controller.MaterialManager.MaterialMoved -= MaterialManager_MaterialMoved;
                _controller.JobStatusChanged -= Controller_JobStatusChanged;
                _controller.SubstrateIdReadingHasBeenFinished -= Controller_SubstrateIdReadingHasBeenFinished;
                _e90Standard.SubstrateInstantiated -= E90Standard_SubstrateInstantiated;
                _e90Standard.SubstrateReadingStateChanged -= E90Standard_SubstrateReadingStateChanged;
                _e87Standard.TransferStateChanged -= E87Standard_TransferStateChanged;
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Event Handlers

        private void MaterialManager_MaterialMoved(object sender, MaterialMovedEventArgs e)
        {
            if (e.Material is not Substrate
                || _controller.State is OperatingModes.Maintenance or OperatingModes.Engineering
                || e.OldLocation == null
                || e.NewLocation == null)
            {
                return;
            }

            try
            {
                var substrateId = _e90Standard.GetSubstrateLocation(e.OldLocation.Name).SubstID;
                if (e.NewLocation.Container is Carrier)
                {
                    _e90Standard.IntegrationServices.NotifySubstrateTransportAtDestination(substrateId, e.NewLocation.Name);
                }
                else
                {
                    _e90Standard.IntegrationServices.NotifySubstrateTransportAtWork(substrateId, e.NewLocation.Name);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error occurred in E90 services.");
            }
        }

        private void Controller_JobStatusChanged(object sender, Equipment.Devices.Controller.EventArgs.JobStatusChangedEventArgs e)
        {
            if (e.Job == null || _controller.State is OperatingModes.Maintenance or OperatingModes.Engineering)
            {
                return;
            }

            switch (e.Job.Status)
            {
                case JobStatus.Created:
                    foreach (var substrate in e.Job.Wafers)
                    {
                        substrate.PropertyChanged += Substrate_PropertyChanged;
                    }
                    break;
                case JobStatus.Stopping:
                    foreach (var substrate in e.Job.Wafers)
                    {
                        var substrateId = _e90Standard.GetSubstrateLocation(substrate.Location.Name).SubstID;
                        var stopConfig = E30Standard.EquipmentConstantsServices.GetValueByWellKnownName(ECs.StopConfig).ValueTo<StopConfig>();

                        switch (stopConfig)
                        {
                            case StopConfig.CancelProcess:
                            case StopConfig.FinishProcessForWaferInPm:
                                //In this case, wafers that are waiting process won't be processed so they are skipped
                                if (substrate.Status == WaferStatus.WaitingProcess)
                                {
                                    _e90Standard.IntegrationServices.NotifySubstrateProcessing(substrateId, IsWaferUsedByOtherPj(substrateId) ? SubstProcState.NeedsProcessing : SubstProcState.Skipped);
                                }
                                break;
                            case StopConfig.FinishProcessForAllWafersOnTools:
                                //In this case, only wafer still in carrier can be skipped, the others will be processed
                                if (substrate.Status == WaferStatus.WaitingProcess
                                    && substrate.Location is CarrierSubstrateLocation)
                                {
                                    _e90Standard.IntegrationServices.NotifySubstrateProcessing(substrateId, IsWaferUsedByOtherPj(substrateId) ? SubstProcState.NeedsProcessing : SubstProcState.Skipped);
                                }
                                break;
                        }
                    }
                    break;
                case JobStatus.Completed:
                case JobStatus.Failed:
                    foreach (var substrate in e.Job.Wafers)
                    {
                        substrate.PropertyChanged -= Substrate_PropertyChanged;
                    }
                    break;
            }
        }

        private void Substrate_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is not Substrate substrate
                || e.PropertyName != nameof(Substrate.Status)
                || _controller.State is OperatingModes.Maintenance or OperatingModes.Engineering)
            {
                return;
            }

            try
            {
                var substrateId = _e90Standard.GetSubstrateLocation(substrate.Location.Name).SubstID;
                switch (substrate.Status)
                {
                    case WaferStatus.WaitingProcess:
                        _e90Standard.IntegrationServices.NotifySubstrateProcessing(substrateId, SubstProcState.NeedsProcessing);
                        break;
                    case WaferStatus.Processing:
                        _e90Standard.IntegrationServices.NotifySubstrateProcessing(substrateId, SubstProcState.InProcess);
                        break;
                    case WaferStatus.Processed:
                        _e90Standard.IntegrationServices.NotifySubstrateProcessing(substrateId, IsWaferUsedByOtherPj(substrateId) ? SubstProcState.NeedsProcessing : SubstProcState.Processed);
                        App.ControllerInstance.CountersManager.IncrementCounter(CounterDefinition.ProcessedSubstrateCounter);
                        break;
                    case WaferStatus.ProcessingFailed:
                        _e90Standard.IntegrationServices.NotifySubstrateProcessing(substrateId, IsWaferUsedByOtherPj(substrateId) ? SubstProcState.NeedsProcessing : SubstProcState.Rejected);
                        break;
                    case WaferStatus.Aborted:
                        _e90Standard.IntegrationServices.NotifySubstrateProcessing(substrateId, SubstProcState.Aborted);
                        break;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error occurred in E90 services.");
            }
        }

        private void E90Standard_SubstrateInstantiated(object sender, SubstrateEventArgs e)
        {
            if (_e90Standard.GetSubstrate(e.Substrate.ObjID) is { } substrate)
            {
                substrate.SubstType = SubstType.Wafer;
                substrate.SubstUsage = SubstUsage.Product;
            }

            if (App.ControllerInstance.ControllerEquipmentManager.LoadPorts.Values.FirstOrDefault(
                    lp => lp.Carrier != null && e.LocationId.Contains(lp.Carrier.Name)) is not { } deviceLoadPort)
            {
                return;
            }

            if (deviceLoadPort.Carrier.MaterialLocations.FirstOrDefault(
                    ml => ml.Name.Equals(e.LocationId)) is not { } materialLocation)
            {
                return;
            }

            if (materialLocation.Material is not Wafer wafer)
            {
                return;
            }

            wafer.SubstrateId = e.Substrate.ObjID;
            wafer.LotId = e.Substrate.LotID;
            wafer.EquipmentID = _e90Standard.E30Standard.EquipmentConstantsServices
                .GetValueByWellKnownName(ECs.EqpName)
                .ValueTo<string>();
            wafer.DeviceID = _e90Standard.E30Standard.Connection.Configuration.DeviceID.ToString();


            //Check processjob
            foreach (var job in Helpers.Helpers.CheckSettingUpJobDone())
            {
                App.UtoInstance.GemController.SetupJob(job);
            }
        }

        private void E90Standard_SubstrateReadingStateChanged(object sender, SubstrateReadingStateChangedEventArgs e)
        {
            if (e.NewState is SubstIDStatus.Confirmed)
            {
                App.ControllerInstance.ControllerEquipmentManager.Aligner.Location.Wafer
                    .AcquiredId = e.Substrate.AcquiredID;
            }
        }

        private void Controller_SubstrateIdReadingHasBeenFinished(object sender, Equipment.Devices.Controller.EventArgs.SubstrateIdReadingHasBeenFinishedEventArgs e)
        {
            if (e is null)
            {
                return;
            }

            try
            {
                _e90Standard.IntegrationServices.NotifySubstrateReadingDone(e.IsSuccess, e.SubstrateId, e.AcquiredId);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error occurred in E90 services.");
            }

            if (IsLocalMode() && _e90Standard.GetSubstrate(e.SubstrateId) is { } substrate)
            {
                switch (substrate.SubstIDStatus)
                {
                    case SubstIDStatus.NotConfirmed:
                        break;
                    case SubstIDStatus.WaitingForHost:
                        if (e.IsSuccess && !e.AcquiredId.Contains("*"))
                        {
                            _e90Standard.StandardServices.ProceedWithSubstrate(
                                substrate.ObjID,
                                substrate.SubstLocID);
                        }
                        else
                        {
                            _e90Standard.StandardServices.CancelSubstrate(
                                substrate.ObjID,
                                substrate.SubstLocID);
                        }
                        break;
                    case SubstIDStatus.Confirmed:
                        _controller.ProceedWithSubstrate();
                        break;
                    case SubstIDStatus.ConfirmationFailed:
                        _controller.CancelSubstrate();
                        break;
                }
            }
        }

        private void E87Standard_TransferStateChanged(object sender, TransferStateChangedArgs e)
        {
            var associatedCarrier = e.LoadPort.AssociatedCarrier;
            if (associatedCarrier is null)
            {
                return;
            }

            // Gets substrates located in carrier which needs processing
            var needsProcessingSubstrates = _e90Standard
                .GetCarrierSubstrates(associatedCarrier.ObjID)
                .Where(s => s.SubstProcState == SubstProcState.NeedsProcessing)
                .ToArray();

            // For each substrates set processing state to SKIPPED
            foreach (var substrate in needsProcessingSubstrates)
            {
                _e90Standard.IntegrationServices.NotifySubstrateProcessing(
                    substrate.ObjID,
                    SubstProcState.Skipped);
            }
        }

        #endregion

        #region Private

        private bool IsWaferUsedByOtherPj(string substrateId)
        {
            var jobList = App.UtoInstance.JobQueueManager.JobQueue.Where(j => j.Wafers.Find(w => w.SubstrateId == substrateId) != null).ToList();
            return jobList.Count > 1 || jobList.Any(j => (j.LoopMode && j.ProcessJob.JobState != JobState.STOPPING) || j.CurrentExecution < j.NumberOfExecutions);
        }

        #endregion

        #region Public

        public void ClearSubstrates()
        {
            foreach (var substrate in _e90Standard.Substrates.Where(w => w.SubstLocID != w.SubstSource).ToList())
            {
                _e90Standard.DisposeSubstrate(substrate);
            }
        }

        #endregion
    }
}
