using System.ServiceProcess;

using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Host
{
    partial class AnaWindowsService : ServiceBase
    {
        private AnaServer _analyseServer;
        public AnaWindowsService()
        {
            InitializeComponent();
            _analyseServer = ClassLocator.Default.GetInstance<AnaServer>();
        }

        protected override void OnStart(string[] args)
        {
            _analyseServer.Start();
        }

        protected override void OnStop()
        {
            _analyseServer.Stop();
        }
    }
}
