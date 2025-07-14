using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using LightningChartLib.WPF.Charting;
using LightningChartLib.WPF.Charting.Series3D;
using LightningChartLib.WPF.Charting.Titles;
using LightningChartLib.WPF.Charting.Views.View3D;

using UnitySC.Shared.Data.ColorMap;
using UnitySC.Shared.Data.FormatFile;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Topography;
using UnitySC.Shared.ResultUI.Common.ViewModel.Charts;
using UnitySC.Shared.ResultUI.Common.ViewModel.Dialogs;
using UnitySC.Shared.ResultUI.Common.ViewModel.ImageViewer;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.Helper;

using Color = System.Drawing.Color;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Topography
{
    public class TopographyThumbnailViewerVM : ThumbnailViewerVM
    {
        private Bitmap _bitmap;

        public TopographyThumbnailViewerVM(PointSelectorBase pointSelector) : base(pointSelector)
        {
        }

        #region Overrides of ThumbnailViewerVM

        protected override void Clear()
        {
            _bitmap?.Dispose();
            _bitmap = null;
        }

        public override void LoadFile(string filePath)
        {
            Task.Factory.StartNew(() =>
            {
                using (var format3daFile = new MatrixFloatFile(filePath, -1))
                {
                    float[] matrix = MatrixFloatFile.AggregateChunks(format3daFile.GetChunkStatus(), format3daFile);
                    int width = format3daFile.Header.Width;
                    int height = format3daFile.Header.Height;
                    GenerateBitmap(width, height, matrix);
                }

                RaiseCommandsCanExecute();
            });
        }

        protected override string GetResultImage(MeasurePointDataResultBase pointData)
        {
            if (pointData is TopographyPointData repeta)
            {
                return repeta.ResultImageFileName;
            }

            return null;
        }

        protected override bool OpenImageViewerCommandCanExecute() => _bitmap != null;

        protected override void OpenImageViewerCommandExecute()
        {
            if (_bitmap == null) return;

            var image = ImageHelper.ConvertToBitmapSource(_bitmap);
            
            void ExportAction(string destFileName)
            {
                _bitmap.Save(destFileName);
            }
            
            var dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
            var imageViewer = new ImageViewerViewModel(image, ExportAction, "png", "Thumbnail.png", false);
            dialogService.Show(this, new GenericMvvmDialogViewModel("Thumbnail Viewer", imageViewer));
        }

        private void GenerateBitmap(int width, int height, float[] matrix)
        {
            var defaultcolormap = ColorMapHelper.GetThumbnailColorMap();
            var colors = defaultcolormap.Colors;
            int colorsLength = colors.Length;

            _bitmap = new Bitmap(width, height);
            var graphics = Graphics.FromImage(_bitmap);
            graphics.Clear(Color.Transparent);

            float max = matrix.Max();
            float min = matrix.Min();

            float range = max - min;
            float a = colorsLength / range;
            float b = -min * colorsLength / range;

            Color GetPixel(int x, int y)
            {
                float value = GetMeasureFromCoordinate(x, y, matrix, width);
                int colorIndex = ColorMapHelper.GetColorIndexFromValue(value, a, b, colorsLength);
                return colorIndex < 0 ? Color.Transparent : colors[colorIndex];
            }

            ImageHelper.ProcessBitmap(_bitmap, 0, GetPixel);

            Application.Current?.Dispatcher?.Invoke(() =>
            {
                PointImage = ImageHelper.ConvertToBitmapSource(_bitmap);
            });
        }
        
        private static float GetMeasureFromCoordinate(int x, int y, float[] measures, int rowSize)
        {
            int valueIndex = x + y * rowSize;
            if (valueIndex >= measures.Length) return 0;
            return measures[valueIndex];
        }

        #endregion

        #region Overrides of ThumbnailViewerVM

        public override void Dispose()
        {
            Clear();
            base.Dispose();
        }

        #endregion
    }
}
