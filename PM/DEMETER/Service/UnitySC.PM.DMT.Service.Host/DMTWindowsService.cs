using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using UnitySC.PM.DMT.Service.Implementation;
using UnitySC.PM.Shared;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.DMT.Service.Host
{
    public partial class DMTWindowsService : ServiceBase
    {
        private DMTServer _dmtServer;

        public DMTWindowsService()
        {
            InitializeComponent();
            _dmtServer = ClassLocator.Default.GetInstance<DMTServer>();
        }

        protected override void OnStart(string[] args)
        {
            _dmtServer.Start();
        }

        protected override void OnStop()
        {
            _dmtServer.Stop();
        }
    }
}
