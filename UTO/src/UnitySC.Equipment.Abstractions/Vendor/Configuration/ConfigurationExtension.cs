using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;

using Agileo.Common.Configuration;
using Agileo.Common.Logging;
using Agileo.Common.Tracing;

using UnitySC.Equipment.Abstractions.Vendor.Devices;

namespace UnitySC.Equipment.Abstractions.Vendor.Configuration
{
    /// <summary>
    /// Provides services extending configuration features of Agileo.
    /// </summary>
    public static class ConfigurationExtension
    {
        private static readonly Dictionary<string, IConfigManager> _configurationManageCache = new();

        #region ConfigManager

        public static ConfigManager<TConfig> LoadDeviceConfiguration<TConfig, TDevice>(
            string deviceConfigRootPath,
            ILogger logger,
            int instanceId = 0,
            Func<TConfig> createDefaultConfiguration = null)
            where TConfig : IConfiguration
            where TDevice : IConfigurableDevice<TConfig>
        {
            var deviceType = typeof(TDevice);
            return LoadDeviceConfiguration(
                deviceConfigRootPath,
                $"./Devices/{deviceType.Name}/Resources",
                logger,
                instanceId,
                createDefaultConfiguration);
        }

        public static ConfigManager<T> LoadDeviceConfiguration<T>(
            this IConfigurableDevice<T> configurableDevice,
            string deviceConfigRootPath,
            ILogger logger,
            int instanceId = 0)
            where T : IConfiguration
            => LoadDeviceConfiguration(
                deviceConfigRootPath,
                configurableDevice.RelativeConfigurationDir,
                logger,
                instanceId,
                configurableDevice.CreateDefaultConfiguration);

        public static ConfigManager<T> LoadDeviceConfiguration<T>(
            this IConfigurableDevice<T> configurableDevice,
            string deviceConfigRootPath,
            string configurationFileName,
            ILogger logger)
            where T : IConfiguration
            => LoadDeviceConfiguration(
                deviceConfigRootPath,
                configurableDevice.RelativeConfigurationDir,
                configurationFileName,
                logger,
                configurableDevice.CreateDefaultConfiguration);

        private static ConfigManager<T> LoadDeviceConfiguration<T>(
            string deviceConfigRootPath,
            string deviceConfigDir,
            ILogger logger,
            int instanceId,
            Func<T> createDefaultConfiguration)
            where T : IConfiguration
        {
            var configurationFileName = instanceId > 0 ? $"Configuration{instanceId}.xml" : "Configuration.xml";
            return LoadDeviceConfiguration<T>(
                deviceConfigRootPath,
                deviceConfigDir,
                configurationFileName,
                logger,
                createDefaultConfiguration);
        }

        private static ConfigManager<T> LoadDeviceConfiguration<T>(
            string deviceConfigRootPath,
            string deviceConfigDir,
            string configurationFileName,
            ILogger logger,
            Func<T> createDefaultConfiguration)
            where T : IConfiguration
        {
            if (string.IsNullOrEmpty(deviceConfigRootPath))
            {
                deviceConfigRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly()?.Location) ?? string.Empty;
            }

            var deviceResourcesDir = Path.GetFullPath(Path.Combine(deviceConfigRootPath, deviceConfigDir));
            var configurationPath = Path.Combine(deviceResourcesDir, configurationFileName);
            return BuildConfigManager<T>(configurationPath)
                .LoadOrDefault(configurationPath, createDefaultConfiguration, logger);
        }

        /// <summary>
        /// Builds an instance of <see cref="ConfigManager{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of configuration.</typeparam>
        /// <param name="configurationFilePath">The configuration file path.</param>
        /// <param name="configurationXmlSchemaPath">The configuration XML schema path (optional).</param>
        /// <returns>An instance of <see cref="ConfigManager{T}"/>.</returns>
        private static ConfigManager<T> BuildConfigManager<T>(
            string configurationFilePath,
            string configurationXmlSchemaPath = null)
            where T : IConfiguration
        {
            var absoluteFilePath = ToAbsolutePath(configurationFilePath);

            if (_configurationManageCache.TryGetValue(absoluteFilePath, out var cachedConfigManager))
            {
                return cachedConfigManager as ConfigManager<T>;
            }

            var configManager = new ConfigManager<T>(
                new XmlSerializerStoreStrategy(
                    absoluteFilePath,
                    ToAbsolutePath(configurationXmlSchemaPath),
                    false),
                new XmlSerializerCloneStrategy(),
                new XmlSerializerCompareStrategy<T>());

            _configurationManageCache.Add(absoluteFilePath, configManager);

            return configManager;
        }

