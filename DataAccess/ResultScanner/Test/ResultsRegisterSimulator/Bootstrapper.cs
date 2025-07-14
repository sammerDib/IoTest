using CommunityToolkit.Mvvm.Messaging;

using MvvmDialogs.FrameworkDialogs;

using UnitySC.DataAccess.Service.Implementation;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ExceptionDialogs;

namespace ResultsRegisterSimulator
{
    public class Bootstrapper
    {
        public static void Register()
        {
            SerilogInit.InitWithCurrentAppConfig();

            // Logger with caller name
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));

            // Logger without caller name
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));

            // Message
            ClassLocator.Default.Register<IMessenger, WeakReferenceMessenger>(true);

            ClassLocator.Default.Register<ResultSupervisor>(true);

            ClassLocator.Default.Register<RegisterSupervisor>(true);

            ClassLocator.Default.Register(typeof(IDbRecipeService), typeof(RecipeService));

            ClassLocator.Default.Register(typeof(IToolService), typeof(ToolService));

            ViewModelLocator.Register();

            //Service used to display dialog
            ClassLocator.Default.Register<IDialogOwnerService>(() => new DialogOwnerService(ClassLocator.Default.GetInstance<MainRegisterVM>(),
                                                                                  frameworkDialogFactory: new CustomFrameworkDialogFactory()),
                                                                                  true);

            DataAccessConfiguration.SettingsFilePath = @".\DataAccessConfiguration.xml";

            ClassLocator.Default.GetInstance<ExceptionManager>().Init();

            ClassLocator.Default.GetInstance<MainRegisterVM>().Init();
        }
    }
}
