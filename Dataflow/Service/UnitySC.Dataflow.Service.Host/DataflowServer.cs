using UnitySC.Dataflow.Service.Interface;
using UnitySC.Shared.Dataflow.PM.Service.Interface;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.FDC;
using UnitySC.Shared.FDC.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.dataflow.Service.Host
{
    public class DataflowServer : BaseServer
    {
        private IDAP _dap;        
        private IUTODFService _utoDFService;
        private IPMDFService _pmDFService;
        private IDFManager _dfManager;
        private readonly IFDCService _fdcService;


        public DataflowServer(ILogger logger) : base(logger)
        {
            _dap = ClassLocator.Default.GetInstance<IDAP>();                       
            _utoDFService = ClassLocator.Default.GetInstance<IUTODFService>();
            _pmDFService = ClassLocator.Default.GetInstance<IPMDFService>();
            _dfManager = ClassLocator.Default.GetInstance<IDFManager>();
            _fdcService = ClassLocator.Default.GetInstance<IFDCService>();
        }

        public override void Start()
        {
            StartService((BaseService)_dap);
            StartService((BaseService)_utoDFService);
            StartService((BaseService)_pmDFService);
            StartService((BaseService)_fdcService);
            _dfManager.Init();
            ClassLocator.Default.GetInstance<FDCManager>().StartMonitoringFDC();
        }

        public override void Stop()
        {
            StopAllServiceHost();
        }
    }
}
