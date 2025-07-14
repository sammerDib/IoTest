using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Agileo.AlarmModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.Saliences;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Filters;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Alarms
{
    public class Actives : BaseAlarmViewer
    {
        protected override IDataTableSource DataTableSource => Alarms;

        public static string NameofMuteBuzzer => nameof(AlarmsResources.ALARMS_MUTE_BUZZER);

        public DataTableSource<AlarmOccurrence> Alarms { get; } = new();

        #region Conditional Visualisation

        private bool _autoRemoveAlarmOnCleared = true;

        /// <summary>
        /// Enable or disable automatic alarms removal on alarm status changed from Set to Cleared.
        /// This property can be changed at any time by the configuration without restart required.
        /// </summary>
        public bool AutoRemoveAlarmOnCleared
        {
            get => _autoRemoveAlarmOnCleared;
            set
            {
                if (_autoRemoveAlarmOnCleared == value)
                {
                    return;
                }

                _autoRemoveAlarmOnCleared = value;
                OnPropertyChanged(nameof(AutoRemoveAlarmOnCleared));
                InitializeCollection();
            }
        }

        private readonly Func<AlarmOccurrence, bool> _notClearedDisplayAlarmPredicate = occurrence
            => occurrence.State == AlarmState.Set || !occurrence.Acknowledged;

        private readonly Func<AlarmOccurrence, bool> _onlySetDisplayAlarmPredicate =
            occurrence => occurrence.State == AlarmState.Set;

        private Func<AlarmOccurrence, bool> VisualisationAlarmPredicate
            => AutoRemoveAlarmOnCleared ? _onlySetDisplayAlarmPredicate : _notClearedDisplayAlarmPredicate;

        #endregion

        #region Filter

        // This filter is used only if AutoRemoveAlarmOnCleared is false
        private readonly FilterCollection<AlarmOccurrence, AlarmState> _stateFilter;

        #endregion

        #region Bindable Properties

        private AlarmOccurrence _selectedOccurrence;

        public AlarmOccurrence SelectedOccurrence
        {
            get => _selectedOccurrence;
            set
            {
                if (_selectedOccurrence == value)
                {
                    return;
                }

                _selectedOccurrence = value;
                OnPropertyChanged(nameof(SelectedOccurrence));
            }
        }

        #endregion

        public Actives()
            : this(null, $"{nameof(Actives)} DesignTime Constructor")
        {
        }

        public Actives(IAlarmCenter alarmCenter)
            : this(alarmCenter, nameof(AlarmsResources.ALARMS_ACTIVES), PathIcon.ActiveAlarms)
        {
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public Actives(IAlarmCenter alarmCenter, string relativeId, IIcon icon = null)
            : base(alarmCenter, relativeId, icon)
        {
            #region Filters

            var setTimePeriodFilter = new FilterPeriod<AlarmOccurrence>(
                nameof(AlarmsResources.ALARMS_SET_TIME_PERIODE),
                occurrence => occurrence.SetTimeStamp);

            var sourceFilter = new FilterCollection<AlarmOccurrence, string>(
                nameof(AlarmsResources.ALARMS_SOURCE),
                () => Alarms.Select(occ => occ.Alarm.ProviderName).Distinct().ToList(),
                occurrence => occurrence.Alarm.ProviderName);

            var acknowledgedFilter = new FilterCollection<AlarmOccurrence, bool>(
                nameof(AlarmsResources.ALARMS_ACKNOWLEDGED),
                () => new List<bool> { true, false },
                occurrence => occurrence.Acknowledged);

            _stateFilter = new FilterCollection<AlarmOccurrence, AlarmState>(
                nameof(AlarmsResources.ALARMS_STATE),
                () => new List<AlarmState> { AlarmState.Set, AlarmState.Cleared },
                occurrence => occurrence.State);

            var ackTimePeriodFilter = new FilterPeriod<AlarmOccurrence>(
                nameof(AlarmsResources.ALARMS_ACKNOWLEDGED_TIME_PERIOD),
                occurrence => occurrence.AcknowledgedTimeStamp);

            var ackByFilter = new FilterCollection<AlarmOccurrence, string>(
                nameof(AlarmsResources.ALARMS_ACKNOWLEDGED_BY),
                () => Alarms.Where(occ => !string.IsNullOrWhiteSpace(occ.AcknowledgedBy))
                    .Select(occ => occ.AcknowledgedBy)
                    .Distinct()
                    .ToList(),
                occurrence => occurrence.AcknowledgedBy);

            Alarms.Filter.Add(setTimePeriodFilter);
            Alarms.Filter.Add(sourceFilter);
            Alarms.Filter.Add(acknowledgedFilter);
            Alarms.Filter.Add(ackTimePeriodFilter);
            Alarms.Filter.Add(ackByFilter);

            #endregion

            #region Commands

            Commands.Insert(
                0,
                new BusinessPanelCommand(
                    nameof(AlarmsResources.ALARMS_ACKNOWLEDGE),
                    AcknowledgeCommand,
                    PathIcon.AlarmAcknowledged));
            Commands.Insert(
                1,
                new BusinessPanelCommand(
                    nameof(AlarmsResources.ALARMS_ACKNOWLEDGE_ALL),
                    AcknowledgeAllCommand,
                    PathIcon.AlarmAcknowledgeAll));

            #endregion
        }

        #region Commands

        #region AcknowledgeCommand

        private ICommand _acknowledgeCommand;

        public ICommand AcknowledgeCommand
            => _acknowledgeCommand
               ?? (_acknowledgeCommand = new DelegateCommand(AcknowledgeCommandExecute, AcknowledgeCommandCanExecute));

        private bool AcknowledgeCommandCanExecute()
            => SelectedOccurrence != null
               && !SelectedOccurrence.Acknowledged
               && SelectedOccurrence.State == AlarmState.Set;

        private void AcknowledgeCommandExecute()
        {
            var currentUserName = AgilControllerApplication.Current.AccessRights.CurrentUser?.Name ?? "-";
            AlarmCenter.Services.AcknowledgeAlarm(SelectedOccurrence, currentUserName);
        }

        #endregion

        #region AcknowledgeAllCommand

        private ICommand _acknowledgeAllCommand;

        public ICommand AcknowledgeAllCommand
            => _acknowledgeAllCommand
               ?? (_acknowledgeAllCommand = new DelegateCommand(
                   AcknowledgeAllCommandExecute,
                   AcknowledgeAllCommandCanExecute));

        private bool AcknowledgeAllCommandCanExecute()
            => Alarms.Any(al => !al.Acknowledged && al.State == AlarmState.Set);

        private void AcknowledgeAllCommandExecute()
            => Popups.Show(
                new Popup(
                    nameof(AlarmsResources.ALARMS_VERIFICATION),
                    nameof(AlarmsResources.ALARMS_ACKNOWLEDGE_ALL_MESSAGE))
                {
                    SeverityLevel = MessageLevel.Warning,
                    Commands =
                    {
                        new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_NO)),
                        new PopupCommand(
                            nameof(Agileo.GUI.Properties.Resources.S_YES),
                            new DelegateCommand(
                                () =>
                                {
                                    foreach (var alarmOccurrence in Alarms.Where(al => !al.Acknowledged )
                                                 .ToList())
                                    {
                                        var currentUserName =
                                            AgilControllerApplication.Current.AccessRights.CurrentUser?.Name ?? "-";
                                        AlarmCenter.Services.AcknowledgeAlarm(alarmOccurrence, currentUserName);
                                    }
                                }))
                    }
                });

        #endregion

        #region Csv

        protected override bool ExportCsvCommandCanExecute() => Alarms.SourceView?.Count > 0;

        protected override void ExportCsvCommandExecute() => ExportToCsv(Alarms.SourceView, "ActiveAlarms.csv");

        #endregion

        #endregion

        private void AlarmOccurrenceStateChanged(object sender, AlarmOccurrenceEventArgs args)
        {
            var lastSelectedOccurence = SelectedOccurrence;
            var occurrence = args.AlarmOccurrence;
            if (VisualisationAlarmPredicate.Invoke(occurrence))
            {
                Alarms.RemoveAll(oc => oc.Equals(occurrence));

                Alarms.Add(occurrence);
            }
            else
            {
                //The occurrence must be correspond to another instance in the list
                Alarms.RemoveAll(oc => oc.Equals(occurrence));
            }

            UpdateUserAttentions();

            //Reselect last selected occurence
            var newSelectedOccurrence = lastSelectedOccurence != null
                ? Alarms.SingleOrDefault(alarmOccurrence => alarmOccurrence.Equals(lastSelectedOccurence))
                : null;
            SelectedOccurrence = newSelectedOccurrence;
        }

        private void UpdateUserAttentions()
        {
            Saliences.ResetAll();
            var saliences = Alarms.Count(occurrence => !occurrence.Acknowledged);
            for (var i = 0; i < saliences; i++)
            {
                Saliences.Add(SalienceType.Alarm);
            }
        }

        private void InitializeCollection()
        {
            //Display dynamically the State filter
            if (AutoRemoveAlarmOnCleared)
            {
                _stateFilter.SelectedValues.Clear();
                Alarms.Filter.Filters.Remove(_stateFilter);
            }
            else
            {
                if (!Alarms.Filter.Filters.Contains(_stateFilter))
                {
                    Alarms.Filter.Filters.Insert(3, _stateFilter);
                }
            }

            var initialCollection = AlarmCenter.Repository.GetAlarmOccurrences().Where(VisualisationAlarmPredicate);
            Alarms.Reset(initialCollection);
        }

        #region Setup / Dispose

        public override void OnSetup()
        {
            AlarmCenter.Services.AlarmOccurrenceStateChanged += AlarmOccurrenceStateChanged;
            InitializeCollection();

            base.OnSetup();
        }

        protected override void Dispose(bool disposing)
        {
            AlarmCenter.Services.AlarmOccurrenceStateChanged -= AlarmOccurrenceStateChanged;
            base.Dispose(disposing);
        }

        #endregion
    }
}
