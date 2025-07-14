using System;
using System.Collections.Generic;
using System.IO;

using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.Result.StandaloneClient.Models
{
    public class FolderEntry : ExplorerEntry
    {
        public string FolderName { get; }

        public List<ExplorerEntry> Children { get; private set; }

        public FolderEntry(string path, string folderName) : base(path)
        {
            FolderName = folderName;
        }

        public void BuildChildren()
        {
            var children = new List<ExplorerEntry>();

            try
            {
                // Folders
                string[] folderEntries = Directory.GetDirectories(Path);
                foreach (string entry in folderEntries)
                {
                    string folderName = entry.Substring(entry.LastIndexOf('\\') + 1);
                    children.Add(new FolderEntry(entry, folderName));
                }

                // Files
                string[] fileEntries = Directory.GetFiles(Path);
                foreach (string entry in fileEntries)
                {
                    var fileEntry = new FileEntry(entry);
                    if (fileEntry.ExtensionId > -1)
                    {
                        children.Add(fileEntry);
                    }
                }

                Children = children;
            }
            catch (Exception ex)
            {
                var notifierVM = ClassLocator.Default.GetInstance<NotifierVM>();
                notifierVM.AddMessage(new Message(MessageLevel.Error, $"An error occurred when opening a folder or a file. Error : " + ex.Message));
            }
        }
    }
}
