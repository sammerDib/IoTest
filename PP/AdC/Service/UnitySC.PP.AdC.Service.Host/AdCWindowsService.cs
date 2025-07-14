using System.ServiceProcess;

//using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.PP.ADC.Service.Host
{
    partial class ADCWindowsService : ServiceBase
    {
        public ADCWindowsService()
        {
            InitializeComponent();
        }

        ADCServer _adcService = null;

        protected override void OnStart(string[] args)
        {

            _adcService = ClassLocator.Default.GetInstance<ADCServer>();
            _adcService.Start();

            //var globalStatusService = ClassLocator.Default.GetInstance<IGlobalStatusService>();

        }

        protected override void OnStop()
        {
            if (_adcService != null)
                _adcService.Stop();
        }
    }
}
