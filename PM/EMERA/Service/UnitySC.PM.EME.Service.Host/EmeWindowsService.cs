using System.ServiceProcess;

using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Service.Host
{
    internal partial class EmeWindowsService : ServiceBase
    {
        private readonly EmeServer _emeServer;
        public EmeWindowsService()
        {
            InitializeComponent();
            _emeServer = ClassLocator.Default.GetInstance<EmeServer>();
        }

        protected override void OnStart(string[] args)
        {
            _emeServer.Start();
        }

        protected override void OnStop()
        {
            _emeServer.Stop();
        }
    }
}
