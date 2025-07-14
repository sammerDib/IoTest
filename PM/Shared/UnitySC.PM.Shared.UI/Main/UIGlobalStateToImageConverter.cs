using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.Shared.UI.Main
{
    public class UIGlobalStateToGeometryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object res = null;
            if (value is UIGlobalStates globalState)
            {
                switch (globalState)
                {
                    case UIGlobalStates.NotInitialized:
                        res = Application.Current.FindResource("NotInitializedGeometry");
                        break;

                    case UIGlobalStates.Initializing:
                        res = Application.Current.FindResource("InitializingGeometry");
                        break;

                    case UIGlobalStates.Available:
                        res = Application.Current.FindResource("CheckCircleSolidGeometry");
                        break;

                    case UIGlobalStates.Error:
                        res = Application.Current.FindResource("ErrorGeometry");
                        break;

                    case UIGlobalStates.Maintenance:
                        res = Application.Current.FindResource("MaintenanceGeometry");
                        break;

                    case UIGlobalStates.InUse:
                        res = Application.Current.FindResource("LockSolidGeometry");
                        break;
                }
            }

            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class UIGlobalStateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object res = null;
            if (value is UIGlobalStates globalState)
            {
                switch (globalState)
                {
                    case UIGlobalStates.NotInitialized:
                        res = Application.Current.FindResource("IconsColor");
                        break;

                    case UIGlobalStates.Initializing:
                        res = Application.Current.FindResource("IconsColor");
                        break;

                    case UIGlobalStates.Available:
                        res = Application.Current.FindResource("IconsColor");
                        break;

                    case UIGlobalStates.Error:
                        res = Application.Current.FindResource("IconsErrorColor");
                        break;

                    case UIGlobalStates.Maintenance:
                        res = Application.Current.FindResource("IconsColor");
                        break;

                    case UIGlobalStates.InUse:
                        res = Application.Current.FindResource("IconsErrorColor");
                        break;
                }
            }

            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
