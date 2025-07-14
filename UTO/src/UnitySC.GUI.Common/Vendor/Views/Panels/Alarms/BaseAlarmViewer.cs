using System.Collections.Generic;
using System.Windows.Input;

using Agileo.AlarmModeling;
using Agileo.Common.Localization;
using Agileo.GUI.Commands;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;

using Microsoft.Win32;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Alarms
{
    public abstract class BaseAlarmViewer : BusinessPanel
    {
        protected readonly IAlarmCenter AlarmCenter;

        protected abstract IDataTableSource DataTableSource { get; }

        static BaseAlarmViewer()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(AlarmsResources)));
        }

        protected BaseAlarmViewer(IAlarmCenter alarmCenter, string relativeId, IIcon icon = null) : base(relativeId,
            icon)
        {
            AlarmCenter = alarmCenter;

            Commands.Add(new BusinessPanelCommand(nameof(AlarmsResources.ALARMS_EXPORT_CSV), ExportCsvCommand, PathIcon.CSV));
        }

        #region Commands

        private ICommand _exportCsvCommand;

        public ICommand ExportCsvCommand => _exportCsvCommand ??
                                            (_exportCsvCommand = new DelegateCommand(ExportCsvCommandExecute,
                                                ExportCsvCommandCanExecute));

        protected abstract bool ExportCsvCommandCanExecute();

        protected abstract void ExportCsvCommandExecute();

        #endregion

        public void ExportToCsv<T>(IEnumerable<T> collection, string defaultFileName)
        {
            var sfd = new SaveFileDialog
            {
                AddExtension = true,
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = defaultFileName
            };
            if (sfd.ShowDialog() != true) return;
            CsvFileHelper.WriteRecords(collection, sfd.FileName);
            DisplayExportingSuccessfully(sfd.FileName);
        }

        protected void DisplayExportingSuccessfully(string filePath)
        {
            var exportingSuccessfullyUserMessage = new UserMessage(MessageLevel.Success, nameof(AlarmsResources.ALARMS_EXPORTING_SUCCESS));
            exportingSuccessfullyUserMessage.Commands.Add(OpenFileDirectory.GetUserMessageCommand(nameof(AlarmsResources.ALARMS_OPEN_FOLDER), filePath));
            exportingSuccessfullyUserMessage.CanUserCloseMessage = true;
            Messages.Show(exportingSuccessfullyUserMessage);
        }

        public override void OnHide()
        {
            DataTableSource.Filter.IsOpen = false;
            base.OnHide();
        }
    }
}
