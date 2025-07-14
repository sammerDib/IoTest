using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using MvvmDialogs;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.Help
{
    internal class HelpImageDisplayVM : IModalDialogViewModel
    {
        public bool? DialogResult { get; }

        public event PropertyChangedEventHandler PropertyChanged;

                public ImageSource HelpImageSource { get; }
        public HelpImageDisplayVM(string imageSourcePath)
        {
            HelpImageSource = new BitmapImage(new Uri(imageSourcePath, UriKind.RelativeOrAbsolute));
        }
    }
}
