using System.Windows;
using System.Windows.Controls;

using UnitySC.PM.DMT.CommonUI.ViewModel.ExposureSettings;
using UnitySC.PM.DMT.Shared.UI.ViewModel;

namespace UnitySC.PM.DMT.CommonUI.View.ExposureSettings
{
    /// <summary>
    /// Interaction logic for ExposureSettingsWithAuto.xaml
    /// </summary>
    public partial class ExposureSettingsWithAuto : UserControl
    {
        private ExposureSettingsWithAutoVM exposureSettingWithAutoVm;

        public ExposureSettingsWithAuto()
        {
            InitializeComponent();
        }

        private void ExposureSettingsWithAuto_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ExposureSettingsWithAutoVM.EditExposureTime))
            {
                if (!DisplayApplyButton)
                {
                    exposureSettingWithAutoVm.ExposureTimeMs = (DataContext as ExposureSettingsWithAutoVM).EditExposureTime;
                    exposureSettingWithAutoVm.ExposureTimeStatus = ExposureTimeStatus.Valid;
                }
            }


        }

        public bool DisplayApplyButton
        {
            get { return (bool)GetValue(DisplayApplyButtonProperty); }
            set { SetValue(DisplayApplyButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayApplyButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayApplyButtonProperty =
            DependencyProperty.Register("DisplayApplyButton", typeof(bool), typeof(ExposureSettingsWithAuto), new PropertyMetadata(true));



        public bool DisplayComputeButton
        {
            get { return (bool )GetValue(DisplayComputeButtonProperty); }
            set { SetValue(DisplayComputeButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for .  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayComputeButtonProperty =
            DependencyProperty.Register("DisplayComputeButton", typeof(bool), typeof(ExposureSettingsWithAuto), new PropertyMetadata(true));



        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                exposureSettingWithAutoVm = (this.DataContext as ExposureSettingsWithAutoVM);
                exposureSettingWithAutoVm.PropertyChanged += ExposureSettingsWithAuto_PropertyChanged;
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (exposureSettingWithAutoVm != null)
                exposureSettingWithAutoVm.PropertyChanged -= ExposureSettingsWithAuto_PropertyChanged;
        }
    }
}
