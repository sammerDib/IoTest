using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.UIComponents.MarkupExtensions
{
    /// <summary>
    /// Allows to dynamically invoke a <see cref="Delegate"/> from a <see cref="Binding"/> while using Bindings as parameters.
    /// </summary>
    public class FuncBinding : MultiBinding, IMultiValueConverter
    {
        private Binding _bindingToFunc;

        #region Properties

        [DefaultValue(null)]
        public new IValueConverter Converter { get; set; }

        [DefaultValue(null)]
        public new object ConverterParameter { get; set; }

        public Binding BindingToFunc
        {
            get => _bindingToFunc;
            set
            {
                if (_bindingToFunc != null) Bindings.Remove(_bindingToFunc);
                _bindingToFunc = value;
                if (_bindingToFunc != null) Bindings.Insert(0, _bindingToFunc);
            }
        }

        #endregion

        #region Implementation of IMultiValueConverter

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (Notifier.IsInDesignModeStatic) return DependencyProperty.UnsetValue;

            if (values.Length < 1) return DependencyProperty.UnsetValue;

            if (!(values[0] is Delegate func)) return DependencyProperty.UnsetValue;

            var parameters = values.Skip(1).ToArray();

            if (parameters.Any(x => x == DependencyProperty.UnsetValue || x == BindingOperations.DisconnectedSource))
            {
                return DependencyProperty.UnsetValue;
            }

            var invoke = func.DynamicInvoke(parameters);

            if (Converter != null)
            {
                return Converter.Convert(invoke, targetType, ConverterParameter, culture);
            }

            return invoke;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        public FuncBinding()
        {
            base.Converter = this;
        }

        // ReSharper disable once UnusedMember.Global
        public FuncBinding(string path)
        {
            base.Converter = this;
            BindingToFunc = new Binding(path);
        }
    }
}
