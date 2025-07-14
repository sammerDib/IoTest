using MvvmDialogs.FrameworkDialogs;
using MvvmDialogs.FrameworkDialogs.FolderBrowser;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using MvvmDialogs.FrameworkDialogs.SaveFile;

using UnitySC.Shared.UI.Dialog.FrameworkDialogs.FolderBrowser;
using UnitySC.Shared.UI.Dialog.FrameworkDialogs.OpenFile;
using UnitySC.Shared.UI.Dialog.FrameworkDialogs.SaveFile;

namespace UnitySC.UTO.Controller.Views.Panels.Integration.Dialog
{
    public class UnityDialogFactory : CustomFrameworkDialogFactory
    {
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
