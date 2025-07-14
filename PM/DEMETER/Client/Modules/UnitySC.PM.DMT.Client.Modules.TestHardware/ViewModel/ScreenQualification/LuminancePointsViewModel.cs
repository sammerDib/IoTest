using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace UnitySC.PM.DMT.Client.Modules.TestHardware.ViewModel
{
    public class LuminancePointsViewModel : ObservableRecipient
    {
        private AutoRelayCommand _deleteAllLuminanceCommand;

        private bool _isValid;

        private LuminancePointViewModel _selectedPoint;
        public string StepName { get; set; }

        public bool IsModified { get; set; }
        
        public bool IsValid
        {
            get => _isValid;
            set
            {
                if (_isValid != value)
                {
                    _isValid = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Grayscale { get; set; }

        public Side Side { get; set; }

        public List<LuminancePointViewModel> LuminancePoints { get; set; } = new List<LuminancePointViewModel>();

        public LuminancePointViewModel SelectedPoint
        {
            get => _selectedPoint;
            set
            {
                if (_selectedPoint != value)
                {
                    _selectedPoint = value;
                    OnPropertyChanged();
                }
            }
        }

        public AutoRelayCommand DeleteAllLuminanceCommand =>
            _deleteAllLuminanceCommand ?? (_deleteAllLuminanceCommand = new AutoRelayCommand(
                () =>
                {
                    LuminancePoints.ForEach(x => x.Luminance = null);
                },
                () => { return LuminancePoints.Any(x => x.Luminance.HasValue); }));

        public BitmapSource CreateImages(int screenWidth, int screenHeight, double circleStrokeThickness, double measureCircleSize)
        {
            var backgroundColor = Grayscale == 0 ? Color.Black : Color.White;
            var foregroundColor = Grayscale == 0 ? Color.White : Color.Black;

            using (var bmp = new Bitmap(screenWidth, screenHeight, PixelFormat.Format24bppRgb))
            {
                using (var grf = Graphics.FromImage(bmp))
                {
                    using (var brush = new SolidBrush(backgroundColor))
                    {
                        grf.FillRectangle(brush, 0, 0, screenWidth, screenHeight);

                        using (var pen = new Pen(foregroundColor, (float)circleStrokeThickness))
                        {
                            foreach (var point in LuminancePoints)
                            {
                                grf.DrawEllipse(pen, (float)point.LeftPosition, (float)point.TopPosition,
                                    (float)measureCircleSize, (float)measureCircleSize);
                            }
                        }
                    }
                }

                return ConvertToBitmapSource(bmp);
            }
        }

        private static BitmapSource ConvertToBitmapSource(Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Rgb24, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);

            return bitmapSource;
        }

        public void Validate()
        {
            IsValid = LuminancePoints.All(x => x.Luminance.HasValue);
        }
    }
}
