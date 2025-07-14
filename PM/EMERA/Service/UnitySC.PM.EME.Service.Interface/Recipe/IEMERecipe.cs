using System.Collections.Generic;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.EME.Service.Interface.Recipe.Execution;

namespace UnitySC.PM.EME.Service.Interface.Recipe
{
    public interface IEMERecipe
    {
        string FileVersion { get; set; }
        List<Acquisition> Acquisitions { get; set; }
        ExecutionSettings Execution { get; set; }
        Step Step { get; set; }
        bool IsSaveResultsEnabled { get; set; }
    }
}
