using System;

using Agileo.Common.Logging;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;
using Agileo.Semi.Communication.Abstractions.E173.Model;
using Agileo.Semi.Communication.Abstractions.E5;

using Microsoft.Win32;

using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.SequenceLog
{
    public class SmnViewerPanel : BaseSequenceLogPanel
    {
        private readonly ILogger _logger;

        public BusinessPanelCommand OpenFile { get; }

        public SmnViewerPanel(string relativeId, IIcon icon = null) : base(relativeId, icon)
        {
            OpenFile = new BusinessPanelCommand(nameof(SequenceLogResources.SEQUENCE_LOG_OPEN_FILE),
                new DelegateCommand(OpenSmnFileExecute), PathIcon.Undo);

            Commands.Insert(0, OpenFile);

            _logger = Logger.GetLogger(nameof(SmnViewerPanel));
        }

        private static string OpenSmnFile()
        {
            string fileName = null;
            var dialog = new OpenFileDialog { Filter = "smn file (*.smn)|*.smn" };

            if (dialog.ShowDialog() == true)
            {
                fileName = dialog.FileName;
            }

            return fileName;
        }

        private void OpenSmnFileExecute()
        {
            var fileName = OpenSmnFile();

            if (fileName == null)
            {
                return;
            }

            try
            {
                var scenario = SECSMessageScenario.LoadFromFile(fileName);
                var messages = scenario.SECSMessage.ConvertAll(m => new Message(m));

                SequenceTableSource.Clear();

                foreach (var message in messages)
                {
                    SequenceTableSource.Add(ToSecsMessage(message));
                }

                SequenceTableSource.UpdateFilterPossibleValues();
                Messages.HideAll();
            }
            catch (Exception ex)
            {
                Messages.Show(new UserMessage(MessageLevel.Error,
                    new LocalizableText(nameof(SequenceLogResources.SEQUENCE_LOG_FILE_OPEN_ERROR), fileName)));
                _logger.Error(ex);
            }
        }
    }
}
