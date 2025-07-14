using System.Collections.Generic;
using UnitySC.DataAccess.Dto;

namespace UnitySC.PM.HLS.Service.Interface.Recipe
{
    public interface IHLSRecipe
    {
        string FileVersion { get; set; }

        Step Step { get; set; }
    }
}
