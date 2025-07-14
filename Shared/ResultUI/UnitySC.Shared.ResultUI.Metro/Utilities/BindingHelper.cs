using System.Windows;
using System.Windows.Data;

namespace UnitySC.Shared.ResultUI.Metro.Utilities
{
    public static class BindingHelper
    {
        private class DummyFrameworkElement : FrameworkElement
        {
            public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(object), typeof(DummyFrameworkElement), new UIPropertyMetadata(null));

            public object Value
            {
                get { return GetValue(ValueProperty); }
                set => SetValue(ValueProperty, value);
            }
        }

        /// <summary>
        /// Retrieves the value of a Binding from a given context.
        /// </summary>
        /// <param name="dataContext">Data source object</param>
        /// <param name="binding">Binding expression</param>
        /// <returns>Binding return value</returns>
        public static object GetValue(object dataContext, BindingBase binding)
        {
            var dummyFrameworkElement = new DummyFrameworkElement
            {
                DataContext = dataContext
            };
            BindingOperations.SetBinding(dummyFrameworkElement, DummyFrameworkElement.ValueProperty, binding);

            if (dummyFrameworkElement.Value is string stringValue)
            {
                if (stringValue.Equals("-")) return string.Empty;
            }

            return dummyFrameworkElement.Value;
        }
    }
}
