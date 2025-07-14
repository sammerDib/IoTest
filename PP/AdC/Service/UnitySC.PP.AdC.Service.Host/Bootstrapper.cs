using System.IO;

using ADCEngine;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Dataflow.Service.Interface;
using UnitySC.PM.Shared.UserManager.Service.Implementation;
using UnitySC.PM.Shared.UserManager.Service.Interface;
using UnitySC.PP.ADC.Service.Implementation;
using UnitySC.PP.ADC.Service.Interface;
using UnitySC.PP.ADC.Service.Interface.Recipe;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PP.ADC.Service.Host
{
    public static class Bootstrapper
    {
        private const string PPConfigurationFilePath = @"Configuration\PPConfiguration.xml";



        public static void Register()
        {

            SerilogInit.Init("log.ADCServiceHost.config");


            //SerilogInit.InitForConsole();

            // Logger with caller name
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));

            // Logger without caller name
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));

            // Message
            ClassLocator.Default.Register<IMessenger, WeakReferenceMessenger>(true);


            // Hardware manager
            //ClassLocator.Default.Register<HardwareManager>(true);

            // global status service
            // //ClassLocator.Default.Register<GlobalStatusService>(true);
            // //ClassLocator.Default.Register<IGlobalStatusServer>(() => ClassLocator.Default.GetInstance<GlobalStatusService>());
            // //ClassLocator.Default.Register<IGlobalStatusService>(() => ClassLocator.Default.GetInstance<GlobalStatusService>());

            ADCConfiguration.Bootstrapper.Register();

            ClassLocator.Default.Register<IADCRecipeService>(() => ClassLocator.Default.GetInstance<AdcRecipeService>());
            ClassLocator.Default.Register<IADCService>(() => ClassLocator.Default.GetInstance<ADCService>());

            ClassLocator.Default.Register<IAdcExecutor>(() => ClassLocator.Default.GetInstance<AdcExecutor>());



            ClassLocator.Default.Register<ServiceInvoker<IDAP>>(() =>
                new ServiceInvoker<IDAP>(
                    "wfDAP",
                    ClassLocator.Default.GetInstance<SerilogLogger<IDAP>>(),
                    ClassLocator.Default.GetInstance<IMessenger>()
                    ));
        

            // PM Configuration
            ClassLocator.Default.Register<PPConfigurationADC>(() => PPConfigurationADC.Init(Path.Combine(Path.GetDirectoryName(PathHelper.GetExecutingAssemblyPath()), PPConfigurationFilePath)), true);

            // PM User service
            ClassLocator.Default.Register(typeof(IPMUserService), typeof(PMUserService), true);
        }
    }
}
