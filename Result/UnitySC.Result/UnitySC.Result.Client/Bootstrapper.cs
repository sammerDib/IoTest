using CommunityToolkit.Mvvm.Messaging;

using MvvmDialogs.FrameworkDialogs;

using UnitySC.Result.Client.ViewModel;
using UnitySC.Result.CommonUI.ViewModel;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ExceptionDialogs;

//using System.ServiceModel;

namespace UnitySC.Result.Client
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

            // ViewModel locator
            MainViewModelLocator.Register();
            //
            CommonUI.Bootstrapper.Register();

            //Service used to display dialog
            ClassLocator.Default.Register<IDialogOwnerService>(() => new DialogOwnerService(ClassLocator.Default.GetInstance<MainResultVM>(),
                                                                                  frameworkDialogFactory: new CustomFrameworkDialogFactory()));

            ClassLocator.Default.GetInstance<ExceptionManager>().Init();
            CommonUI.Bootstrapper.Init();
        }
    }
}
