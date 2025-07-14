using System;
using System.Globalization;
using System.Windows;

using Agileo.GUI;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    public class LocalizedUnitsNet : UnitsNet
    {
        public LocalizedUnitsNet()
        {
            if (Application.Current is AgilControllerApplication application)
            {
                AddHandler(UnloadedEvent, new RoutedEventHandler(OnUnloaded));
                AddHandler(LoadedEvent, new RoutedEventHandler(OnLoaded));
            }
        }

        private void OnCultureChanged(object sender, EventArgs e)
        {
            FormatProvider = Application.Current is AgilControllerApplication application ? application.Localizer.CurrentCulture : CultureInfo.InvariantCulture;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (Application.Current is AgilControllerApplication application)
            {
                application.Localizer.CultureChanged -= OnCultureChanged;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Application.Current is AgilControllerApplication application)
            {
                application.Localizer.CultureChanged += OnCultureChanged;
            }
        }

    }
}