        /// <summary>
        /// Attempts to load the configuration from specified file path.
        /// In case of load failure, ask user if the default configuration should be use.
        /// </summary>
        /// <param name="configManager">The instance this extension method applies on.</param>
        /// <param name="configurationFilePath">Path to the configuration file to load.</param>
        /// <param name="createDefaultConfiguration">Function that is used to create default instance of Configuration in case the file can't be loaded.</param>
        /// <param name="logger">Logger instance for diagnostics.</param>
        /// <returns><paramref name="configManager"/></returns>
        /// <exception cref="InvalidOperationException">
        /// When user cancels the use of default configuration instance
        /// -or- when an error occurs while loading the default configuration.
        /// </exception>
        private static ConfigManager<T> LoadOrDefault<T>(
            this ConfigManager<T> configManager,
            string configurationFilePath,
            Func<T> createDefaultConfiguration,
            ILogger logger)
            where T : IConfiguration
        {
            // Try to load the configuration from disk
            if (configManager.Load())
            {
                logger.Debug(
                    configManager.Loaded.AsAttachment(),
                    "Configuration successfully loaded from '{ConfigurationFilePath}'",
                    configurationFilePath);
                return configManager;
            }

            // Loading configuration failed, log and ask user confirmation to use default configuration
            logger.Error(
                "Errors while loading '{DeviceName}' configuration from '{ConfigurationFilePath}': {Errors}",
                logger.Name,
                configurationFilePath,
                configManager.Errors);

            var message =
                $"Error while loading '{logger.Name}' configuration from '{configurationFilePath}'.{Environment.NewLine}{configManager.Errors}";

            if (createDefaultConfiguration == null)
            {
                throw new InvalidOperationException(message);
            }

            var result = MessageBox.Show(
                $"{message}{Environment.NewLine}Do you want to use the default configuration ?",
                "Configuration error",
                MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
            {
                throw new InvalidOperationException(message);
            }

            // Try to load default configuration
            if (!configManager.Load(createDefaultConfiguration()))
            {
                logger.Error(
                    "Failed to load default configuration for {DeviceName}. Errors: {Errors}",
                    logger.Name,
                    configManager.Errors);
                message = $"Failed to load default configuration for '{logger.Name}'.";
                throw new InvalidOperationException(message);
            }

            logger.Debug(configManager.Loaded.AsAttachment(), "Default configuration loaded.");

            // Save default configuration when successfully loaded.
            configManager.Save();

            return configManager;
        }

        #endregion ConfigManager

        #region IConfiguration

        /// <summary>
        /// Casts the <see cref="IConfiguration"/> instance into the specified type.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration"/> instance to cast.</param>
        /// <typeparam name="TConfig">Type into which <paramref name="configuration"/> should be cast.</typeparam>
        /// <returns><paramref name="configuration"/> as <typeparamref name="TConfig"/>.</returns>
        /// <exception cref="InvalidCastException">When <paramref name="configuration"/> cannot be cast into <typeparamref name="TConfig"/>.</exception>
        public static TConfig Cast<TConfig>(this IConfiguration configuration)
            where TConfig : class, IConfiguration
            => (TConfig)configuration;

        /// <summary>
        /// Tries to cast the <see cref="IConfiguration"/> instance into the specified type.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration"/> instance to cast.</param>
        /// <typeparam name="TConfig">Type into which <paramref name="configuration"/> should be cast.</typeparam>
        /// <returns><paramref name="configuration"/> as <typeparamref name="TConfig"/> when cast is possible; otherwise <c>null</c>.</returns>
        public static TConfig TryCast<TConfig>(this IConfiguration configuration)
            where TConfig : class, IConfiguration
            => configuration as TConfig;

        #endregion IConfiguration

        #region Private Methods

        /// <summary>
        /// Converts a path to an absolute path if it's relative.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private static string ToAbsolutePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }

            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

            var currentAssemblyFolder = Path.GetDirectoryName(assembly.Location);
            if (!Path.IsPathRooted(path) && !string.IsNullOrEmpty(currentAssemblyFolder))
            {
                return Path.GetFullPath(Path.Combine(currentAssemblyFolder, path));
            }

            return path;
        }

        #endregion Private Methods
    }
}
