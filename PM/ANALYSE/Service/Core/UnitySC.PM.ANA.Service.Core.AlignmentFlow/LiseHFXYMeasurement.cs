using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.AlignmentFlow
{
    public class LiseHFXYMeasurement
    {
        private ILogger _logger;
        private CancellationTokenSource _cancellationTokenSrc;
        private readonly AnaHardwareManager _hardwareManager;

        private Delegate _reportProgress;

        public LiseHFXYMeasurement(ILogger logger, Delegate reportProgress = null)
        {
            _logger = logger;
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _reportProgress = reportProgress;
        }

        public LiseHFXYIntermediateResult DoMeasurement(AlignmentLiseHFXYAnalysisInput input, CancellationToken cancellationToken)
        {
            var rand = new Random(123);
            int index = 0;
            const int size = 21;
            var result = new LiseHFXYIntermediateResult { StepSize = input.StepSize};
            while (!cancellationToken.IsCancellationRequested && index < size)
            {
                result.TSV[index] = 150 +  (10 * rand.NextDouble() - 5);
                _reportProgress?.DynamicInvoke(100.0*index/size, result.TSV);
                index++;
                Task.Delay(1000).Wait();
            }
            return result;
        }
    }
}
