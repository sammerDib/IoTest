using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace UnitySC.Shared.ResultUI.Common.Converters
{
    /// <summary>
    /// Dynamically invokes the function bound at index 0, passing the other bindings as parameters.
    /// </summary>
    public class InvokeFuncMultiConverter : IMultiValueConverter
    {
        #region Implementation of IMultiValueConverter

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 1)
                return DependencyProperty.UnsetValue;

            if (values.Any(x => x == DependencyProperty.UnsetValue || x == BindingOperations.DisconnectedSource))
            {
                //One of the parameters comes from an element container that has been deleted from the visual element tree or has not yet been initialized.
                return DependencyProperty.UnsetValue;
            }

            if (!(values[0] is Delegate func))
                return DependencyProperty.UnsetValue;
            
            object[] parameters = values.Skip(1).ToArray();
            return func.DynamicInvoke(parameters);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
