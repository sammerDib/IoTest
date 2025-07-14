using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using UnitySC.Shared.Tools;

namespace UnitySC.DataAccess.Service.Host
{
    partial class DataAccessWindowsService : ServiceBase
    {
        private DataAccessService _dataAccessService;
        public DataAccessWindowsService()
        {
            InitializeComponent();
            _dataAccessService = ClassLocator.Default.GetInstance<DataAccessService>();
        }

        protected override void OnStart(string[] args)
        {
            _dataAccessService.Start();
        }

        protected override void OnStop()
        {
            _dataAccessService.Stop();
        }
    }
}
