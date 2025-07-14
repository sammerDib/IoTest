using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.Shared.Data.ColorMap;
using UnitySC.Shared.ResultUI.Common.Helpers;
using UnitySC.Shared.UI.Helper;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.ImageViewer
{
    public class MatrixViewFinderVM : ObservableObject, IDisposable
    {
        #region Fields

        private Bitmap _bitmap;
        private ColorMap _colorMap;
        private float _min;
        private float _max;

        #endregion

        #region Properties

        /// <summary>
        /// Original matrix used for the calculation of the required matrix area.
        /// </summary>
        public MatrixDefinition OriginalMatrix { get; }

        /// <summary>
        /// Thumbnail Matrix used for image display
        /// </summary>
        private MatrixDefinition ThumbnailMatrix { get; set; }

        private int _startX;

        public int StartX
        {
            get => _startX;
            set
            {
                SetProperty(ref _startX, value);
                SetProperty(ref _percentStartX, (double)StartX / OriginalMatrix.Width, nameof(PercentStartX));
                OnZoomAreaChanged();
            }
        }

        private int _startY;

        public int StartY
        {
            get => _startY;
            set
            {
                SetProperty(ref _startY, value);
                SetProperty(ref _percentStartY, (double)StartY / OriginalMatrix.Height, nameof(PercentStartY));
                OnZoomAreaChanged();
            }
        }

        private int _endX;

        public int EndX
        {
            get => _endX;
            set
            {
                SetProperty(ref _endX, value);
                SetProperty(ref _percentEndX, (double)EndX / OriginalMatrix.Width, nameof(PercentEndX));
                OnZoomAreaChanged();
            }
        }

        private int _endY;

        public int EndY
        {
            get => _endY;
            set
            {
                SetProperty(ref _endY, value);
                SetProperty(ref _percentEndY, (double)EndY / OriginalMatrix.Height, nameof(PercentEndY));
                OnZoomAreaChanged();
            }
        }

        public int RectWidth => EndX - StartX;

        public int RectHeight => EndY - StartY;

        #region Percent Coordinates

        private double _percentStartX;

        public double PercentStartX
        {
            get => _percentStartX;
            set
            {
                SetProperty(ref _percentStartX, value);
                SetProperty(ref _startX, (int)(value * OriginalMatrix.Width), nameof(StartX));
                OnZoomAreaChanged();
            }
        }

        private double _percentStartY;

        public double PercentStartY
        {
            get => _percentStartY;
            set
            {
                SetProperty(ref _percentStartY, value);
                SetProperty(ref _startY, (int)(value * OriginalMatrix.Height), nameof(StartY));
                OnZoomAreaChanged();
            }
        }

        private double _percentEndX = 1;

        public double PercentEndX
        {
            get => _percentEndX;
            set
            {
                SetProperty(ref _percentEndX, value);
                SetProperty(ref _endX, (int)(value * OriginalMatrix.Width), nameof(EndX));
                OnZoomAreaChanged();
            }
        }

        private double _percentEndY = 1;

        public double PercentEndY
        {
            get => _percentEndY;
            set
            {
                SetProperty(ref _percentEndY, value);
                SetProperty(ref _endY, (int)(value * OriginalMatrix.Height), nameof(EndY));
                OnZoomAreaChanged();
            }
        }

        #endregion

        private ImageSource _image;

        public ImageSource Image
        {
            get => _image;
            private set => SetProperty(ref _image, value);
        }

        private bool _manipulationInProgress;

        public bool ManipulationInProgress
        {
            get { return _manipulationInProgress; }
            set
            {
                SetProperty(ref _manipulationInProgress, value);
                if (value == false)
                {
                    // Initiates the final interpolation at the end of the user manipulation.
                    OnZoomAreaChanged();
                }
            }
        }

        #endregion

        public event EventHandler<Rect> MatrixRectChanged;

        public event EventHandler<Rect> MatrixRectChanging;

        public MatrixViewFinderVM(MatrixDefinition originalMatrix, ColorMap colorMap, float min, float max)
        {
            OriginalMatrix = originalMatrix;

            SetProperty(ref _startX, 0, nameof(StartX));
            SetProperty(ref _startY, 0, nameof(StartY));
            SetProperty(ref _endX, originalMatrix.Width, nameof(EndX));
            SetProperty(ref _endY, originalMatrix.Height, nameof(EndY));

            // By default creates a full area matrix.
            OnZoomAreaChanged();

            Task.Factory.StartNew(() =>
            {
                // The limit is reduced in order to increase the performance of the Thumbnail generation.
                ThumbnailMatrix = MatrixDefinitionHelper.Interpolate(OriginalMatrix, null, 300);
                UpdateThumbnail(colorMap, min, max);
            });
        }

        #region Commands

        private AutoRelayCommand _setFullCommand;

        public AutoRelayCommand SetFullCommand => _setFullCommand ?? (_setFullCommand = new AutoRelayCommand(SetFullCommandExecute));

        private void SetFullCommandExecute()
        {
            SetProperty(ref _startX, 0, nameof(StartX));
            SetProperty(ref _startY, 0, nameof(StartY));
            SetProperty(ref _endX, OriginalMatrix.Width, nameof(EndX));
            SetProperty(ref _endY, OriginalMatrix.Height, nameof(EndY));

            SetProperty(ref _percentStartX, 0, nameof(PercentStartX));
            SetProperty(ref _percentStartY, 0, nameof(PercentStartY));
            SetProperty(ref _percentEndX, 1, nameof(PercentEndX));
            SetProperty(ref _percentEndY, 1, nameof(PercentEndY));

            OnPropertyChanged(nameof(PercentStartX));
            OnPropertyChanged(nameof(PercentStartY));
            OnPropertyChanged(nameof(PercentEndX));
            OnPropertyChanged(nameof(PercentEndY));

            OnZoomAreaChanged();
        }

        #endregion

        #region Public Methods

        public void UpdateThumbnail(ColorMap colorMap, float min, float max)
        {
            _colorMap = colorMap;
            _min = min;
            _max = max;

            GenerateBitmap();
        }

        public void SetViewRect(Rect rect)
        {
            ManipulationInProgress = true;

            StartX = Math.Max(0, (int)rect.TopLeft.X);
            StartY = Math.Max(0, (int)rect.TopLeft.Y);
            EndX = Math.Min(OriginalMatrix.Width, (int)rect.BottomRight.X);
            EndY = Math.Min(OriginalMatrix.Height, (int)rect.BottomRight.Y);

            ManipulationInProgress = false;
        }

        #endregion

        #region Private Methods

        private void GenerateBitmap()
        {
            if (ThumbnailMatrix == null || _colorMap == null) return;

            Task.Factory.StartNew(() =>
            {
                if (ThumbnailMatrix.Height == 0 || ThumbnailMatrix.Width == 0) return;

                var colors = _colorMap.Colors;
                int colorsLength = colors.Length;

                if (_bitmap != null)
                {
                    _bitmap.Dispose();
                    _bitmap = null;
                }

                _bitmap = new Bitmap(ThumbnailMatrix.Width, ThumbnailMatrix.Height);
                var graphics = Graphics.FromImage(_bitmap);
                graphics.Clear(System.Drawing.Color.Transparent);

                float range = _max - _min;
                float a = colorsLength / range;
                float b = -_min * colorsLength / range;

                System.Drawing.Color GetPixel(int x, int y)
                {
                    float value = GetMeasureFromCoordinate(x, y, ThumbnailMatrix.Values, ThumbnailMatrix.Width);
                    return _colorMap.GetColorFromValue(value, a, b);
                }

                try
                {
                    ImageHelper.ProcessBitmap(_bitmap, 0, GetPixel);
                }
                catch (Exception)
                {
                    //ignored
                }

                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    Image = ImageHelper.ConvertToBitmapSource(_bitmap);
                });
            });
        }

        private static float GetMeasureFromCoordinate(int x, int y, IReadOnlyList<float> measures, int rowSize)
        {
            int valueIndex = x + y * rowSize;
            if (valueIndex >= measures.Count) return 0;
            return measures[valueIndex];
        }

        private void OnZoomAreaChanged()
        {
            OnPropertyChanged(nameof(RectWidth));
            OnPropertyChanged(nameof(RectHeight));

            MatrixRectChanging?.Invoke(this, new Rect(new System.Windows.Point(StartX, StartY), new System.Windows.Point(EndX, EndY)));

            // Prevents recalculation of the matrix when the user is manipulating its size.
            if (!ManipulationInProgress)
            {
                MatrixRectChanged?.Invoke(this, new Rect(new System.Windows.Point(StartX, StartY), new System.Windows.Point(EndX, EndY)));
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Image = null;
        }

        #endregion
    }
}
