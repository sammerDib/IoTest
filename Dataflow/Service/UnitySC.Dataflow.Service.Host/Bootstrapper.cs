using CommunityToolkit.Mvvm.Messaging;
using UnitySC.ADCAS300Like.Common;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Dataflow.Configuration;
using UnitySC.Dataflow.Manager;
using UnitySC.Dataflow.Service.Interface;
using UnitySC.PM.Shared;
using UnitySC.Shared.Configuration;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.FDC;
using UnitySC.Shared.FDC.Interface;
using UnitySC.Shared.FDC.Service;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Dataflow.Service.Host
{
    public static class Bootstrapper
    {
        public static void Register(string[] args = null)
        {
            // Dataflow Configuration
            var currentConfiguration = new ServiceDFConfigurationManager(args);
            ClassLocator.Default.Register<IServiceDFConfigurationManager>(() => currentConfiguration, true);

            //Automation Configuration
            var currentAutomationConfiguration = new AutomationConfiguration(ActorType.DataflowManager, currentConfiguration?.InputConfigurationName);
            ClassLocator.Default.Register<IAutomationConfiguration>(() => currentAutomationConfiguration, true);

            //ADC Configuration
            var currentADCConfiguration = new ADCConfiguration(currentConfiguration?.InputConfigurationName);
            ClassLocator.Default.Register<IADCConfiguration>(() => currentADCConfiguration, true);
            var adcsConfig = ADCsConfigs.Init(currentADCConfiguration.ADCConfigurationFilePath);
            ClassLocator.Default.Register<ADCsConfigs>(() => adcsConfig, true);

            // Init logger
            SerilogInit.Init(currentConfiguration.LogConfigurationFilePath);

            // DataCollectionConvert
            
            ClassLocator.Default.Register<IDataCollectionConvert,DataCollectionConvert>(true);

            // Messenger
            ClassLocator.Default.Register<IMessenger, WeakReferenceMessenger>(true);

            // Logger with caller name
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));

            // Logger without caller name
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));

            // TC Configuration
            ClassLocator.Default.Register<DFServerConfiguration>(() => DFServerConfiguration.Init(currentConfiguration.DFServerConfigurationFilePath), true);

            // Connection au service de Base de données
            ClassLocator.Default.Register<ServiceInvoker<IDbRecipeService>>(() =>
               new ServiceInvoker<IDbRecipeService>(
                   "RecipeService",
                   ClassLocator.Default.GetInstance<SerilogLogger<IDbRecipeService>>(),
                   ClassLocator.Default.GetInstance<IMessenger>(),
                   ClassLocator.Default.GetInstance<DFServerConfiguration>().DataAccessAddress));

  

            // Connection au DAP
            ClassLocator.Default.Register<ServiceInvoker<IDAP>>(() =>
                new ServiceInvoker<IDAP>(
                    "DAP",
                    ClassLocator.Default.GetInstance<SerilogLogger<IDAP>>(),
                    ClassLocator.Default.GetInstance<IMessenger>(),
                    ClassLocator.Default.GetInstance<DFServerConfiguration>().DAPAddress));


            UnitySC.Dataflow.Manager.Bootstrapper_Service.Register();
            // Database FDC service
            ClassLocator.Default.Register(typeof(IFDCService), typeof(FDCService), true);

            ClassLocator.Default.Register(() => new FDCManager(currentConfiguration.FDCsConfigurationFilePath, currentConfiguration.FDCsPersistentDataFilePath), true);

            ClassLocator.Default.Register<SendFdcSupervisor>(true);

  
        }
    }
}
