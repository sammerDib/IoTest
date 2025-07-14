using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

using UnitySC.PM.ANA.Client.Proxy.Configuration;
using UnitySC.PM.ANA.Client.Proxy.Helpers;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Configuration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    ///
    /// </summary>
    public partial class App : Application
    {
        public string AppVersion;
        public App()
        {

            string[] args = null;
            try
            {

                args = Environment.GetCommandLineArgs();
                if(args != null && args.Length > 1)
                    args = args.Skip(1).ToArray();

                Bootstrapper.Register(args);

                var configuration = ClassLocator.Default.GetInstance<IClientConfigurationManager>();
                var clientConfiguration = ClassLocator.Default.GetInstance<ClientConfiguration>();


                

                var logger = ClassLocator.Default.GetInstance<ILogger>();
                logger.Information("******************************************************************************************");
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                AppVersion = fileVersionInfo.ProductVersion;
                logger.Information("ANALYSE CLIENT Version: " + AppVersion);
                logger.Information("******************************************************************************************");

                // Set the waferLess addresses if needed
                var isWaferLessMode = ClassLocator.Default.GetInstance<IClientConfigurationManager>().IsWaferLessMode;

                if (isWaferLessMode)
                {
                    logger.Information("******************************************************************************************");
                    logger.Information(" WAFER LESS MODE ");
                    logger.Information("******************************************************************************************");


                    var analyseServiceAddress =ClientConfigurationWaferLess.GetAnalyseServiceAddress();
                    foreach (var actorAddress in clientConfiguration.ActorAddresses)
                    {
                        actorAddress.Address= ClientConfigurationWaferLess.GetAnalyseServiceAddress();
                    }
                }
                
                logger.Information($"Configuration manager status: {configuration.GetStatus()}");
                var dataAccessAddress = ClientConfiguration.GetDataAccessAddress();
                var pmAddress = ClientConfiguration.GetServiceAddress(UnitySC.Shared.Data.Enum.ActorType.ANALYSE);
                
                logger.Information($"DataAccess Address: {dataAccessAddress}");
                logger.Information($"PM Address: {pmAddress}");

                //Set Deployment Key for Arction components -- à offusquer

                string deploymentKey = "lgCAAMDQWCaC7NkBJABVcGRhdGVhYmxlVGlsbD0yMDI1LTEwLTE1I1JldmlzaW9uPTAI/z8AAAAA/AEy9k+WSPYWl8F5j8DR3/xHV8rmwsD6bgDd0+Kv9Aki7UvzB0KJxrUPqzZmcZ1hEhm6bFr+fezEYuukPWEkI7pybF86LTroOuA934Gci/KuDUrhHiaqtxFeaR30Gcgr25NjTyEpauRATjQ4BFk32TnkLwotmJoCv+HYAJkkvd85VCzS0o5fd4w99JHK3/XtyJSYL8/OCCrqumTQZm5A8s7q95M8AfxmeLTEUjPFJp/k+m0oTPHF4er+PTE/m1R/r1+yL6ZeiCzkuFB5m4vLE1vxa7ZEp0aRQ01Xw+0LPPBusgBj4089eXfVWH3DsnFfDmPrFn63MByaFqpzT/hK4J0EiXGqHRaGz8CCiRVxAO3mAT7DirAypxLrrF+142Z3f3iQnd88mRsFiTN2rqfbZDFmPPaK2j4LwDwqKiaVCOz6ISQpG8W7UOMSZjX1KnMiS+FdQRYJJPuuE0WGRMutSyrNHGawAsMY6J4hOh4hDsJsRgN3onrFG+pCHwFG/fUD154=";
                // Setting Deployment Key for fully bindable chart

                LightningChartLib.WPF.ChartingMVVM.LightningChart.SetDeploymentKey(deploymentKey);
                LightningChartLib.WPF.Charting.LightningChart.SetDeploymentKey(deploymentKey);
                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(),"Initialisation error with args "+string.Join("", args.Skip(1).ToArray()));
            }
        }
    }
}
