using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.UserManager.Service.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.UI.Proxy
{
    public class PMUserSupervisor : IUserSupervisor
    {
        public event UserChangedEventHandler UserChanged;

        private ServiceInvoker<IPMUserService> _pmUserService;
        private ILogger _logger;
        private ActorType _actorType;

        public PMUserSupervisor(ILogger logger, ActorType actorType)
        {
            var endPoint = "PMUserService";
            endPoint = actorType + endPoint;
            _pmUserService = new ServiceInvoker<IPMUserService>(endPoint, ClassLocator.Default.GetInstance<SerilogLogger<IPMUserService>>(), ClassLocator.Default.GetInstance<IMessenger>(), ClientConfiguration.GetServiceAddress(actorType));
            _logger = logger;
            _actorType = actorType;
        }

        public UnifiedUser CurrentUser { get; private set; }

        public UnifiedUser Connect(string user, string password)
        {
            _logger.Information("Connect " + user);
            var oldUserName = CurrentUser?.Name;
            CurrentUser = null;
            var pmConfiguration = ClassLocator.Default.GetInstance<SharedSupervisors>().GetGlobalStatusSupervisor(_actorType).Configuration;
            try
            {
                if (pmConfiguration.ChamberKey == -1 && pmConfiguration.ToolKey == -1)
                    throw new Exception("Toolkey and ChamberKey are not well defined in configuration");
                else
                    CurrentUser = _pmUserService.Invoke(x => x.Connect(user, password, pmConfiguration.ChamberKey, pmConfiguration.ToolKey));
                if (oldUserName != CurrentUser?.Name && UserChanged != null)
                    UserChanged.Invoke(CurrentUser);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"An error occurred while trying to connect user {user}. <{ex.Message}>");
            }
            return CurrentUser;
        }

        public void Disconnect()
        {
            var oldUserName = CurrentUser?.Name;
            CurrentUser = null;
            if (oldUserName != CurrentUser?.Name && UserChanged != null)
                UserChanged.Invoke(CurrentUser);
        }
    }
}
