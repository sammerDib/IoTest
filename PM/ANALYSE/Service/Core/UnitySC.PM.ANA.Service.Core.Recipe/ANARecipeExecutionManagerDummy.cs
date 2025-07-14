using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.ANA.Service.Core.Recipe
{
    public class ANARecipeExecutionManagerDummy : ANARecipeExecutionManager
    {
        public ANARecipeExecutionManagerDummy(ILogger logger, AnaHardwareManager hardwareManager, IReferentialManager referentialManager, PMConfiguration pmConfiguration) : base(logger, hardwareManager, referentialManager, pmConfiguration)
        {
        }
    }
}
