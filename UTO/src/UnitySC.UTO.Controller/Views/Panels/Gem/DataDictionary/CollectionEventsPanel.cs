using System.ComponentModel;
using System.Linq;

using Agileo.Common.Localization;
using Agileo.GUI.Commands;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;
using Agileo.Semi.Gem.Abstractions.E30;

using UnitySC.GUI.Common.Vendor;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.UTO.Controller.Views.Panels.Gem.DataDictionary.Reports;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.DataDictionary
{
    public class CollectionEventsPanel : BusinessPanel
    {
        static CollectionEventsPanel()
        {
            DataTemplateGenerator.Create(typeof(CollectionEventsPanel), typeof(CollectionEventsView));
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(DataDictionaryPanelsResources)));
        }

        public CollectionEventsPanel() : this($"{nameof(CollectionEventsPanel)} DesignTime Constructor") { }

        public CollectionEventsPanel(string relativeId, IIcon icon = null) : base(relativeId, icon)
        {
            Events.Search.AddSearchDefinition(nameof(DataDictionaryPanelsResources.GEMPANELS_ID), variable => variable.ID.ToString(), true);
            Events.Search.AddSearchDefinition(nameof(DataDictionaryPanelsResources.GEMPANELS_NAME), variable => variable.Name, true);

            Events.Sort.SetCurrentSorting(nameof(E30Event.ID), ListSortDirection.Ascending);

            Events.Filter.AddRangeFilter(nameof(DataDictionaryPanelsResources.GEMPANELS_ID), variable => variable.ID, () => Events);

            Commands.Add(new BusinessPanelCommand(nameof(DataDictionaryPanelsResources.GEMPANELS_SEND_EVENT), new DelegateCommand(SendEvent, () => SelectedEvent != null), PathIcon.Lightning));

            GoToReportCommand = new DelegateCommand<string>(
                GoToReportCommandExecute,
                GoToReportCommandCanExecute);
        }

        public DataTableSource<E30Event> Events { get; } = new DataTableSource<E30Event>();

        private E30Event _selectedEvent;

        public E30Event SelectedEvent
        {
            get { return _selectedEvent; }
            set { SetAndRaiseIfChanged(ref _selectedEvent, value); }
        }

        private void SendEvent()
        {
            App.ControllerInstance.GemController.E30Std.DataServices.SendEvent(SelectedEvent.ID);
        }

        protected void ReloadVariables()
        {
            if (App.ControllerInstance.GemController.IsSetupDone)
            {
                var e30Events = App.ControllerInstance.GemController.E30Std.DataServices.GetEvents().ToList();

                DispatcherHelper.DoInUiThreadAsynchronously(() => Events.Reset(e30Events));
            }
        }

        public override void OnSetup()
        {
            base.OnSetup();
            ReloadVariables();

            _reportPanel ??= AgilControllerApplication.Current.UserInterface.BusinessPanels
                .OfType<E30ReportsPanel>()
                .FirstOrDefault();

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

        #region Go to report

        private E30ReportsPanel _reportPanel;

        public DelegateCommand<string> GoToReportCommand { get; }

        private void GoToReportCommandExecute(string reportId)
            => _reportPanel.Navigate(reportId);

        private bool GoToReportCommandCanExecute(string reportId)
        {
            return _reportPanel is { CanNavigateTo: true };
        }

        #endregion

        #region Event handler

        private void Connection_PrimaryMessageReceived(
            object sender,
            Agileo.Semi.Communication.Abstractions.MessageEventArgs e)
        {
            if (e.Message.StreamAndFunction is "S2F35" or "S2F37")
            {
                ReloadVariables();
            }
        }

        #endregion
    }
}
