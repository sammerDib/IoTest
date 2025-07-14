using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Configuration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.DMT.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        enum ErrorCode : Int32
        {
            Success = 0,
            Error = 1,
            ArgumentError = 2,
            FileNotFoundError = 3,
        }

        public App()
        {

            var args = Environment.GetCommandLineArgs();
            if(args != null && args.Length > 1)
                args = args.Skip(1).ToArray();

            try
            {
                Bootstrapper.Register(args);
            }
            catch (ArgumentException)
            {
                MessageBox.Show(
                    "Unable to start application. Please provide required arguments.\n" +
                    "Common Usage : -c <ClientConfigurationPathOrName>\n" +
                    "Notice : if configuration is not defined Computer Name is used instead.",
                    "Application Start failure",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                Environment.Exit((int)ErrorCode.ArgumentError);
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(
                    $"Unable to start application. File not found.\n{ex.Message}",
                    "Application Start failure",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                Environment.Exit((int)ErrorCode.FileNotFoundError);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Unable to start application.\n{ex.Message}",
                    "Application Start failure",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                Environment.Exit((int)ErrorCode.Error);
            };

            var configuration = ClassLocator.Default.GetInstance<IClientConfigurationManager>();

            ILogger _logger = ClassLocator.Default.GetInstance<ILogger<object>>();
            _logger.Information("******************************************************************************************");
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;
            _logger.Information("DEMETER CLIENT Version: " + version);
            _logger.Information("******************************************************************************************");

            _logger.Information($"Configuration manager status: {configuration.GetStatus()}");
            var dataAccessAddress = ClientConfiguration.GetDataAccessAddress();
            var pmAddress = ClientConfiguration.GetServiceAddress(UnitySC.Shared.Data.Enum.ActorType.DEMETER);
            _logger.Information($"DataAccess Address: {dataAccessAddress}");
            _logger.Information($"PM Address: {pmAddress}");


            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        }
    }
}
