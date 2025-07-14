using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Forms;

using CommunityToolkit.Mvvm.Messaging;

using Serilog;

using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;

using MvvmDialogs.FrameworkDialogs;
using UnitySC.Shared.UI.Dialog;
using ADC.ViewModel;
using UnitySC.Shared.Tools.Service;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.PM.Shared;
using UnitySC.Shared.Data;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.PM.Shared.Configuration;
using ADC.User;
namespace ADC
{
    public static class Bootstrapper
    {
        public static void Register(string[] args = null)
        {

            ClassLocator.Default.Register<IMessenger, WeakReferenceMessenger>(true);

            // Proxy for services in DataAccess
            ClassLocator.Default.Register<DbRecipeServiceProxy>(true);
            ClassLocator.Default.Register<DbToolServiceProxy>(true);
            ClassLocator.Default.Register<DbRegisterResultServiceProxy>(true);
            
            ClassLocator.Default.Register(() => new ServiceInvoker<IDbRecipeService>("RecipeService", ClassLocator.Default.GetInstance<SerilogLogger<IDbRecipeService>>(), ClassLocator.Default.GetInstance<IMessenger>(), ClientConfiguration.GetDataAccessAddress()));


            // Dialogue service
            ClassLocator.Default.Register<IDialogOwnerService>(() => new DialogOwnerService(ClassLocator.Default.GetInstance<MainWindowViewModel>(), frameworkDialogFactory: new CustomFrameworkDialogFactory()), true);


            var moduleConfigFilePath = @".\Configuration\ModuleConfiguration.xml";
            if (args != null && args.Length > 1 && args[0]=="-mc"  ) 
            {
                moduleConfigFilePath = args[1];
            }

            ClassLocator.Default.Register<ModuleConfiguration>(() => ModuleConfiguration.Init(moduleConfigFilePath), true);

            var clientConfigFilePath = @".\Configuration\ClientConfiguration.xml";
            if (args != null && args.Length > 3 && args[2] == "-cc")
            {
                clientConfigFilePath = args[3];
            }

            ClassLocator.Default.Register<ClientConfiguration>(() => ClientConfiguration.Init(clientConfigFilePath), true);


            // Logger with caller name
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));

            // Logger without caller name
            ClassLocator.Default.Register(typeof(UnitySC.Shared.Logger.ILogger), typeof(SerilogLogger<object>));
            
            ClassLocator.Default.Register<SharedSupervisors>(true);

            ClassLocator.Default.Register<IUserSupervisor>(() => new ADCUserSupervisor() , true);


            // Service
            Services.Services.Register();

            // ViewModel
            ViewModel.ViewModelLocator.Register();

            // Init de la Base de Données
            //...........................
            bool useExportedDataBase = Convert.ToBoolean(ConfigurationManager.AppSettings["DatabaseConfig.UseExportedDatabase"]);
        
            //Database.Service.BootStrapper.DefaultRegister(useExportedDataBase); // Database.Service. DEPRECATED ---

            if (!useExportedDataBase)
            {
                // Premier appel base de données pour initialiser la connexion
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                       bool DbIsConnectedAndVersionOK = ClassLocator.Default.GetInstance<DbToolServiceProxy>().CheckDatabaseVersion();
                        if (!DbIsConnectedAndVersionOK)
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke((() =>
                            {
                                Log.Error("Database version is not up to date");
                                // Attention la dialog modal peux passer en arrière plan derrière la main window (dirty code en attendant l'implementation du notifier dans l'adc)
                                MessageBox.Show("Database version is not up to date", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }));
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke((() =>
                        {
                            Log.Error(ex, "Remote database initialisation error - Check DataAccess Connection");
                            // Attention la dialog modal peux passer en arrière plan derrière la main window (dirty code en attendant l'implementation du notifier dans l'adc)
                            MessageBox.Show("Remote Database error - Check DataAccess Connection", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }));
                    }
                });
            }
        }



    }
}
