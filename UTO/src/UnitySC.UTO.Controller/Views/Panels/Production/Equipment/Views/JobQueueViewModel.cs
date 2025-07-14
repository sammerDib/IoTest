using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using Agileo.GUI.Commands;
using Agileo.GUI.Components.Tools;

using UnitySC.GUI.Common.Resources;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.UTO.Controller.JobQueuer;

namespace UnitySC.UTO.Controller.Views.Panels.Production.Equipment.Views
{
    public sealed class JobQueueViewModel : Tool
    {
        #region Fields

        private readonly List<JobSpecification> _selectedJobs = new();
        private readonly bool _isUsedAsTool;

        #endregion

        #region Constructors

        static JobQueueViewModel()
        {
            DataTemplateGenerator.Create(typeof(JobQueueViewModel), typeof(JobQueueView));
        }

        public JobQueueViewModel() : base(nameof(L10N.TOOL_JOB_QUEUE), PathIcon.ListView)
        {
            DisplayZone = VerticalAlignment.Bottom;

            Jobs = new DataTableSource<JobSpecification>();
            App.ControllerInstance.JobQueueManager.QueueChanged += JobQueuer_QueueChanged;
            App.ControllerInstance.GemController.E40Std.ProcessJobChanged += E40Std_ProcessJobChanged;

            _isUsedAsTool = App.ControllerInstance.GemController.E87Std.LoadPorts.Count > 2;

            SelectedJobs = new ReadOnlyCollection<JobSpecification>(_selectedJobs);

            JobSpecificationIsSelectedFunc = JobSpecificationIsSelected;
        }

        #endregion

        #region Properties

        public DataTableSource<JobSpecification> Jobs { get; }

        public ReadOnlyCollection<JobSpecification> SelectedJobs { get; }

        private bool _jobSelectionFlag;

        public bool JobSelectionFlag
        {
            get => _jobSelectionFlag;
            set => SetAndRaiseIfChanged(ref _jobSelectionFlag, value);
        }

        public Func<JobSpecification, bool, bool> JobSpecificationIsSelectedFunc { get; }

        #endregion

        #region Commands

        private DelegateCommand<JobSpecification> _toggleJobSelectionCommand;

        public DelegateCommand<JobSpecification> ToggleJobSelectionCommand
            => _toggleJobSelectionCommand ??= new DelegateCommand<JobSpecification>(SelectJobCommandExecute, SelectJobCommandCanExecute);

        private bool SelectJobCommandCanExecute(JobSpecification arg) => arg != null;

        private void SelectJobCommandExecute(JobSpecification arg)
        {
            if (_selectedJobs.Contains(arg))
            {
                _selectedJobs.Remove(arg);
            }
            else
            {
                _selectedJobs.Add(arg);
            }

            JobSelectionFlag = !JobSelectionFlag;
        }

        #endregion

        #region Private methods

        private bool JobSpecificationIsSelected(JobSpecification job, bool _) => job != null && _selectedJobs.Contains(job);

        #endregion

        #region Event Handlers

        private void JobQueuer_QueueChanged(object sender, EventArgs e)
        {
            var newJobList = App.ControllerInstance.JobQueueManager.JobQueue;
            Jobs.Reset(newJobList);

            foreach (var selectedJob in _selectedJobs.ToList())
            {
                if (!newJobList.Contains(selectedJob))
                {
                    _selectedJobs.Remove(selectedJob);
                }
            }

            if (newJobList.Count == 1)
            {
                _selectedJobs.Clear();
                _selectedJobs.Add(newJobList[0]);
                JobSelectionFlag = !JobSelectionFlag;
            }
        }

        private void E40Std_ProcessJobChanged(object sender, Agileo.Semi.Gem300.Abstractions.E40.ProcessJobStateChangedEventArgs e)
        {
            Jobs.UpdateCollection();

            if (_isUsedAsTool)
            {
                IsOpen = true;
            }
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                App.ControllerInstance.JobQueueManager.QueueChanged -= JobQueuer_QueueChanged;
                App.ControllerInstance.GemController.E40Std.ProcessJobChanged -= E40Std_ProcessJobChanged;
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
