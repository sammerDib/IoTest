using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.DataAccess.ResultScanner.Interface;
using UnitySC.DataAccess.Service.Implementation;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.FDC;
using UnitySC.Shared.FDC.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.DataAccess.Service.Host
{
    /// <summary>
    /// Service to acess to SQL DATA
    /// </summary>
    public class DataAccessService
    {
        private readonly Dictionary<string, ServiceHost> _hosts = new Dictionary<string, ServiceHost>();
        private readonly ILogger _logger;
        private readonly IResultService _resultService;
        private readonly IDbRecipeService _recipeService;
        private readonly IRegisterResultService _registerService;
        private readonly IToolService _toolService;
        private readonly IUserService _userService;
        private readonly ILogService _logService;
        private readonly IDBMaintenanceService _dbMaintenanceService;
        private readonly IFDCService _fdcService;
        private readonly ISendFdcService _sendFdcService;

        public DataAccessService(IResultService resultService, IRegisterResultService registerService, IDbRecipeService recipeService, IToolService toolservice, IUserService userService, ILogService logService, ILogger logger, IDBMaintenanceService dbMaintenanceService, IFDCService fdcService, ISendFdcService sendFdcService)
        {
            _resultService = resultService;
            _recipeService = recipeService;
            _logger = logger;
            _registerService = registerService;
            _toolService = toolservice;
            _userService = userService;
            _logService = logService;
            _dbMaintenanceService = dbMaintenanceService;
            _fdcService = fdcService;
            _sendFdcService = sendFdcService;
        }

        public void Start()
        {
            string conString = DataAccessConfiguration.Instance.DbConnectionString; 
            _logger.Verbose($"Entity ConnectionString in App.config = {conString}");

            var ecsb = new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder(conString);
            using (var sqlCon = new System.Data.SqlClient.SqlConnection(ecsb.ProviderConnectionString))
            {
                _logger.Information(".");
                _logger.Information("-------------------------------------");
                _logger.Information("--     SQL Database Connection     --");
                _logger.Information("-------------------------------------");
                _logger.Information($"-- Data Source = {sqlCon.DataSource}");
                _logger.Information($"-- Database = {sqlCon.Database}");
                _logger.Information("-------------------------------------");
                _logger.Information(".");
            }

            var isDatabaseUptodate = _resultService.CheckDatabaseVersion();
            if (!isDatabaseUptodate.Result)
            {
                _logger.Error($"-----------------------------");
                _logger.Error($"- Database is not up to date ");
                foreach(var msg in isDatabaseUptodate.Messages)
                    _logger.Error($"- => {msg.UserContent}");
                _logger.Error($"-----------------------------");

            }

            StartService("result", _resultService);
            StartService("recipe", _recipeService);
            StartService("result registration", _registerService);
            StartService("tool", _toolService);
            StartService("user", _userService);
            StartService("log", _logService);
            StartService("dbMaintenance", _dbMaintenanceService);
            StartService("fdcService", _fdcService);
            StartService("sendFdcService", _sendFdcService);
            _logger.Information("Start Result scanner");
            ClassLocator.Default.GetInstance<IResultScanner>().Start();
            ClassLocator.Default.GetInstance<FDCManager>().StartMonitoringFDC();
        }

        public void Stop()
        {
            var hoststcopy = new Dictionary<string, ServiceHost>(_hosts);
            foreach (var kvp in hoststcopy)
                StopService(name: kvp.Key, host: kvp.Value);
        }

        private void StartService(string name, object service)
        {
            var host = new ServiceHost(service);
            foreach (var endpoint in host.Description.Endpoints)
            {
                _logger.Information($"Creating {name} service on {endpoint.Address}");
            }
            host.Open();
            _hosts.Add(name, host);
        }

        private void StopService(string name, ServiceHost host)
        {
            _logger.Information($"Stop {name} service...");
            if (_hosts.ContainsKey(name))
                _hosts.Remove(name);
            host.Close();
        }     
    }
}
