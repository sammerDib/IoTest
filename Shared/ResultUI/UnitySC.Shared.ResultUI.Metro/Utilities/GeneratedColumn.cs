using System.Windows.Data;

using UnitySC.Shared.UI.ViewModel.AdvancedGridView;

namespace UnitySC.Shared.ResultUI.Metro.Utilities
{
    public class GeneratedColumn
    {
        public SortDefinition SortDefinition { get; set; }

        public string HeaderName { get; set; }

        public BindingBase ValueBinding { get; set; }
    }

    public class GeneratedStateColumn : GeneratedColumn
    {
        public BindingBase StateBinding { get; set; }
    }
}
