using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Configuration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.HLS.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            string[] args = null;
            try
            {
                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

                args = Environment.GetCommandLineArgs();
                Bootstrapper.Register(args);

                var configuration = ClassLocator.Default.GetInstance<IClientConfigurationManager>();
                var clientConfiguration = ClassLocator.Default.GetInstance<ClientConfiguration>();
                var logger = ClassLocator.Default.GetInstance<ILogger>();
                logger.Information($"Configuration manager status: {configuration.GetStatus()}");
                var dataAccessAddress = ClientConfiguration.GetDataAccessAddress();
                var pmAddress = ClientConfiguration.GetServiceAddress(UnitySC.Shared.Data.Enum.ActorType.HeLioS);
                logger.Information($"DataAccess Address: {dataAccessAddress}");
                logger.Information($"PM Address: {pmAddress}");

                //Set Deployment Key for Arction components -- à offusquer

                string deploymentKey = "lgCAABUmf/b6wdcBJABVcGRhdGVhYmxlVGlsbD0yMDIzLTEwLTE1I1JldmlzaW9uPTAI/z8AAAAA/AFFX6y/MzYmOVJwTEraPWtRsi1IHBKr09iBNzQ6Aeuw4H9M3Rj1Z8kCqF4v6sErxZhjRIAbEaM0GTgbU4taoTDKSyRkST664d7e/eFbww+xjwZVB4LQInVkHpQ2IFGsRNkcKdqDGqJ4eNgusc+ygVYpSL6IbA5/S4D1Yo7J82Lr7jTm6C3ujsTN4BsNcOSJSNpmKeGi0TYq2lHgFoZiSk4405LxLCN8iVOWSEfOE1q/qBwDzIz4yXUSnP9X346rhV8VFvsbYxjZMUjGYpCOlZH8HPuZ+sEIBg7ZBSWTyDHicLMSutP5jP9WYPzAo8KDdHXLTQOvvA1kjdQ+xCIkTzKSeebYHmxXsms2Wm5qCC8jLpJzgYdBaeDEEmsTMZtkmxtZfYjl53/CbuDZb73K/toZfbJX55lDZ16qv4kdDeo2/FCFXuB4RbBTFr8h7mj0bgAsC0/TEPGonRUorZHbH0RzRFQySYJJQDdz/MyoE29bsTPaQYvcHoli6/E43xQUOXE=";

                // Setting Deployment Key for fully bindable chart

                Arction.Wpf.ChartingMVVM.LightningChart.SetDeploymentKey(deploymentKey);
                Arction.Wpf.Charting.LightningChart.SetDeploymentKey(deploymentKey);
                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(),"Initialisation error with args "+string.Join("", args.Skip(1).ToArray()));
            }
        }
    }
}
