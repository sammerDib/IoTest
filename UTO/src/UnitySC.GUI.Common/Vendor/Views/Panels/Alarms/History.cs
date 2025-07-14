using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Agileo.AlarmModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Filters;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Search;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Alarms
{
    public class HistoryDataTableSource : DataTableSource<AlarmOccurrence>
    {
        private readonly IAlarmCenter _alarmCenter;

        private List<string> _possibleProviderNames;
        private List<string> _possibleAcknowledgedNames;

        private readonly FilterPeriod<AlarmOccurrence> _setTimePeriodFilter;
        private readonly FilterCollection<AlarmOccurrence, string> _sourceFilter;
        private readonly FilterCollection<AlarmOccurrence, AlarmState> _stateFilter;
        private readonly FilterCollection<AlarmOccurrence, bool> _acknowledgedFilter;
        private readonly FilterPeriod<AlarmOccurrence> _ackTimePeriodFilter;
        private readonly FilterCollection<AlarmOccurrence, string> _ackByFilter;
        private readonly FilterPeriod<AlarmOccurrence> _clearedTimePeriodFilter;

        public HistoryDataTableSource(IAlarmCenter alarmCenter)
        {
            _alarmCenter = alarmCenter;
            _setTimePeriodFilter = new FilterPeriod<AlarmOccurrence>(nameof(AlarmsResources.ALARMS_SET_TIME_PERIODE), occurrence => occurrence.SetTimeStamp);

            _sourceFilter = new FilterCollection<AlarmOccurrence, string>(nameof(AlarmsResources.ALARMS_SOURCE),
                () => _possibleProviderNames, null);

            _stateFilter = new FilterCollection<AlarmOccurrence, AlarmState>(nameof(AlarmsResources.ALARMS_STATE),
                () => new List<AlarmState> { AlarmState.Cleared, AlarmState.Set }, null);

            _acknowledgedFilter = new FilterCollection<AlarmOccurrence, bool>(nameof(AlarmsResources.ALARMS_ACKNOWLEDGED),
                () => new List<bool> { true, false }, null);

            _ackTimePeriodFilter = new FilterPeriod<AlarmOccurrence>(nameof(AlarmsResources.ALARMS_ACKNOWLEDGED_TIME_PERIOD), occurrence => occurrence.AcknowledgedTimeStamp);

            _ackByFilter = new FilterCollection<AlarmOccurrence, string>(nameof(AlarmsResources.ALARMS_ACKNOWLEDGED_BY),
                () => _possibleAcknowledgedNames, null);

            _clearedTimePeriodFilter = new FilterPeriod<AlarmOccurrence>(nameof(AlarmsResources.ALARMS_CLEARED_TIME_PERIOD), occurrence => occurrence.ClearedTimeStamp);

            Filter.Add(_stateFilter);
            Filter.Add(_setTimePeriodFilter);
            Filter.Add(_clearedTimePeriodFilter);
            Filter.Add(_sourceFilter);
            Filter.Add(_acknowledgedFilter);
            Filter.Add(_ackTimePeriodFilter);
            Filter.Add(_ackByFilter);
        }

        #region Overrides of DataTableSource<AlarmOccurrence>

        public override void UpdateCollection()
        {
            var allOccurrences = _alarmCenter.Repository.GetAlarmOccurrences().ToList();
            _possibleProviderNames = allOccurrences.Select(occurrence => occurrence.Alarm.ProviderName).Distinct().ToList();
            _possibleAcknowledgedNames = allOccurrences.Where(occ => !string.IsNullOrWhiteSpace(occ.AcknowledgedBy)).Select(occ => occ.AcknowledgedBy).Distinct().ToList();

            // Filters are used as parameters of the alarmCenter
            // So it is necessary to filter the collection before applying the internal logic of the DataTableSource
            var providerNameCollection = _sourceFilter.SelectedItems.Any() ? _sourceFilter.SelectedItems : null;
            var state = _stateFilter.SelectedItems.Count == 1 ? _stateFilter.SelectedItems.Single() : (AlarmState?)null;
            var startDate = _setTimePeriodFilter.StartDateUsed ? _setTimePeriodFilter.StartDate : (DateTime?)null;
            var endDate = _setTimePeriodFilter.EndDateUsed ? _setTimePeriodFilter.EndDate : (DateTime?)null;
            var acknowledged = _acknowledgedFilter.SelectedItems.Count == 1 ? _acknowledgedFilter.SelectedItems.Single() : (bool?)null;
            var startAcknowledgementDate = _ackTimePeriodFilter.StartDateUsed ? _ackTimePeriodFilter.StartDate : (DateTime?)null;
            var endAcknowledgementDate = _ackTimePeriodFilter.EndDateUsed ? _ackTimePeriodFilter.EndDate : (DateTime?)null;

            var acknowledgedByList = _ackByFilter.SelectedItems;
            var startClearedDate = _clearedTimePeriodFilter.StartDateUsed ? _clearedTimePeriodFilter.StartDate : (DateTime?)null;
            var endClearedDate = _clearedTimePeriodFilter.EndDateUsed ? _clearedTimePeriodFilter.EndDate : (DateTime?)null;

            SourceList.Clear();
            SourceList.AddRange(_alarmCenter.Repository.GetAlarmOccurrences(providerNameCollection, state, startDate, endDate, acknowledged, startAcknowledgementDate, endAcknowledgementDate, acknowledgedByList, startClearedDate, endClearedDate));

            base.UpdateCollection();
        }

        protected override IEnumerable<AlarmOccurrence> ApplyFilter(IEnumerable<AlarmOccurrence> sortedCollection)
        {
            // The filters have already been applied by the alarm center API
            return sortedCollection;
        }

        #endregion
    }

    public class History : BaseAlarmViewer
    {
        protected override IDataTableSource DataTableSource => Alarms;

        public HistoryDataTableSource Alarms { get; }

        private readonly UserMessage _refreshUserMessage;

        #region Bindable Properties

        private AlarmOccurrence _selectedAlarm;

        public AlarmOccurrence SelectedAlarm
        {
            get { return _selectedAlarm; }
            set
            {
                if (_selectedAlarm == value) return;
                _selectedAlarm = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public History() : this(null, $"{nameof(History)} DesignTime Constructor")
        {
        }

        public History(IAlarmCenter alarmCenter) : this(alarmCenter, nameof(AlarmsResources.ALARMS_HISTORY), PathIcon.AlarmHistory)
        {
        }

        public History(IAlarmCenter alarmCenter, string relativeId, IIcon icon = null) : base(alarmCenter, relativeId, icon)
        {
            _refreshUserMessage = new UserMessage(MessageLevel.Info, nameof(AlarmsResources.ALARMS_REFRESH_MESSAGE));
            _refreshUserMessage.Commands.Add(new UserMessageCommand(nameof(AlarmsResources.ALARMS_RELOAD), new DelegateCommand(Refresh), PathIcon.Refresh)
            {
                CloseMessageAfterExecute = true
            });
            _refreshUserMessage.CanUserCloseMessage = true;

            Alarms = new HistoryDataTableSource(AlarmCenter);

            #region Sorts

            //Set default sorting
            Alarms.Sort.SetCurrentSorting(nameof(AlarmOccurrence.SetTimeStamp), ListSortDirection.Descending);

            #endregion

            Alarms.Search.AddSearchDefinition(new SearchDefinition<AlarmOccurrence>(nameof(AlarmsResources.ALARMS_TEXT), occurrence => occurrence.Text));
            Alarms.Search.AddSearchDefinition(new SearchDefinition<AlarmOccurrence>(nameof(AlarmsResources.ALARMS_ENGLISH_TEXT), occurrence => occurrence.EnglishText));

            #region Commands

            Commands.Insert(0, new BusinessPanelCommand(nameof(AlarmsResources.ALARMS_RELOAD), new DelegateCommand(Refresh), PathIcon.Refresh));

            #endregion
        }

        #region Commands

        #region Csv

        protected override bool ExportCsvCommandCanExecute() => Alarms.SourceView != null && Alarms.SourceView.Count > 0;

        protected override void ExportCsvCommandExecute() => ExportToCsv(Alarms.SourceView, "AlarmHistory.csv");

        #endregion

        #endregion

        public override void OnSetup()
        {
            AlarmCenter.ModelBuilder.AlarmsChanged += ModelBuilderOnAlarmsChanged;
            Refresh();
            base.OnSetup();
        }

        public override void OnShow()
        {
            base.OnShow();
            Refresh();
        }

        protected override void Dispose(bool disposing)
        {
            AlarmCenter.ModelBuilder.AlarmsChanged -= ModelBuilderOnAlarmsChanged;
            base.Dispose(disposing);
        }

        private void ModelBuilderOnAlarmsChanged(object sender, EventArgs e)
        {
            Messages.Show(_refreshUserMessage);
        }

        private void Refresh()
        {
            Messages.Hide(_refreshUserMessage);
            Alarms.UpdateCollection();
        }
    }
}
