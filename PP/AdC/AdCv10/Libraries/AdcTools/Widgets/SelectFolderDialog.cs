using Microsoft.WindowsAPICodePack.Dialogs;

namespace AdcTools.Widgets
{
    public class SelectFolderDialog
    {
        public static bool ShowDialog(ref string path)
        {
            CommonOpenFileDialog dlg = new CommonOpenFileDialog();
            dlg.IsFolderPicker = true;
            if (!string.IsNullOrEmpty(path))
                dlg.InitialDirectory = path;
            if (dlg.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
            {
                path = dlg.FileName;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string ShowDialog(string path)
        {
            ShowDialog(ref path);
            return path;
        }
    }
}
