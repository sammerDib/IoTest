using CommunityToolkit.Mvvm.Messaging;

using Moq;

using SimpleInjector;
using UnitySC.ADCAS300Like.Common;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Dataflow.Configuration;
using UnitySC.Dataflow.Manager;
using UnitySC.Dataflow.Operations.Implementation;
using UnitySC.Dataflow.Service.Implementation;
using UnitySC.Dataflow.Service.Interface;
using UnitySC.PM.Shared;
using UnitySC.Shared.Configuration;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.PM.Operations.Interface.UTOOperations;
using UnitySC.Shared.TC.Shared.Operations.Implementation;
using UnitySC.Shared.TC.Shared.Operations.Interface;
using UnitySC.Shared.TC.Shared.Service.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Dataflow.Service.Test
{
    public static class Bootstrapper
    {
        public static void Register(Container container)
        {
            ClassLocator.ExternalInit(container, true);
            // Dataflow Configuration
            var currentConfiguration = new FakeDFConfiguration();
            ClassLocator.Default.Register<IServiceDFConfigurationManager>(() => currentConfiguration, true);

            //Automation Configuration
            var currentAutomationConfiguration = new AutomationConfiguration(ActorType.DataflowManager, currentConfiguration?.InputConfigurationName);
            ClassLocator.Default.Register<IAutomationConfiguration>(() => currentAutomationConfiguration, true);

            //ADC Configuration
            var currentADCConfiguration = new ADCConfiguration(currentConfiguration?.InputConfigurationName);
            ClassLocator.Default.Register<IADCConfiguration>(() => currentADCConfiguration, true);
            var adcsConfig = ADCsConfigs.Init(currentADCConfiguration.ADCConfigurationFilePath);
            ClassLocator.Default.Register<ADCsConfigs>(() => adcsConfig, true);

            // DataCollectionConvert            
            ClassLocator.Default.Register<IDataCollectionConvert,DataCollectionConvert>(true);

            // Messenger
            ClassLocator.Default.Register<IMessenger, WeakReferenceMessenger>(true);

            // Logger with caller name
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));


            // Logger without caller name
            var mockLogger = Mock.Of<SerilogLogger<object>>();
            ClassLocator.Default.Register<ILogger>(() => mockLogger);

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


            ClassLocator.Default.Register<IDFManager, DFManager>(singleton: true);
            ClassLocator.Default.Register<ICommonEventOperations, CommonEventOperations>(singleton: true);
            ClassLocator.Default.Register<ICommonEventServiceCB, UTODFService>(singleton: true);
            ClassLocator.Default.Register<IPMDFOperations, PMDFOperations>(singleton: true);

  
        }
    }
}
