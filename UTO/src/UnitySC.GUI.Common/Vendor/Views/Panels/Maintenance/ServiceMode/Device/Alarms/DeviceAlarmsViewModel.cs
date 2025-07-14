using System;

using Agileo.AlarmModeling;
using Agileo.GUI.Components;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.Alarms
{
    public sealed class DeviceAlarmsViewModel : Notifier, IDisposable
    {
        #region Fields

        private readonly IAlarmCenter _alarmCenter;
        private readonly string _providerClassName;
        private readonly int _instanceId;

        #endregion

        public DeviceAlarmsViewModel(IAlarmCenter alarmCenter, string providerClassName, int instanceId)
        {
            _instanceId = instanceId;
            _providerClassName = providerClassName;
            _alarmCenter = alarmCenter;

            AlarmSource.Sort.AddSortDefinition(nameof(Alarm.Name), alarm => alarm.Name);
            AlarmSource.Sort.AddSortDefinition(nameof(Alarm.Description), alarm => alarm.Description);
            AlarmSource.Sort.AddSortDefinition(nameof(Alarm.Id), alarm => alarm.Id);
            AlarmSource.Sort.AddSortDefinition(nameof(Alarm.State), alarm => alarm.State);
            AlarmSource.Sort.AddSortDefinition(nameof(Alarm.Acknowledged), alarm => alarm.Acknowledged);

            // Set sorting by Id by default
            AlarmSource.Sort.SetCurrentSorting(nameof(Alarm.Id));

            _alarmCenter.ModelBuilder.AlarmsChanged += OnAlarmsChanged;
            Update();
        }

        #region Properties

        public DataTableSource<Alarm> AlarmSource { get; set; } = new();

        #endregion

        private void OnAlarmsChanged(object sender, EventArgs e) => Update();

        private void Update() => AlarmSource.Reset(_alarmCenter.Repository.GetProviderAlarms(_providerClassName, _instanceId));

        public void Dispose()
        {
            _alarmCenter.ModelBuilder.AlarmsChanged -= OnAlarmsChanged;
            AlarmSource?.Dispose();
        }
    }
}
