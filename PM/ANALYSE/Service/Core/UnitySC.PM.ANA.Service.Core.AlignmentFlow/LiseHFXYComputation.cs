using System;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.AlignmentFlow
{
    public class LiseHFXYComputation
    {
        private ILogger _logger;
        private CancellationTokenSource _cancellationTokenSrc;
        private readonly AnaHardwareManager _hardwareManager;

        private Delegate _reportProgress;

        public LiseHFXYComputation(ILogger logger, Delegate reportProgress = null)
        {
            _logger = logger;
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _reportProgress = reportProgress;
        }

        public AlignmentLiseHFXYAnalysisResult DoComputation(LiseHFXYIntermediateResult input, CancellationToken cancellationToken)
        {
            int index = 0;
            int size = 50;
            while (!cancellationToken.IsCancellationRequested && index < size)
            {
                Task.Delay(50).Wait();
                _reportProgress?.DynamicInvoke(100.0*index/size,null);
                index++;
            }

            return new AlignmentLiseHFXYAnalysisResult();
        }
    }
}
