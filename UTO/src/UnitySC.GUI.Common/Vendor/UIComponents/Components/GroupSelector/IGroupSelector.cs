using System.Collections;
using System.Windows.Input;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.GroupSelector
{
    public interface IGroupSelector
    {
        IEnumerable SelectedGroups { get; }

        IEnumerable Groups { get; }

        bool SelectedGroupsChangedFlag { get; }

        ICommand SelectGroupCommand { get; }

        ICommand CheckAllCommand { get; }

        ICommand UncheckAllCommand { get; }

        ICommand InvertSelectionCommand { get; }
    }
}
