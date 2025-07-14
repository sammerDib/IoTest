using System.Diagnostics;
using System.IO;

using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;

using UnitySC.GUI.Common.Resources;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

namespace UnitySC.GUI.Common.Vendor.Helpers
{
    public static class OpenFileDirectory
    {
        public static UserMessageCommand GetUserMessageCommand(string commandId, string filePath)
            => new UserMessageCommand(commandId, new DelegateCommand(() => ProcessStart(filePath)), PathIcon.Folder);

        public static void ProcessStart(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            // Use /select as an argument to explorer.exe to select the file designated by filePath.
            Process.Start("explorer.exe", "/select,\"" + filePath + "\"");
        }

        public static UserMessage GetUserMessage(LocalizableText message, string filePath)
        {
            var userMessage = new UserMessage(MessageLevel.Info, message);
            var command = GetUserMessageCommand(nameof(L10N.SHOW_IN_FILE_EXPLORER), filePath);
            userMessage.Commands.Add(command);
            userMessage.CanUserCloseMessage = true;
            return userMessage;
        }
    }
}
