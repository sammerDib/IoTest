using System.Windows;
using System.Windows.Controls;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.GroupSelector
{
    /// <summary>
    /// Interaction logic for GroupSelectorView.xaml
    /// </summary>
    public partial class GroupSelectorView : UserControl
    {
        public GroupSelectorView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty GroupTemplateProperty = DependencyProperty.Register(
            nameof(GroupTemplate), typeof(DataTemplate), typeof(GroupSelectorView), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate GroupTemplate
        {
            get { return (DataTemplate)GetValue(GroupTemplateProperty); }
            set { SetValue(GroupTemplateProperty, value); }
        }
    }
}
