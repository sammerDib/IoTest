using System.Windows;

namespace UnitySC.Shared.ResultUI.Common.Components.Generic.Filters
{
    /// <summary>
    /// Interaction logic for FilterPanelView.xaml
    /// </summary>
    public partial class FilterPanelView
    {
        public FilterPanelView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty CustomContentProperty = DependencyProperty.Register(nameof(CustomContent), typeof(object), typeof(FilterPanelView), new PropertyMetadata(default(object)));

        public object CustomContent
        {
            get { return GetValue(CustomContentProperty); }
            set { SetValue(CustomContentProperty, value); }
        }
    }
}
