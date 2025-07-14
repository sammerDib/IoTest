using System.Diagnostics;
using System.Windows;

using MvvmDialogs.FrameworkDialogs.FolderBrowser;

using UnitySC.Shared.UI.Dialog.FrameworkDialogs.FolderBrowser;

namespace DeepLearningSoft48.Utils
{
    public class SelectFolderDialog
    {
        public static string GetFolderPath(Window owner)
        {
            var settings = new FolderBrowserDialogSettings();

            var dialog = new CustomFolderBrowserDialog(settings);


            if (dialog.ShowDialog(owner) == true)
            {
                Debug.WriteLine("The following folder has been selected: " + settings.SelectedPath);
                return settings.SelectedPath;
            }
            else
            {
                Debug.WriteLine("No folder has been selected.");
                return null;
            }
        }
    }
}
