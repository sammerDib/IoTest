using System;
using System.Linq;
using System.Threading;
using System.Windows;

using Agileo.Common.Localization;
using Agileo.Common.Tracing;
using Agileo.Common.Tracing.Listeners;
using Agileo.GUI.Commands;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;

using UnitySC.GUI.Common;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Filters;
using UnitySC.GUI.Common.Vendor.UIComponents.GuiExtended;
using UnitySC.GUI.Common.Vendor.Views.Panels.Diagnostic.TraceViewer;

namespace UnitySC.EFEM.Rorze.GUI.Views.Panels.Maintenance.Communication
{
    public class CommunicationVisualizerPanel : BusinessPanel
    {
        #region Fields

        private bool _needToUpdateFilters;

        private readonly Timer _refreshFiltersTimer;

        private BufferListener _bufferListener;

        #endregion Fields

        #region Constructors & Cleaners

        static CommunicationVisualizerPanel()
        {
            DataTemplateGenerator.Create(typeof(CommunicationVisualizerPanel), typeof(CommunicationVisualizerPanelView));
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(CommunicationTraceResources)));
        }

        public CommunicationVisualizerPanel(string relativeId, IIcon icon = null) : base(relativeId, icon)
        {
            DataTableSource.MaxItemNumber = 10000;

            DataTableSource.Filter.Add(
                new FilterCollection<CommunicationTrace, string>(
                    nameof(CommunicationTraceResources.MSG_CORRESPONDENT),
                    () => DataTableSource.Select(occ => occ.Correspondent).Distinct().ToList(),
                    occurrence => occurrence.Correspondent));
            DataTableSource.Filter.AddEnumFilter(nameof(CommunicationTraceResources.MSG_DIRECTION), trace => trace.Direction);
            DataTableSource.Filter.Add(new FilterPeriod<CommunicationTrace>(nameof(CommunicationTraceResources.MSG_DATE), trace => trace.Date)
            {
                UseHoursMinutesSeconds = true
            });

            DataTableSource.Search.AddSearchDefinition(nameof(CommunicationTraceResources.MSG_CORRESPONDENT), trace => trace.Correspondent);
            DataTableSource.Search.AddSearchDefinition(nameof(CommunicationTraceResources.MSG_DIRECTION), trace => trace.Direction.ToString());
            DataTableSource.Search.AddSearchDefinition(nameof(CommunicationTraceResources.MSG_CONTENT), trace => trace.Content);

            Commands.Add(new BusinessPanelToggleCommand(nameof(TraceViewerResources.TRACEVIEWER_PAUSE_RESUME),
                new BusinessPanelCommand(nameof(TraceViewerResources.TRACEVIEWER_RESUME), ResumeCommand, IconFactory.PathGeometryFromRessourceKey("PlayIcon")),
                new BusinessPanelCommand(nameof(TraceViewerResources.TRACEVIEWER_PAUSE), PauseCommand, IconFactory.PathGeometryFromRessourceKey("PauseIcon"))));
            Commands.Add(new BusinessPanelCommand(nameof(TraceViewerResources.TRACEVIEWER_CLEAR_ALL), ClearAllCommand, IconFactory.PathGeometryFromRessourceKey("DeleteIcon")));

            ScrollToDownCommand = new BusinessPanelCheckToggleCommand(nameof(TraceViewerResources.TRACEVIEWER_SCROLL_TO_END), () => { }, () => { }, IconFactory.PathGeometryFromRessourceKey("ScrollToEndIcon"))
            {
                IsChecked = true
            };
            Commands.Add(ScrollToDownCommand);

            _refreshFiltersTimer = new Timer(RefreshFilters, null, 500, 500);

            CopyContentCommand = new DelegateCommand<CommunicationTrace>(trace =>
            {
                if (trace != null)
                {
                    Clipboard.SetText(trace.Content);
                }
            });
        }

        #endregion Constructors & Cleaners

        #region Properties

        public RealTimeDataTableSource<CommunicationTrace> DataTableSource { get; } =
            new RealTimeDataTableSource<CommunicationTrace>();

        #endregion Properties

        #region Data Management

        private void AddMessage(DateTime date, bool isSendMessage, string message, string interlocutorId = null)
        {
            var trace = new CommunicationTrace(
                date,
                interlocutorId ?? "???",
                isSendMessage ? CommunicationTrace.MessageDirection.Sent : CommunicationTrace.MessageDirection.Received,
                message.Replace("\r", ""));

            lock (DataTableSource)
            {
                DataTableSource.Add(trace);
                _needToUpdateFilters = true;
            }
        }

        private void RefreshFilters(object _)
        {
            if (!_needToUpdateFilters) return;

            lock (DataTableSource)
            {
                DataTableSource.UpdateFilterPossibleValues();
                _needToUpdateFilters = false;
            }
        }

        #endregion Data Management

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

        #endregion Pause

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

        #endregion Resume

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

        #endregion Clear All

        public DelegateCommand<CommunicationTrace> CopyContentCommand { get; }

        public BusinessPanelCheckToggleCommand ScrollToDownCommand { get; }

        #endregion Commands

        #region Methods

        public void AddTrace(TraceLine traceLine)
        {
            // We consider only communication logs
            if (!traceLine.Source.StartsWith("Com"))
                return;

            // Get date from trace
            var date = traceLine.Timestamp.DateTime;

            // Trace message is marked as "[SENT]" when message has been sent by EFEM Controller, and by "[RECV]" if received.
            var isSend = traceLine.Text.Contains("[SENT]");

            // Remove the [SENT] or [RECV] tag and the space character after.
            var message = traceLine.Text.Replace("[SENT] ", "").Replace("[RECV] ", "");

            // Remove the "Com - " from correspondent ID
            var correspondent = traceLine.Source.Replace("Com - ", "");

            AddMessage(date, isSend, message, correspondent);
        }

        public string Name { get; set; } = nameof(CommunicationVisualizerPanel);

        #endregion Methods

        #region IdentifiableElement

        public override void OnSetup()
        {
            base.OnSetup();

            var tracer = (TraceManager)App.Instance.Tracer;
            _bufferListener = (BufferListener)(dynamic)tracer.Listeners["BufferListener"];

            foreach (var traceLine in _bufferListener.Lines)
            {
                AddTrace(traceLine);
            }

            _bufferListener.LineAdding += AddTrace;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _bufferListener.LineAdding -= AddTrace;
                _refreshFiltersTimer.Dispose();

                DataTableSource.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion IdentifiableElement
    }
}
