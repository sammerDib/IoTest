using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Execution
{
    public enum RecipeProgressState
    {
        AutoFocusInProgress,
        AutoLightInProgress,
        EdgeAlignmentInProgress,
        MarkAlignmentInProgress,
        Measuring,
        ComputeMeasureFromSubMeasures,
        SubMeasuring,
        InPause,
        Canceled,
        Error,
        Success
     }
}
