using System.Collections.Generic;

using Agileo.EquipmentModeling;
using Agileo.ProcessingFramework;

using UnitySC.Equipment.Abstractions.Vendor.ProcessExecution;
using UnitySC.Equipment.Abstractions.Vendor.ProcessExecution.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.ProcessModule;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.RecipeProcessModule
{
    // Implement interface IMaterialLocationContainer if the device has material locations
    [Device(IsAbstract = true)]
    [Interruption(Kind = InterruptionKind.Abort)]
    public interface IRecipeProcessModule : IProcessModule
    {
        #region Statuses

        [Status(Documentation = "Notify external if a program is currently loaded")]
        bool IsProgramLoaded { get; }
        [Status(Documentation = "The current processor state", IsLoggingActivated = true)]
        ProcessorState ProcessorState { get; }
        [Status(Documentation = "the current program execution state", IsLoggingActivated = false)]
        ProgramExecutionState ProgramExecutionState { get; }

        #endregion

        #region Properties

        Dictionary<RecipeSteps, RecipeProgressionInfo> RecipeProgressionInfos { get; }

        RecipeElapsedTimeInfo RecipeElapsedTimeInfo { get; }

        #endregion

    }
}
