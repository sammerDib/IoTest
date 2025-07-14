using UnitySC.PM.DMT.CommonUI.ViewModel;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.DMT.CommonUI
{
    public class Bootstrapper
    {
        public static void Register()
        {
            ClassLocator.Default.Register<RecipeSupervisor>(true);

            ClassLocator.Default.Register<CalibrationSupervisor>(true);

            ClassLocator.Default.Register<ScreenSupervisor>(true);

            ClassLocator.Default.Register<CameraSupervisor>(true);

            ClassLocator.Default.Register<AlgorithmsSupervisor>(true);

            ClassLocator.Default.Register<IExecutionMode, ExecutionMode>(true);

            ClassLocator.Default.Register<IRecipeManager, MainRecipeEditionVM>(true);

            ClassLocator.Default.Register<UserSupervisor>(true);

            // ViewModel Locator
            ClassLocator.Default.Register<MainRecipeEditionVM, MainRecipeEditionVM>(true);

        }
    }
}
