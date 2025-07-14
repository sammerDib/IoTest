using System.Threading;

using UnitySC.PM.AGS.Service.Interface.AcquisitionService;
using UnitySC.PM.AGS.Service.Interface.RecipeService;
using UnitySC.PM.AGS.Service.Interface.Flow;
using UnitySC.PM.Shared.Data;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.AGS.Service.Implementation.Acquisition
{
    public class Acquisition
    {
        private ILogger _logger;
        private PmWaferInfo _pmWaferInfo;
        private ArgosRecipe _recipe;
        private bool _overwriteOutput;
        private AcquisitionFlow _runnningAcquisitionFlow = null;

        public Acquisition(ILogger logger, ArgosRecipe recipe, PmWaferInfo waferInfo,
            bool overwriteOutput)
        {
            _logger = logger;
            _pmWaferInfo = waferInfo;
            _recipe = recipe;
            _overwriteOutput = overwriteOutput;
        }

        public AcquisitionResult Start()
        {
            var flowInput = new AcquisitionInput();
            var _runnningAcquisitionFlow = new AcquisitionFlow(flowInput);
            var result = _runnningAcquisitionFlow.Execute();
            return result;
        }

        public void Abort()
        {
            if (_runnningAcquisitionFlow != null)
            {
                _runnningAcquisitionFlow.CancellationToken = new CancellationToken(true);
            }
        }
    }
}
