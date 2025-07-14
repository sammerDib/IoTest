using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.ANA.EP.Mountains.Interface;
using UnitySC.PM.ANA.EP.Mountains.Server.Implementation;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.EP.Mountains.Server.Host
{
    public class Bootstrapper
    {
        public static void Register(string[] args = null)
        {
            SerilogInit.Init(@".\mountainsServerLog.config");

            // Logger with caller name
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));

            // Logger without caller name
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));

            ClassLocator.Default.Register<MountainsConfiguration>(()=>MountainsConfiguration.Init(@".\MountainsConfiguration.xml"), true);

            ClassLocator.Default.Register<IMountainsGatewayService, MountainsGatewayService>(true);
            ClassLocator.Default.Register<IMessenger>(() => WeakReferenceMessenger.Default, true);
            ClassLocator.Default.Register<MountainsActiveXSupervisor>(true);
        }
    }
}
