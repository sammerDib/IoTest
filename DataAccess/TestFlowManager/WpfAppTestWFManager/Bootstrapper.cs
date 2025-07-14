using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Messaging;

using UnitySC.DataAccess.Service.Implementation.Workflow;
using UnitySC.DataAccess.Service.Implementation.Workflow.Service;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.DataAccess.Service.Interface.Workflow;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

using static WpfAppTestFlowManager.MainViewModel;

namespace WpfAppTestFlowManager
{
    public static class Bootstrapper
    {
        public static void Register()
        {


          //  var currentConfiguration = new ClientConfigurationManager(null);


            SerilogInit.Init("log.DataAccessService.config");
            //SerilogInit.InitWithCurrentAppConfig();




            // Message
            ClassLocator.Default.Register<GalaSoft.MvvmLight.Messaging.IMessenger, GalaSoft.MvvmLight.Messaging.Messenger>(true);

            // Logger with caller name
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));

            // Logger without caller name
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));



           // ILogger log = ClassLocator.Default.GetInstance<SerilogLogger<object>>();

         //   log.Information("Démarrage Application");


            ClassLocator.Default.Register<ServiceInvoker<IDbRecipeService>>(() =>
                new ServiceInvoker<IDbRecipeService>(
                    "RecipeService",
                    ClassLocator.Default.GetInstance<SerilogLogger<IDbRecipeService>>(),
                    ClassLocator.Default.GetInstance<IMessenger>()
                    ));


            
            ClassLocator.Default.Register<ServiceInvoker<IDAP>>(() =>
                new ServiceInvoker<IDAP>(
                    "wfDAP",
                    ClassLocator.Default.GetInstance<SerilogLogger<IDAP>>(),
                    ClassLocator.Default.GetInstance<IMessenger>()
                    ));


            ClassLocator.Default.Register<ServiceInvoker<IWorkflowManagerPP>>(() =>
                new ServiceInvoker<IWorkflowManagerPP>(
                    "wfDAP",
                    ClassLocator.Default.GetInstance<SerilogLogger<IWorkflowManagerPP>>(),
                    ClassLocator.Default.GetInstance<IMessenger>()
                    ));


            //                    ClientConfiguration.GetDataAccessAddress()));


            ClassLocator.Default.Register<MainViewModel>();



            // WorkflowManager => DataAccesServiceBase,IWorkflowManager, ITCWorkflowManager,IWorkflowManagerDAP,IPMWorkflowManager,IPPWorkflowManager

            ClassLocator.Default.Register<WorkflowManager>( true);


            ClassLocator.Default.Register<IWorkflowManagerSupervision, WorkflowManagerSupervisionService>(true);
            ClassLocator.Default.Register<IDAP, DAPService>(true);

            ClassLocator.Default.Register<IWorkflowManager, WorkflowManager>(true);


            ClassLocator.Default.Register<ITCWorkflowManager, WorkflowManager>(true);
            ClassLocator.Default.Register<IWorkflowManagerDAP, WorkflowManager>(true);
            ClassLocator.Default.Register<IPMWorkflowManager, WorkflowManager>(true);
            ClassLocator.Default.Register<IPPWorkflowManager, WorkflowManager>(true);


            ClassLocator.Default.Register<DuplexServiceInvoker<IPMWorkflowManager>>(() =>
                new DuplexServiceInvoker<IPMWorkflowManager>(

                     new InstanceContext( new WorkflowManagerCallback()),


                    "wfPSDActor",
                    
                    ClassLocator.Default.GetInstance<SerilogLogger<IPMWorkflowManager>>(),
                    ClassLocator.Default.GetInstance<IMessenger>()
                    ));


        }


    }
}
