using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.Semi.Gem.Abstractions.E30;
using Agileo.Semi.Gem300.Abstractions.E40;

using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.EventArgs;
using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager;
using UnitySC.Equipment.Abstractions.Vendor.JobDefinition;
using UnitySC.UTO.Controller.Remote.Services;

namespace UnitySC.UTO.Controller.Remote.Observers
{
    internal class E40Observer : E30StandardSupport
    {
        #region Fields

        private readonly IE40Standard _e40Standard;
        private Equipment.Devices.Controller.Controller _controller;
        private AbstractDataFlowManager _dataFlowManager;

        #endregion

        #region Constructor

        public E40Observer(IE40Standard e40Standard, IE30Standard e30Standard, ILogger logger)
            : base(e30Standard, logger)
        {
            _e40Standard = e40Standard;
        }

        #endregion

        #region Overrides

        public override void OnSetup(Agileo.EquipmentModeling.Equipment equipment)
        {
            base.OnSetup(equipment);

            _controller = Equipment.AllOfType<Equipment.Devices.Controller.Controller>().First();
            _controller.JobStatusChanged += ControllerJobStatusChanged;

            _dataFlowManager = Equipment.AllOfType<AbstractDataFlowManager>().First();
            _dataFlowManager.ProcessModuleAcquisitionCompleted += DataFlowManager_ProcessModuleAcquisitionCompleted;

            _e40Standard.ProcessJobDisposed += E40Standard_ProcessJobDisposed;
            _e40Standard.ProcessJobChanged += E40Standard_ProcessJobChanged;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _controller.JobStatusChanged -= ControllerJobStatusChanged;
                _dataFlowManager.ProcessModuleAcquisitionCompleted -= DataFlowManager_ProcessModuleAcquisitionCompleted;
                _e40Standard.ProcessJobDisposed -= E40Standard_ProcessJobDisposed;
                _e40Standard.ProcessJobChanged -= E40Standard_ProcessJobChanged;
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Event Handlers

        private void ControllerJobStatusChanged(object sender, Equipment.Devices.Controller.EventArgs.JobStatusChangedEventArgs e)
        {
            try
            {
                if (e.Job.Status is JobStatus.Completed)
                {
                    _e40Standard.IntegrationServices.NotifyMaterialProcessIsComplete(e.Job.Name);
                }

                if (e.Job.Status is JobStatus.Stopped)
                {
                    _e40Standard.IntegrationServices.NotifyJobHasBeenStopped(e.Job.Name);
                }

                if (e.Job.Status is JobStatus.Paused)
                {
                    _e40Standard.IntegrationServices.NotifyJobHasBeenPaused(e.Job.Name);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error occurred in E40 services.");
            }
        }

        private void E40Standard_ProcessJobChanged(object sender, ProcessJobStateChangedEventArgs e)
        {
            switch (e.NewState)
            {
                case JobState.PROCESSCOMPLETE:
                    _dataFlowManager.SignalEndOfProcessJob(e.ProcessJob.ObjID);
                    break;
            }
        }

        private void E40Standard_ProcessJobDisposed(object sender, ProcessJobEventArgs e)
        {
            _dataFlowManager.SignalEndOfProcessJob(e.ProcessJob.ObjID);
        }

        private void DataFlowManager_ProcessModuleAcquisitionCompleted(
            object sender, ProcessModuleRecipeEventArgs e)
        {
            //Wait result watchdog
            Task.Run(
                () =>
                {
                    var jobId = e.ProcessModule.Location.Wafer.ProcessJobId;
                    var substId = e.ProcessModule.Location.Wafer.SubstrateId;

                    if (_controller.Jobs.FirstOrDefault(j => j.Name == jobId && j.Status == JobStatus.Executing) is not { } job)
                    {
                        //job already disposed
                        return;
                    }
                    
                    var startTime = DateTime.Now;
                    while ((DateTime.Now - startTime).TotalMinutes < App.ControllerInstance.ControllerConfig.ResultReceptionTimeoutDuration)
                    {
                        if (job.WaferResultReceived.Contains(substId))
                        {
                            // result received
                            return;
                        }

                        //Check every 1sec
                        Thread.Sleep(1000);
                    }

                    //abort job (no result received)
                    Logger.Error($"Abort PJ {jobId}: No result received for subst {substId}.");

                    _e40Standard.StandardServices.Command(
                        jobId,
                        CommandName.ABORT,
                        new List<CommandParameter>());
                });
        }

        #endregion

        #region Public

        public void ClearProcessJob()
        {
            foreach (var processJob in _e40Standard.ProcessJobs.Where(pj=>pj.JobState == JobState.QUEUED_POOLED).ToList())
            {
                _e40Standard.StandardServices.Command(
                    processJob.ObjID,
                    CommandName.CANCEL,
                    new List<CommandParameter>());
            }
        }

        #endregion
    }
}
