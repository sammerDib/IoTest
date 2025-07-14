using System.ComponentModel;
using System.Linq;

using Agileo.Common.Localization;
using Agileo.GUI.Commands;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;
using Agileo.Semi.Gem.Abstractions.E30;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.DataDictionary.Alarms
{
    public class E30AlarmsPanel : BusinessPanel
    {
        #region Private Methods

        private void ReloadAlarms()
        {
            if (App.UtoInstance.GemController.IsSetupDone)
            {
                DispatcherHelper.DoInUiThreadAsynchronously(
                    () => { Alarms.Reset(App.UtoInstance.GemController.E30Std.AlarmServices.GetAlarms()); });
            }
        }

        #endregion

        #region Overrides

        public override void OnSetup()
        {
            base.OnSetup();
            ReloadAlarms();
            App.UtoInstance.GemController.E30Std.AlarmServices.AlarmChanged += AlarmServices_AlarmChanged;
        }

        protected override void Dispose(bool disposing)
        {
            if (App.UtoInstance.GemController.IsSetupDone && disposing)
            {
                App.UtoInstance.GemController.E30Std.AlarmServices.AlarmChanged -= AlarmServices_AlarmChanged;
            }
            base.Dispose(disposing);
        }

        public override void OnShow()
        {
            base.OnShow();
            ReloadAlarms();
        }

        #endregion

        #region Event Handlers

        private void AlarmServices_AlarmChanged(object sender, AlarmEventArgs e)
        {
            E30Alarm previousSelectedAlarm = null;
            if (SelectedAlarm != null)
            {
                previousSelectedAlarm = SelectedAlarm;
            }
            ReloadAlarms();

            if (previousSelectedAlarm == null)
            {
                return;
            }

            var alarm = Alarms.FirstOrDefault(alarm => alarm.ID == previousSelectedAlarm.ID);
            if (alarm != null)
            {
                SelectedAlarm = alarm;
            }
        }

        #endregion

        #region Constructors

        static E30AlarmsPanel()
        {
            DataTemplateGenerator.Create(typeof(E30AlarmsPanel), typeof(E30AlarmsView));
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(E30AlarmPanelResources)));
        }

        public E30AlarmsPanel()
            : this($"{nameof(E30AlarmsPanel)} DesignTime Constructor")
        {
        }

        public E30AlarmsPanel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
            Alarms.Search.AddSearchDefinition(
                nameof(DataDictionaryPanelsResources.GEMPANELS_ID),
                alarm => alarm.ID.ToString(),
                true);
            Alarms.Search.AddSearchDefinition(
                nameof(DataDictionaryPanelsResources.GEMPANELS_NAME),
                alarm => alarm.Name,
                true);

            Alarms.Search.AddSearchDefinition(
                nameof(E30AlarmPanelResources.GEMPANELS_ALARM_TEXT),
                alarm => alarm.Text,
                true);

            Alarms.Search.AddSearchDefinition(
                nameof(E30AlarmPanelResources.GEMPANELS_ALARM_SET_CEID),
                alarm => alarm.SetEvent.ID.ToString());

            Alarms.Search.AddSearchDefinition(
                nameof(E30AlarmPanelResources.GEMPANELS_ALARM_CLEAR_CEID),
                alarm => alarm.ClearEvent.ID.ToString());

            Alarms.Sort.SetCurrentSorting(nameof(E30Alarm.ID), ListSortDirection.Ascending);

            Alarms.Filter.AddRangeFilter(
                nameof(DataDictionaryPanelsResources.GEMPANELS_ID),
                alarm => alarm.ID,
                () => Alarms);

            Commands.Add(
                new BusinessPanelCommand(
                    nameof(E30AlarmPanelResources.GEMPANELS_SET_ALARM),
                    new DelegateCommand(SetAlarm, () => SelectedAlarm is { IsSet: false }),
                    PathIcon.AlarmSet));

            Commands.Add(
                new BusinessPanelCommand(
                    nameof(E30AlarmPanelResources.GEMPANELS_CLEAR_ALARM),
                    new DelegateCommand(ClearAlarm, () => SelectedAlarm is { IsSet: true }),
                    PathIcon.AlarmAcknowledged));

            Commands.Add(new BusinessPanelCommand(nameof(DataDictionaryPanelsResources.GEMPANELS_REFRESH),
                new DelegateCommand(ReloadAlarms), PathIcon.Refresh));
        }

        #endregion

        #region Properties

        public DataTableSource<E30Alarm> Alarms { get; } = new();

        private E30Alarm _selectedAlarm;

        public E30Alarm SelectedAlarm
        {
            get => _selectedAlarm;
            set => SetAndRaiseIfChanged(ref _selectedAlarm, value);
        }

        #endregion

        #region Commands

        private void SetAlarm() => App.UtoInstance.GemController.E30Std.AlarmServices.SetAlarm(SelectedAlarm.ID);

        private void ClearAlarm() => App.UtoInstance.GemController.E30Std.AlarmServices.ClearAlarm(SelectedAlarm.ID);

        #endregion
    }
}
