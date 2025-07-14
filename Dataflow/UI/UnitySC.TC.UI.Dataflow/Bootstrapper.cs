using System;
using System.IO;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.TC.UI.Dataflow.ViewModel;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Dataflow.Service.Interface;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.PM.Shared.UI.Recipes.Management.ViewModel;
using UnitySC.Shared.Dataflow.Shared.Configuration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;


namespace UnitySC.TC.UI.Dataflow
{
    public class Bootstrapper
    {
        private const string DFConfigPath = "Configuration";
        private const string DFClientConfigurationFilename = "DFClientConfiguration.xml";
        public static void Register(string[] args = null)
        {
            string rootDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName;
            string DFClientConfigFilePath = DFClientConfigurationFilename;
            if (args != null && args.Length > 1)
            {
                rootDir = args[1];
            }
            else
            {
                rootDir = Path.Combine(rootDir, DFConfigPath);
            }
            DFClientConfigFilePath = Path.Combine(rootDir, $"{DFClientConfigFilePath}");

            if (!File.Exists(DFClientConfigFilePath))
            {
                throw new Exception($"Missing dataflow config file <{DFClientConfigFilePath}>");
            }

            var currentDFClientConfiguration = DFClientConfiguration.Init(DFClientConfigFilePath);

            ClassLocator.Default.Register<IDFClientConfiguration>(() => currentDFClientConfiguration, true);

            ClassLocator.Default.Register<DataflowViewModel>(true);            
            ClassLocator.Default.Register<DataflowUTOSimulatorViewModel>(true);


            ClassLocator.Default.Register<ServiceInvoker<IDbRecipeService>>(() => new ServiceInvoker<IDbRecipeService>("RecipeService", ClassLocator.Default.GetInstance<SerilogLogger<IDbRecipeService>>(), ClassLocator.Default.GetInstance<IMessenger>(), ClientConfiguration.GetDataAccessAddress()));


            // <endpoint address="net.tcp://localhost:2222/WorkflowManagerSupervision" binding="netTcpBinding" bindingConfiguration="DefaultNetTcpConfiguration" contract="UnitySC.DataAccess.Service.Interface.Workflow.IWorkflowManagerSupervision" name="WorkflowManagerSupervision" />
            ClassLocator.Default.Register<ServiceInvoker<IDAP>>(() => new ServiceInvoker<IDAP>("DAP", ClassLocator.Default.GetInstance<SerilogLogger<IDAP>>(), ClassLocator.Default.GetInstance<IMessenger>()));
            // PMs Shared supervisors
            ClassLocator.Default.Register<SharedSupervisors>(true);

            var ExternalControls = new ExternalUserControls(new SerilogLogger<object>());
            // External user controls
            ClassLocator.Default.Register<ExternalUserControls>(() => ExternalControls);
            ExternalControls.Init(currentDFClientConfiguration.ExternalUserControlsDir);
            try
            {
                foreach (var pmInit in ExternalControls.PmInits)
                {
                    if ((pmInit.Value.ActorType == UnitySC.Shared.Data.Enum.ActorType.ANALYSE) || (pmInit.Value.ActorType == UnitySC.Shared.Data.Enum.ActorType.EMERA))
                    {
                        pmInit.Value.BootStrap();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
