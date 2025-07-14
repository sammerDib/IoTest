using System.ServiceProcess;
using UnitySC.PM.LIGHTSPEED.Service.Implementation;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.LIGHTSPEED.Service.Host
{
    public partial class LightspeedWindowsService : ServiceBase
    {
        private LSServer _lightspeedServer;

        public LightspeedWindowsService()
        {
            InitializeComponent();
            _lightspeedServer = ClassLocator.Default.GetInstance<LSServer>();
        }

        protected override void OnStart(string[] args)
        {
            _lightspeedServer.Start();
        }

        protected override void OnStop()
        {
            _lightspeedServer.Stop();
        }
    }
}
