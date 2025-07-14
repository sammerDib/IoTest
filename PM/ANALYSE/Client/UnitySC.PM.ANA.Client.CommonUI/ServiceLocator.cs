using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.PM.ANA.Client.Proxy.Recipe;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.CommonUI
{
    public class ServiceLocator : Proxy.ServiceLocator
    {
        public static INavigationManager NavigationManager => ClassLocator.Default.GetInstance<INavigationManager>();
        public static IRecipeManager RecipeManager => ClassLocator.Default.GetInstance<IRecipeManager>();

        public static IDialogOwnerService DialogService => ClassLocator.Default.GetInstance<IDialogOwnerService>();

        public static ServiceInvoker<IDbRecipeService> DbRecipeService => ClassLocator.Default.GetInstance<ServiceInvoker<IDbRecipeService>>();
    }
}
