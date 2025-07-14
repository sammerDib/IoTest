using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using Serilog;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.Shared.Configuration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.UI.Proxy
{
    public class UserSupervisor
    {
        private UnitySC.Shared.Logger.ILogger _logger;
        private IMessenger _messenger;
        private ServiceInvoker<IUserService> _userService;

        public UserSupervisor(ILogger<UserSupervisor> logger, IMessenger messenger)
        {
            _userService = new ServiceInvoker<IUserService>("UserService", ClassLocator.Default.GetInstance<SerilogLogger<IUserService>>(), ClassLocator.Default.GetInstance<IMessenger>(), ClientConfiguration.GetDataAccessAddress());
            _logger = logger;
            _messenger = messenger;

        }

        public User GetUser(int userId)
        {
            var user = _userService.Invoke(x => x.GetUser(userId));
            return user;
        }
    }
}
