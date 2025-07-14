using CommunityToolkit.Mvvm.Messaging;

using MvvmDialogs.FrameworkDialogs;

using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.Result._3DaViewer
{
    public class Bootstrapper
    {
        public static void Register()
        {
            // Logger with caller name
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));

            // Logger without caller name
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));

            // Message
            ClassLocator.Default.Register<IMessenger, WeakReferenceMessenger>(true);
                        
            // Service used to display dialog
            ClassLocator.Default.Register<IDialogOwnerService>(() => new DialogOwnerService(App.Instance.MainWindowViewModel, frameworkDialogFactory: new CustomFrameworkDialogFactory()));            

        }
    }
}
