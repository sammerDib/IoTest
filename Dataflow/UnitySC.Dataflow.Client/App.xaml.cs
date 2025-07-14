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

namespace UnitySC.Dataflow.Client
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
                args = Environment.GetCommandLineArgs();
                Bootstrapper.Register(args);

                var configuration = ClassLocator.Default.GetInstance<IClientConfigurationManager>();
                var clientConfiguration = ClassLocator.Default.GetInstance<ClientConfiguration>();
                var logger = ClassLocator.Default.GetInstance<ILogger>();
                logger.Information($"Configuration manager status: {configuration.GetStatus()}");
                var dataAccessAddress = ClientConfiguration.GetDataAccessAddress();
                var pmAddress = ClientConfiguration.GetServiceAddress(UnitySC.Shared.Data.Enum.ActorType.DataflowManager);
                logger.Information($"DataAccess Address: {dataAccessAddress}");
                logger.Information($"PM Address: {pmAddress}");


                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Initialisation error with args " + string.Join("", args.Skip(1).ToArray()));

            }

        }
    }
}
