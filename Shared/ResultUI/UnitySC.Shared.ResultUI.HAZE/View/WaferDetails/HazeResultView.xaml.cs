using System;

namespace UnitySC.Shared.ResultUI.HAZE.View.WaferDetails
{
    /// <summary>
    /// Interaction logic for HazeResultView.xaml
    /// </summary>
    public partial class HazeResultView
    {
        public HazeResultView()
        {
            InitializeComponent();
        }
        
        private void HazeMapView_OnProfileDrawn(object sender, EventArgs e)
        {
            ChartTabControl.Select(ProfileTab);
            ChartExpander.IsExpanded = true;
        }
    }
}
