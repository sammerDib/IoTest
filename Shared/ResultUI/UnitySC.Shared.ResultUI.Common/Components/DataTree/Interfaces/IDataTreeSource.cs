using System.Collections;
using System.Windows.Input;

using UnitySC.Shared.ResultUI.Common.Components.DataTree.DragDrop;
using UnitySC.Shared.ResultUI.Common.Components.Generic.Filters;
using UnitySC.Shared.ResultUI.Common.Components.Generic.Search;
using UnitySC.Shared.ResultUI.Common.Components.Generic.Sort;

namespace UnitySC.Shared.ResultUI.Common.Components.DataTree.Interfaces
{
    public interface IDataTreeSource
    {
        ISearchEngine Search { get; }

        ICollection SourceView { get; }

        IDragDropEngine DragDropBehavior { get; }
        
        FilterEngine Filter { get; }

        ISortEngine Sort { get; }

        object SelectedValue { get; set; }

        bool OnKeyDown(KeyEventArgs keyEventArgs);
    }
}
