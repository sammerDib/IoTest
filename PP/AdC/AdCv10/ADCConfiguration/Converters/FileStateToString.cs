using System;
using System.Windows.Data;

namespace ADCConfiguration.Converters
{
    public class FileStateToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string res = string.Empty;
            if (value != null)
            {
                FileState fileState = (FileState)value;
                switch (fileState)
                {
                    case FileState.Missing:
                        res = "The external file define in the recipe is missing";
                        break;
                    case FileState.New:
                        res = "New";
                        break;
                    case FileState.ToUpdate:
                        res = "To update";
                        break;
                    case FileState.NewVersionCreated:
                        res = "New version is created";
                        break;
                    case FileState.IdenticalFile:
                        res = "Identical file";
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

