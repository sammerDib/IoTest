using System.Collections;
using System.Windows.Input;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.Core.KeyGesture;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Filters;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Search;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Sort;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree.DragDrop;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree.Interfaces
{
    /// <summary>
    /// Interface defining the contract between the generic <see cref="DataTreeSource{T}"/> implementation and the <see cref="Controls.DataTree"/> control.
    /// </summary>
    public interface IDataTreeSource : IKeyGestureHandler
    {
        ICommand SortCommand { get; }

        ISearchEngine Search { get; }

        ICollection SourceView { get; }

        IDragDropEngine DragDropBehavior { get; }

        FilterEngine Filter { get; }

        ISortEngine Sort { get; }

        object SelectedValue { get; set; }

        bool IsFocused { get; set; }
    }
}
