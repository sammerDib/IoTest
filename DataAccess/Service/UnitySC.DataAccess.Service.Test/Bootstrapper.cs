using UnitySC.DataAccess.ResultScanner.Interface;
using UnitySC.DataAccess.Service.Implementation;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.DataAccess.Service.Test
{
    public static class Bootstrapper
    {
        public static object BootstrapperLock = new object();
        public static bool IsRegister { get; private set; }

        public static void Register()
        {
            lock (BootstrapperLock)
            {
                if (!IsRegister)
                {
                    DataAccessConfiguration.SettingsFilePath = @".\DataAccessConfigurationTest.xml";

                    SerilogInit.InitWithCurrentAppConfig();

                    // Logger with caller name
                    ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));
                    // Logger without caller name
                    ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));
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

                    // Result service
                    ClassLocator.Default.Register(typeof(IResultService), typeof(ResultService));

                    // Recipe service
                    ClassLocator.Default.Register(typeof(IDbRecipeService), typeof(RecipeService));

                    // Register tool service
                    ClassLocator.Default.Register(typeof(IToolService), typeof(ToolService));

                    // Register user service
                    ClassLocator.Default.Register(typeof(IUserService), typeof(UserService));

                    IsRegister = true;
                }
            }
        }
    }
}
