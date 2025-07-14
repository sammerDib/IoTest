using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using UnitySC.Shared.Data.ColorMap;
using UnitySC.Shared.Data.FormatFile;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.ResultUI.Common.ViewModel.Dialogs;
using UnitySC.Shared.ResultUI.Common.ViewModel.ImageViewer;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.Helper;

using Color = System.Drawing.Color;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail
{
    public class MatrixThumbnailViewerVM : ThumbnailViewerVM
    {
        private Bitmap _bitmap;
        private MatrixDefinition _currentMatrix;
        private GenericMvvmDialogViewModel _currentDialog;

        private readonly Func<MeasurePointDataResultBase, string> _getResultImageFunc;

        private IDialogOwnerService DialogService => ClassLocator.Default.GetInstance<IDialogOwnerService>();

        public MatrixThumbnailViewerVM(PointSelectorBase pointSelector, Func<MeasurePointDataResultBase, string>  getResultImageFunc) : base(pointSelector)
        {
            _getResultImageFunc = getResultImageFunc;
        }

        #region Overrides of ThumbnailViewerVM

        protected override void Clear()
        {
            _bitmap?.Dispose();
            _bitmap = null;

            if (_currentDialog != null)
            {
                DialogService.Close(_currentDialog);
                _currentDialog = null;
            }

            _currentMatrix = null;
        }

        public override void LoadFile(string filePath)
        {
            Task.Factory.StartNew(() =>
            {
                using (var format3daFile = new MatrixFloatFile(filePath, -1))
                {
                    _currentMatrix = MatrixDefinition.FromMatrixFloatFile(format3daFile);
                    GenerateBitmap();
                }

                RaiseCommandsCanExecute();
            });
        }

        protected override string GetResultImage(MeasurePointDataResultBase pointData) => _getResultImageFunc?.Invoke(pointData);

        protected override bool OpenImageViewerCommandCanExecute() => _currentMatrix != null;

        protected override void OpenImageViewerCommandExecute()
        {
            _currentDialog = new GenericMvvmDialogViewModel("Thumbnail Viewer", new MatrixViewerViewModel(_currentMatrix));
            DialogService.Show(this, _currentDialog);
        }
        
        private void GenerateBitmap()
        {
            if (_currentMatrix == null) return;

            var colorMap = ColorMapHelper.GetThumbnailColorMap();

            var colors = colorMap.Colors;
            int colorsLength = colors.Length;

            _bitmap = new Bitmap(_currentMatrix.Width, _currentMatrix.Height);
            var graphics = Graphics.FromImage(_bitmap);
            graphics.Clear(Color.Transparent);

            var valuesWithoutNaNs = _currentMatrix.Values.Where(f => !float.IsNaN(f)).ToList();
            if (valuesWithoutNaNs.Count <= 1)
            {
                valuesWithoutNaNs.Add(-1.0f);
                valuesWithoutNaNs.Add(1.0f);
            }
            float max = valuesWithoutNaNs.Max();
            float min = valuesWithoutNaNs.Min();

            float range = max - min;
            float a = colorsLength / range;
            float b = -min * colorsLength / range;

            Color GetPixel(int x, int y)
            {
                float value = GetMeasureFromCoordinate(x, y, _currentMatrix.Values, _currentMatrix.Width);
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
