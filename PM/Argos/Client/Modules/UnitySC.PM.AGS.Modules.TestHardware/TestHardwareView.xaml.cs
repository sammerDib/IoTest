using System.Windows.Controls;

namespace UnitySC.PM.AGS.Modules.TestHardware
{
    /// <summary>
    /// Interaction logic for TestHardwareView.xaml
    /// </summary>
    public partial class TestHardwareView : UserControl
    {
        public TestHardwareView()
        {
            InitializeComponent();
        }

        //private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    (this.DataContext as TestHardwareViewModel).IsVisible = (bool)e.NewValue;
        //}

        //private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        //{
        //    //Mil.Instance.Free();
        //}
    }
}
