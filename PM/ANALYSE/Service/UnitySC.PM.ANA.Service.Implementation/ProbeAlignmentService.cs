using UnitySC.PM.ANA.Service.Interface.Alignment;
using UnitySC.Shared.Tools.Service;

using System;
using System.ServiceModel;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Client.Proxy.Alignment;
using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.AlignmentFlow;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ProbeAlignmentService : DuplexServiceBase<IProbeAlignmentServiceCallback>, IProbeAlignmentService

    {
        #region Fields

        private AnaHardwareManager _anaHardwareManager;
        private LiseHFXYAnalysisFlow _liseHFXYAnalysisFlow;

        private FlowTask<AlignmentLiseHFXYAnalysisInput, AlignmentLiseHFXYAnalysisResult,
            AlignmentLiseHFXYAnalysisConfiguration> _liseHFXYAnalysisFlowTask;

        #endregion

        #region Constructors

        public ProbeAlignmentService(ILogger logger) : base(logger, ExceptionType.ToolSetupException)
        {
            _anaHardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
        }

        #endregion

        #region XYAnalysis

        public Response<VoidResult> StartXYMeasurement(AlignmentLiseHFXYAnalysisInput input)
        {
            _logger?.Information("Received StartXYMeasurement request");
            return InvokeVoidResponse(messageContainer =>
            {
                _liseHFXYAnalysisFlowTask =
                    new FlowTask<AlignmentLiseHFXYAnalysisInput, AlignmentLiseHFXYAnalysisResult,
                        AlignmentLiseHFXYAnalysisConfiguration>(new LiseHFXYAnalysisFlow(input));
                Task.Run(() => _liseHFXYAnalysisFlowTask.Start());
            });
        }

        public Response<VoidResult> InterruptXYMeasurement()
        {
            _logger?.Information("Received InterruptXYMeasurement request");
            return InvokeVoidResponse(messageContainer =>
            {
                _liseHFXYAnalysisFlowTask?.Cancel();
            });
        }

        #endregion

        #region BeamProfiler

        public Response<object> StartBeamProfiler()
        {
            _logger?.Information("Received StartBeamProfiler request");
            throw new NotImplementedException();
        }

        public Response<object> InterruptBeamProfiler()
        {
            _logger?.Information("Received InterruptBeamProfiler request");
            throw new NotImplementedException();
        }

        #endregion

        #region SpectrumCharact

        public Response<object> StartSpectrumCharact()
        {
            _logger?.Information("Received StartSpectrumCharact request");
            throw new NotImplementedException();
        }

        public Response<object> InterruptSpectrumCharact()
        {
            _logger?.Information("Received InterruptSpectrumCharact request");
            throw new NotImplementedException();
        }

        #endregion
    }
}
