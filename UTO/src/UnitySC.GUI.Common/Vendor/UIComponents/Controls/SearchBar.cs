using System.Windows;
using System.Windows.Controls;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Search;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    public class SearchBar : Control
    {
        static SearchBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchBar), new FrameworkPropertyMetadata(typeof(SearchBar)));
        }

        public static readonly DependencyProperty SearchEngineProperty = DependencyProperty.Register(
            nameof(SearchEngine), typeof(ISearchEngine), typeof(SearchBar), new PropertyMetadata(default(ISearchEngine)));

        public ISearchEngine SearchEngine
        {
            get { return (ISearchEngine)GetValue(SearchEngineProperty); }
            set { SetValue(SearchEngineProperty, value); }
        }
    }
}
