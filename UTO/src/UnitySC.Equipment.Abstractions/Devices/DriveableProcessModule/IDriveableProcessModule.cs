using System;
using System.Collections.Generic;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitsNet;

using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.EventArgs;
using UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule.Conditions;
using UnitySC.Equipment.Abstractions.Devices.ProcessModule;
using UnitySC.Equipment.Abstractions.Devices.ProcessModule.Conditions;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.Conditions;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Conditions;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.Shared.Data;

using Alarm = Agileo.AlarmModeling.Alarm;

namespace UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule
{
    [Device(IsAbstract = true)]
    public interface IDriveableProcessModule : IProcessModule
    {
        #region Status

        [Status]
        ActorType ActorType { get; }

        [Status]
        ProcessModuleState ProcessModuleState { get; }

        [Status]
        ProcessModuleState PreviousProcessModuleState { get; }

        [Status]
        string SelectedRecipe { get; }

        [Status]
        Ratio RecipeProgress { get; }

        [Status]
        EnumPMTransferState TransferState { get; }

        [Status]
        bool TransferValidationState { get; }

        #endregion

        #region Commands

        #region Material

        [Command(
            Documentation =
                "Prepare the process module (unclamp, move pins...) for material deposit/removal.")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsInService))]
        void PrepareTransfer(Shared.TC.Shared.Data.TransferType transferType, RobotArm arm, MaterialType materialType, SampleDimension dimension);

        [Command(Documentation = "Prepare the process module (clamp, move pins...) for process")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsInService))]
        void PostTransfer();

        [Command(Documentation = "Start recipe on the process module")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsIdle))]
        [Pre(Type = typeof(IsInService))]
        void SelectRecipe(Wafer wafer);

        [Command(Documentation = "Start recipe on the process module")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsInService))]
        void StartRecipe();

        [Command(Documentation = "Abort recipe on the process module")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsRecipeRunning))]
        [Pre(Type = typeof(IsInService))]
        void AbortRecipe();

        [Command(Documentation = "Reset smoke detector alarm")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsInService))]
        void ResetSmokeDetectorAlarm();

        #endregion

        #endregion

        #region Events

        #region Material

        event EventHandler<EventArgs> ReadyToTransfer;

        event EventHandler<EventArgs> SmokeDetected;

        #endregion

        #region Equipment Constants

        event EventHandler<EquipmentConstantChangedEventArgs> EquipmentConstantChanged;

        #endregion

        #region Status Variables

        event EventHandler<StatusVariableChangedEventArgs> StatusVariableChanged;

        #endregion

        #region Collection Events

        event EventHandler<CollectionEventEventArgs> CollectionEventRaised;

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

        double GetAlignmentAngle();

        string GetMessagesConfigurationPath(string path);

        #endregion
    }
}
