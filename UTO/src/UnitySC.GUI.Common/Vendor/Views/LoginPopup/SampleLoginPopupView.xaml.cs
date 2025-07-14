using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace UnitySC.GUI.Common.Vendor.Views.LoginPopup
{
    /// <summary>
    /// Interaction logic for LogInPopupView.xaml
    /// </summary>
    public partial class SampleLoginPopupView : UserControl
    {
        public SampleLoginPopupView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Used to focus automatically Users combobox at the Login panel opening
        /// </summary>
        private void LogInPopupView_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                Dispatcher.BeginInvoke(
                    DispatcherPriority.ContextIdle,
                    new Action(delegate
                    {
                        CbUserName.Focus();
                    }));
            }
        }
    }
}
