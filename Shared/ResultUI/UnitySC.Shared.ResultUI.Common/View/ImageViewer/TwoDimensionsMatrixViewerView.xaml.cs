using System.Windows;
using System.Windows.Data;

using UnitySC.Shared.ResultUI.Common.ViewModel.ImageViewer;

namespace UnitySC.Shared.ResultUI.Common.View.ImageViewer
{
    /// <summary>
    /// Interaction logic for TwoDimensionsMatrixViewerView.xaml
    /// </summary>
    public partial class TwoDimensionsMatrixViewerView
    {
        public TwoDimensionsMatrixViewerView()
        {
            InitializeComponent();
            SetBinding(SelectProfileChartFlagProperty, new Binding(nameof(TwoDimensionsMatrixViewerVM.ShowProfileChartFlag)));
        }

        public static readonly DependencyProperty SelectProfileChartFlagProperty = DependencyProperty.Register(
            nameof(SelectProfileChartFlag), typeof(bool), typeof(TwoDimensionsMatrixViewerView), new PropertyMetadata(default(bool), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TwoDimensionsMatrixViewerView self)
            {
                self.ChartExpander.IsExpanded = true;
                self.ChartTabControl.Select(self.ProfileTabItem);
            }
        }

        public bool SelectProfileChartFlag
        {
            get { return (bool)GetValue(SelectProfileChartFlagProperty); }
            set { SetValue(SelectProfileChartFlagProperty, value); }
        }
    }
}
