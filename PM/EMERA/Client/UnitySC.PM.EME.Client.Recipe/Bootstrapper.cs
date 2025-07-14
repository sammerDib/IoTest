using System.Globalization;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Client.Proxy.Axes;
using UnitySC.PM.EME.Client.Proxy.Calibration;
using UnitySC.PM.EME.Client.Proxy.Camera;
using UnitySC.PM.EME.Client.Proxy.Chuck;
using UnitySC.PM.EME.Client.Proxy.FilterWheel;
using UnitySC.PM.EME.Client.Proxy.KeyboardMouseHook;
using UnitySC.PM.EME.Client.Proxy.Light;
using UnitySC.PM.EME.Client.Proxy.Recipe;
using UnitySC.PM.EME.Client.Recipe.ViewModel;
using UnitySC.PM.EME.Client.Recipe.ViewModel.Navigation;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.EME.Service.Interface.Camera;
using UnitySC.PM.EME.Service.Interface.Chuck;
using UnitySC.PM.EME.Service.Interface.FilterWheel;
using UnitySC.PM.EME.Service.Interface.Light;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.PM.Shared.Hardware.ClientProxy.Referential;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Client.Recipe
{
    public class Bootstrapper
    {
        public static void Register()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;            
            ClassLocator.Default.Register<RecipeEditionVM>(true);
            ClassLocator.Default.Register<IRecipeManager, RecipeEditionVM>(true);

            ClassLocator.Default.Register<IEMERecipeService, EMERecipeSupervisor>(true);
            ClassLocator.Default.Register<INavigationManagerForRecipeEdition, NavigationManagerForRecipeEdition>(true);

            ClassLocator.Default.Register<IKeyboardMouseHook, KeyboardMouseHook>(true);
            ClassLocator.Default.Register<IEMELightService, LightsSupervisor>(true);
            ClassLocator.Default.Register<IEMELightServiceCallback, LightsSupervisor>(true);
            ClassLocator.Default.Register<LightsSupervisor>(true);
            ClassLocator.Default.Register<LightBench>(true);
            ClassLocator.Default.Register<IEMEChuckService, ChuckSupervisorEx>(true);
            ClassLocator.Default.Register(GetChuckVM, true);
            ClassLocator.Default.Register<ICalibrationService, CalibrationSupervisor>(true);
            ClassLocator.Default.Register<ICameraServiceEx, UnitySC.PM.EME.Client.Proxy.Camera.CameraSupervisorEx>(true);
            ClassLocator.Default.Register<CameraBench>(true);
            ClassLocator.Default.Register<IFilterWheelService, FilterWheelSupervisor>(true);
            ClassLocator.Default.Register<FilterWheelBench>(true);
            ClassLocator.Default.Register<Mapper>();
        }
        private static ChuckVM GetChuckVM()
        {           
            return new ChuckVM(ClassLocator.Default.GetInstance<ChuckSupervisorEx>(),
                ClassLocator.Default.GetInstance<CalibrationSupervisor>(),
                ClassLocator.Default.GetInstance<EmeraMotionAxesSupervisor>(),
                ClassLocator.Default.GetInstance<ReferentialSupervisor>(),
                ClassLocator.Default.GetInstance<SharedSupervisors>().GetGlobalStatusSupervisor(ActorType.EMERA),                
                ClassLocator.Default.GetInstance<ILogger>(),
                ClassLocator.Default.GetInstance<IMessenger>());
        }
    }
}

