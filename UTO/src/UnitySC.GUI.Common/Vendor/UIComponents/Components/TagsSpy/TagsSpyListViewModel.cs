using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.MessageDataBus;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Filters;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Sort;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.GroupSelector;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.TagsSpy
{
    public enum SelectedTagTemplate
    {
        Boolean,
        Enumerate,
        Various
    }

    public class TagsSpyListViewModel : Notifier
    {
        #region Fields

        private readonly IMessageDataBus _messageDataBus;
        private readonly Action<BaseTag> _onTagAddedToSpy;
        private readonly Action<BaseTag> _onTagRemovedFromSpy;
        private readonly IEnumerable<Type> _allTagTypes;

        #endregion

        #region Properties

        public Func<Group, bool, string> UpdateVisibleItemsCountFunc { get; }

        private bool _visibleItemsCountFlag;

        public bool VisibleItemsCountFlag
        {
            get { return _visibleItemsCountFlag; }
            set
            {
                SetAndRaiseIfChanged(ref _visibleItemsCountFlag, value);
            }
        }

        public DataTableSource<BaseTag> TagSource { get; } = new DataTableSource<BaseTag>();

        public GroupSelector<Group> GroupSelector { get; }

        private BaseTag _selectedTag;

        /// <summary>
        /// Switch the selected view will be update
        /// </summary>
        public BaseTag SelectedTag
        {
            get => _selectedTag;
            set
            {
                if (SetAndRaiseIfChanged(ref _selectedTag, value))
                {
                    // update input tag value depending on tag value type
                    UpdatePossibleTagValues(_selectedTag);
                }
            }
        }

        /// <summary>
        /// Get all possible <see cref="Enum"/> values of selected tag (if it is of type Enum)
        /// </summary>
        public List<string> EnumTagValues { get; } = new List<string>();

        private SelectedTagTemplate _selectedTagType;

        /// <summary>
        /// Used to know the template to used for editing selected tag
        /// </summary>
        public SelectedTagTemplate SelectedTagType
        {
            get => _selectedTagType;
            set => SetAndRaiseIfChanged(ref _selectedTagType, value);
        }

        private object _valueToWrite;

        /// <summary>
        /// Set / Get the new value for the selected tag
        /// </summary>
        public object ValueToWrite
        {
            get => _valueToWrite;
            set
            {
                if (SetAndRaiseIfChanged(ref _valueToWrite, value))
                {
                    UpdateCanApplyValue();
                }
            }
        }

        private bool _canApplyValue;

        public bool CanApplyValue
        {
            get => _canApplyValue;
            set => SetAndRaiseIfChanged(ref _canApplyValue, value);
        }

        public List<BaseTag> SpiedTags { get; } = new List<BaseTag>();

        /// <summary>
        /// This variable is used to notify the view to interpret spied tags.
        /// </summary>
        public bool SpiedTagsChangedFlag { get; private set; }

        public List<BaseTag> FavoriteTags { get; } = new List<BaseTag>();

        #endregion

        public TagsSpyListViewModel(
            IMessageDataBus messageDataBus,
            Action<BaseTag> onTagAddedToSpy,
            Action<BaseTag> onTagRemovedFromSpy)
        {

            _messageDataBus = messageDataBus;
            _onTagAddedToSpy = onTagAddedToSpy;
            _onTagRemovedFromSpy = onTagRemovedFromSpy;

            UpdateVisibleItemsCountFunc = (tagGroup, b) =>
            {
                if (GroupSelector == null) return string.Empty;
                var total = tagGroup.TagList.Count;
                var visibleItemsCount = GroupSelector.SelectedGroups.Contains(tagGroup)
                    ? tagGroup.TagList.Intersect(TagSource.SourceView).Count()
                    : 0;
                return total == visibleItemsCount ? $"({total})" : $"({visibleItemsCount}/{total})";
            };

            TagSource.AutoUpdate = false;

            TagSource.Sort.DefineConstantSortingDefinition(
                new SortDefinition<BaseTag>("Favorite", tag => !FavoriteTags.Contains(tag)));

            GroupSelector = new GroupSelector<Group>(() =>
            {
                return _messageDataBus.Groups.Select(group => group.Value);
            }, RefreshTagsList, group => group.Name);

            GroupSelector.Refresh();

            // GroupName sorting first to applied it by default
            TagSource.Sort
                .AddSortDefinition(nameof(BaseTag.GroupName), tag => tag.GroupName)
                .AddSortDefinition(nameof(ExternalTag<object>.Quality), tag => (int?)tag.GetQuality() ?? -1)
                .AddSortDefinition(nameof(ExternalTag<object>.Path), tag => tag.GetPath() ?? string.Empty)
                .AddSortDefinition(nameof(ExternalTag<object>.ClientID), tag => tag.GetClientId() ?? -1);

            TagSource.Search.AddSearchDefinition(new InvariantText(nameof(BaseTag.Name)), tag => tag.Name, true);

            TagSource.Filter.Add(new FilterSwitch<BaseTag>(new InvariantText("Only favorites"), tag => FavoriteTags.Contains(tag)));
            TagSource.Filter.Add(new FilterSwitch<BaseTag>(new InvariantText("Only spied"), tag => SpiedTags.Contains(tag)));
            TagSource.Filter.AddRangeFilter(nameof(BaseTag.UID), tag => tag.UID, () => TagSource);

            _allTagTypes = _messageDataBus.Groups.SelectMany(pair => pair.Value.TagList)
                .Select(tag => tag.ValueType)
                .Distinct();
            TagSource.Filter.Add(
                new FilterCollection<BaseTag, Type>(nameof(Type), () => _allTagTypes, tag => tag.ValueType));

            RefreshTagsList();

            TagSource.PropertyChanged += TagSource_PropertyChanged;
        }

        protected void RefreshTagsList()
        {
            DispatcherHelper.DoInUiThreadAsynchronously(() =>
            {
                TagSource.Reset(GroupSelector.SelectedGroups.SelectMany(activeGrp => activeGrp.TagList));
            });
        }

        #region Privates

        private void TagSource_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DataTableSource<BaseTag>.SourceView))
            {
                VisibleItemsCountFlag = !VisibleItemsCountFlag;
            }
        }

        private void NotifySpiedTagsChanged()
        {
            SpiedTagsChangedFlag = !SpiedTagsChangedFlag;
            OnPropertyChanged(nameof(SpiedTagsChangedFlag));
            OnPropertyChanged(nameof(SpiedTags));
        }

        private void NotifyFavoriteTagsChanged()
        {
            TagSource.UpdateCollection();
        }

        private void UpdateCanApplyValue()
        {
            CanApplyValue = SelectedTag != null && SelectedTag.CanBeSetWith(ValueToWrite.ToString());
        }

        private void UpdatePossibleTagValues(BaseTag selectedTag)
        {
            if (selectedTag != null)
            {
                var selectedTagTypeArgs = SelectedTag.ValueType;
                if (selectedTagTypeArgs?.IsEnum ?? false)
                {
                    EnumTagValues.Clear();
                    SelectedTagType = SelectedTagTemplate.Enumerate;
                    var nbEnum = 0;
                    foreach (var enumValue in Enum.GetNames(selectedTagTypeArgs))
                    {
                        EnumTagValues.Add(enumValue);

                        if (enumValue == SelectedTag.GetValue()?.ToString())
                        {
                            ValueToWrite = EnumTagValues[nbEnum];
                        }

                        ++nbEnum;
                    }

                    OnPropertyChanged(nameof(EnumTagValues));
                }
                else if (selectedTagTypeArgs?.Name == nameof(Boolean))
                {
                    SelectedTagType = SelectedTagTemplate.Boolean;
                    ValueToWrite = SelectedTag.GetValue();
                }
                else
                {
                    SelectedTagType = SelectedTagTemplate.Various;
                    ValueToWrite = SelectedTag.GetValue();
                }
            }
            else
            {
                ValueToWrite = null;
            }

            UpdateCanApplyValue();
        }

        #endregion

        #region Commands

        private ICommand _applyValueCommand;

        public ICommand ApplyValueCommand => _applyValueCommand ?? (_applyValueCommand = new DelegateCommand(CanApplyValueCommandExecute, CanApplyValueCommandCanExecute));

        private bool CanApplyValueCommandCanExecute()
        {
            return CanApplyValue;
        }

        private void CanApplyValueCommandExecute()
        {
            SelectedTag.TrySetValue(ValueToWrite.ToString());
        }

        private ICommand _addTagToSpiedCommand;

        public ICommand AddTagToSpiedCommand => _addTagToSpiedCommand ?? (_addTagToSpiedCommand = new DelegateCommand<BaseTag>(AddTagToSpiedCommandExecute, AddTagToSpiedCanExecute));

        private bool AddTagToSpiedCanExecute(BaseTag arg)
        {
            if (arg == null)
            {
                return false;
            }

            return arg.ValueType != typeof(string);
        }

        private void AddTagToSpiedCommandExecute(BaseTag tag)
        {
            if (tag == null)
            {
                return;
            }

            SpiedTags.Add(tag);
            NotifySpiedTagsChanged();
            _onTagAddedToSpy(tag);
        }

        private ICommand _removeTagFromSpiedCommand;

        public ICommand RemoveTagFromSpiedCommand => _removeTagFromSpiedCommand ?? (_removeTagFromSpiedCommand = new DelegateCommand<BaseTag>(RemoveTagFromSpiedCommandExecute));

        private void RemoveTagFromSpiedCommandExecute(BaseTag tag)
        {
            if (tag == null)
            {
                return;
            }

            SpiedTags.Remove(tag);
            NotifySpiedTagsChanged();
            _onTagRemovedFromSpy(tag);
        }

        private ICommand _addTagToFavoriteCommand;

        public ICommand AddTagToFavoriteCommand => _addTagToFavoriteCommand ?? (_addTagToFavoriteCommand = new DelegateCommand<BaseTag>(AddTagToFavoriteCommandExecute));

        private void AddTagToFavoriteCommandExecute(BaseTag tag)
        {
            if (tag != null)
            {
                FavoriteTags.Add(tag);
            }

            NotifyFavoriteTagsChanged();
        }

        private ICommand _removeTagFromFavoriteCommand;

        public ICommand RemoveTagFromFavoriteCommand => _removeTagFromFavoriteCommand ?? (_removeTagFromFavoriteCommand = new DelegateCommand<BaseTag>(RemoveTagFromFavoriteCommandExecute));

        private void RemoveTagFromFavoriteCommandExecute(BaseTag tag)
        {
            if (tag != null)
            {
                FavoriteTags.Remove(tag);
            }

            NotifyFavoriteTagsChanged();
        }

        #endregion Commands
    }
}
