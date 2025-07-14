using System;
using System.ComponentModel;

using Agileo.AlarmModeling;
using Agileo.GUI.Components;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.Alarms
{
    public sealed class DeviceAlarmOccurrencesViewModel : Notifier, IDisposable
    {
        #region Fields

        private readonly IAlarmCenter _alarmCenter;
        private readonly string _providerClassName;
        private readonly int _instanceId;

        #endregion

        public DeviceAlarmOccurrencesViewModel(IAlarmCenter alarmCenter, string providerClassName, int instanceId)
        {
            _instanceId = instanceId;
            _providerClassName = providerClassName;
            _alarmCenter = alarmCenter;

            AlarmSource.Sort.AddSortDefinition(nameof(AlarmOccurrence.SetTimeStamp), alarm => alarm.SetTimeStamp);
            AlarmSource.Sort.AddSortDefinition(nameof(AlarmOccurrence.Alarm.Name), alarm => alarm.Alarm.Name);
            AlarmSource.Sort.AddSortDefinition(nameof(AlarmOccurrence.Acknowledged), alarm => alarm.Acknowledged);
            AlarmSource.Sort.AddSortDefinition(nameof(AlarmOccurrence.AcknowledgedTimeStamp), alarm => alarm.AcknowledgedTimeStamp);
            AlarmSource.Sort.AddSortDefinition(nameof(AlarmOccurrence.State), alarm => alarm.State);
            AlarmSource.Sort.AddSortDefinition(nameof(AlarmOccurrence.ClearedTimeStamp), alarm => alarm.ClearedTimeStamp);

            // Set sorting by SetTimeStamp by default
            AlarmSource.Sort.SetCurrentSorting(nameof(AlarmOccurrence.SetTimeStamp), ListSortDirection.Descending);

            _alarmCenter.Services.AlarmOccurrenceStateChanged += OnOccurrenceStateChanged;
            Update();
        }

        #region Properties

        public DataTableSource<AlarmOccurrence> AlarmSource { get; set; } = new();

        #endregion

        private void OnOccurrenceStateChanged(object sender, AlarmOccurrenceEventArgs args) => Update();

        private void Update() => AlarmSource.Reset(_alarmCenter.Repository.GetAlarmOccurrences(_providerClassName, _instanceId));

        public void Dispose()
        {
            _alarmCenter.Services.AlarmOccurrenceStateChanged -= OnOccurrenceStateChanged;
            AlarmSource?.Dispose();
        }
    }
}
