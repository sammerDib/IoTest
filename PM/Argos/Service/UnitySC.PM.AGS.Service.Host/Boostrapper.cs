using UnitySC.PM.AGS.Hardware.Manager;
using UnitySC.PM.AGS.Service.Core.Referentials;
using UnitySC.PM.AGS.Service.Implementation;
using UnitySC.PM.AGS.Service.Implementation.Proxy;
using UnitySC.PM.AGS.Service.Interface.RecipeService;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Implementation;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.PM.Shared.UserManager.Service.Implementation;
using UnitySC.PM.Shared.UserManager.Service.Interface;
using UnitySC.Shared.Data.Configuration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.AGS.Service.Host
{
    public static class Bootstrapper
    {
        public static void Register(string[] args = null)
        {
            // Configuration
            var currentConfiguration = new ServiceConfigurationManager(args);
            
            // Init Logger
            SerilogInit.Init(currentConfiguration.LogConfigurationFilePath);
            
            // ConfigurationManager
            ClassLocator.Default.Register<IServiceConfigurationManager>(() => currentConfiguration, true);

            // Logger with caller name
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));

            // Logger without caller name
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));

            // Message
            ClassLocator.Default.Register<GalaSoft.MvvmLight.Messaging.IMessenger, GalaSoft.MvvmLight.Messaging.Messenger>(true);

            // Recipe service
            ClassLocator.Default.Register(typeof(IRecipeService), typeof(ArgosRecipeService), true);
            // Hardware manager
            ClassLocator.Default.Register(typeof(ArgosHardwareManager), typeof(ArgosHardwareManager), singleton: true);
            //ClassLocator.Default.Register<HardwareManager>(() => ClassLocator.Default.GetInstance<ArgosHardwareManager>(), singleton: true);

            // global status service
            ClassLocator.Default.Register<GlobalStatusService>(true);
            ClassLocator.Default.Register<IGlobalStatusServer>(() => ClassLocator.Default.GetInstance<GlobalStatusService>());
            ClassLocator.Default.Register<IGlobalStatusService>(() => ClassLocator.Default.GetInstance<GlobalStatusService>());

            // Referential management
            ClassLocator.Default.Register(typeof(IReferentialManager), typeof(ReferentialManager), true);

            //Todo register service

            // PM Configuration
            ClassLocator.Default.Register<PMConfiguration>(() => PMConfiguration.Init(ClassLocator.Default.GetInstance<IServiceConfigurationManager>().PMConfigurationFilePath), true);

            ClassLocator.Default.Register<DBRecipeServiceProxy>(true);

            // PM User service
            ClassLocator.Default.Register(typeof(IPMUserService), typeof(PMUserService), true);
        }
    }
}
