using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.Semi.Communication.Abstractions.E5;
using Agileo.Semi.Gem300.Abstractions.E40;
using Agileo.Semi.Gem300.Abstractions.E87;
using Agileo.Semi.Gem300.Abstractions.E94;

using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Devices.Controller.JobDefinition;
using UnitySC.GUI.Common.Equipment.LoadPort;
using UnitySC.Shared.Data.Enum;
using UnitySC.UTO.Controller.Remote.Constants;
using UnitySC.UTO.Controller.Remote.Helpers;

using Carrier = UnitySC.Equipment.Abstractions.Material.Carrier;
using Recipe = Agileo.Semi.Gem300.Abstractions.E40.Recipe;

namespace UnitySC.UTO.Controller.JobQueuer
{
    public class JobQueueManager : IDisposable
    {
        #region Fields
        
        private static IE40Standard E40Std => App.ControllerInstance.GemController.E40Std;
        private static IE94Standard E94Std => App.ControllerInstance.GemController.E94Std;
        private static IE87Standard E87Std => App.ControllerInstance.GemController.E87Std;

        private readonly ILogger _logger;
        private readonly object _lockAddJob = new object();

        private readonly List<JobSpecification> _memJobQueue;
        #endregion

        #region Properties

        public List<JobSpecification> JobQueue { get; private set; }

        public ControllerEquipmentManager EquipmentManager { get; }

        public UnitySC.Equipment.Devices.Controller.Controller Controller { get; }

        #endregion

        #region Constructor

        public JobQueueManager()
        {
            _logger = Logger.GetLogger(nameof(JobQueueManager));

            JobQueue = new List<JobSpecification>();
            _memJobQueue = new List<JobSpecification>();

            EquipmentManager = App.ControllerInstance.ControllerEquipmentManager;

            if (EquipmentManager.Controller is not Equipment.Devices.Controller.Controller controller)
            {
                throw new InvalidCastException("Invalid Controller");
            }

            Controller = controller;
            Controller.LastWaferEntry += Controller_LastWaferEntry;

            E40Std.ProcessJobChanged += E40Std_ProcessJobChanged;
            E40Std.ProcessJobDisposed += E40Std_ProcessJobDisposed;
            E94Std.ControlJobInstantiated += E94Std_ControlJobInstantiated;
            E94Std.ControlJobDisposed += E94Std_ControlJobDisposed;
            E87Std.SlotMapStateChanged += E87Std_SlotMapStateChanged;
            
        }

        #endregion

        #region Public Methods

        public void SaveJobs()
        {
            if (!App.UtoInstance.ControllerConfig.JobRecreateAfterInit)
            {
                return;
            }

            _memJobQueue.Clear();
            foreach (var job in JobQueue.Where(j => j.ProcessJob.JobState == JobState.QUEUED_POOLED).ToList())
            {
                _memJobQueue.Add(job);
                _logger.Debug($"Mem Job: {job.ControlJob.ObjID}");
            }
        }

        public void RestoreJobs()
        {
            ClearJobQueue();

            if (!App.UtoInstance.ControllerConfig.JobRecreateAfterInit)
            {
                return;
            }

            foreach (var job in _memJobQueue)
            {
                JobQueue.Add(job);
                _logger.Debug($"Restore Job: {job.ControlJob.ObjID}");
                OnQueueChanged();

                StartJob(job);

                //Needed for unique pjId
                Thread.Sleep(1000);
            }
        }

        public void AddJob(
            bool loopMode,
            uint numberOfExecutions,
            Dictionary<Carrier, List<IndexedSlotState>> selectedSlotsByCarrier,
            string selectedRecipe,
            bool ocrReading,
            OcrProfile ocrProfile)
        {
            var materialNameList = new List<MaterialNameListElement>();
            var carrierInputSpec = new Collection<string>();
            var wafers = new List<Wafer>();

            foreach (var selectedSlots in selectedSlotsByCarrier)
            {
                Helpers.BuildSubstratesListByCarrier(selectedSlots.Key, selectedSlots.Value, wafers);
                Helpers.BuildInputSpecByCarrier(selectedSlots.Key, selectedSlots.Value, carrierInputSpec, materialNameList);
            }

            var newJob = new JobSpecification(
                materialNameList,
                carrierInputSpec,
                wafers,
                selectedRecipe,
                ocrReading,
                ocrProfile?.Name,
                loopMode,
                numberOfExecutions);

            JobQueue.Add(newJob);
            OnQueueChanged();

            StartJob(newJob);
        }

