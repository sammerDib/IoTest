using System;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Alarms.Analysis
{
    public partial class AnalysisPanelView
    {
        public AnalysisPanelView()
        {
            InitializeComponent();
            DataContextChanged += AnalysisPanelView_DataContextChanged;
        }

        private void AnalysisPanelView_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is AnalysisPanel oldContext)
            {
                oldContext.TakeScreenShotRequested -= OnTakeScreenShotRequested;
            }

            if (e.NewValue is AnalysisPanel newContext)
            {
                newContext.TakeScreenShotRequested += OnTakeScreenShotRequested;
            }
        }

        private void OnTakeScreenShotRequested(object sender, EventArgs e)
        {
            if (DataContext is AnalysisPanel analysisPanel)
            {
                analysisPanel.TakeScreenShotExecute(this);
            }
        }
    }
}
