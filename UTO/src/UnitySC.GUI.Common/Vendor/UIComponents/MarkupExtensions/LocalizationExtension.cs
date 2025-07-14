using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

using Agileo.GUI;

namespace UnitySC.GUI.Common.Vendor.UIComponents.MarkupExtensions
{
    [MarkupExtensionReturnType(typeof(IFormatProvider))]
    public class LocalizationExtension : UpdatableMarkupExtension
    {
        public LocalizationExtension()
        {
            if (Application.Current is AgilControllerApplication application)
            {
                application.Localizer.CultureChanged += Localizer_CultureChanged;
            }
        }

        private void Localizer_CultureChanged(object sender, EventArgs e)
        {
            UpdateValue(ProvideValueInternal(null));
        }

        protected override object ProvideValueInternal(IServiceProvider serviceProvider)
        {
            if (!(TargetObject is FrameworkElement))
            {
                throw new InvalidOperationException($"{nameof(LocalizationExtension)} can only be used on a FrameworkElement instance.");
            }
            return Application.Current is AgilControllerApplication ? ((AgilControllerApplication)Application.Current).Localizer.CurrentCulture : CultureInfo.InvariantCulture;
        }
    }
}
