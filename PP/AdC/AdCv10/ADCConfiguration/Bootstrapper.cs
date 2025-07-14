using ADCConfiguration.ViewModel;
using ADCConfiguration.ViewModel.Administration;
using ADCConfiguration.ViewModel.Recipe;
using ADCConfiguration.ViewModel.Tool;
using ADCConfiguration.ViewModel.Tool.TreeView;
using ADCConfiguration.ViewModel.Users;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.Tools;

namespace ADCConfiguration
{
    public static class Bootstrapper
    {
        public static void Register()
        {
            ClassLocator.Default.Register<IMessenger, WeakReferenceMessenger>(true);
            //ServiceLocator.SetLocatorProvider(() => ClassLocator.Default);

            // Service Technique 
            Services.Services.Register();

            // Service Base de Données
            //ClassLocator.Default.Register<IUserService, UserService>(true);
            //ClassLocator.Default.Register<IRecipeService, RecipeService>(true);
            //ClassLocator.Default.Register<IConfigurationService, ConfigurationService>(true);
            //ClassLocator.Default.Register<IImportExportService, ImportExportService>(true);
            //ClassLocator.Default.Register<IToolService, ToolService>(true);
            //ClassLocator.Default.Register<IVidService, VidService>(true);
            //ClassLocator.Default.Register<IWaferTypeService, WaferTypeService>(true);
            //ClassLocator.Default.Register<ILogService, LogService>(true);

            // ViewModel
            ViewModelLocator.Register();
            Services.Services.Instance.NavigationService.Register<MainMenuViewModel>(Services.NavNameEnum.MainMenu);
            Services.Services.Instance.NavigationService.Register<ImportRecipeViewModel>(Services.NavNameEnum.ImportRecipe);
            Services.Services.Instance.NavigationService.Register<ExportRecipeViewModel>(Services.NavNameEnum.ExportRecipe);
            Services.Services.Instance.NavigationService.Register<RecipeHistoryViewModel>(Services.NavNameEnum.RecipeHistory);
            Services.Services.Instance.NavigationService.Register<EditUserViewModel>(Services.NavNameEnum.UserManager);
            //Services.Services.Instance.NavigationService.Register<ToolConfigurationViewModel>(Services.NavNameEnum.Tool);
            //Services.Services.Instance.NavigationService.Register<ExportConfigurationViewModel>(Services.NavNameEnum.ExportConfiguration);
            //Services.Services.Instance.NavigationService.Register<EditWaferTypeViewModel>(Services.NavNameEnum.WaferTypes);
            Services.Services.Instance.NavigationService.Register<EditVidViewModel>(Services.NavNameEnum.Vids);
            //Services.Services.Instance.NavigationService.Register<MachineConfigurationHistoryViewModel>(Services.NavNameEnum.MachineConfigurationArchive);
            Services.Services.Instance.NavigationService.Register<LogsViewModel>(Services.NavNameEnum.Logs);
            Services.Services.Instance.NavigationService.Register<ArchivedRecipeViewModel>(Services.NavNameEnum.ArchivedRecipe);

            // Mil
            //libMIL.Mil.Instance.Allocate();
        }
    }
}
