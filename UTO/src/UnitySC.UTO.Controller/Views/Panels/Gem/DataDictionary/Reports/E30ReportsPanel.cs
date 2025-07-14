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

namespace UnitySC.UTO.Controller.Views.Panels.Gem.DataDictionary.Reports
{
    public class E30ReportsPanel : BusinessPanel
    {
        #region Private Methods

        private void ReloadReports()
        {
            if (App.UtoInstance.GemController.IsSetupDone)
            {
                var e30Reports = App.UtoInstance.GemController.E30Std.DataServices.GetReports().ToList();
                DispatcherHelper.DoInUiThreadAsynchronously(() => Reports.Reset(e30Reports));
            }
        }

        #endregion

        #region Overrides

        public override void OnSetup()
        {
            base.OnSetup();
            ReloadReports();

            App.ControllerInstance.GemController.E30Std.Connection.PrimaryMessageReceived += Connection_PrimaryMessageReceived;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                App.ControllerInstance.GemController.E30Std.Connection.PrimaryMessageReceived -= Connection_PrimaryMessageReceived;
            }
        }
        #endregion

        #region Constructors

        static E30ReportsPanel()
        {
            DataTemplateGenerator.Create(typeof(E30ReportsPanel), typeof(E30ReportsView));
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(E30ReportsPanelResources)));
        }

        public E30ReportsPanel()
            : this($"{nameof(E30ReportsPanel)} DesignTime Constructor")
        {
        }

        public E30ReportsPanel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
            Reports.Search.AddSearchDefinition(
                nameof(DataDictionaryPanelsResources.GEMPANELS_ID),
                report => report.ID.ToString(),
                true);
            Reports.Search.AddSearchDefinition(
                nameof(DataDictionaryPanelsResources.GEMPANELS_NAME),
                report => report.Name,
                true);

            Reports.Sort.SetCurrentSorting(nameof(E30Report.ID), ListSortDirection.Ascending);

            Reports.Filter.AddRangeFilter(
                nameof(DataDictionaryPanelsResources.GEMPANELS_ID),
                report => report.ID,
                () => Reports);

            Commands.Add(new BusinessPanelCommand(nameof(DataDictionaryPanelsResources.GEMPANELS_REFRESH),
                new DelegateCommand(ReloadReports), PathIcon.Refresh));
        }

        #endregion

        #region Properties

        public DataTableSource<E30Report> Reports { get; } = new();

        private E30Report _selectedReport;

        public E30Report SelectedReport
        {
            get => _selectedReport;
            set => SetAndRaiseIfChanged(ref _selectedReport, value);
        }

        #endregion

        #region public

        public void Navigate(string reportName)
        {
            if (!TryNavigate())
            {
                return;
            }

            var selectedReport = Reports.Find(x => x.Name.Equals(reportName));
            if (selectedReport != null)
            {
                SelectedReport = selectedReport;
            }
        }

        #endregion

        #region Event handler

        private void Connection_PrimaryMessageReceived(
            object sender,
            Agileo.Semi.Communication.Abstractions.MessageEventArgs e)
        {
            if (e.Message.StreamAndFunction is "S2F33")
            {
                ReloadReports();
            }
        }

        #endregion
    }
}
