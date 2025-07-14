using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;

using Agileo.Common.Configuration;
using Agileo.Common.Localization;
using Agileo.Common.Tracing;
using Agileo.Common.Tracing.Filters;
using Agileo.Common.Tracing.Listeners;
using Agileo.GUI.Commands;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;

using UnitySC.GUI.Common.Vendor.Views.Panels.Diagnostic.TraceViewer;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Filters;
using UnitySC.GUI.Common.Vendor.UIComponents.GuiExtended;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Diagnostic.TraceViewer
{
    public class TraceViewerPanel : BusinessPanel
    {
        #region Fields

        private BufferListener _bufferListener;

        private readonly Timer _refreshFiltersTimer;

        private bool _needToUpdateFilters;

        #region Filters

        private readonly FilterCollection<TraceLine, string> _sourceFilter;
        private readonly FilterCollection<TraceLine, TraceLevelType> _levelFilter;

        #endregion

        #endregion Fields

        #region Constructor

        static TraceViewerPanel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(TraceViewerResources)));
        }

        public TraceViewerPanel(string relativeId, IIcon icon = null) : base(relativeId, icon)
        {
            #region Filters

            _sourceFilter = new FilterCollection<TraceLine, string>(
                nameof(TraceViewerResources.TRACEVIEWER_SOURCE),
                () => DataTableSource.Select(occ => occ.Source).Distinct().ToList(),
                occurrence => occurrence.Source);

            DataTableSource.Filter.Add(_sourceFilter);
            _levelFilter = DataTableSource.Filter.AddEnumFilter(nameof(TraceViewerResources.TRACEVIEWER_LEVEL), trace => trace.LogLevel);
            DataTableSource.Filter.Add(new FilterPeriod<TraceLine>(nameof(TraceViewerResources.TRACEVIEWER_DATETIME), trace => trace.Timestamp.DateTime)
            {
                UseHoursMinutesSeconds = true
            });

            #endregion

            #region Search

            DataTableSource.Search.AddSearchDefinition(nameof(TraceViewerResources.TRACEVIEWER_DESCRIPTION), trace => trace.Text, true);
            DataTableSource.Search.AddSearchDefinition(nameof(TraceViewerResources.TRACEVIEWER_SOURCE), trace => trace.Source, true);

            #endregion

            #region Commands

            Commands.Add(new BusinessPanelToggleCommand(nameof(TraceViewerResources.TRACEVIEWER_PAUSE_RESUME),
                new BusinessPanelCommand(nameof(TraceViewerResources.TRACEVIEWER_RESUME), ResumeCommand, PathIcon.Play),
                new BusinessPanelCommand(nameof(TraceViewerResources.TRACEVIEWER_PAUSE), PauseCommand, PathIcon.Pause)));
            Commands.Add(new BusinessPanelCommand(nameof(TraceViewerResources.TRACEVIEWER_CLEAR_ALL), ClearAllCommand, PathIcon.Delete));

            ScrollToDownCommand = new BusinessPanelCheckToggleCommand(nameof(TraceViewerResources.TRACEVIEWER_SCROLL_TO_END), () => {}, () => { }, PathIcon.ScrollToEnd)
            {
                IsChecked = true
            };
            Commands.Add(ScrollToDownCommand);

            #endregion

            _refreshFiltersTimer = new Timer(RefreshFilters, null, 1000, 2000);
        }

        #endregion Constructor

        #region Properties

        public RealTimeDataTableSource<TraceLine> DataTableSource { get; } = new RealTimeDataTableSource<TraceLine>()
        {
            MaxItemNumber = 5000
        };

        private TraceLine _selectedTraceLine;

        public TraceLine SelectedTraceLine
        {
            get => _selectedTraceLine;
            set
            {
                SetAndRaiseIfChanged(ref _selectedTraceLine, value);
                OnPropertyChanged(nameof(SelectedAttachment));
                AttachmentIsExpanded = HasAttachment(value);
            }
        }

        public string SelectedAttachment
        {
            get
            {
                var trace = SelectedTraceLine;

                if (trace?.TraceParamLine == null)
                {
                    return string.Empty;
                }

                if (!string.IsNullOrEmpty(trace.TraceParamLine.StringAttachment))
                {
                    return trace.TraceParamLine.StringAttachment;
                }

                if (trace.TraceParamLine.ExceptionAttachment != null)
                {
                    return trace.TraceParamLine.ExceptionAttachment.ToString();
                }

                if (trace.TraceParamLine.Attachment != null)
                {
                    if (trace.TraceParamLine.Attachment is string stringAttachment)
                    {
                        return stringAttachment;
                    }

                    return trace.TraceParamLine.Attachment.ToString();
                }

                return string.Empty;
            }
        }

        private bool _attachmentIsExpanded;

        public bool AttachmentIsExpanded
        {
            get => _attachmentIsExpanded;
            set
            {
                SetAndRaiseIfChanged(ref _attachmentIsExpanded, value);
                if (!value) AttachmentIsFullScreen = false;
            }
        }

        public BusinessPanelCheckToggleCommand ScrollToDownCommand { get; }

        public ObservableCollection<TracerFilter> PredefinedFilters { get; } = new();

        private bool _attachmentIsFullScreen;

        public bool AttachmentIsFullScreen
        {
            get { return _attachmentIsFullScreen; }
            set { SetAndRaiseIfChanged(ref _attachmentIsFullScreen, value); }
        }

        #endregion Properties

        #region Commands

        #region Pause

        private DelegateCommand _pauseCommand;

        /// <summary>
        /// Pauses the real-time update of the collection. Added items will be buffered.
        /// </summary>
        public DelegateCommand PauseCommand => _pauseCommand ?? (_pauseCommand = new DelegateCommand(PauseCommandExecute, PauseCommandCanExecute));

        private bool PauseCommandCanExecute() => !DataTableSource.IsPaused;

        private void PauseCommandExecute()
        {
            lock (DataTableSource)
            {
                DataTableSource.Pause();
            }
        }

        #endregion

        #region Resume

        private DelegateCommand _resumeCommand;

        /// <summary>
        /// Resumes real-time updating of the collection. Buffered items will be added to the collection.
        /// </summary>
        public DelegateCommand ResumeCommand => _resumeCommand ?? (_resumeCommand = new DelegateCommand(ResumeCommandExecute, ResumeCommandCanExecute));

        private bool ResumeCommandCanExecute() => DataTableSource.IsPaused;

        private void ResumeCommandExecute()
        {
            lock (DataTableSource)
            {
                DataTableSource.Resume();
                _needToUpdateFilters = true;
            }
        }

        #endregion

        #region Clear All

        private DelegateCommand _clearAllCommand;

        /// <summary>
        /// Opens a popup to the user allowing to clear all traces. Also empties the buffers.
        /// </summary>
        public DelegateCommand ClearAllCommand => _clearAllCommand ?? (_clearAllCommand = new DelegateCommand(ClearAllCommandExecute));

        private void ClearAllCommandExecute()
        {
            var clearAllPopup = new Popup(nameof(TraceViewerResources.TRACEVIEWER_CLEAR_ALL))
            {
                Commands =
                {
                    new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)),
                    new PopupCommand(nameof(TraceViewerResources.TRACEVIEWER_CLEAR_ALL), new DelegateCommand(
                        () =>
                        {
                            lock (DataTableSource)
                            {
                                DataTableSource.Clear();
                                DataTableSource.UpdateFilterPossibleValues();
                            }
                        }))
                }
            };
            Popups.Show(clearAllPopup);
        }

        #endregion

        #region Copy Clipboard

        private DelegateCommand _copyToClipboardCommand;

        /// <summary>
        /// Copies the attachment of the selected trace to the clipboard.
        /// </summary>
        public DelegateCommand CopyToClipboardCommand => _copyToClipboardCommand ?? (_copyToClipboardCommand = new DelegateCommand(CopyToClipboardCommandExecute, CopyToClipboardCommandCanExecute));

        private bool CopyToClipboardCommandCanExecute()
        {
            return HasAttachment(SelectedTraceLine);
        }

        private void CopyToClipboardCommandExecute()
        {
            var attachment = SelectedTraceLine?.TraceParamLine?.StringAttachment;
            if (!string.IsNullOrWhiteSpace(attachment))
            {
                Clipboard.SetText(attachment);
                Messages.Show(new UserMessage(MessageLevel.Info, nameof(TraceViewerResources.TRACEVIEWER_COPIED_TO_CLIPBOARD))
                {
                    SecondsDuration = 2
                });
            }
        }

        #endregion

        #region Apply Predefined Filter

        private DelegateCommand<TracerFilter> _applyPredifinedFilerCommand;

        /// <summary>
        /// Applies a predefined filter from the settings.
        /// </summary>
        public DelegateCommand<TracerFilter> ApplyPredifinedFilerCommand => _applyPredifinedFilerCommand ?? (_applyPredifinedFilerCommand = new DelegateCommand<TracerFilter>(ApplyPredifinedFilerCommandExecute, ApplyPredifinedFilerCommandCanExecute));

        private static bool ApplyPredifinedFilerCommandCanExecute(TracerFilter traceFilter) => traceFilter != null;

        private void ApplyPredifinedFilerCommandExecute(TracerFilter traceFilter)
        {
            if (traceFilter != null)
            {
                _sourceFilter.SelectedItems.Clear();
                foreach (var source in traceFilter.Source.SelectedItems)
                {
                    _sourceFilter.SelectedItems.Add(source);
                }

                _levelFilter.SelectedItems.Clear();
                foreach (var level in traceFilter.Level.SelectedItems)
                {
                    if (Enum.TryParse(level, out TraceLevelType levelType))
                    {
                        _levelFilter.SelectedItems.Add(levelType);
                    }
                }
            }
        }

        #endregion

        #region Enter/Exit Attachment FullScreen

        private ICommand _enterAttachmentFullScreenCommand;

        public ICommand EnterAttachmentFullScreenCommand => _enterAttachmentFullScreenCommand ?? (_enterAttachmentFullScreenCommand = new DelegateCommand(EnterAttachmentFullScreenCommandExecute, EnterAttachmentFullScreenCommandCanExecute));

        private bool EnterAttachmentFullScreenCommandCanExecute() => !AttachmentIsFullScreen;

        private void EnterAttachmentFullScreenCommandExecute() => AttachmentIsFullScreen = true;

        private ICommand _exitAttachmentFullScreenCommand;

        public ICommand ExitAttachmentFullScreenCommand => _exitAttachmentFullScreenCommand ?? (_exitAttachmentFullScreenCommand = new DelegateCommand(ExitAttachmentFullScreenCommandExecute, ExitAttachmentFullScreenCommandCanExecute));

        private bool ExitAttachmentFullScreenCommandCanExecute() => AttachmentIsFullScreen;

        private void ExitAttachmentFullScreenCommandExecute() => AttachmentIsFullScreen = false;

        #endregion

        #endregion

        #region Private Methods

        private void RefreshFilters(object _)
        {
            if (!_needToUpdateFilters) return;

            lock (DataTableSource)
            {
                DataTableSource.UpdateFilterPossibleValues();
                _needToUpdateFilters = false;
            }
        }

        private void ApplyConfiguration()
        {
            var diagnosticsConfiguration = App.Instance?.Config?.Diagnostics;
            if (diagnosticsConfiguration == null)
                return;

            DataTableSource.MaxItemNumber = (int)diagnosticsConfiguration.DataLogMaxItemCount;
            PredefinedFilters.Clear();
            foreach (var filter in diagnosticsConfiguration.DataLogFilters.Filters.OfType<TracerFilter>())
            {
                PredefinedFilters.Add(filter);
            }
        }

        private void ConfigManager_OnCurrentChanged(object sender, ConfigurationChangedEventArgs e)
            => DispatcherHelper.DoInUiThreadAsynchronously(ApplyConfiguration);

        private void AddTrace(TraceLine traceLine)
        {
            lock (DataTableSource)
            {
                DataTableSource.Add(traceLine);
                _needToUpdateFilters = true;
            }
        }

        private static bool HasAttachment(TraceLine traceLine) => traceLine?.TraceParamLine != null;

        #endregion

        #region Overrides of IdentifiableElement

        public override void OnSetup()
        {
            base.OnSetup();

            ApplyConfiguration();

            App.Instance.ConfigurationManager.CurrentChanged += ConfigManager_OnCurrentChanged;

            var tracer = (TraceManager)App.Instance.Tracer;
            _bufferListener = (BufferListener)(dynamic)tracer.Listeners["BufferListener"];

            if (_bufferListener == null)
                return;

            foreach (var traceLine in _bufferListener.Lines)
            {
                AddTrace(traceLine);
            }

            _bufferListener.LineAdding += AddTrace;
        }

        protected override void Dispose(bool disposing)
        {
            App.Instance.ConfigurationManager.CurrentChanged -= ConfigManager_OnCurrentChanged;

            _bufferListener.LineAdding -= AddTrace;
            _refreshFiltersTimer.Dispose();

            DataTableSource.Dispose();

            base.Dispose(disposing);
        }

        #endregion
    }
}
