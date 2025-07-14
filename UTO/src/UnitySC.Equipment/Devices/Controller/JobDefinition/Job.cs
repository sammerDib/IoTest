using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.GUI.Components;

using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor.JobDefinition;
using UnitySC.Equipment.Devices.Controller.Activities.WaferFlow.Enum;

namespace UnitySC.Equipment.Devices.Controller.JobDefinition
{
    public class Job : Notifier
    {
        #region Properties

        /// <summary>
        /// Gets the name of the process job
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the name of the control job
        /// </summary>
        public string ControlJobId { get; }

        /// <summary>
        /// Gets the name of the recipe
        /// </summary>
        public string RecipeName { get; }

        private JobStatus _status = JobStatus.Created;

        /// <summary>
        /// Gets the job status
        /// </summary>
        public JobStatus Status
        {
            get => _status;
            set => SetAndRaiseIfChanged(ref _status, value);
        }

        /// <summary>
        /// Gets the list of substrate to be handled in the job
        /// </summary>
        public List<Wafer> Wafers { get; }

        /// <summary>
        /// Gets the list of the remaining substrate to be handled
        /// </summary>
        public ObservableCollection<Wafer> RemainingWafers { get; }

        /// <summary>
        /// Gets the list of the substrate for which result has been received
        /// </summary>
        public List<string> WaferResultReceived { get; }

        private DateTime _startTime = DateTime.MinValue;

        /// <summary>
        /// Gets the job's start time
        /// </summary>
        public DateTime StartTime
        {
            get => _startTime;
            set => SetAndRaiseIfChanged(ref _startTime, value);
        }

        private DateTime _endTime = DateTime.MinValue;

        /// <summary>
        /// Gets the job's end time
        /// </summary>
        public DateTime EndTime
        {
            get => _endTime;
            set => SetAndRaiseIfChanged(ref _endTime, value);
        }

        /// <summary>
        /// Ocr profile used during the job
        /// </summary>
        public OcrProfile OcrProfile { get; }


        private StopConfig _stopConfig;

        public StopConfig StopConfig
        {
            get => _stopConfig;
            set => SetAndRaiseIfChanged(ref _stopConfig, value);
        }
        #endregion

        #region Constructor

        /// <summary>
        /// The job to be executed
        /// </summary>
        /// <param name="name">The name of the job</param>
        /// <param name="controlJobId">The name of the control job</param>
        /// <param name="recipeName">The name of the recipe</param>
        /// <param name="wafers">List of wafers</param>
        /// <param name="profile">The OCR profile to use</param>
        public Job(
            string name,
            string controlJobId,
            string recipeName,
            List<Wafer> wafers,
            OcrProfile profile)
        {
            Name = name;
            ControlJobId = controlJobId;
            RecipeName = recipeName;
            Status = JobStatus.Created;
            Wafers = new List<Wafer>(wafers);
            RemainingWafers = new ObservableCollection<Wafer>(wafers);
            StartTime = DateTime.MinValue;
            EndTime = DateTime.MinValue;
            OcrProfile = profile;
            WaferResultReceived = new List<string>();
        }

        #endregion

        #region Public

        public bool IsCompleted()
        {
            if (RemainingWafers.Count != 0)
            {
                return false;
            }

            return Wafers.All(wafer => WaferResultReceived.Contains(wafer.SubstrateId));
        }

        #endregion
    }
}
