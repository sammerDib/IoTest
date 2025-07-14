using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace UnitySC.Shared.UI.Controls.WizardNavigationControl
{
    public class IsFirstItemConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[1] is ObservableCollection<INavigable>))
                return false;


            ObservableCollection<INavigable> col = (ObservableCollection<INavigable>)values[1];
            if (col[0].Equals(values[0]))
                return true;
            else
                return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }


    public class IsLastItemConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[1] is ObservableCollection<INavigable>))
                return false;


            ObservableCollection<INavigable> col = (ObservableCollection<INavigable>)values[1];
            if (col[col.Count - 1].Equals(values[0]))
                return true;
            else
                return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }


    public class WizardNavigationControl : ListView
    {
    }
}
