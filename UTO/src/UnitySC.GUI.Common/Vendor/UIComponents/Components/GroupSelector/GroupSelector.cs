using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Agileo.GUI.Commands;
using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.GroupSelector
{
    public class GroupSelector<T> : Notifier, IGroupSelector
    {
        #region Implementation of IGroupSelector

        IEnumerable IGroupSelector.SelectedGroups => SelectedGroups;
        IEnumerable IGroupSelector.Groups => Groups;

        #endregion

        private readonly Func<IEnumerable<T>> _getGroups;
        private readonly Action _onGroupSelectionChanged;
        private readonly Func<T, object> _getGroupId;

        public GroupSelector(Func<IEnumerable<T>> getGroups, Action onGroupSelectionChanged, Func<T, object> getGroupId = null)
        {
            _getGroups = getGroups;
            _onGroupSelectionChanged = onGroupSelectionChanged;
            _getGroupId = getGroupId;
        }

        public List<T> SelectedGroups { get; } = new List<T>();

        private List<T> _groups;

        public List<T> Groups
        {
            get => _groups;
            set => SetAndRaiseIfChanged(ref _groups, value);
        }

        private bool _selectedGroupsChangedFlag;

        public bool SelectedGroupsChangedFlag
        {
            get => _selectedGroupsChangedFlag;
            private set => SetAndRaiseIfChanged(ref _selectedGroupsChangedFlag, value);
        }

        public void Refresh()
        {
            var ids = new List<object>();
            if (_getGroupId != null)
            {
                ids.AddRange(SelectedGroups.Select(group => _getGroupId(group)));
            }

            SelectedGroups.Clear();
            Groups = _getGroups().ToList();

            if (_getGroupId != null)
            {
                foreach (var group in Groups)
                {
                    //Add all group to selected by default
                    SelectedGroups.Add(group);
                }
            }
            else
            {
                // Auto Select the first available group
                var firstOrDefault = Groups.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    SelectedGroups.Add(firstOrDefault);
                }
            }

            NotifySelectionChanged();
        }

        public void Select(T group)
        {
            if (group == null) return;
            if (SelectedGroups.Contains(group)) return;
            SelectedGroups.Add(group);
            NotifySelectionChanged();
        }

        public void Unselect(T group)
        {
            if (group == null) return;
            if (!SelectedGroups.Contains(group)) return;
            SelectedGroups.Remove(group);
            NotifySelectionChanged();
        }

        private void NotifySelectionChanged()
        {
            _onGroupSelectionChanged();
            SelectedGroupsChangedFlag = !SelectedGroupsChangedFlag;
        }

        #region Commands

        private ICommand _selectGroupCommand;

        public ICommand SelectGroupCommand => _selectGroupCommand ?? (_selectGroupCommand = new DelegateCommand<T>(SelectGroupCommandExecute));

        private void SelectGroupCommandExecute(T group)
        {
            if (group == null) return;
            if (SelectedGroups.Contains(group))
            {
                SelectedGroups.Remove(group);
            }
            else
            {
                SelectedGroups.Add(group);
            }

            NotifySelectionChanged();
        }

        private ICommand _checkAllCommand;

        public ICommand CheckAllCommand => _checkAllCommand ?? (_checkAllCommand = new DelegateCommand(CheckAllCommandExecute));

        private void CheckAllCommandExecute()
        {
            SelectedGroups.Clear();
            SelectedGroups.AddRange(Groups);
            NotifySelectionChanged();
        }

        private ICommand _uncheckAllCommand;

        public ICommand UncheckAllCommand => _uncheckAllCommand ?? (_uncheckAllCommand = new DelegateCommand(UncheckAllCommandExecute));

        private void UncheckAllCommandExecute()
        {
            SelectedGroups.Clear();
            NotifySelectionChanged();
        }

        private ICommand _invertSelectionCommand;

        public ICommand InvertSelectionCommand => _invertSelectionCommand ?? (_invertSelectionCommand = new DelegateCommand(InvertSelectionCommandExecute));

        private void InvertSelectionCommandExecute()
        {
            foreach (var group in Groups)
            {
                if (SelectedGroups.Contains(group))
                {
                    SelectedGroups.Remove(group);
                }
                else
                {
                    SelectedGroups.Add(group);
                }
            }
            NotifySelectionChanged();
        }

        #endregion
    }
}
