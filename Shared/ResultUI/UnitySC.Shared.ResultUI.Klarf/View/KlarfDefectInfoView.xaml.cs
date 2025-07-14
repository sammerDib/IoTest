using System.Windows.Controls;

namespace UnitySC.Shared.ResultUI.Klarf.View
{
    /// <summary>
    /// Interaction logic for KlarfDefectInfoView.xaml
    /// </summary>
    public partial class KlarfDefectInfoView : UserControl
    {
        public KlarfDefectInfoView()
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
