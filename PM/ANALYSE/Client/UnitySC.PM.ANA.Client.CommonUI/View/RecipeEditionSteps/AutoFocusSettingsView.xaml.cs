using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;

namespace UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps
{
    /// <summary>
    /// Interaction logic for AutoFocusSettingsView.xaml
    /// </summary>
    public partial class AutoFocusSettingsView : UserControl
    {
        public AutoFocusSettingsView()
        {
            InitializeComponent();
        }

        public AutoFocusSettingsVM AutoFocusSettings
        {
            get { return (AutoFocusSettingsVM)GetValue(AutoFocusSettingsProperty); }
            set { SetValue(AutoFocusSettingsProperty, value); }
        }

        public static readonly DependencyProperty AutoFocusSettingsProperty =
            DependencyProperty.Register(nameof(AutoFocusSettings), typeof(AutoFocusSettingsVM), typeof(AutoFocusSettingsView), new PropertyMetadata(null));

        public AvailableAutoFocus AvailableAutoFocusTypes
        {
            get { return (AvailableAutoFocus)GetValue(AvailableAutoFocusTypesProperty); }
            set { SetValue(AvailableAutoFocusTypesProperty, value); }
        }

        public static readonly DependencyProperty AvailableAutoFocusTypesProperty =
            DependencyProperty.Register(nameof(AvailableAutoFocusTypes), typeof(AvailableAutoFocus), typeof(AutoFocusSettingsView), new PropertyMetadata(AvailableAutoFocus.Camera));

        public bool CanSelectLiseObjective
        {
            get { return (bool)GetValue(CanSelectLiseObjectiveProperty); }
            set { SetValue(CanSelectLiseObjectiveProperty, value); }
        }

        public static readonly DependencyProperty CanSelectLiseObjectiveProperty =
            DependencyProperty.Register(nameof(CanSelectLiseObjective), typeof(bool), typeof(AutoFocusSettingsView), new PropertyMetadata(true));



        public bool CanSelectCameraObjective
        {
            get { return (bool)GetValue(CanSelectCameraObjectiveProperty); }
            set { SetValue(CanSelectCameraObjectiveProperty, value); }
        }

        public static readonly DependencyProperty CanSelectCameraObjectiveProperty =
            DependencyProperty.Register(nameof(CanSelectCameraObjective), typeof(bool), typeof(AutoFocusSettingsView), new PropertyMetadata(true));




    }




    public class LiseAutofocusToVisibilityConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var autoFocusType = (AutoFocusType)value;
            if ((autoFocusType == AutoFocusType.Lise) || (autoFocusType == AutoFocusType.LiseAndCamera))
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class CameraAutofocusToVisibilityConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var autoFocusType = (AutoFocusType)value;
            if ((autoFocusType == AutoFocusType.Camera) || (autoFocusType == AutoFocusType.LiseAndCamera))
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
