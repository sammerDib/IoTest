using System;
using System.IO;
using System.Windows.Media.Imaging;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.TSV;
using UnitySC.Shared.ResultUI.Common.ViewModel.Dialogs;
using UnitySC.Shared.ResultUI.Common.ViewModel.ImageViewer;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Tsv
{
    public class TsvThumbnailViewerVM : ThumbnailViewerVM
    {
        private string _imagePath;

        public TsvThumbnailViewerVM(PointSelectorBase pointSelector) : base(pointSelector)
        {
        }

        #region Overrides of ThumbnailViewerVM

        protected override void Clear()
        {
            _imagePath = null;
        }

        public override void LoadFile(string filePath)
        {
            PointImage = new BitmapImage(new Uri(filePath));
            _imagePath = filePath;

            RaiseCommandsCanExecute();
        }

        protected override string GetResultImage(MeasurePointDataResultBase pointData)
        {
            if (pointData is TSVPointData repeta)
            {
                return repeta.ResultImageFileName;
            }

            return null;
        }

        protected override bool OpenImageViewerCommandCanExecute() => !string.IsNullOrWhiteSpace(_imagePath);

        protected override void OpenImageViewerCommandExecute()
        {
            if (string.IsNullOrWhiteSpace(_imagePath)) return;

            string extension = Path.GetExtension(_imagePath);
            string fileName = Path.GetFileName(_imagePath);
            var image = new BitmapImage(new Uri(_imagePath));

            var dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();

            void ExportAction(string destFileName)
            {
                File.Copy(_imagePath, destFileName);
            }

            var imageViewer = new ImageViewerViewModel(image, ExportAction, extension, fileName, false);
            dialogService.Show(this, new GenericMvvmDialogViewModel("Thumbnail Viewer", imageViewer));
        }

        #endregion
    }
}
