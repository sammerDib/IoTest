using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace UnitySC.Shared.UI.Helper
{
    public static class TabControlHelper
    {
        public static void SelectFirstVisibleItem(this TabControl tabControl)
        {
            tabControl.Dispatcher.InvokeAsync(() =>
            {
                var tabItems = tabControl.Items.OfType<TabItem>();
                var firstVisibleItem = tabItems.FirstOrDefault(item => item.Visibility == Visibility.Visible);
                if (firstVisibleItem != null)
                {
                    firstVisibleItem.IsSelected = true;
                }
            }, DispatcherPriority.ApplicationIdle);
        }
    }
}
