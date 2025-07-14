using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using LightningChartLib.WPF.Charting;
using LightningChartLib.WPF.Charting.SeriesXY;
using LightningChartLib.WPF.Charting.Titles;

using UnitySC.Shared.Tools;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.Common.RawSignal
{
    public class RawFFTSignalChart : RawSignalChart
    {
        private readonly int _annotationLocYFromTop = 12;
        private readonly int _annotationLocXFromRight = 118;

        private class FFTSignalDebuffered
        {
            public List<float> XValues { get; private set; }
            public List<float> YValues { get; private set; }
            public byte Saturation { get; private set; }
            public byte Version { get; private set; }

            public int PointsCount => XValues?.Count ?? 0; 

            public FFTSignalDebuffered(byte[] buffer)
            {
                if (buffer == null)
                    throw new ArgumentNullException("Null FFT buffer");

                ToSignal(buffer);
            }

            private void ToSignal(byte[] buffer)
            { 
                Version = buffer[0];
                switch (Version)
                {
                    case 0: 
                        int cnt = buffer.Length;
                        if (((cnt-2) % sizeof(float)) != 0)
                             throw new Exception($"Bad buffer legnth should be aligned on float size");

                        Saturation = buffer[1];

                        int bufFloatSize = (cnt - 2) / sizeof(float);
                        var bufFloat = new float[bufFloatSize];
                        Buffer.BlockCopy(buffer, 2, bufFloat, 0, cnt-2);

                        XValues = new List<float>(bufFloatSize / 2);
                        YValues = new List<float>(bufFloatSize / 2);
                        for (int i = 0; i < bufFloatSize; i += 2)
                        {
                            XValues.Add(bufFloat[i]);
                            YValues.Add(bufFloat[i+1]);
                        }
                        break;

                    default:
                        throw new Exception($"Buffer Version Not Handle, application update is required [v={Version}] ");
                }

            
            }
        }

        #region Constructor

        public RawFFTSignalChart() : base(string.Empty) 
        {
            //Set the legend box
            Chart.ViewXY.LegendBoxes[0].Position = LegendBoxPositionXY.TopCenter;

            Chart.SizeChanged += RawFFTChart_SizeChanged;
        }

        private void RawFFTChart_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            UpdateChart(() =>
            {
                if (Chart.ViewXY.Annotations?.Count != 0)
                {
                    foreach (var annotation in Chart.ViewXY.Annotations)
                    {
                        if(annotation != null) 
                           annotation.LocationScreenCoords.X = Chart.ActualWidth - _annotationLocXFromRight;
                    }
                }
            });
        }

        #endregion

        #region Public Methods

        public void Generate(ICollection<byte[]> repetaSignalBuffers)
        {
            UpdateChart(() =>
            {
                Chart.ViewXY.PointLineSeries.Clear();
                Chart.ViewXY.Annotations.Clear();

                var random = new Random(12345);  // use fixed seed to have the same color order from a measure to another
                byte lastSat = 0;
                int repetaIndex = 0;
                foreach (var buffer in repetaSignalBuffers)
                {
                    //temporary solution, generate random color for the repeta raw signal
                    byte r = Convert.ToByte(random.Next(1, 255));
                    byte g = Convert.ToByte(random.Next(1, 255));
                    byte b = Convert.ToByte(random.Next(1, 255));

                    if (buffer == null || buffer.Length <= 2) continue;

                    FFTSignalDebuffered rawFFTSignal = null;
                    try
                    {
                         rawFFTSignal = new FFTSignalDebuffered(buffer);
                    }
                    catch (Exception ex)
                    {
                        Application.Current?.Dispatcher.Invoke(() =>
                        {
                            var notifierVm = ClassLocator.Default.GetInstance<UI.ViewModel.NotifierVM>();
                            notifierVm.AddMessage(new Tools.Service.Message(Tools.Service.MessageLevel.Error, $"Signal Decoding Error : {ex.Message}"));
                        });
                        continue;
                    }
                
                    lastSat = rawFFTSignal.Saturation;
                    var chartPoints = new List<SeriesPoint>(rawFFTSignal.PointsCount);

                    for (int p = 0; p < rawFFTSignal.PointsCount; p++)
                    {
                        chartPoints.Add(new SeriesPoint(rawFFTSignal.XValues[p], rawFFTSignal.YValues[p]));
                    }

                    var pointLineSeries = new PointLineSeries(Chart.ViewXY, GetAxisX(), GetAxisY())
                    {
                        Highlight = Highlight.None,
                        Title = new SeriesTitle { Text = $"{repetaIndex + 1} : Sat {rawFFTSignal.Saturation}%", Color = Color.FromRgb(10, 10, 10) },
                        LineStyle =
                        {
                            Color = Color.FromRgb(r, g, b)
                        }
                    };

                    Chart.ViewXY.PointLineSeries.Add(pointLineSeries);
                    pointLineSeries.AddPoints(chartPoints.ToArray(), false);

                    repetaIndex++;
                }

                if (IsFirstGeneration)
                {
                    Chart.ViewXY.ZoomToFit();
                    IsFirstGeneration = false;
                }

                Chart.ViewXY.LegendBoxes[0].Visible = repetaIndex > 1;
                if(repetaIndex == 1)
                {
                    var annotatioSaturation = new LightningChartLib.WPF.Charting.Annotations.AnnotationXY()
                    {
                        Visible = true,
                        LocationCoordinateSystem = CoordinateSystem.ScreenCoordinates,
                        AllowTargetMove = false,
                        AllowUserInteraction = false,
                        AllowResize = false,
                        AllowDragging = false,
                        Anchor = new PointDoubleXY(0, 0),
                        Fill = new Fill() { GradientFill = GradientFill.Solid, Color = Color.FromArgb(37,238,238,238) },
                        AutoSizePadding = 5,
                        BorderVisible = false,
                        Highlight = Highlight.None,
                        Text = $"Sat = {lastSat} % ",
                        Shadow = new Shadow(){Visible = true, Color = Color.FromArgb( 80, 120, 120, 120)},
                        RenderBehindAxes = false,
                        Style = AnnotationStyle.Rectangle
                    };

                    annotatioSaturation.LocationScreenCoords.X = Chart.ActualWidth - _annotationLocXFromRight;
                    annotatioSaturation.LocationScreenCoords.Y = _annotationLocYFromTop;

                    Chart.ViewXY.Annotations.Add(annotatioSaturation);
                }
            });
        }

        #endregion
    }
}
