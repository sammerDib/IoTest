using System.Windows.Controls;

namespace UnitySC.Shared.ResultUI.Common.View
{
    /// <summary>
    /// Interaction logic for ThumbnailsDefectsView.xaml
    /// </summary>
    public partial class ThumbnailsDefectsView : UserControl
    {
        public ThumbnailsDefectsView()
        {
            InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var lv = (ListView)sender;
                var test = e.AddedItems;
                lv.Items.MoveCurrentTo(e.AddedItems[0]);
                lv.ScrollIntoView(e.AddedItems[0]);
            };
        }
    }
}