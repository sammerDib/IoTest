using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.Dataflow.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;



namespace UnitySC.Dataflow.Client.Supervision
{
    public static class Bootstrapper
    {
        public static void Register()
        {


            //  var currentConfiguration = new ClientConfigurationManager(null);


            SerilogInit.Init("log.DataAccessService.config");
            //SerilogInit.InitWithCurrentAppConfig();




            // Message
            ClassLocator.Default.Register<IMessenger, WeakReferenceMessenger>(true);

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


            //                    ClientConfiguration.GetDataAccessAddress()));


            ClassLocator.Default.Register<MainViewModel>();

        }


    }
}
