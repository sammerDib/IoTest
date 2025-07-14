using System.Collections;
using System.Windows.Input;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Filters;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Search;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Sort;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables
{
    /// <summary>
    /// Data source used by the DataTable control
    /// </summary>
    public interface IDataTableSource
    {
        ICommand SortCommand { get; }

        ICollection SourceView { get; }

        FilterEngine Filter { get; }

        ISortEngine Sort { get; }

        ISearchEngine Search { get; }
    }
}
