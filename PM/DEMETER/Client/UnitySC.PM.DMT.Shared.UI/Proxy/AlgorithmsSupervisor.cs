using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Interface.Recipe;
using UnitySC.PM.DMT.Shared.UI.Message;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.DMT.Shared.UI.Proxy
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class AlgorithmsSupervisor : IDMTAlgorithmsServiceCallback
    {
        private readonly DuplexServiceInvoker<IDMTAlgorithmsService> _algorithmsService;
        private readonly InstanceContext _instanceContext;
        private readonly IMessenger _messenger;
        private ILogger _logger;

        public AlgorithmsSupervisor(ILogger<AlgorithmsSupervisor> logger, ILogger<IDMTAlgorithmsService> serviceLogger, IMessenger messenger)
        {
            _logger = logger;
            _messenger = messenger;
            _instanceContext = new InstanceContext(this);
            _algorithmsService = new DuplexServiceInvoker<IDMTAlgorithmsService>(_instanceContext,
                "DEMETERAlgorithmsService",
                serviceLogger, _messenger,
                s => s.Subscribe());
        }

        void IDMTAlgorithmsServiceCallback.ReportProgress(RecipeStatus status)
        {
            _messenger.Send(new RecipeMessage { Status = status });
        }

        public void StartAutoExposure(MeasureBase measure)
        {
            _algorithmsService.Invoke(s => s.StartAutoExposureOnMeasure(measure));
        }

        public void StartAutoExposure(Side side, MeasureType measureType, bool ignorePerspectiveCalibration = false)
        {
            _algorithmsService.Invoke(s => s.StartAutoExposure(side, measureType, ignorePerspectiveCalibration));
        }

        public void CancelAutoExposure()
        {
            _algorithmsService.Invoke(s => s.CancelAutoExposure());
        }
    }
}
