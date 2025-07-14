using System.Collections.Generic;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.Shared.Configuration;
using UnitySC.Shared.Data;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.UI.Proxy
{
    [CallbackBehaviorAttribute(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DBMaintenanceSupervisor : IDBMaintenanceServiceCallback
    {
        private InstanceContext _instanceContext;
        private ILogger _logger;
        private IMessenger _messenger;
        private DuplexServiceInvoker<IDBMaintenanceService> _dbMaintenanceService;

        public delegate void DbMaintenanceProgressChanged(DbOperationProgressMessage progressMessage);

        public event DbMaintenanceProgressChanged DbMaintenanceProgressChangedEvent;

        public DBMaintenanceSupervisor(ILogger<DBMaintenanceSupervisor> logger, IMessenger messenger)
        {
            _instanceContext = new InstanceContext(this);
            _dbMaintenanceService = new DuplexServiceInvoker<IDBMaintenanceService>(_instanceContext,
                "DBMaintenanceService", 
                ClassLocator.Default.GetInstance<SerilogLogger<IDBMaintenanceService>>(),
                messenger,
                x => x.SubscribeToChanges(),
                ClientConfiguration.GetDataAccessAddress());
            _logger = logger;
            _messenger = messenger;
        }

        public void DbOperationProgressChanged(DbOperationProgressMessage progressMessage)
        {
            DbMaintenanceProgressChangedEvent?.Invoke(progressMessage);
        }

        public bool BackupDB()
        {
            bool backupRes = _dbMaintenanceService.Invoke(x => x.BackupDB());
            return backupRes;
        }

        public bool RestoreDB(string backupPath)
        {
            bool restoreRes = _dbMaintenanceService.Invoke(x => x.RestoreDB(backupPath));
            return restoreRes;
        }

        public List<string> GetBackupsList()
        {
            return _dbMaintenanceService.Invoke(x => x.GetBackupsList());
        }

        public bool RepairDB(int userId)
        {
            bool repairRes = _dbMaintenanceService.Invoke(x => x.RepairDB(userId));
            return repairRes;
        }
    }
}
