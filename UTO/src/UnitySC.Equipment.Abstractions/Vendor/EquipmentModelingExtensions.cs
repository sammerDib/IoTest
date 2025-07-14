using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.AlarmModeling;
using Agileo.Common.Localization;
using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.ModelingFramework;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Alarms;
using UnitySC.Equipment.Abstractions.Vendor.Material;

namespace UnitySC.Equipment.Abstractions.Vendor
{
    /// <summary>
    /// Provides services extending Equipment Modeling framework from Agileo.
    /// </summary>
    public static class EquipmentModelingExtensions
    {
        /// <summary>
        /// Used to format AlarmId to an unique AlarmKey necessary for handling
        /// Alarms from several devices
        /// </summary>
        /// <param name="device"></param>
        /// <param name="alarmId"></param>
        /// <returns></returns>
        public static string FormatAlarmUniqueKey(this Device device, string alarmId)
            => string.Concat(device.Name, "_", device.InstanceId, alarmId);

        public static AlarmOccurrence SetAlarmByKey(this IAlarmServices alarmServices, Device device, string alarmKey)
        {
            var alarmUniqueKey = FormatAlarmUniqueKey(device, alarmKey);
            return device.Alarms.Any(a => a.Name == alarmUniqueKey && a.State != AlarmState.Set)
                ? alarmServices.SetAlarm(device, alarmUniqueKey)
                : null;
        }

        public static AlarmOccurrence ClearAlarmByKey(this IAlarmServices alarmServices, Device device, string alarmKey)
        {
            var alarmUniqueKey = FormatAlarmUniqueKey(device, alarmKey);
            return device.Alarms.Find(alarmUniqueKey).State == AlarmState.Set
                ? alarmServices.ClearAlarm(device, alarmUniqueKey)
                : null;
        }

        /// <summary>
        /// Used to UnFormat AlarmKey from AlarmCenter to AlarmId
        /// </summary>
        /// <param name="device"></param>
        /// <param name="alarmKey"></param>
        /// <returns></returns>
        private static int UnFormatAlarmUniqueKey(this Device device, string alarmKey)
        {
            var replace = string.Concat(device.Name, "_", device.InstanceId);
            var value = alarmKey.Replace(replace, string.Empty);
            _ = int.TryParse(value, out var res);
            return res;
        }

        /// <summary>
        /// Loads the device's alarms from a CSV file.
        /// </summary>
        /// <param name="device">Device instance to work with.</param>
        /// <param name="alarmCenter">The alarm center instance.</param>
        /// <param name="csvFilePath">Path to the CSV file containing alarm definitions.</param>
        /// <param name="providerName">Name of the created <see cref="ErrorProvider"/>.</param>
        /// <param name="logger">The logger to use.</param>
        /// <exception cref="ArgumentNullException">
        /// When <paramref name="csvFilePath"/> or <paramref name="logger"/> is null.
        /// </exception>
        public static ErrorProvider LoadAlarms(
            this Device device,
            IAlarmCenter alarmCenter,
            string csvFilePath,
            string providerName,
            ILogger logger)
        {
            // Check arguments
            if (String.IsNullOrEmpty(csvFilePath))
            {
                throw new ArgumentNullException(nameof(csvFilePath));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            // Create the alarm provider, used to read CSV files and allow localization of alarms
            var provider = new ErrorProvider(device, providerName, csvFilePath, logger);
            LocalizationManager.AddLocalizationProvider(provider);

            // Get the object used to register alarms in the system, allowing to benefit from A²ECF features
            var alarmBuilder = alarmCenter.ModelBuilder;

            // Create the alarm instances for each definitions found in CSV file
            var alarmResources = provider.GetErrors();
            foreach (var alarmKey in alarmResources.Keys)
            {
                if (device.Alarms.Find(alarmKey) != null)
                {
                    continue;
                }

                // Create the alarm and add it to the device
                var alarm = alarmBuilder.CreateAlarm(
                    alarmKey,
                    provider.GetString(alarmKey, LocalizationManager.DefaultCulture),
                    device.UnFormatAlarmUniqueKey(alarmKey));
                device.Alarms.SafeAdd(alarm);
            }

            // Register all alarms created for the device
            alarmBuilder.AddAlarms(device);

            return provider;
        }

        public static void SetupAlarms(this Device device)
        {
            if (!TryGetAlarmCenter(device, out var alarmCenter))
            {
                return;
            }

            foreach (var alarmDefinition in device.DeviceType.AlarmsDefinition)
            {
                var alarm = alarmCenter.ModelBuilder.CreateAlarm(
                    device.FormatAlarmUniqueKey(alarmDefinition.Name),
                    alarmDefinition.Description,
                    alarmDefinition.Id);
                device.Alarms.SafeAdd(alarm);
            }

            alarmCenter.ModelBuilder.AddAlarms(device);
        }

        /// <summary>
        /// Control if alarm exists in Catalog and add to it doesn't
        /// </summary>
        /// <param name="device"></param>
        /// <param name="alarmId"></param>
        /// <param name="alarmText"></param>
        /// <param name="logger"></param>
        public static void VerifyAlarmInCatalog(this Device device, int alarmId, int alarmText, ILogger logger)
        {
            if (!TryGetAlarmCenter(device, out var alarmCenter))
            {
                return;
            }

            var alarmUniqueKey = device.FormatAlarmUniqueKey(alarmId.ToString());
            if (device.Alarms.Any(a => a.Name == alarmUniqueKey))
            {
                return;
            }

            var alarm = alarmCenter.ModelBuilder.CreateAlarm(
                alarmUniqueKey,
                $"{alarmText} ({alarmId}).",
                alarmId);

            device.Alarms.SafeAdd(alarm);

            alarmCenter.ModelBuilder.AddAlarms(device);

            logger.Warning("Unknown Error (id: {AlarmId} - Text: {AlarmText})", alarmId, alarmText);
        }

        public static bool TryGetAlarmCenter(this Device device, out IAlarmCenter alarmCenter)
        {
            alarmCenter = device.GetEquipment()?.AlarmCenter;
            return alarmCenter is not null;
        }

        public static IEnumerable<SubstrateLocation> GetSubstrateLocations(this Device device)
        {
            return device.AllOfType<IMaterialLocationContainer>()
                .SelectMany(
                    materialLocationContainer => materialLocationContainer.MaterialLocations
                        .Where(loc => loc is SubstrateLocation)
                        .Cast<SubstrateLocation>());
        }

        public static IEnumerable<Substrate> GetSubstrates(this Device device)
        {
            return device.AllOfType<IMaterialLocationContainer>()
                .SelectMany(mlc => mlc.MaterialLocations.OfType<SubstrateLocation>())
                .Select(sl => sl.Substrate)
                .Where(s => s != null);
        }

        public static IEnumerable<Substrate> GetSubstrates(this Agileo.EquipmentModeling.Equipment equipment)
        {
            return equipment.AllOfType<IMaterialLocationContainer>()
                .SelectMany(mlc => mlc.MaterialLocations.OfType<SubstrateLocation>())
                .Select(sl => sl.Substrate)
                .Where(s => s != null);
        }
    }
}
