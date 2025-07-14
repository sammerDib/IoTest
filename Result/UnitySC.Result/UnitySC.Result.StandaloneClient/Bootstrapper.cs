using CommunityToolkit.Mvvm.Messaging;

using MvvmDialogs.FrameworkDialogs;

using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Factory;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ExceptionDialogs;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.Result.StandaloneClient
{
    public static class Bootstrapper
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

            ClassLocator.Default.Register<NotifierVM>(true);

            // Service interne pour la gestion des format de resultats
            ClassLocator.Default.Register(typeof(IResultDataFactory), typeof(ResultDataFactory), true);

            //Service used to display dialog
            ClassLocator.Default.Register<IDialogOwnerService>(() => new DialogOwnerService(App.Instance.MainWindowViewModel, frameworkDialogFactory: new CustomFrameworkDialogFactory()));

            ClassLocator.Default.GetInstance<ExceptionManager>().Init();
        }
    }
}
