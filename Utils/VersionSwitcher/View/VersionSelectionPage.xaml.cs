using CommunityToolkit.Mvvm.DependencyInjection;

using VersionSwitcher.ViewModel;

namespace VersionSwitcher.View
{
    public partial class VersionSelectionPage
    {
        public VersionSelectionPage()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetService<VersionSelectionViewModel>();
        }
    }
}