        public void RemoveJob(JobSpecification job)
        {
            if (JobQueue.Contains(job))
            {
                _logger.Debug($"Job with PjId {job.ProcessJob.ObjID} is removed from the queue");
                JobQueue.Remove(job);
                OnQueueChanged();
            }
        }

        public void ClearJobQueue()
        {
            _logger.Debug("Job queue is cleared");
            JobQueue.Clear();
            OnQueueChanged();
        }

        public bool IsCarrierStillNeeded(string carrierId)
        {
            var jobList = JobQueue.Where(
                    j => j.MaterialNameList.Any(
                        m => m.CarrierID == carrierId)).ToList();

            return jobList.Any(j => j.CurrentExecution < j.NumberOfExecutions || j.LoopMode);
        }
        #endregion

        #region Events

        public event EventHandler QueueChanged;
        private void OnQueueChanged()
        {
            _logger.Debug($"Job queue count changed => {JobQueue.Count}");
            QueueChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Private Methods

        private string GenerateId(List<string> idsAlreadyDefined, string prefix)
        {
            var dateTimeNow = DateTime.Now;
            var id = $"{prefix}_{dateTimeNow:yyyyMMddhhmmss}";
            var count = idsAlreadyDefined.Count(s => s.Contains(id));

            if (count != 0 )
            {
                count++;
                id += $"_{count}";
            }

            return id;
        }

        private void StartJob(JobSpecification job)
        {
            lock (_lockAddJob)
            {
                //$"PJ_{dateTimeNow:yyyyMMddhhmmss}"
                var prJobId = GenerateId(E40Std.ProcessJobs.Select(pj=>pj.ObjID).ToList(),"PJ");

                //$"CJ_{dateTimeNow:yyyyMMddhhmmss}"
                var ctrlJobId = GenerateId(E94Std.ControlJobs.Select(cj => cj.ObjID).ToList(), "CJ"); 

                var result = E40Std.StandardServices.CreateEnh(
                    prJobId,
                    job.MaterialNameList,
                    new Recipe()
                    {
                        ID = job.RecipeName,
                        Method = !job.OcrReading
                            ? RecipeMethod.RecipeOnly
                            : RecipeMethod.RecipeWithVariableTuning,
                        Variables = !job.OcrReading
                            ? new Collection<RecipeVariable>()
                            : new Collection<RecipeVariable>()
                            {
                                new(
                                    RecipeParameter.OcrProfileName,
                                    DataItem.FromObject(
                                        job.OcrProfileName,
                                        DataItemFormat.ASC)),
                            }
                    },
                    ProcessStart.AutomaticStart,
                    new List<string>());

                if (result.Status.IsFailure)
                {
                    JobQueue.Remove(job);
                    OnQueueChanged();
                    return;
                }

                job.ProcessJob = E40Std.GetProcessJob(prJobId);

                job.ControlJob = E94Std.AddControlJob(
                    ctrlJobId,
                    job.CarrierInputSpec,
                    new Collection<MaterialOutSpecification>(),
                    new Collection<ProcessingControlSpecification>()
                    {
                        new() { PRJobID = prJobId }
                    },
                    ProcessOrderManagement.LIST,
                    StartMethod.Auto);

                _logger.Debug(
                    $"A new job is started: CJ => {job.ControlJob.ObjID} / PJ => {job.ProcessJob.ObjID}");
            }
        }

        private void SelectNextCj()
        {
            if (App.ControllerInstance.ControllerConfig.DisableParallelControlJob
                || (App.ControllerInstance.ControllerConfig.UnloadCarrierBetweenJobs && !App.UtoInstance.GemController.IsControlledByHost ))
            {
                return;
            }

            if (Controller.TryGetDevice<AbstractDataFlowManager>() is { DataflowState: TC_DataflowStatus.Maintenance })
            {
                return;
            }

            //search next CJ to select or start
            if (E94Std.QueuedControlJobs.FirstOrDefault() is { } nextCjToSelect)
            {
                E94Std.IntegrationServices.Select(nextCjToSelect);
            }
        }

        #endregion

        #region Event Handlers

        private void Controller_LastWaferEntry(object sender, EventArgs e)
        {
            SelectNextCj();
        }

        private void E40Std_ProcessJobDisposed(object sender, ProcessJobEventArgs e)
        {
            foreach (var jobSpecification in JobQueue.ToList())
            {
                if (jobSpecification.ProcessJob == null)
                {
                    RemoveJob(jobSpecification);
                }
            }
        }

        private void E40Std_ProcessJobChanged(object sender, ProcessJobStateChangedEventArgs e)
        {
            var job = JobQueue.FirstOrDefault(j => j.ProcessJob.ObjID == e.ProcessJob.ObjID);
            switch (e.NewState)
            {
                case JobState.PROCESSCOMPLETE:
                    if (job != null)
                    {
                        if (job.LoopMode || job.CurrentExecution < job.NumberOfExecutions)
                        {
                            if (App.ControllerInstance.ControllerConfig.UnloadCarrierBetweenJobs)
                            {
                                return;
                            }

                            Task.Run(
                                () =>
                                {
                                    StartJob(job);
                                });
                        }
                        else
                        {
                            RemoveJob(job);
                        }
                    }

                    break;
                    
                case JobState.PROCESSING:
                    if (job != null && e.PreviousState is not JobState.PAUSED)
                    {
                        job.CurrentExecution++;
                        _logger.Debug($"Job with PjId {job.ProcessJob.ObjID}: Current execution number changed => {job.CurrentExecution}");
                    }

                    break;
                    
                case JobState.STOPPED:
                    if (job != null)
                    {
                        RemoveJob(job);
                    }
                    break;

                case JobState.STOPPING:
                    SelectNextCj();
                    break;
            }
        }

        private void E94Std_ControlJobInstantiated(object sender, ControlJobEventArgs e)
        {
            if (App.ControllerInstance.GemController.IsControlledByHost)
            {
                foreach (var processingControlSpecification in e.ControlJob
                             .ProcessingControlSpecifications)
                {
                    var processJob = E40Std.GetProcessJob(processingControlSpecification.PRJobID);

                    var wafers = new List<Wafer>();
                    foreach (var materialNameListElement in processJob.CarrierIDSlotsAssociation)
                    {
                        foreach (var slotId in materialNameListElement.SlotIds)
                        {
                            wafers.Add(new Wafer($"{materialNameListElement.CarrierID}.{slotId}"));
                        }
                    }

                    var job = new JobSpecification(
                        processJob.CarrierIDSlotsAssociation.ToList(),
                        new Collection<string>(e.ControlJob.CarrierInputSpecifications),
                        wafers,
                        processJob.RecipeID,
                        false,
                        string.Empty,
                        false,
                        1);

                    job.ControlJob = e.ControlJob;
                    job.ProcessJob = processJob;

                    JobQueue.Add(job);
                }

                OnQueueChanged();
            }
        }

        private void E94Std_ControlJobDisposed(object sender, ControlJobEventArgs e)
        {
            if (App.ControllerInstance.GemController.IsControlledByHost)
            {
                var job = JobQueue.FirstOrDefault(
                    x => x.ControlJob != null && x.ControlJob.ObjID == e.ControlJob.ObjID);
                if (job != null)
                {
                    JobQueue.Remove(job);
                    OnQueueChanged();
                }
            }
        }

        private void E87Std_SlotMapStateChanged(object sender, SlotMapStateChangedArgs e)
        {
            if (e.CurrentState != SlotMapStatus.VerificationOk)
            {
                return;
            }

            if (!App.ControllerInstance.ControllerConfig.UnloadCarrierBetweenJobs || App.UtoInstance.GemController.IsControlledByHost)
            {
                return;
            }

            var jobList = JobQueue.Where(
                j => j.MaterialNameList.Any(
                    m => m.CarrierID == e.Carrier.ObjID)).ToList();

            foreach (var jobSpecification in jobList.Where(j => j.CurrentExecution < j.NumberOfExecutions || j.LoopMode))
            {
                if (jobSpecification.CarrierInputSpec.All(carrier=> E87Std.GetCarrierById(carrier) is {} e87Carrier
                        && e87Carrier.CarrierAccessingStatus == AccessingStatus.NotAccessed
                        && e87Carrier.SlotMapStatus == SlotMapStatus.VerificationOk))
                {
                    StartJob(jobSpecification);
                }
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _logger.Debug("Dispose called");

                if (Controller != null)
                {
                    Controller.LastWaferEntry -= Controller_LastWaferEntry;
                }

                E40Std.ProcessJobChanged -= E40Std_ProcessJobChanged;
                E40Std.ProcessJobDisposed -= E40Std_ProcessJobDisposed;
                E94Std.ControlJobInstantiated -= E94Std_ControlJobInstantiated;
                E94Std.ControlJobDisposed -= E94Std_ControlJobDisposed;
                E87Std.SlotMapStateChanged -= E87Std_SlotMapStateChanged;
            }
        }

        #endregion
    }
}
