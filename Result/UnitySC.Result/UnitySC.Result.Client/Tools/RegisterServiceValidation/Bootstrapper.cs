using GalaSoft.MvvmLight.Messaging;
using MvvmDialogs.FrameworkDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ExceptionDialogs;

namespace WpfUnityControlRegisterValidation
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
            ClassLocator.Default.Register<IMessenger, Messenger>(true);

           // ClassLocator.Default.Register<MainRegisterVM>(true);

            ClassLocator.Default.Register<RegisterSupervisor>(true);

            //Service used to display dialog
            ClassLocator.Default.Register<IDialogOwnerService>(() => new DialogOwnerService(ClassLocator.Default.GetInstance<MainRegisterVM>(),
                                                                                                frameworkDialogFactory: new CustomFrameworkDialogFactory()));
            // ViewModel locator 
            ViewModelLocator.Register();

            ClassLocator.Default.GetInstance<ExceptionManager>().Init();

            ClassLocator.Default.GetInstance<MainRegisterVM>().Init();

        }
    }
}
