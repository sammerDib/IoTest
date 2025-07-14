using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using Agileo.Common.Tracing.Filters;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;

using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Setup.Diagnostic
{
    public class FilterEditorPopupContent : Notifier
    {
        #region Properties

        public TracerFilter Filter { get; }

        public ReadOnlyCollection<string> PossibleSources { get; set; }

        public ObservableCollection<string> SourcesCollection { get; set; } = new();

        public ObservableCollection<FilterParameter> LevelsCollection { get; set;} = new();

        private string _editedSource = string.Empty;

        public string EditedSource
        {
            get { return _editedSource; }
            set
            {
                if (_editedSource == value) return;
                _editedSource = value;
                OnPropertyChanged(nameof(EditedSource));
            }
        }

        #endregion

        public Func<string, Visibility> ErrorIsVisibleFunc { get; }

        //Design mode constructor
        // ReSharper disable once UnusedMember.Global
        public FilterEditorPopupContent() : this(null, new ReadOnlyCollection<string>(new List<string>
            {
                "Access",
                "Agileo.SEMI",
                "GEM",
                "Operator"
            }))
        {
        }

        public FilterEditorPopupContent(IFilter filter, ReadOnlyCollection<string> possibleSources)
        {
            PossibleSources = possibleSources;
            if (filter != null)
            {
                Filter = (TracerFilter)((TracerFilter)filter).Clone();
            }
            else
            {
                Filter = new TracerFilter("New filter");
            }

            SourcesCollection.AddRange(Filter.Source.Items);

            foreach (var levelItem in Filter.Level.Items)
            {
                LevelsCollection.Add(new FilterParameter(levelItem, Filter.Level.SelectedItems.Contains(levelItem)));
            }

            ErrorIsVisibleFunc = filterName => !AllowedSourceName(filterName)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private ICommand _addSourceCommand;

        public ICommand AddSourceCommand => _addSourceCommand ??= new DelegateCommand(AddSourceCommandExecute, AddSourceCommandCanExecute);

        private bool AddSourceCommandCanExecute() => !string.IsNullOrWhiteSpace(EditedSource) && AllowedSourceName(EditedSource) && !SourcesCollection.Any(s => s.Equals(EditedSource));

        private void AddSourceCommandExecute()
        {
            Filter.Source.AddItem(EditedSource);
            Filter.Source.Update();

            SourcesCollection.Add(EditedSource);
            EditedSource = string.Empty;
        }

        public void SaveFilter()
        {
            Filter.Level.SelectItems(LevelsCollection.Where(x => x.IsActivated).Select(x => x.SourceName).ToArray());
            Filter.Level.Update();

            Filter.Source.SelectItems(SourcesCollection.ToArray());
            Filter.Source.Update();
        }

        private static bool AllowedSourceName(string sourceName) => sourceName.Count(x => x.Equals('*')) <= 1;
    }

    /// <summary>
    /// Model used to visualize a filter lines
    /// </summary>
    public class FilterParameter
    {
        public FilterParameter(string name, bool isActivated)
        {
            SourceName = name;
            IsActivated = isActivated;
        }

        public string SourceName { get; }

        public bool IsActivated { get; set; }
    }
}
