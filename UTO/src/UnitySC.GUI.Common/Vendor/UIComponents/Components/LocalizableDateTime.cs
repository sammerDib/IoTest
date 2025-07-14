using System;

using Agileo.Common.Localization;
using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components
{
    /// <summary>
    /// Useful to create a localized <see cref="DateTime"/> that will be automatically updated when <see cref="E:Agileo.Common.Localization.LocalizationManager.CultureChanged" /> event will sent
    /// </summary>
    public class LocalizableDateTime : Notifier, IDisposable
    {
        private readonly DateTime _dateTime;
        private readonly string _format;
        private readonly bool _useLocalizationKey;

        private LocalizableDateTime(DateTime dateTime, string format, bool useLocalizationKey)
        {
            _dateTime = dateTime;
            _format = format;
            _useLocalizationKey = useLocalizationKey;
            LocalizationManager.Instance.CultureChanged += InstanceOnCultureChanged;
        }

        /// <summary>
        /// Creates an instance of LocalizableDateTime that automatically throws a propertyChanged on LocalizationManager culture changed.
        /// Use a standard formatting string. (Default "G")
        /// </summary>
        /// <param name="dateTime">Instant time to format</param>
        /// <param name="format">Standard formatting string</param>
        /// <returns>LocalizableDateTime</returns>
        public static LocalizableDateTime WithStandardFormat(DateTime dateTime, string format = "G")
        {
            return new LocalizableDateTime(dateTime, format, false);
        }

        /// <summary>
        /// Creates an instance of LocalizableDateTime that automatically throws a propertyChanged on LocalizationManager culture changed.
        /// Use a custom localization key as formatting string.
        /// </summary>
        /// <param name="dateTime">Instant time to format</param>
        /// <param name="key">Resource key</param>
        /// <returns>LocalizableDateTime</returns>
        public static LocalizableDateTime WithCustomFormat(DateTime dateTime, string key)
        {
            return new LocalizableDateTime(dateTime, key, true);
        }

        private void InstanceOnCultureChanged(object sender, EventArgs e) => OnPropertyChanged(nameof(Value));

        /// <summary>
        /// Get formatted time.
        /// </summary>
        public string Value => ToString();

        /// <inheritdoc />
        public override string ToString()
        {
            var formatter = _format;
            if (_useLocalizationKey)
            {
                formatter = LocalizationManager.GetString(_format);
            }
            return _dateTime.ToString(formatter, LocalizationManager.Instance.CurrentCulture);
        }

        #region IDisposable
        
        private bool _disposedValue;
        /// <inheritdoc cref="IDisposable.Dispose"/>>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    LocalizationManager.Instance.CultureChanged -= InstanceOnCultureChanged;
                }
                
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
