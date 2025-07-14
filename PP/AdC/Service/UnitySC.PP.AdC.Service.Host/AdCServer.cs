//using UnitySC.PM.Shared.Status.Service.Interface;
using System.ServiceModel;

using ADCEngine;

using UnitySC.PM.Shared.UserManager.Service.Interface;
using UnitySC.PP.ADC.Service.Interface;
using UnitySC.PP.ADC.Service.Interface.Recipe;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PP.ADC.Service.Host
{

    public class ADCServer : BaseServer
    {
        private ILogger _logger;
        //private IGlobalStatusService _globalStatusService;
        private IPMUserService _pmUserService;
        private IADCRecipeService _adCRecipeService;

        private IAdcExecutor _adcExecutor;
        private IADCService _adCService;

        public ADCServer(ILogger logger) : base(logger)
        {
            _logger = logger;
            //_globalStatusService = ClassLocator.Default.GetInstance<IGlobalStatusService>();
            _pmUserService = ClassLocator.Default.GetInstance<IPMUserService>();

            _adCRecipeService = ClassLocator.Default.GetInstance<IADCRecipeService>();

            // moteur de l'ADC
            _adcExecutor = ClassLocator.Default.GetInstance<IAdcExecutor>();

            // Wcf Ettendu par rapport a IAdcExecutor, IAdcAcquisition
            _adCService = ClassLocator.Default.GetInstance<IADCService>();



        }

        private ServiceHost _adcExecutorhost = null;

        public override void Start()
        {
            //StartService((BaseService)_globalStatusService);
            StartService((BaseService)_pmUserService);

            StartService((BaseService)_adCRecipeService);



            _adcExecutorhost = new ServiceHost(_adcExecutor);
            foreach (var endpoint in _adcExecutorhost.Description.Endpoints)
                _logger.Information("Creating service on \"" + endpoint.Address + "\"");
            _adcExecutorhost.Open();


            StartService((BaseService)_adCService);
        }

        public override void Stop()
        {
            _adcExecutorhost.Close();

            StopAllServiceHost();
        }
    }
}

