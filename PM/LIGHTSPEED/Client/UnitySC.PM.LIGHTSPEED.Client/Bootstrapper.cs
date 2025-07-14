using MvvmDialogs.FrameworkDialogs;
using GalaSoft.MvvmLight.Messaging;
using UnitySC.PM.Shared.Hardware.ClientProxy.Chamber;
using UnitySC.PM.Shared.Hardware.ClientProxy.Global;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ExceptionDialogs;

namespace UnitySC.PM.LIGHTSPEED.Client
{
    public class Bootstrapper
    {
        public static void Register()
        {
            SerilogInit.InitWithCurrentAppConfig();

            // Message
            ClassLocator.Default.Register<IMessenger, Messenger>(true);

            // Logger with caller name
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));

            // Logger without caller name
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));

            //Service used to display dialog
            ClassLocator.Default.Register<IDialogOwnerService>(() => new DialogOwnerService(ClassLocator.Default.GetInstance<MainViewModel>(), frameworkDialogFactory: new CustomFrameworkDialogFactory()), true);

            // Main view model
            ClassLocator.Default.Register<MainViewModel>(true);

            // Used for UnitySC.PM.PSD.CommonUI => TC Integration
            ClassLocator.Default.Register<SharedSupervisors>(true);

            // Used for standalone application => All modules
            ClassLocator.Default.Register<GlobalDeviceSupervisor>(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetGlobalDeviceSupervisor(UnitySC.Shared.Data.Enum.ActorType.LIGHTSPEED), true);
            ClassLocator.Default.Register<GlobalStatusSupervisor>(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetGlobalStatusSupervisor(UnitySC.Shared.Data.Enum.ActorType.LIGHTSPEED), true);
            ClassLocator.Default.Register<IUserSupervisor>(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetUserSupervisor(UnitySC.Shared.Data.Enum.ActorType.LIGHTSPEED), true);
            ClassLocator.Default.Register<ChamberSupervisor>(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetChamberSupervisor(UnitySC.Shared.Data.Enum.ActorType.LIGHTSPEED), true);

            UnitySC.PM.Shared.UI.Main.Bootstrapper.Register();

            CommonUI.Bootstrapper.Register();

            ClassLocator.Default.GetInstance<ExceptionManager>().Init();
        }
    }
}
