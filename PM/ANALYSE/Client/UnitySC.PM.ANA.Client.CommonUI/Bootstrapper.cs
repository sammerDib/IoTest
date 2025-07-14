using System;
using System.Globalization;

using UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps.Measures;
using UnitySC.PM.ANA.Client.CommonUI.ViewModel;
using UnitySC.PM.ANA.Client.CommonUI.ViewModel.Navigation;
using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Alignment;
using UnitySC.PM.ANA.Client.Proxy.Axes;
using UnitySC.PM.ANA.Client.Proxy.Calibration;
using UnitySC.PM.ANA.Client.Proxy.Chuck;
using UnitySC.PM.ANA.Client.Proxy.Context;
using UnitySC.PM.ANA.Client.Proxy.KeyboardMouseHook;
using UnitySC.PM.ANA.Client.Proxy.Light;
using UnitySC.PM.ANA.Client.Proxy.Measure;
using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.ANA.Client.Proxy.Recipe;
using UnitySC.PM.Shared.Hardware.ClientProxy.Referential;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.PM.Shared.UI.Recipes.Management;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.ANA.Client.CommonUI
{
    public class Bootstrapper
    {
        public static void Register()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            // NotifierVM
            ClassLocator.Default.Register<NotifierVM>(true);

            if (!ClassLocator.Default.IsRegistered<GlobalStatusSupervisor>())
            {
                // It is already registered by the client but this register is usefull when executed inside UTO
                ClassLocator.Default.Register<GlobalStatusSupervisor>(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetGlobalStatusSupervisor(UnitySC.Shared.Data.Enum.ActorType.ANALYSE), true);
            }

            if (!ClassLocator.Default.IsRegistered<PMViewModel>())
            {
                ClassLocator.Default.Register<PMViewModel>(() => new PMViewModel(ActorType.ANALYSE, ApplicationMode.Production, null, false), true);
            }

            ClassLocator.Default.Register<RecipeEditionVM>(true);
            ClassLocator.Default.Register<IRecipeManager, RecipeEditionVM>(true);

            ClassLocator.Default.Register<RecipesManagementViewModel>(true);

            ClassLocator.Default.Register<ANARecipeSupervisor>(true);

            ClassLocator.Default.Register<INavigationManager, NavigationManager>(true);

            ClassLocator.Default.Register<AxesSupervisor>(true);

            // Ana
            ClassLocator.Default.Register<CamerasSupervisor>(true);

            ClassLocator.Default.Register<LightsSupervisor>(true);

            ClassLocator.Default.Register<AlgosSupervisor>(true);

            ClassLocator.Default.Register<ReferentialSupervisor>(true);

            ClassLocator.Default.Register<CalibrationSupervisor>(true);

            ClassLocator.Default.Register<MeasureSupervisor>(true);

            //// Probe supervisor
            ClassLocator.Default.Register<ProbesSupervisor>(true);

            ClassLocator.Default.Register<IProbesFactory, ProbesVieModelFactory>(true);
                    
            // Alignment Service
            ClassLocator.Default.Register<ProbeAlignmentSupervisor>(true);

            // Mapper mvvm
            ClassLocator.Default.Register<Mapper>();

            ClassLocator.Default.Register<IKeyboardMouseHook, KeyboardMouseHook>(true);

            // Context manager
            ClassLocator.Default.Register<ContextSupervisor>(true);

            // User supervisor
            ClassLocator.Default.Register<UserSupervisor>(true);

            // Mountains
            ClassLocator.Default.Register<MountainsWPFControl>(true);

            //Chuck supervisor
            ClassLocator.Default.Register<ChuckSupervisor>(true);
        }
    }
}
