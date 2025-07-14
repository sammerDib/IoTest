using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;

using Agileo.Common.Localization;

namespace UnitySC.GUI.Common.Vendor.UIComponents.MarkupExtensions
{
    public class LocalizableEnumBinding : MultiBinding, IMultiValueConverter, IWeakEventListener, INotifyPropertyChanged
    {
        private bool _cultureChangedFlag;

        public bool CultureChangedFlag
        {
            get { return _cultureChangedFlag; }
            set
            {
                if (_cultureChangedFlag == value) return;
                _cultureChangedFlag = value;
                OnPropertyChanged();
            }
        }

        #region Implementation of IMultiValueConverter

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length > 0 && values[0] is Enum enumValue)
            {
                var enumType = enumValue.GetType();
                string value = enumType + "." + enumValue;
                return LocalizationManager.GetString(value);
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        public LocalizableEnumBinding(string path)
        {
            Converter = this;
            Bindings.Add(new Binding(path));
            // Just to be notified of localization changed
            Bindings.Add(new Binding(nameof(CultureChangedFlag))
            {
                Source = this
            });
            CultureChangedEventManager.AddListener(LocalizationManager.Instance, this);
        }

        ~LocalizableEnumBinding()
        {
            CultureChangedEventManager.RemoveListener(LocalizationManager.Instance, this);
        }

        #region Implementation of IWeakEventListener

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType != typeof(CultureChangedEventManager))
            {
                return false;
            }

            CultureChangedFlag = !CultureChangedFlag;
            return true;
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
