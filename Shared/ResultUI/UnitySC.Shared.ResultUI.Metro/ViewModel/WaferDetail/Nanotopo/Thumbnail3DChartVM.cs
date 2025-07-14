using System.Windows.Media;

using LightningChartLib.WPF.Charting;
using LightningChartLib.WPF.Charting.Series3D;
using LightningChartLib.WPF.Charting.Titles;
using LightningChartLib.WPF.Charting.Views.View3D;

using UnitySC.Shared.UI.Chart;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Nanotopo
{
    public class Thumbnail3DChartVM : BaseLineChart
    {
        private readonly SurfaceGridSeries3D _surface;

        public Thumbnail3DChartVM()
        {
            Chart = new LightningChart
            {
                ChartBackground = new Fill()
                {
                    Color = System.Windows.Media.Color.FromArgb(255, 40, 40, 40),
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
                        OrientationMode = OrientationModes.XYZ_Mixed
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
                WireframeType = SurfaceWireframeType3D.WireframePalettedByY,
                ContourLineType = ContourLineType3D.None,
                ColorSaturation = 90,
                AllowCellTrace = false,
                AllowUserInteraction = false,
                Highlight = Highlight.None,
                Title = new SeriesTitle3D
                {
                    Text = "Depth"
                }
            };

            Chart.View3D.SurfaceGridSeries3D.Add(_surface);

            //Hide walls 
            foreach (var wall in Chart.View3D.GetWalls())
            {
                wall.Visible = false;
            }
        }

        private void CreatePalette(float min, float max)
        {
            float range = max - min;

            var palette = new ValueRangePalette(_surface);
            palette.Steps.Clear(); // Remove existing palette steps.
            palette.Steps.Add(new PaletteStep(palette, Colors.DarkBlue, min));
            palette.Steps.Add(new PaletteStep(palette, System.Windows.Media.Color.FromArgb(255, 0, 180, 0), min + range * 0.33));
            palette.Steps.Add(new PaletteStep(palette, Colors.Yellow, min + range * 0.66));
            palette.Steps.Add(new PaletteStep(palette, Colors.Red, min + range));
            palette.Type = PaletteType.Gradient;
            palette.MinValue = min;

            _surface.ContourPalette = palette;
        }

        public void SetData(int width, int height, float[] matrix)
        {
            UpdateChart(() =>
            {
                float max = float.MinValue;
                float min = float.MaxValue;

                var data = new SurfacePoint[width, height];

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        float value = matrix[i * height + j];
                        data[i, j].Y = value;

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
                
                CreatePalette(min, max);

                Chart.View3D.Dimensions.SetValues(width, height, max-min);

                _surface.Data = data;
                _surface.SetRangesXZ(0, height, 0, width);

                Chart.View3D.XAxisPrimary3D.SetRange(_surface.RangeMinX, _surface.RangeMaxX);
                Chart.View3D.XAxisPrimary3D.Title.Text = "Height";

                Chart.View3D.YAxisPrimary3D.SetRange(min, max);
                Chart.View3D.YAxisPrimary3D.Title.Text = "Depth";
                Chart.View3D.YAxisPrimary3D.Units.Text = "nm";

                Chart.View3D.ZAxisPrimary3D.SetRange(_surface.RangeMinZ, _surface.RangeMaxZ);
                Chart.View3D.ZAxisPrimary3D.Title.Text = "Width";

            });
        }
    }
}
