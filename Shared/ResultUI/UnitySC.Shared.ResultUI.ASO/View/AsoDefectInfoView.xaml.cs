using System.Windows.Controls;

namespace UnitySC.Shared.ResultUI.ASO.View
{
    /// <summary>
    /// Interaction logic for AsoDefectInfoView.xaml
    /// </summary>
    public partial class AsoDefectInfoView : UserControl
    {
        public AsoDefectInfoView()
        {
            InitializeComponent();
        }

        private void LvImages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var lv = (ListView)sender;
                lv.Items.MoveCurrentTo(e.AddedItems[0]);
                lv.ScrollIntoView(e.AddedItems[0]);
            }
        }
    }
}
