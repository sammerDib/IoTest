using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

using Agileo.Common.Localization;
using Agileo.Common.Logging;
using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Alarms.Enums;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.Alarms
{
    public class ErrorProvider : ILocalizationProvider, IDisposable
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ErrorProvider" /> class.
        /// </summary>
        /// <param name="device">device provider</param>
        /// <param name="name">The name of the provider.</param>
        /// <param name="errorFilePattern">The error file pattern.</param>
        /// <param name="logger">The logger.</param>
        public ErrorProvider(Device device, string name, string errorFilePattern, ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _localizedErrors = new Dictionary<CultureInfo, Dictionary<string, ErrorModel>>();
            _errorFilePattern = errorFilePattern;
            _device = device;
            Name = name;

            if (string.IsNullOrWhiteSpace(_errorFilePattern))
            {
                Logger.Debug("No error file pattern specified: No errors will be provided.");
            }
        }

        #endregion Constructors

        #region Properties

        protected ILogger Logger { get; }

        #endregion Properties

        #region Fields

        private readonly string _errorFilePattern;
        private readonly Dictionary<CultureInfo, Dictionary<string, ErrorModel>> _localizedErrors;
        private List<CultureInfo> _supportedCultures;

        private readonly Device _device;

        #endregion Fields

        #region Getter Methods

        /// <summary>
        ///     Gets the error description from an error identifier.
        /// </summary>
        /// <param name="errorId">The error identifier.</param>
        /// <returns>The corresponding error description.</returns>
        public virtual string GetDescriptionFromErrorId(int errorId)
            => GetError(errorId)?.Description ?? "No description available";

        /// <summary>
        ///     Gets the error hint from an error identifier.
        /// </summary>
        /// <param name="errorId">The error identifier.</param>
        /// <returns>The corresponding error hint.</returns>
        public virtual string GetHintFromErrorId(int errorId) => GetError(errorId)?.Hint ?? string.Empty;

        public ReadOnlyDictionary<string, ErrorModel> GetErrors()
        {
            var currentCulture = LocalizationManager.Instance.CurrentCulture;
            var defaultCulture = LocalizationManager.DefaultCulture;
            var errors = new Dictionary<string, ErrorModel>();

            if (_localizedErrors.ContainsKey(currentCulture))
            {
                errors = _localizedErrors[currentCulture];
            }
            else if (_localizedErrors.ContainsKey(defaultCulture))
            {
                errors = _localizedErrors[defaultCulture];
            }

            return new ReadOnlyDictionary<string, ErrorModel>(errors);
        }

        public ErrorModel GetError(int errorId)
        {
            var alarmUniqueKey = _device.FormatAlarmUniqueKey(errorId.ToString());

            var errors = GetErrors();
            return errors.ContainsKey(alarmUniqueKey) ? errors[alarmUniqueKey] : default(ErrorModel);
        }

        private string GetErrorFilePath(string errorFilePattern, CultureInfo culture)
        {
            if (errorFilePattern == null)
            {
                return null;
            }

            if (culture == null)
            {
                throw new ArgumentNullException(nameof(culture));
            }

            // Compute the filename depending on culture
            // Pattern is like: .\Configuration_General\CSV\{0}_EC_Errors.csv
            // File name is like: .\Configuration_General\CSV\en_EC_Errors.csv
            var errorFilePath = string.Format(errorFilePattern, culture.TwoLetterISOLanguageName.ToLower());

            // Ensure file exists (there are errors to load)
            if (!File.Exists(errorFilePath))
            {
                Logger.Warning(
                    "No file found for culture {CultureName} matching pattern '{ErrorFilePattern}'.",
                    culture.Name,
                    errorFilePattern);
                return null;
            }

            return errorFilePath;
        }

        private List<CultureInfo> GetSupportedCultures(string errorFilePattern)
        {
            var supportedCultures = new List<CultureInfo>();

            if (string.IsNullOrWhiteSpace(errorFilePattern))
            {
                return supportedCultures;
            }

            var folderPath = Path.GetDirectoryName(errorFilePattern);
            if (folderPath == null)
            {
                return supportedCultures;
            }

            // Replace placeholder with regex
            var regex = new Regex(string.Format(Path.GetFileName(errorFilePattern), "([\\D]{2})"));

            // Parse all files in folder
            foreach (var fileName in Directory.GetFiles(folderPath))
            {
                // Check if filename matches the error file pattern with a two-letter culture name
                var match = regex.Match(fileName);
                if (match.Success)
                {
                    supportedCultures.Add(new CultureInfo(match.Groups[1].Value));
                }
            }

            return supportedCultures;
        }

        #endregion Getter Methods

        #region ILocalizationProvider

        public string Name { get; }

        public IEnumerable<CultureInfo> SupportedCultures => _supportedCultures;

        public string GetString(string key, CultureInfo culture)
        {
            if (!_localizedErrors.ContainsKey(culture))
            {
                return null;
            }

            // Ensure the error is known; otherwise return null (error might be in another provider)
            if (!_localizedErrors[culture].ContainsKey(key))
            {
                return null;
            }

            // Return the value
            var error = _localizedErrors[culture][key];
            return $"{error.Description}{(string.IsNullOrWhiteSpace(error.Hint) ? string.Empty : $" - {error.Hint}")}";
        }

        /// <summary>
        ///     Initializes the errors from file based on the error file pattern given in constructor.
        /// </summary>
        public void Initialize()
        {
            _supportedCultures = GetSupportedCultures(_errorFilePattern);
            foreach (var culture in _supportedCultures)
            {
                InitializeInternal(culture);
            }
        }

        public void LoadResources(CultureInfo culture)
        {
            // Nothing to do here, all resources loaded with Initialize method
        }

        #endregion ILocalizationProvider

        #region Other Methods

        /// <summary>
        ///     Cleanups all errors from memory.
        /// </summary>
        public virtual void Cleanup() => _localizedErrors.Clear();

        private static IEnumerable<ErrorModel> ParseCsvErrorFile(string csvErrorFilePath)
        {
            using var csvStream = new StreamReader(csvErrorFilePath);
            while (!csvStream.EndOfStream)
            {
                var line = csvStream.ReadLine();
                if (line == null)
                {
                    continue;
                }

                var values = line.Split(';');
                if (values.Length < 3)
                {
                    continue;
                }

                var id = int.Parse(values[0]);

                var criticity = AlarmCriticity.Undefined;
                if (values.Length >= 4 && Enum.TryParse(values[3], out AlarmCriticity _criticity))
                {
                    criticity = _criticity;
                }

                yield return new ErrorModel(id, values[1], values[2], criticity);
            }
        }

        protected virtual void InitializeInternal(CultureInfo culture)
        {
            if (culture == null)
            {
                throw new ArgumentNullException(nameof(culture));
            }

            var csvErrorFilePath = GetErrorFilePath(_errorFilePattern, culture);
            if (string.IsNullOrWhiteSpace(csvErrorFilePath))
            {
                return;
            }

            // Load the errors from file
            foreach (var error in ParseCsvErrorFile(csvErrorFilePath))
            {
                AddError(culture, error);
            }

            // Check we got some errors
            if (!_localizedErrors.ContainsKey(culture) || _localizedErrors[culture].Values.Count == 0)
            {
                Logger.Warning("No errors loaded for culture {CultureName}", culture.Name);
            }
        }

        private void AddError(CultureInfo culture, ErrorModel error)
        {
            if (!_localizedErrors.ContainsKey(culture))
            {
                _localizedErrors.Add(culture, new Dictionary<string, ErrorModel>());
            }

            var uniqueKey = _device.FormatAlarmUniqueKey(error.Id.ToString());
            if (!_localizedErrors[culture].ContainsKey(uniqueKey))
            {
                _localizedErrors[culture].Add(uniqueKey, error);
            }
        }

        public void AddError(ErrorModel error) => AddError(LocalizationManager.Instance.CurrentCulture, error);

        #endregion Other Methods

        #region IDisposable

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Cleanup();
            }
        }

        #endregion IDisposable
    }
}
