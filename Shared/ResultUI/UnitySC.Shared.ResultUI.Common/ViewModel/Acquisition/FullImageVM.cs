using System;
using System.IO;
using System.Windows.Media.Imaging;

using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Base.Acquisition;
using UnitySC.Shared.ResultUI.Common.ViewModel.ImageViewer;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.Acquisition
{
    public class FullImageVM : ResultWaferVM
    {
        private ImageViewerViewModel _imageVM;

        public ImageViewerViewModel ImageViewerVM
        {
            get => _imageVM;
            set => SetProperty(ref _imageVM, value);
        }

        public FullImageVM(IResultDisplay resDisplay) : base(resDisplay)
        {
        }
        
        #region Overrides
        
        public override string FormatName => "FullImage"; 

        public override void UpdateResData(IResultDataObject resdataObj)
        {
            if (!(resdataObj is FullImageResult))
                throw new Exception($"Bad result data format ! expected full image");

            base.UpdateResData(resdataObj);

            string extension = Path.GetExtension(ResultDataObj.ResFilePath);
            string fileName = Path.GetFileName(ResultDataObj.ResFilePath);
            var image = new BitmapImage(new Uri(ResultDataObj.ResFilePath));

            void ExportAction(string destFileName)
            {
                File.Copy(ResultDataObj.ResFilePath, destFileName);
            }
            ImageViewerVM = new ImageViewerViewModel(image, ExportAction, extension, fileName, false);
        }

        #endregion
    }
}
