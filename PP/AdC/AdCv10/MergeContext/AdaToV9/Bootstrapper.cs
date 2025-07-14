using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.Tools;

namespace AdaToAdc
{
    public static class Bootstrapper
    {
        public static void Register()
        {
            ClassLocator.Default.Register<IMessenger, WeakReferenceMessenger>(true);
   
        }
    }
}
