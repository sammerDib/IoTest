using MvvmDialogs.FrameworkDialogs.FolderBrowser;
using MvvmDialogs.FrameworkDialogs.MessageBox;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using MvvmDialogs.FrameworkDialogs.SaveFile;

using UnitySC.Shared.UI.Dialog.FrameworkDialogs.FolderBrowser;
using UnitySC.Shared.UI.Dialog.FrameworkDialogs.MessageBox;
using UnitySC.Shared.UI.Dialog.FrameworkDialogs.OpenFile;
using UnitySC.Shared.UI.Dialog.FrameworkDialogs.SaveFile;

namespace MvvmDialogs.FrameworkDialogs
{
    /// <summary>
    /// Use to create custom framework dialog
    /// </summary>
    public class CustomFrameworkDialogFactory : DefaultFrameworkDialogFactory
    {
        /// <inheritdoc />
        public override IMessageBox CreateMessageBox(MessageBoxSettings settings)
        {
            return new CustomMessageBox(settings);
        }

        public override IFrameworkDialog CreateFolderBrowserDialog(FolderBrowserDialogSettings settings)
        {
            return new CustomFolderBrowserDialog(settings);
        }

        public override IFrameworkDialog CreateOpenFileDialog(OpenFileDialogSettings settings)
        {
            return new CustomOpenFileDialog(settings);
        }

        public override IFrameworkDialog CreateSaveFileDialog(SaveFileDialogSettings settings)
        {
            return new CustomSaveFileDialog(settings);
        }
    }
}