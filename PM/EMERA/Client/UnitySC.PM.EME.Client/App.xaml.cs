using System;
using System.Globalization;
using System.Linq;
using System.Windows;

using LightningChartLib.WPF.ChartingMVVM;

using UnitySC.PM.EME.Client.Proxy.Camera;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Client
{
    public partial class App : Application
    {
        private ILogger _logger;
        public App()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            string[] args = Array.Empty<string>();
            try
            {
                args = Environment.GetCommandLineArgs();
                if (args != null && args.Length > 1)
                    args = args.Skip(1).ToArray();

                Bootstrapper.Register(args);
                _logger = ClassLocator.Default.GetInstance<ILogger>();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Initialisation error with args " + string.Join("", args.Skip(1).ToArray()));
            }

            SetupLightningChart();

            base.OnStartup(e);

            //Attach an event handler for unhandled exceptions.
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
        }

        private void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception;
            if (exception != null)
            {
                _logger.Error(exception.ToString());
                StopStreamingIfActive();
            }
        }
        private static void SetupLightningChart()
        {
            const string deploymentKey =
                "lgCAAMDQWCaC7NkBJABVcGRhdGVhYmxlVGlsbD0yMDI1LTEwLTE1I1JldmlzaW9uPTAI/z8AAAAA/AEy9k+WSPYWl8F5j8DR3/xHV8rmwsD6bgDd0+Kv9Aki7UvzB0KJxrUPqzZmcZ1hEhm6bFr+fezEYuukPWEkI7pybF86LTroOuA934Gci/KuDUrhHiaqtxFeaR30Gcgr25NjTyEpauRATjQ4BFk32TnkLwotmJoCv+HYAJkkvd85VCzS0o5fd4w99JHK3/XtyJSYL8/OCCrqumTQZm5A8s7q95M8AfxmeLTEUjPFJp/k+m0oTPHF4er+PTE/m1R/r1+yL6ZeiCzkuFB5m4vLE1vxa7ZEp0aRQ01Xw+0LPPBusgBj4089eXfVWH3DsnFfDmPrFn63MByaFqpzT/hK4J0EiXGqHRaGz8CCiRVxAO3mAT7DirAypxLrrF+142Z3f3iQnd88mRsFiTN2rqfbZDFmPPaK2j4LwDwqKiaVCOz6ISQpG8W7UOMSZjX1KnMiS+FdQRYJJPuuE0WGRMutSyrNHGawAsMY6J4hOh4hDsJsRgN3onrFG+pCHwFG/fUD154=";
            LightningChart.SetDeploymentKey(deploymentKey);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            //Processing when the application is normally closed.
            StopStreamingIfActive();
        }
        private void StopStreamingIfActive()
        {
            try
            {
                var cameraBench = ClassLocator.Default.GetInstance<CameraBench>();
                if (cameraBench.IsStreaming)
                {
                    Current?.Dispatcher.Invoke(async () =>
                    {
                        await cameraBench.StopStreamingAsync();                        
                        cameraBench.Unsubscribe();
                    });                   
                }
            }
            catch (Exception ex)
            {
                _logger?.Error(ex.Message, "Application exit failed");
            }

        }
    }
}
