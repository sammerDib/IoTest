using System.Collections.Generic;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.ANA.Service.Interface.Recipe.Alignment;
using UnitySC.PM.ANA.Service.Interface.Recipe.AlignmentMarks;
using UnitySC.PM.ANA.Service.Interface.Recipe.Execution;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Interface.Recipe.WaferMap;

namespace UnitySC.PM.ANA.Service.Interface.Recipe
{
    public interface IANARecipe
    {
        string FileVersion { get; set; }
        List<MeasureSettingsBase> Measures { get; set; }
        List<MeasurePoint> Points { get; set; }
        Step Step { get; set; }

        AlignmentSettings Alignment { get; set; }

        WaferMapSettings WaferMap { get; set; }

        AlignmentMarksSettings AlignmentMarks { get; set; }

        ExecutionSettings Execution { get; set; }
    }
}
