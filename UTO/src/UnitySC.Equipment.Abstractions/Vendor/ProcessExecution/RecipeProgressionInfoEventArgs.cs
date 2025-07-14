using System;
using System.Collections.Generic;

using Agileo.ProcessingFramework;

using UnitySC.Equipment.Abstractions.Vendor.ProcessExecution.Enums;

namespace UnitySC.Equipment.Abstractions.Vendor.ProcessExecution
{
    public class RecipeProgressionInfoEventArgs : EventArgs
    {
        public IDictionary<RecipeSteps, RecipeProgressionInfo> RecipeProgressionInfo;

        public RecipeSteps CurrentStep { get; set; }

        public bool IsProgramLoaded { get; set; }

        public Program Program { get; set; }
    }
}
