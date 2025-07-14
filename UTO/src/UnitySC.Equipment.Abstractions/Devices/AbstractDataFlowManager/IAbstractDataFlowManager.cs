using System;
using System.Collections.Generic;

using Agileo.EquipmentModeling;

using UnitySC.DataAccess.Dto;
using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.Conditions;
using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.EventArgs;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.Conditions;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.Shared.Data;

using Alarm = Agileo.AlarmModeling.Alarm;

namespace UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager
{
    // Implement interface IMaterialLocationContainer if the device has material locations
    [Device(IsAbstract = true)]
    public interface IAbstractDataFlowManager : IUnityCommunicatingDevice
    {
        #region Status

        [Status]
        bool IsStopCancelAllJobsRequested { get; }

        [Status]
        TC_DataflowStatus DataflowState { get; }

        #endregion

        #region Commands

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsRecipeAvailable))]
        void StartRecipe(MaterialRecipe materialRecipe, string processJobId);

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        void AbortRecipe(string jobId);

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(WaferPresentInRunningRecipe))]
        void StartJobOnMaterial(DataflowRecipeInfo recipe, Wafer wafer);

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        void GetAvailableRecipes();

        #endregion

        #region Properties

        List<DataflowRecipeInfo> AvailableRecipes { get; }

        #endregion

        #region Events

        #region Equipment Constants

        event EventHandler<EquipmentConstantChangedEventArgs> EquipmentConstantChanged;

        #endregion

        #region Status Variables

        event EventHandler<StatusVariableChangedEventArgs> StatusVariableChanged;

        #endregion

        #region Collection Events

        event EventHandler<CollectionEventEventArgs> CollectionEventRaised;

        #endregion

        #region Process Module Events

        event EventHandler<ProcessModuleRecipeEventArgs> ProcessModuleRecipeStarted;

        event EventHandler<ProcessModuleRecipeEventArgs> ProcessModuleRecipeCompleted;

        #endregion

        #region DataFlow Events

        event EventHandler<DataFlowRecipeEventArgs> DataFlowRecipeStarted;

        event EventHandler<DataFlowRecipeEventArgs> DataFlowRecipeCompleted;

        event EventHandler<DataFlowRecipeEventArgs> DataFlowRecipeAdded;

        event EventHandler<DataFlowRecipeEventArgs> DataFlowRecipeModified;

        event EventHandler<DataFlowRecipeEventArgs> DataFlowRecipeDeleted;

        #endregion

        #endregion

        #region Methods

        IEnumerable<Alarm> GetActiveAlarms();

        /// <summary>Gets the list of equipment constants</summary>
        /// <param name="ids">
        /// If ids is null or empty, returns the entire list of equipment constants
        /// </param>
        /// <returns>The list of requested equipment constants</returns>
        IEnumerable<EquipmentConstant> GetEquipmentConstants(List<int> ids);

        bool SetEquipmentConstant(EquipmentConstant equipmentConstant);

        /// <summary>Gets the list of status variables</summary>
        /// <param name="ids">If null or empty, returns the entire list of status variables</param>
        /// <returns>The list of requested status variables</returns>
        IEnumerable<StatusVariable> GetStatusVariables(List<int> ids);

        IEnumerable<CommonEvent> GetCollectionEvents();

        void SwitchToManualMode();

        void SignalEndOfProcessJob(string processJobId);

        UTOJobProgram GetJobProgramFromRecipe(DataflowRecipeInfo recipe);

        #endregion
    }
}
