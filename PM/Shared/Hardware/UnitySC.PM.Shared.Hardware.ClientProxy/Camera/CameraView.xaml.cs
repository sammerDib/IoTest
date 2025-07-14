using System.Windows;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Camera
{
    /// <summary>
    /// Interaction logic for CameraView.xaml
    /// </summary>
    public partial class CameraView : UserControl
    {
        public CameraView()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var previousDataContext = e.OldValue as ObservableRecipient;
            if (previousDataContext != null)
            {
                previousDataContext.IsActive = false;
            }
        }
    }
}
