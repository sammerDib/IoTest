using System;

using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Status.Service.Implementation;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.PM.Shared.UserManager.Service.Implementation;
using UnitySC.PM.Shared.UserManager.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

using UnitySC.PM.HLS.Hardware.Manager;
using UnitySC.Shared.Data.Configuration;

namespace UnitySC.PM.HLS.Service.Host
{
    public static class Bootstrapper
    {
      
        public static void Register(string[] args = null)
        {
            // Configuration
            var currentConfiguration = new ServiceConfigurationManager(args);

            // Init logger
            SerilogInit.Init(currentConfiguration.LogConfigurationFilePath);
            ClassLocator.Default.Register<IServiceConfigurationManager>(() => currentConfiguration, true);

            // Logger with caller name
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));

            // Logger without caller name
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));

            // Message
            ClassLocator.Default.Register<GalaSoft.MvvmLight.Messaging.IMessenger, GalaSoft.MvvmLight.Messaging.Messenger>(true);

            // Hardware manager
            ClassLocator.Default.Register(typeof(HlsHardwareManager), typeof(HlsHardwareManager), singleton: true);
            ClassLocator.Default.Register(typeof(HardwareManager), typeof(HlsHardwareManager), singleton: true);
            ClassLocator.Default.Register<IHardwareManager>(() => ClassLocator.Default.GetInstance<HlsHardwareManager>());

            // global status service
            ClassLocator.Default.Register<GlobalStatusService>(true);
            ClassLocator.Default.Register<IGlobalStatusServer>(() => ClassLocator.Default.GetInstance<GlobalStatusService>());
            ClassLocator.Default.Register<IGlobalStatusService>(() => ClassLocator.Default.GetInstance<GlobalStatusService>());

            // PM Configuration
            ClassLocator.Default.Register<PMConfiguration>(() => PMConfiguration.Init(ClassLocator.Default.GetInstance<IServiceConfigurationManager>().PMConfigurationFilePath), true);

            // PM User service
            ClassLocator.Default.Register(typeof(IPMUserService), typeof(PMUserService), true);


        }
    }
}
