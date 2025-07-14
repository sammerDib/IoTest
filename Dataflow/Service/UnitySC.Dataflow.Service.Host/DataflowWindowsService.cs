using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceProcess;
using System.Threading.Tasks;

using Serilog;

using UnitySC.dataflow.Service.Host;
using UnitySC.Dataflow.Service.Interface;
using UnitySC.Shared.Dataflow.PM.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.Dataflow.Service.Host
{
    partial class DataflowWindowsService : ServiceBase
    {
        private DataflowServer _dataflowServer;

        public DataflowWindowsService()
        {
            InitializeComponent();   
            _dataflowServer = ClassLocator.Default.GetInstance<DataflowServer>();
        }

        protected override void OnStart(string[] args)
        {
            _dataflowServer.Start();
        }

        protected override void OnStop()
        {
            _dataflowServer.Stop();
        }
    }
}
