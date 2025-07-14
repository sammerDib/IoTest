using System;
using System.Linq;

using Agileo.AlarmModeling;
using Agileo.GUI.Services.Icons;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Filters;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Search;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Alarms
{
    public class Catalog : BaseAlarmViewer
    {
        protected override IDataTableSource DataTableSource => Alarms;

        public DataTableSource<Alarm> Alarms { get; } = new DataTableSource<Alarm>();

        #region Bindable Properties

        private Alarm _selectedAlarm;

        public Alarm SelectedAlarm
        {
            get { return _selectedAlarm; }
            set
            {
                if (_selectedAlarm == value) return;
                _selectedAlarm = value;
                OnPropertyChanged(nameof(SelectedAlarm));
            }
        }

        #endregion

        public Catalog() : this(null, $"{nameof(Catalog)} DesignTime Constructor")
        {
        }

        public Catalog(IAlarmCenter alarmCenter) : this(alarmCenter, nameof(AlarmsResources.ALARMS_CATALOG), PathIcon.AlarmCatalog)
        {
        }

        public Catalog(IAlarmCenter alarmCenter, string relativeId, IIcon icon = null) : base(alarmCenter, relativeId, icon)
        {
            #region Filters

            var sourceFilter = new FilterCollection<Alarm, string>(nameof(AlarmsResources.ALARMS_SOURCE),
                () => Alarms.Select(alarm => alarm.ProviderName).Distinct().ToList(),
                alarm => alarm.ProviderName);

            Alarms.Filter.Add(sourceFilter);

            #endregion

            Alarms.Search.AddSearchDefinition(new SearchDefinition<Alarm>(nameof(AlarmsResources.ALARMS_ID), alarm => alarm.Id.ToString()));
            Alarms.Search.AddSearchDefinition(new SearchDefinition<Alarm>(nameof(AlarmsResources.ALARMS_KEY), alarm => alarm.Name));
        }

        #region Commands

        #region Csv

        protected override bool ExportCsvCommandCanExecute() => Alarms.SourceView != null && Alarms.SourceView.Count > 0;

        protected override void ExportCsvCommandExecute() => ExportToCsv(Alarms.SourceView, "AlarmCatalog.csv");

        #endregion

        #endregion

        private void ModelBuilder_AlarmsChanged(object sender = null, EventArgs e = null)
        {
            Alarms.Reset(AlarmCenter.Repository.GetAlarms());
        }

        public override void OnSetup()
        {
            // Alarms can changed at runtime ?
            AlarmCenter.ModelBuilder.AlarmsChanged += ModelBuilder_AlarmsChanged;
            ModelBuilder_AlarmsChanged();
            base.OnSetup();
        }

        protected override void Dispose(bool disposing)
        {
            AlarmCenter.ModelBuilder.AlarmsChanged -= ModelBuilder_AlarmsChanged;
            base.Dispose(disposing);
        }
    }
}
