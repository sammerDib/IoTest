using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.ResultScanner.Interface;
using UnitySC.DataAccess.Service.Implementation;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.FDC;
using UnitySC.Shared.FDC.Interface;
using UnitySC.Shared.FDC.Service;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.DataAccess.Service.Host
{
    public static class Bootstrapper
    {
        public static void Register()
        {
            SerilogInit.InitWithCurrentAppConfig();

            // Logger with caller name
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));

            // Logger without caller name
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));

            // Message
            ClassLocator.Default.Register<IMessenger, WeakReferenceMessenger>(true);

            // Service interne pour la gestion des format de resultats
            ClassLocator.Default.Register(typeof(IResultDataFactory), typeof(Shared.Format.Factory.ResultDataFactory), true);

            // Service interne pour la gestion des stats et des imagettes
            ClassLocator.Default.Register<ResultScanner.Implementation.ResultScanner>(true);
            ClassLocator.Default.Register<IResultScanner>(() => ClassLocator.Default.GetInstance<ResultScanner.Implementation.ResultScanner>());
            ClassLocator.Default.Register<IResultScannerServer>(() => ClassLocator.Default.GetInstance<ResultScanner.Implementation.ResultScanner>());

            // Service d'insertion résultats
            ClassLocator.Default.Register<RegisterResultService>(true);
            ClassLocator.Default.Register<IRegisterResultService>(() => ClassLocator.Default.GetInstance<RegisterResultService>());
            ClassLocator.Default.Register<IRegisterResultServer>(() => ClassLocator.Default.GetInstance<RegisterResultService>());

            // Service de récuperation des résultats
            ClassLocator.Default.Register(typeof(IResultService), typeof(ResultService));

            // Service de récuperation des recettes
            ClassLocator.Default.Register(typeof(IDbRecipeService), typeof(RecipeService));

            // Service de gestion du tool
            ClassLocator.Default.Register(typeof(IToolService), typeof(ToolService));

            // serviced de gestionn des utilisateurs
            ClassLocator.Default.Register(typeof(IUserService), typeof(UserService));

            // Service de gestion des logs
            ClassLocator.Default.Register(typeof(ILogService), typeof(LogService));

            // Database maintenance service
            ClassLocator.Default.Register(typeof(IDBMaintenanceService), typeof(DBMaintenanceService));

            // Service used to send the FDCs to the Dataflow and then to UTO
            ClassLocator.Default.Register(typeof(ISendFdcService), typeof(SendFdcService));

            // Database FDC service to provide the FDC information to the UI for the FDCs management
            ClassLocator.Default.Register(typeof(IFDCService), typeof(FDCService));
        }
    }
}
