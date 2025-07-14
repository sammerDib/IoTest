using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

using LightningChartLib.WPF.Charting;
using LightningChartLib.WPF.Charting.Annotations;
using LightningChartLib.WPF.Charting.Series3D;
using LightningChartLib.WPF.Charting.Titles;
using LightningChartLib.WPF.Charting.Views.View3D;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.Shared.Data.ColorMap;
using UnitySC.Shared.UI.Chart;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.Charts.ThreeDimensions
{
    public class ThreeDimensionsChartVM : BaseLineChart
    {
        #region Fields

        private readonly string _unit;
        private readonly string _xUnit;
        private readonly string _yUnit;

        private readonly SurfaceGridSeries3D _surface;
        private readonly PointLineSeries3D _indicator;
        private readonly Annotation3D _annotation;

        private int _height;
        private int _width;
        private float _min;
        private float _max;

        private float _colorMapMin;
        private float _colorMapMax;

        #endregion

        #region Properties

        private Camera3D Camera => Chart.View3D.Camera;

        #endregion

        #region Properties

        public double VerticalRotation
        {
            get => Camera.RotationX;
            set
            {
                Camera.RotationX = value;
                OnPropertyChanged();
            }
        }

        public double MinVerticalRotation => Camera.RotationXMinimum;
        public double MaxVerticalRotation => Camera.RotationXMaximum;

        public double HorizontalRotation
        {
            get => Camera.RotationY;
            set
            {
                Camera.RotationY = value;
                OnPropertyChanged();
            }
        }

        public double MinHorizontalRotation => Camera.RotationYMinimum;
        public double MaxHorizontalRotation => Camera.RotationYMaximum;

        public double SideRotation
        {
            get => Camera.RotationZ;
            set
            {
                Camera.RotationZ = value;
                OnPropertyChanged();
            }
        }

        public double MinSideRotation => Camera.RotationZMinimum;
        public double MaxSideRotation => Camera.RotationZMaximum;

        public double ZoomLevel
        {
            get => MaxZoomLevel - Camera.ViewDistance + MinZoomLevel;
            set
            {
                Camera.ViewDistance = MaxZoomLevel - value + MinZoomLevel;
                OnPropertyChanged();
            }
        }

        public double MinZoomLevel => Camera.MinimumViewDistance;
        public double MaxZoomLevel => 500;

        public bool WireframeMesh
        {
            get => _surface.WireframeType == SurfaceWireframeType3D.Wireframe;
            set
            {
                _surface.WireframeType = value ? SurfaceWireframeType3D.Wireframe : SurfaceWireframeType3D.None;
                OnPropertyChanged();
            }
        }

        private double _zAxisScaling = 1.0;

        public double ZAxisScaling
        {
            get => _zAxisScaling;
            set
            {
                SetProperty(ref _zAxisScaling, value);
                SetValueScaling(value);
            }
        }

        #endregion

        public ThreeDimensionsChartVM(string unit, string xUnit, string yUnit)
        {
            _unit = unit;
            _xUnit = xUnit;
            _yUnit = yUnit;

            Chart = new LightningChart
            {
                ChartBackground = new Fill
                {
                    Color = Color.FromArgb(255, 40, 40, 40),
                    Style = RectFillStyle.ColorOnly,
                    GradientFill = GradientFill.Solid
                },
                Title = new ChartTitle
                {
                    Text = "",
                    Visible = false
                },
                ActiveView = ActiveView.View3D,
                View3D = new View3D
                {
                    Camera = new Camera3D
                    {
                        RotationX = 50,
                        RotationY = -20,
                        OrientationMode = OrientationModes.XYZ_Mixed,
                        RotationXMaximum = 180,
                        RotationYMaximum = 180,
                        RotationZMaximum = 180,
                        RotationXMinimum = -180,
                        RotationYMinimum = -180,
                        RotationZMinimum = -180
                    },
                    Dimensions = new SizeDoubleXYZ
                    {
                        Width = 120,
                        Depth = 120,
                        Height = 120
                    }
                }
            };

            //Create surface grid 
            _surface = new SurfaceGridSeries3D(Chart.View3D, Axis3DBinding.Primary, Axis3DBinding.Primary, Axis3DBinding.Primary)
            {
                WireframeType = SurfaceWireframeType3D.None,
                ContourLineType = ContourLineType3D.None,
                ColorSaturation = 90,
                AllowCellTrace = false,
                AllowUserInteraction = false,
                Highlight = Highlight.None,
                Title = new SeriesTitle3D
                {
                    Text = $"Value ({unit})",
                    Visible = false
                }
            };

            _surface.AllowCellTrace = true;
            _surface.TraceCellChanged += OnSurfaceTraceCellChanged;

            Chart.View3D.SurfaceGridSeries3D.Add(_surface);

            //Hide walls 
            foreach (var wall in Chart.View3D.GetWalls())
            {
                wall.Visible = false;
            }

            //Add an annotation to show targeted point data
            _annotation = new Annotation3D(Chart.View3D, Axis3DBinding.Primary, Axis3DBinding.Primary, Axis3DBinding.Primary)
            {
                TargetCoordinateSystem = AnnotationTargetCoordinates.AxisValues,
                LocationCoordinateSystem = CoordinateSystem.RelativeCoordinatesToTarget,
                ArrowLineStyle = { Color = Colors.Red, Width = 5f },
                Visible = false,
                AllowUserInteraction = false,
                LocationRelativeOffset = new PointDoubleXY { X = 40, Y = -100 },
                Style = AnnotationStyle.RoundedRectangleArrow,
                Shadow = { Visible = false }
            };
            Chart.View3D.Annotations.Add(_annotation);

            //Add a red ball as tracking point indicator 
            _indicator = new PointLineSeries3D(Chart.View3D, Axis3DBinding.Primary, Axis3DBinding.Primary, Axis3DBinding.Primary)
            {
                ShowInLegendBox = false,
                PointStyle =
                {
                    Shape3D = PointShape3D.Sphere,
                    Size3D = new SizeDoubleXYZ
                    {
                        Depth = 2,
                        Height = 2,
                        Width = 2
                    }
                },
                Material = { DiffuseColor = Color.FromArgb(255, 255, 0, 0) },
                AllowUserInteraction = false
            };
            Chart.View3D.PointLineSeries3D.Add(_indicator);

            Chart.AfterRendering += AfterChartRendering;
            Chart.MouseMove += OnChartMouseMove;

            Chart.View3D.XAxisPrimary3D.Title.Text = $"X ({_xUnit})";
            Chart.View3D.YAxisPrimary3D.Title.Text = $"Value ({_unit})";
            Chart.View3D.YAxisPrimary3D.Units.Text = _unit;
            Chart.View3D.ZAxisPrimary3D.Title.Text = $"Y ({_yUnit})";
            Chart.View3D.ZAxisPrimary3D.Reversed = true;
        }

        #region Event Handlers

        private void AfterChartRendering(object sender, AfterRenderingEventArgs e)
        {
            OnPropertyChanged(nameof(VerticalRotation));
            OnPropertyChanged(nameof(HorizontalRotation));
            OnPropertyChanged(nameof(SideRotation));
            OnPropertyChanged(nameof(ZoomLevel));
        }

        private void OnSurfaceTraceCellChanged(object sender, TraceCellChangedEventArgs e)
        {
            UpdateChart(() =>
            {
                //Annotation for value tracking
                double y = _surface.Data[e.NewColumn, e.NewRow].Y;
                int x = e.NewColumn;
                int z = e.NewRow;

                // Get the largest adjacent value to avoid hiding the indicator.
                double maxAdjacentValue = new List<double>
                {
                    _surface.GetDataYValue(x, z),
                    _surface.GetDataYValue(x + 1, z),
                    _surface.GetDataYValue(x - 1, z),
                    _surface.GetDataYValue(x, z + 1),
                    _surface.GetDataYValue(x, z - 1),
                    _surface.GetDataYValue(x + 1, z + 1),
                    _surface.GetDataYValue(x - 1, z - 1),
                    _surface.GetDataYValue(x + 1, z - 1),
                    _surface.GetDataYValue(x - 1, z + 1),
                }.Max();

                // Place Tracking point at the closest triangle's vertex
                _indicator.Points = new[] { new SeriesPoint3D(x + 0.5, maxAdjacentValue, z + 0.5) };

                // Update Annotation
                _annotation.TargetAxisValues.SetValues(x + 0.5, maxAdjacentValue, z + 0.5);
                _annotation.Text = $"Coord: ({x};{z}) Value: {y} {_unit}";
            });
        }

        private void OnChartMouseMove(object sender, MouseEventArgs e)
        {
            UpdateChart(() =>
            {
                //Get object under mouse if any 
                object obj = Chart.GetActiveUserInteractiveDeviceOverObject();

                if (obj is SurfaceGridSeries3D)
                {
                    _indicator.Visible = true;
                    _annotation.Visible = true;
                }
                else
                {
                    _indicator.Visible = false;
                    _annotation.Visible = false;
                }
            });
        }

        #endregion

        #region Public Methods

        public void AllowUserInteraction(bool value)
        {
            _surface.AllowUserInteraction = value;
        }

        public void UpdateColorMap(ColorMap colorMap, float min, float max)
        {
            _colorMapMin = min;
            _colorMapMax = max;

            UpdateChart(() =>
            {
                var colors = colorMap.Colors;

                float range = max - min;
                float paletteStep = range / colors.Length;

                var palette = new ValueRangePalette(_surface);
                palette.Steps.Clear(); // Remove existing palette steps.

                if (colors.Length > 10)
                {
                    int step = colors.Length / 9;

                    for (int i = 0; i < 10; i++)
                    {
                        int colorIndex = step * i;
                        var color = colors[colorIndex];
                        palette.Steps.Add(new PaletteStep(palette, color.ToWpfColor(), min + paletteStep * colorIndex));
                    }
                }
                else
                {
                    for (int index = 0; index < colors.Length; index++)
                    {
                        var color = colors[index];
                        palette.Steps.Add(new PaletteStep(palette, color.ToWpfColor(), min + paletteStep * index));
                    }
                }

                palette.Type = PaletteType.Gradient;
                palette.MinValue = min;

                _surface.ContourPalette = palette;
            });
        }

        public void SetData(int width, int height, float[] matrix, bool showOutOfRangeValue, float? referenceValue)
        {
            // --------------------
            // | Matrix | 2D | 3D |
            // |--------|----|----|
            // | Width  | X  | X  |
            // | Height | Y  | Z  |
            // | Value  | -  | Y  |
            // --------------------

            _width = width;
            _height = height;

            UpdateChart(() =>
            {
                float max = float.MinValue;
                float min = float.MaxValue;

                var data = new SurfacePoint[width, height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        float value = matrix[y * width + x];

                        if (referenceValue.HasValue) value -= referenceValue.Value;

                        if (!showOutOfRangeValue)
                        {
                            if (value > _colorMapMax) value = float.NaN;
                            if (value < _colorMapMin) value = float.NaN;
                        }

                        data[x, y].Y = value;

                        if (value > max)
                        {
                            max = value;
                        }

                        if (value < min)
                        {
                            min = value;
                        }
                    }
                }

                _min = min;
                _max = max;

                SetValueScaling(_zAxisScaling);

                _surface.Data = data;
                _surface.SetRangesXZ(0, width, 0, height);

                Chart.View3D.XAxisPrimary3D.SetRange(_surface.RangeMinX, _surface.RangeMaxX);
                Chart.View3D.YAxisPrimary3D.SetRange(min, max);
                Chart.View3D.ZAxisPrimary3D.SetRange(_surface.RangeMinZ, _surface.RangeMaxZ);
            });
        }

        public void UpdateZAxisTitle(string zAxis)
        {
            UpdateChart(() =>
            {
                Chart.View3D.YAxisPrimary3D.Title.Text = zAxis;
            });
        }

        #endregion

        #region Private Methods

        private void SetValueScaling(double factor)
        {
            double valueRange = (_max - _min) * factor;
            Chart.View3D.Dimensions.SetValues(_width, valueRange, _height);
        }

        #endregion

        #region Commands

        private ICommand _resetCameraCommand;

        public ICommand ResetCameraCommand => _resetCameraCommand ?? (_resetCameraCommand = new AutoRelayCommand(ResetCameraCommandExecute));

        private void ResetCameraCommandExecute()
        {
            UpdateChart(() =>
            {
                Camera.RotationX = 50;
                Camera.RotationY = -20;
                Camera.RotationZ = 0;
                Camera.ViewDistance = 200;
                OnPropertyChanged();
            });
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            Chart.AfterRendering -= AfterChartRendering;
            Chart.MouseMove -= OnChartMouseMove;

            base.Dispose();
        }

        #endregion
    }
}
