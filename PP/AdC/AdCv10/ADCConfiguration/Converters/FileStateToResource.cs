using System;
using System.Windows;
using System.Windows.Data;

namespace ADCConfiguration.Converters
{
    public class FileStateToResource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object res = null;
            if (value != null)
            {
                FileState fileState = (FileState)value;
                switch (fileState)
                {
                    case FileState.Missing:
                        res = Application.Current.FindResource("ErrorADCImage");
                        break;
                    case FileState.New:
                        res = Application.Current.FindResource("NewFileValidADCImage");
                        break;
                    case FileState.ToUpdate:
                        res = Application.Current.FindResource("ImportValidADCImage");
                        break;
                    case FileState.IdenticalFile:
                        res = Application.Current.FindResource("ValidADCImage");
                        break;
                    case FileState.NewVersionCreated:
                        res = Application.Current.FindResource("ValidADCImage");
                        break;
                }
            }

            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
