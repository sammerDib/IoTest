using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.PM.Shared.UI.Recipes.Management;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.UI.Main
{
    public class Bootstrapper
    {
        public static void Register()
        {
            ClassLocator.Default.Register<ExternalUserControls>(true);
            ClassLocator.Default.Register(() => new ServiceInvoker<IDbRecipeService>("RecipeService", ClassLocator.Default.GetInstance<SerilogLogger<IDbRecipeService>>(), ClassLocator.Default.GetInstance<IMessenger>(), ClientConfiguration.GetDataAccessAddress()));
        }
    }
}
