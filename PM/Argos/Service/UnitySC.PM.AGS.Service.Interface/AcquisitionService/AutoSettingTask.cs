using System;

using UnitySC.PM.AGS.Data;
using UnitySC.PM.AGS.Service.Interface.RecipeService;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.AGS.Service.Interface.AcquisitionService
{
    // Members are never used. TODO implementation.

    public class AutoSettingTask
    {
        private ILogger _logger;
        private ArgosRecipe _recipe;
        private IAcquisitionService _acquisitionService;

        public ArgosRecipe Start()
        {
            throw new NotImplementedException();
        }
    }
}
