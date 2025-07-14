using System;
using System.Collections.Generic;

using UnitySC.DataAccess.Dto;
using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.EventArgs;
using UnitySC.Shared.TC.Shared.Data;

using Alarm = Agileo.AlarmModeling.Alarm;

namespace UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager
{
    public partial class AbstractDataFlowManager
    {
        #region Setup

        private void InstanceInitialization()
        {
            // Default configure the instance.
            // Call made from the constructor.
        }

        #endregion

        #region Abstract Commands

        protected abstract void InternalStartRecipe(
            MaterialRecipe materialRecipe,
            string processJobId);

        protected abstract void InternalAbortRecipe(string jobId);

        protected abstract void InternalStartJobOnMaterial(
            DataflowRecipeInfo recipe,
            Material.Wafer wafer);

        protected abstract void InternalGetAvailableRecipes();

        #endregion

        #region Properties

        public List<DataflowRecipeInfo> AvailableRecipes { get; protected set; }

        #endregion Properties

        #region Events

        public event EventHandler<EquipmentConstantChangedEventArgs> EquipmentConstantChanged;
        public event EventHandler<StatusVariableChangedEventArgs> StatusVariableChanged;
        public event EventHandler<CollectionEventEventArgs> CollectionEventRaised;
        public event EventHandler<ProcessModuleRecipeEventArgs> ProcessModuleRecipeStarted;
        public event EventHandler<ProcessModuleRecipeEventArgs> ProcessModuleRecipeCompleted;
        public event EventHandler<ProcessModuleRecipeEventArgs> ProcessModuleAcquisitionCompleted;
        public event EventHandler<DataFlowRecipeEventArgs> DataFlowRecipeStarted;
        public event EventHandler<DataFlowRecipeEventArgs> DataFlowRecipeCompleted;
        public event EventHandler<DataFlowRecipeEventArgs> DataFlowRecipeAdded;
        public event EventHandler<DataFlowRecipeEventArgs> DataFlowRecipeModified;
        public event EventHandler<DataFlowRecipeEventArgs> DataFlowRecipeDeleted;
        public event EventHandler<FdcCollectionEventArgs> FdcCollectionChanged;

        protected void OnEquipmentConstantChanged(EquipmentConstantChangedEventArgs e)
        {
            EquipmentConstantChanged?.Invoke(this, e);
        }

        protected void OnStatusVariableChanged(StatusVariableChangedEventArgs e)
        {
            StatusVariableChanged?.Invoke(this, e);
        }

        protected void OnCollectionEventRaised(CollectionEventEventArgs e)
        {
            CollectionEventRaised?.Invoke(this, e);
        }

        protected void OnProcessModuleRecipeStarted(ProcessModuleRecipeEventArgs e)
        {
            ProcessModuleRecipeStarted?.Invoke(this, e);
        }

        protected void OnProcessModuleRecipeCompleted(ProcessModuleRecipeEventArgs e)
        {
            ProcessModuleRecipeCompleted?.Invoke(this, e);
        }

        protected void OnProcessModuleAcquisitionCompleted(ProcessModuleRecipeEventArgs e)
        {
            ProcessModuleAcquisitionCompleted?.Invoke(this, e);
        }

        protected void OnDataFlowRecipeStarted(DataFlowRecipeEventArgs e)
        {
            DataFlowRecipeStarted?.Invoke(this, e);
        }

        protected void OnDataFlowRecipeCompleted(DataFlowRecipeEventArgs e)
        {
            DataFlowRecipeCompleted?.Invoke(this, e);
        }

        protected void OnDataFlowRecipeAdded(DataFlowRecipeEventArgs e)
        {
            DataFlowRecipeAdded?.Invoke(this, e);
        }

        protected void OnDataFlowRecipeModified(DataFlowRecipeEventArgs e)
        {
            DataFlowRecipeModified?.Invoke(this, e);
        }

        protected void OnDataFlowRecipeDeleted(DataFlowRecipeEventArgs e)
        {
            DataFlowRecipeDeleted?.Invoke(this, e);
        }

        protected void OnFdcCollectionChanged(FdcCollectionEventArgs e)
        {
            try
            {
                FdcCollectionChanged?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                OnUserErrorRaised(ex.Message);
            }
        }

        #endregion Events

        #region Public Methods

        public abstract IEnumerable<Alarm> GetActiveAlarms();

        public abstract IEnumerable<EquipmentConstant> GetEquipmentConstants(List<int> ids);

        public abstract bool SetEquipmentConstant(EquipmentConstant equipmentConstant);

        public abstract IEnumerable<StatusVariable> GetStatusVariables(List<int> ids);

        public abstract IEnumerable<CommonEvent> GetCollectionEvents();

        public abstract void ResetIsStopCancelAllJobsRequested();

        public abstract void SwitchToManualMode();

        public abstract void SignalEndOfProcessJob(string processJobId);

        public abstract UTOJobProgram GetJobProgramFromRecipe(DataflowRecipeInfo recipe);
        public abstract DataflowRecipeInfo GetRecipeInfo(string recipeName);
        public abstract string GetRecipeName(DataflowRecipeInfo recipe);
        public abstract List<string> GetRecipeNames();

        #endregion Public Methods
    }
}
