using System.ServiceProcess;

using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.EP.Mountains.Server.Host
{
    partial class MountainsWindowsService : ServiceBase
    {
        private MountainsServer _mountainsServer;

        public MountainsWindowsService()
        {
            InitializeComponent();
            _mountainsServer = ClassLocator.Default.GetInstance<MountainsServer>();
        }

        protected override void OnStart(string[] args)
        {
            _mountainsServer.Start();
        }

        protected override void OnStop()
        {
            _mountainsServer.Stop();
        }
    }
}
