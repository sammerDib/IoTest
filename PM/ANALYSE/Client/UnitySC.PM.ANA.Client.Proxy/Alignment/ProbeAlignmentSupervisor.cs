using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Alignment;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.Shared.Configuration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

using ProgressType = UnitySC.PM.ANA.Service.Interface.Alignment.ProgressType;

using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.PM.ANA.Client.Proxy.Alignment
{
    public class ProbeAlignmentSupervisor : IProbeAlignmentService, IProbeAlignmentServiceCallback
    {
        #region Fields

        private DuplexServiceInvoker<IProbeAlignmentService> _alignmentService;

        #endregion

        #region Constructors

        public ProbeAlignmentSupervisor()
        {
            var instanceContext = new InstanceContext(this);
            _alignmentService = new DuplexServiceInvoker<IProbeAlignmentService>(instanceContext,
                "ANALYSEAlignmentService", ClassLocator.Default.GetInstance<SerilogLogger<IProbeAlignmentService>>(),
                ClassLocator.Default.GetInstance<IMessenger>(), null,
                ClientConfiguration.GetServiceAddress(UnitySC.Shared.Data.Enum.ActorType.ANALYSE));
        }

        #endregion

        #region XYAnalysis

        public Response<VoidResult> StartXYMeasurement(AlignmentLiseHFXYAnalysisInput input)
        {
            return _alignmentService.TryInvokeAndGetMessages(x => x.StartXYMeasurement(input));
        }

        public Response<VoidResult> InterruptXYMeasurement()
        {
            return _alignmentService.TryInvokeAndGetMessages(x => x.InterruptXYMeasurement());
        }

        public void XYAlignmentProgress(string progress, ProgressType progressType)
        {
            throw new System.NotImplementedException();
        }

        public void XYAlignmentChanged(AlignmentLiseHFXYAnalysisResult xyAnalysis)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region BeamProfiler

        public Response<object> StartBeamProfiler()
        {
            throw new System.NotImplementedException();
        }

        public Response<object> InterruptBeamProfiler()
        {
            throw new System.NotImplementedException();
        }

        public void BeamProfilerChanged(AlignmentLiseHFBeamProfileResult beamProfile)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region SpectrumCharact

        public Response<object> StartSpectrumCharact()
        {
            throw new System.NotImplementedException();
        }

        public Response<object> InterruptSpectrumCharact()
        {
            throw new System.NotImplementedException();
        }

        public void SpectrumCharactChanged(AlignmentLiseHFSpectrumCharacResult spectrumCharac)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
