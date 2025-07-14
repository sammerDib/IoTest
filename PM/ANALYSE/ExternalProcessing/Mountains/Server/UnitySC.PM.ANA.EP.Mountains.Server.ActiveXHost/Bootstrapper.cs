using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.ANA.EP.Mountains.Server.Implementation;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.EP.Mountains.Server.ActiveXHost
{
    public class Bootstrapper
    {
        public static void Register(string[] args = null)
        {
            SerilogInit.Init(@".\activeXHostLog.config");

            // Logger with caller name
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));

            // Logger without caller name
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));

            ClassLocator.Default.Register<IMountainsActiveXService, MountainsActiveXService>();

        }            
    }
}
