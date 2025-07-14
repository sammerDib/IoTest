using System.Windows;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Filters
{
    /// <summary>
    /// Interaction logic for FilterEngineView.xaml
    /// </summary>
    public partial class FilterEngineView 
    {
        public FilterEngineView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty CustomContentProperty = DependencyProperty.Register(
            nameof(CustomContent),
            typeof(object),
            typeof(FilterEngineView),
            new PropertyMetadata(default(object)));

        public object CustomContent
        {
            get => GetValue(CustomContentProperty);
            set => SetValue(CustomContentProperty, value);
        }
    }
}
