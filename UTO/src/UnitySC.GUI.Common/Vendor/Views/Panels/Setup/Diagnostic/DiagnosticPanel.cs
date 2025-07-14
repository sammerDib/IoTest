using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

using AgilController.GUI.Configuration;

using Agileo.Common.Localization;
using Agileo.Common.Tracing;
using Agileo.Common.Tracing.Filters;
using Agileo.Common.Tracing.Listeners;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;

using UnitySC.GUI.Common.Vendor.Configuration;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Setup.Diagnostic
{
    public class DiagnosticPanel : SetupNodePanel<Diagnostics>
    {
        #region Fields

        private BufferListener _bufferListener;

        #endregion

        #region Constructors

        static DiagnosticPanel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(DiagnosticPanelResources)));
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(SetupPanelResources)));
        }

        public DiagnosticPanel()
            : this(nameof(DiagnosticPanelResources.DIAGNOSTICPANEL_PANEL_NAME), PathIcon.Diagnostic)
        {
        }

        public DiagnosticPanel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
            SetupRules();
        }

        #endregion Constructors

        #region Properties

        public string TracesPath
        {
            get => ModifiedConfigNode.TracingConfig.FilePaths.TracesPath;
            set
            {
                if (ModifiedConfigNode.TracingConfig.FilePaths.TracesPath == value)
                {
                    return;
                }

                ModifiedConfigNode.TracingConfig.FilePaths.TracesPath = value;
                OnPropertyChanged();
            }
        }

        public string ArchivePath
        {
            get => ModifiedConfigNode.TracingConfig.FilePaths.ArchivePath;
            set
            {
                if (ModifiedConfigNode.TracingConfig.FilePaths.ArchivePath == value)
                {
                    return;
                }

                ModifiedConfigNode.TracingConfig.FilePaths.ArchivePath = value;
                OnPropertyChanged();
            }
        }

        public string ExportPath
        {
            get => ModifiedConfigNode.ExportPath;
            set
            {
                if (ModifiedConfigNode.ExportPath == value)
                {
                    return;
                }

                ModifiedConfigNode.ExportPath = value;
                OnPropertyChanged();
            }
        }

        private IFilter _selectedFilter;

        public IFilter SelectedFilter
        {
            get => _selectedFilter;
            set => SetAndRaiseIfChanged(ref _selectedFilter, value);
        }

        private string _selectedSource;

        public string SelectedSource
        {
            get => _selectedSource;
            set => SetAndRaiseIfChanged(ref _selectedSource, value);
        }

        private string _editedSource = string.Empty;

        public string EditedSource
        {
            get => _editedSource;
            set => SetAndRaiseIfChanged(ref _editedSource, value);
        }

        private ReadOnlyCollection<string> _sources;

        public ReadOnlyCollection<string> Sources
        {
            get => _sources;
            set => SetAndRaiseIfChanged(ref _sources, value);
        }

        private ReadOnlyCollection<IFilter> _filters;

        public ReadOnlyCollection<IFilter> Filters
        {
            get => _filters;
            set => SetAndRaiseIfChanged(ref _filters, value);
        }

        private ReadOnlyCollection<string> _possibleSources;

        public ReadOnlyCollection<string> PossibleSources
        {
            get => _possibleSources;
            set => SetAndRaiseIfChanged(ref _possibleSources, value);
        }

        private string _currentSupportRequestFileFolder;

        public string CurrentSupportRequestFileFolder
        {
            get => _currentSupportRequestFileFolder;
            set => SetAndRaiseIfChanged(ref _currentSupportRequestFileFolder, value);
        }

        private ICollectionView _supportRequestFileFolderCollection;

        public ICollectionView SupportRequestFileFolderCollection
        {
            get => _supportRequestFileFolderCollection;
            set => SetAndRaiseIfChanged(ref _supportRequestFileFolderCollection, value);
        }

        #endregion

        #region Commands

        #region Sources

        private ICommand _addSourceCommand;

        public ICommand AddSourceCommand
            => _addSourceCommand ??= new DelegateCommand(AddSourceCommandExecute, AddSourceCommandCanExecute);

        private bool AddSourceCommandCanExecute()
            => !string.IsNullOrWhiteSpace(EditedSource)
               && !ModifiedConfigNode.TracingConfig.TraceMonitoring.SwitchedOffTraceSources.Contains(EditedSource);

        private void AddSourceCommandExecute()
        {
            var sources = ModifiedConfigNode.TracingConfig.TraceMonitoring.SwitchedOffTraceSources.ToList();
            sources.Add(EditedSource);
            ModifiedConfigNode.TracingConfig.TraceMonitoring.SwitchedOffTraceSources = sources.ToArray();
            UpdateSourceCollection();
        }

        private ICommand _deleteSourceCommand;

        public ICommand DeleteSourceCommand
            => _deleteSourceCommand ??= new DelegateCommand<string>(DeleteSourceCommandExecute);

        private void DeleteSourceCommandExecute(string filterName)
        {
            var sources = ModifiedConfigNode.TracingConfig.TraceMonitoring.SwitchedOffTraceSources.ToList();
            sources.Remove(filterName);
            ModifiedConfigNode.TracingConfig.TraceMonitoring.SwitchedOffTraceSources = sources.ToArray();
            UpdateSourceCollection();
        }

        #endregion

        #region Filters

        private ICommand _addFilterCommand;

        public ICommand AddFilterCommand => _addFilterCommand ??= new DelegateCommand(AddFilterCommandExecute);

        private void AddFilterCommandExecute()
        {
            UpdatePossibleSourceCollection();
            var filterEditor = new FilterEditorPopupContent(null, PossibleSources.ToList().AsReadOnly());
            Popups.Show(
                new Popup(nameof(DiagnosticPanelResources.DIAGNOSTICPANEL_FILTER_EDITOR))
                {
                    Message = new InvariantText(""),
                    Content = filterEditor,
                    Commands =
                    {
                        new PopupCommand(nameof(DiagnosticPanelResources.DIAGNOSTICPANEL_CANCEL)),
                        new PopupCommand(
                            nameof(DiagnosticPanelResources.DIAGNOSTICPANEL_ADD),
                            new DelegateCommand(
                                () =>
                                {
                                    filterEditor.SaveFilter();
                                    var filters = ModifiedConfigNode.DataLogFilters.Filters.ToList();
                                    filters.Add(filterEditor.Filter);
                                    ModifiedConfigNode.DataLogFilters.Filters = filters.ToArray();
                                    UpdateFilterCollection(filterEditor.Filter.Name);
                                }))
                    }
                });
        }

        private ICommand _editFilterCommand;

        public ICommand EditFilterCommand
            => _editFilterCommand ??= new DelegateCommand<IFilter>(EditFilterCommandExecute);

        private void EditFilterCommandExecute(IFilter filter)
        {
            UpdatePossibleSourceCollection();
            var filterEditor = new FilterEditorPopupContent(filter, PossibleSources.ToList().AsReadOnly());
            Popups.Show(
                new Popup(nameof(DiagnosticPanelResources.DIAGNOSTICPANEL_FILTER_EDITOR))
                {
                    Message = new InvariantText(""),
                    Content = filterEditor,
                    Commands =
                    {
                        new PopupCommand(nameof(DiagnosticPanelResources.DIAGNOSTICPANEL_CANCEL)),
                        new PopupCommand(
                            nameof(DiagnosticPanelResources.DIAGNOSTICPANEL_SAVE),
                            new DelegateCommand(
                                () =>
                                {
                                    filterEditor.SaveFilter();
                                    //Replace the filter instance
                                    var filters = ModifiedConfigNode.DataLogFilters.Filters.ToList();
                                    filters[filters.IndexOf(filter)] = filterEditor.Filter;
                                    //[TLa] Instance assignment does not take into account the source collection
                                    //The source collection is not deserialized (only the selected sources are deserialized)
                                    ModifiedConfigNode.DataLogFilters.Filters = filters.ToArray();
                                    UpdateFilterCollection(filterEditor.Filter.Name);
                                }))
                    }
                });
        }

        private ICommand _deleteFilterCommand;

        public ICommand DeleteFilterCommand
            => _deleteFilterCommand ??= new DelegateCommand<IFilter>(DeleteFilterCommandExecute);

        private void DeleteFilterCommandExecute(IFilter filter)
        {
            var filters = ModifiedConfigNode.DataLogFilters.Filters.ToList();
            filters.Remove(filter);
            ModifiedConfigNode.DataLogFilters.Filters = filters.ToArray();
            UpdateFilterCollection(null);
        }

        #endregion

        #region Storage

        private ICommand _openFilePathCommand;

        public ICommand OpenFilePathCommand => _openFilePathCommand ??= new DelegateCommand(OpenFilePathCommandExecute);

        private void OpenFilePathCommandExecute() => ShowOpenFolderDialog<DiagnosticPanel>(p => p.TracesPath);

        private ICommand _openArchivingPathCommand;

        public ICommand OpenArchivingPathCommand
            => _openArchivingPathCommand ??= new DelegateCommand(OpenArchivingPathCommandExecute);

        private void OpenArchivingPathCommandExecute() => ShowOpenFolderDialog<DiagnosticPanel>(p => p.ArchivePath);

        #endregion

        #region Support Request

        private ICommand _addSupportRequestFileFolderCommand;

        public ICommand AddSupportRequestFileFolderCommand
            => _addSupportRequestFileFolderCommand ??= new DelegateCommand(
                AddSupportRequestFileFolderCommandExecute,
                AddSupportRequestFileFolderCommandCanExecute);

        private bool AddSupportRequestFileFolderCommandCanExecute()
            => !string.IsNullOrWhiteSpace(CurrentSupportRequestFileFolder)
               && (File.Exists(CurrentSupportRequestFileFolder) || Directory.Exists(CurrentSupportRequestFileFolder));

        private void AddSupportRequestFileFolderCommandExecute()
        {
            ModifiedConfigNode.FilesOrFolders.Add(CurrentSupportRequestFileFolder);
            SupportRequestFileFolderCollection.Refresh();
            CurrentSupportRequestFileFolder = string.Empty;
        }

        private ICommand _deleteSupportRequestFileFolderCommand;

        public ICommand DeleteSupportRequestFileFolderCommand
            => _deleteSupportRequestFileFolderCommand ??=
                new DelegateCommand<string>(DeleteSupportRequestFileFolderCommandExecute);

        private void DeleteSupportRequestFileFolderCommandExecute(string fileFolder)
        {
            ModifiedConfigNode.FilesOrFolders.Remove(fileFolder);
            SupportRequestFileFolderCollection.Refresh();
        }

        private ICommand _supportRequestOpenFileCommand;

        public ICommand SupportRequestOpenFileCommand
            => _supportRequestOpenFileCommand ??= new DelegateCommand(SupportRequestOpenFileCommandExecute);

        private void SupportRequestOpenFileCommandExecute()
        {
            if (ShowOpenFileDialog<DiagnosticPanel>(p => p.CurrentSupportRequestFileFolder, "All files (*.*)|*.*")
                && !string.IsNullOrWhiteSpace(CurrentSupportRequestFileFolder))
            {
                AddSupportRequestFileFolderCommand.Execute(null);
            }
        }

        private ICommand _supportRequestOpenFolderCommand;

        public ICommand SupportRequestOpenFolderCommand
            => _supportRequestOpenFolderCommand ??= new DelegateCommand(SupportRequestOpenFolderCommandExecute);

        private void SupportRequestOpenFolderCommandExecute()
        {
            if (ShowOpenFolderDialog<DiagnosticPanel>(p => p.CurrentSupportRequestFileFolder)
                && !string.IsNullOrWhiteSpace(CurrentSupportRequestFileFolder))
            {
                AddSupportRequestFileFolderCommand.Execute(null);
            }
        }

        private ICommand _supportRequestExportPathCommand;

        public ICommand SupportRequestExportPathCommand
            => _supportRequestExportPathCommand ??= new DelegateCommand(SupportRequestExportPathCommandExecute);

        private void SupportRequestExportPathCommandExecute()
            => ShowOpenFolderDialog<DiagnosticPanel>(p => p.ExportPath);

        #endregion

        #endregion

        private void GetConfigValues()
        {
            OnPropertyChanged(nameof(ModifiedConfigNode));
            UpdateSourceCollection();
            UpdateFilterCollection(null);
            UpdatePossibleSourceCollection();
            SupportRequestFileFolderCollection = CollectionViewSource.GetDefaultView(ModifiedConfigNode.FilesOrFolders);
            OnPropertyChanged(null);
        }

        private void UpdateSourceCollection()
        {
            Sources = ModifiedConfigNode.TracingConfig.TraceMonitoring.SwitchedOffTraceSources.ToList().AsReadOnly();
        }

        private void UpdateFilterCollection(string selectedFilterName)
        {
            Filters = ModifiedConfigNode.DataLogFilters.Filters.ToList().AsReadOnly();
            if (string.IsNullOrEmpty(selectedFilterName))
            {
                return;
            }

            var filter = Filters.SingleOrDefault(f => f.Name.Equals(selectedFilterName));
            SelectedFilter = filter;
        }

        private void UpdatePossibleSourceCollection()
        {
            var possibleSource = new List<string>();
            if (_bufferListener != null)
            {
                possibleSource.AddRange(_bufferListener.Lines.Select(line => line.Source).Distinct());
            }

            var sources =
                ModifiedConfigNode.DataLogFilters.Filters?.OfType<TracerFilter>()
                    .Where(f => f.Source.Items != null)
                    .SelectMany(f => f.Source.Items)
                ?? Enumerable.Empty<string>();

            foreach (var source in sources)
            {
                if (!possibleSource.Any(s => s.Equals(source)))
                {
                    possibleSource.Add(source);
                }
            }

            PossibleSources = possibleSource.AsReadOnly();
        }

        #region Override

        protected override bool ChangesNeedRestart => true;

        protected override void UndoChanges()
        {
            base.UndoChanges();
            GetConfigValues();
        }

        public override void OnSetup()
        {
            base.OnSetup();
            if (IsInDesignMode)
            {
                return;
            }

            var traceManager = App.Instance.Tracer as TraceManager;
            _bufferListener = traceManager?.GetListener("BufferListener") as BufferListener;

            GetConfigValues();
        }

        protected override Diagnostics GetNode(ApplicationConfiguration applicationConfiguration)
            => applicationConfiguration?.Diagnostics;

        protected override bool ValidateConfiguration()
        {
            if (!base.ValidateConfiguration())
            {
                return false;
            }

            if (ModifiedConfigNode.TracingConfig.Restrictions.TraceLineMaxLength is < 10 or > uint.MaxValue
                || ModifiedConfigNode.TracingConfig.Restrictions.NumberOfTraceFiles is < 3 or > 512
                || ModifiedConfigNode.TracingConfig.Restrictions.TraceFileMaxSize is < 16 or > 100000
                || ModifiedConfigNode.TracingConfig.Restrictions.NumberOfArchiveFiles is < 1 or > 512
                || ModifiedConfigNode.TracingConfig.Restrictions.ArchiveFileMaxSize is < 16 or > 100000
                || ModifiedConfigNode.DataLogMaxItemCount <= 0)
            {
                return false;
            }

            return true;
        }

        private void SetupRules()
        {
            Rules.Add(
                new DelegateRule(
                    nameof(TracesPath),
                    () =>
                    {
                        if (string.IsNullOrWhiteSpace(TracesPath) || !Directory.Exists(TracesPath))
                        {
                            return LocalizationManager.GetString(
                                nameof(SetupPanelResources.SETUP_MESSAGE_TRACE_PATH_WARNING));
                        }

                        return string.Empty;
                    }));

            Rules.Add(
                new DelegateRule(
                    nameof(ArchivePath),
                    () =>
                    {
                        if (string.IsNullOrWhiteSpace(ArchivePath) || !Directory.Exists(ArchivePath))
                        {
                            return LocalizationManager.GetString(
                                nameof(SetupPanelResources.SETUP_MESSAGE_ARCHIVE_PATH_WARNING));
                        }

                        return string.Empty;
                    }));

            Rules.Add(
                new DelegateRule(
                    nameof(ExportPath),
                    () =>
                    {
                        if (string.IsNullOrWhiteSpace(ExportPath))
                        {
                            return LocalizationManager.GetString(
                                nameof(SetupPanelResources.SETUP_MESSAGE_EXPORT_PATH_WARNING));
                        }

                        return string.Empty;
                    }));
        }

        #endregion
    }
}
