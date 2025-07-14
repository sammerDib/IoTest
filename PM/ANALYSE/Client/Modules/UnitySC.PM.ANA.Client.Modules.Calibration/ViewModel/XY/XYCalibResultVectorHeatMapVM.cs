using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

using LightningChartLib.WPF.Charting;
using LightningChartLib.WPF.Charting.Maps;
using LightningChartLib.WPF.Charting.SeriesXY;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.PM.ANA.Service.Core.Referentials.Converters;
using UnitySC.PM.ANA.Service.Interface.Calibration;

using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Chart;
using UnitySC.Shared.UI.Helper;

namespace UnitySC.PM.ANA.Client.Modules.Calibration.ViewModel.XY
{
    public class XYCalibResultVectorHeatMapVM : BaseHeatMapChartVM
    {

        private CancellationTokenSource _tokenSource = null;
        private double _waferRadius;
        private string _title;
   
        #region HeatMap Stuff
        private const int HeatMapXYSize = 100; // heat map intensity grid size 
        private List<LineIntensityResult> _intensityResults;
      
        private class PointIntensityResult
        {
            public int IntensityGridY { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
            public double Value { get; set; }
        }
        private class LineIntensityResult
        {
            public int IntensityGridX { get; set; }

            public List<PointIntensityResult> IntensityResults { get; } = new List<PointIntensityResult>();
        }
        #endregion

        #region Advanced

        public bool DisplayAllMeasureAsVector { get; set; } = false;
        public double AdvancedSpecMax { get; set; } = 50.0;
        public double AdvancedMax { get; private set; } = double.MinValue;
        public double AdvancedMean { get; private set; } = double.MinValue;
        public double AdvancedInit { get; private set; } = double.MinValue;

        public enum CorrectionType
        {
            XY = 0,
            X = 1,
            Y = 2
        };
        public IEnumerable<CorrectionType> CorrectionTypes { get; private set; }
        private CorrectionType _selectedCorrectionType;
        public CorrectionType SelectedCorrectionType
        {
            get => _selectedCorrectionType; set { if (_selectedCorrectionType != value) { _selectedCorrectionType = value; OnPropertyChanged(); } }
        }

        public void RefreshDisplay()
        {
            Update(_xyCalibdata, SpecValueMin, AdvancedSpecMax); 
        }

        #endregion

        #region Vector Stuff
        private const int VectorXYSize = 21; // vector grid size (centered)
        public int GridVectorSize { get; set; }
        private List<LineVectorResult> _vectorResults;

        private class PointVectorResult
        {
            public int VectorGridY { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
            public double Angle { get; set; }
            public double Length { get; set; }
        }
        private class LineVectorResult
        {
            public int VectorGridX { get; set; }

            public List<PointVectorResult> VectorResults { get; } = new List<PointVectorResult>();
        }
        #endregion 

        public XYCalibrationData StageCorrectionApplied = null;

        private XYCalibrationData _xyCalibdata;

        public XYCalibResultVectorHeatMapVM()
            : base(HeatMapXYSize)
        {
            _title = "XY";
            GridVectorSize = VectorXYSize;

            CorrectionTypes = Enum.GetValues(typeof(CorrectionType)).Cast<CorrectionType>();
        }

        public XYCalibResultVectorHeatMapVM(int vectorGridSize, int heatMapGridSize = HeatMapXYSize)
           : base(heatMapGridSize)
        {
            _title = "XY";
            GridVectorSize = vectorGridSize;

            CorrectionTypes = Enum.GetValues(typeof(CorrectionType)).Cast<CorrectionType>();
        }

        public void Update(XYCalibrationData xyCalibData, double specMin, double specMax)
        {
            _xyCalibdata = xyCalibData;
            _waferRadius = _xyCalibdata.WaferCalibrationDiameter.Millimeters * 0.5;

            if (!xyCalibData.IsInterpReady)
                XYCalibrationCalcul.PreCompute(xyCalibData);

            SpecValueMin = specMin;
            SpecValueMax = specMax;
            if (AdvancedInit == double.MinValue)
            {
                AdvancedInit = SpecValueMax;
                ComputeMeanShift();
            }

            UpdateViewerType(HeatMapPaletteType.ZeroToMax);
        }

        private void FilterWaferShape(double waferRadius)
        {
            int circlePointCount = 2 * HeatMapSide;
            double angleStep = -Math.PI * 2.0 / circlePointCount;
            var circlePoints = new PointDouble2D[circlePointCount];

            for (int i = 0; i < circlePointCount; ++i)
            {
                circlePoints[i].X = waferRadius * Math.Cos(i * angleStep);
                circlePoints[i].Y = waferRadius * Math.Sin(i * angleStep);
            }

            var circle = new PolygonSeries(Chart.ViewXY, Chart.ViewXY.XAxes[0], Chart.ViewXY.YAxes[0])
            {
                ShowInLegendBox = false,
                AllowUserInteraction = false,
                Points = circlePoints,
                Fill =
                {
                    Style = RectFillStyle.None
                },
                Border =
                {
                    Color = Color.FromArgb(0xff, 0x29, 0x3D, 0x29),
                    Width = 2
                }
            };

            Chart.ViewXY.PolygonSeries.Clear();
            Chart.ViewXY.PolygonSeries.Add(circle);

            // Filter HeatMap with wafer shape
            var stencilArea = new StencilArea(HeatMap.Stencil);
            stencilArea.AddPolygon(circlePoints);
            HeatMap.Stencil.AdditiveAreas.Clear();
            HeatMap.Stencil.AdditiveAreas.Add(stencilArea);
        }

        protected static double EuclideanDistance(double x1, double y1, double x2, double y2)
        {
            double xDist = x1 - x2;
            double yDist = y1 - y2;
            return Math.Sqrt(xDist * xDist + yDist * yDist);
        }

        protected void UpdateViewerType(HeatMapPaletteType paletteType)
        {
            SetPaletteType(paletteType);
            UpdateChartData();
        }

        protected void UpdateChartData()
        {
            _tokenSource?.Cancel();
          
            CurrentMinValue = double.MaxValue;
            CurrentMaxValue = double.MinValue;

            IsBusy = true;

            UpdateChart(() =>
            {
                HeatMap.SetRangesXY(-_waferRadius, _waferRadius, -_waferRadius, _waferRadius);
                HeatMap.Title.Text = _title;
                FilterWaferShape(_waferRadius);
                Chart.ViewXY.ZoomToFit();
            });

            _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;

            TaskHelper.DoAsyncOnSystemIdle(() =>
            {
                FeedDataMap(token);
            }, () =>
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                OnFeedDataDone();
            });
        }


        public static void ApplyWaferCenteredAntiClockwiseRotation(Angle angle, XYPosition position)
        {
            if (angle.Degrees != 0)
            {
                angle = -angle;
                var x = position.X;
                var y = position.Y;
                double phi = Math.Atan2(y, x) - angle.Radians;
                double r = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
                position.X = r * Math.Cos(phi);
                position.Y = r * Math.Sin(phi);
            }
        }

        private XYPosition ConvertMotorToWafer(XYPosition xyPosition, bool applyStageCorrections)
        {
            var position = xyPosition.Clone() as XYPosition;

            if (applyStageCorrections)
            {
                var correction = XYCalibrationHelper.ComputeCorrection(position.X, position.Y, StageCorrectionApplied);
                position.X -= correction.Item1.Millimeters;
                position.Y -= correction.Item2.Millimeters;
            }

            // Note de rti pour être plus precis ajouter ici le decalge des objectif centricities (ici en x5 on a un decalage de 0 donc on néglige pour le moment)
            //  position.X += objectiveCalibration.Image.XOffsetum.Millimeters;
            //  position.Y += objectiveCalibration.Image.YOffsetum.Millimeters;

            position.X -= _xyCalibdata.ShiftX.Millimeters;
            position.Y -= _xyCalibdata.ShiftY.Millimeters;
            ApplyWaferCenteredAntiClockwiseRotation(-_xyCalibdata.ShiftAngle, position);

            return position;
        }

        private XYPosition ConvertWaferToMotor(XYPosition xyPosition, bool applyStageCorrections)
        {
            var position = xyPosition.Clone() as XYPosition;
            ApplyWaferCenteredAntiClockwiseRotation(_xyCalibdata.ShiftAngle, position);
            position.X += _xyCalibdata.ShiftX.Millimeters;
            position.Y += _xyCalibdata.ShiftY.Millimeters;

            // Note de rti pour être plus precis ajouter ici le decalge des objectif centricities (ici en x5 on a un decalage de 0 donc on néglige pour le moment)
            //  position.X -= objectiveCalibration.Image.XOffsetum.Millimeters;
            //  position.Y -= objectiveCalibration.Image.YOffsetum.Millimeters;

            if (applyStageCorrections)
            {
               var correction = XYCalibrationHelper.ComputeCorrection(position.X, position.Y, StageCorrectionApplied);
               position.X += correction.Item1.Millimeters;
               position.Y += correction.Item2.Millimeters;
            }

            return position;
        }

        private void ComputeMeanShift()
        {
            if (_xyCalibdata.Corrections.Count > 0)
            {
                double sumDist = 0.0;
                _xyCalibdata.Corrections.ForEach(mes =>
                {
                    double shitdist = EuclideanDistance(mes.ShiftX.GetValueAs(XYCalibrationData.CorrectionUnit), mes.ShiftY.GetValueAs(XYCalibrationData.CorrectionUnit), 0.0, 0.0);
                    sumDist += shitdist;
                });
                AdvancedMean = sumDist / _xyCalibdata.Corrections.Count();
            }
        }


        private void FeedDataMap(CancellationToken token)
        {
            bool applyStageCorrection = StageCorrectionApplied != null;

            //
            // Tasks Feed Intensity Distance HeatMap
            //
            double heatMapToWaferRatio = _waferRadius * 2.0 / HeatMapSide;
            var heatMapTasks = new List<Task<LineIntensityResult>>();
            int heatMapGrid = HeatMapSide; // to avoid changing changing grid size during tasks runs
            for (int i = 0; i < heatMapGrid; ++i)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                int intensityGridX = i;

                var task = new Task<LineIntensityResult>(() =>
                {
                    if (token.IsCancellationRequested) return null;

                    var lineInterpolation = new LineIntensityResult
                    {
                        IntensityGridX = intensityGridX
                    };

                    for (int j = 0; j < heatMapGrid; ++j)
                    {
                        if (token.IsCancellationRequested) return null;

                        double x = intensityGridX * heatMapToWaferRatio - _waferRadius;
                        double y = j * heatMapToWaferRatio - _waferRadius;

                        var pointInterpolation = new PointIntensityResult
                        {
                            IntensityGridY = j,
                            X = x,
                            Y = y
                        };

                        lineInterpolation.IntensityResults.Add(pointInterpolation);

                        // Exclude data points that are not in the wafer circle + a security margin
                        if (EuclideanDistance(x, y, 0.0, 0.0) > (_waferRadius + 2.0))
                        {
                            pointInterpolation.Value = double.NaN;
                            continue;
                        }

                        // wafer to motor referential
                        var motorPosition = ConvertWaferToMotor( new XYPosition(new WaferReferential(), x, y), applyStageCorrection);
                        var Shifts = XYCalibrationHelper.ComputeCorrection(motorPosition.X, motorPosition.Y, _xyCalibdata);
                        if (SelectedCorrectionType != CorrectionType.XY)
                        {
                            bool useXOnly = SelectedCorrectionType == CorrectionType.X;
                            Shifts = new Tuple<Length, Length>(useXOnly ? Shifts.Item1 : 0.Micrometers(), useXOnly ? 0.Micrometers() : Shifts.Item2);
                        }

                        double value = EuclideanDistance(Shifts.Item1.GetValueAs(XYCalibrationData.CorrectionUnit), Shifts.Item2.GetValueAs(XYCalibrationData.CorrectionUnit),0.0,0.0); 
                      
                        pointInterpolation.Value = value;
                        if (value < CurrentMinValue) CurrentMinValue = value;
                        if (value > CurrentMaxValue) CurrentMaxValue = value;
                    }

                    return lineInterpolation;
                });
                heatMapTasks.Add(task);
                task.Start(TaskScheduler.Current);
            }

            var vectorsTasks = new List<Task<LineVectorResult>>();
            if (!DisplayAllMeasureAsVector)
            {
                //
                // Tasks Feed Vectors arrows
                //
                double vectorsToWaferRatio = _waferRadius * 2.0 / GridVectorSize;
                double vMargin = vectorsToWaferRatio * 0.5; // to center teh grid , if gris is an odd number, an arrow will be placced at wafer center
                int gridVSize = GridVectorSize; // to avoid changing changing grid size during tasks runs
                                                //double vFactor = Math.Sqrt(2)*vectorsToWaferRatio / SpecValueMax;
                double vFactor = Math.Sqrt(2) * _waferRadius * 0.1 / SpecValueMax;
                for (int i = 0; i < gridVSize; ++i)
                {
                    if (token.IsCancellationRequested) return;

                    int vGridX = i;
                    var task = new Task<LineVectorResult>(() =>
                    {
                        if (token.IsCancellationRequested) return null;

                        var lineVectors = new LineVectorResult
                        {
                            VectorGridX = vGridX
                        };

                        for (int j = 0; j < GridVectorSize; ++j)
                        {
                            if (token.IsCancellationRequested) return null;

                            double x = vMargin + vGridX * vectorsToWaferRatio - _waferRadius;
                            double y = vMargin + j * vectorsToWaferRatio - _waferRadius;

                            var pointVector = new PointVectorResult
                            {
                                VectorGridY = j,
                                X = x,
                                Y = y
                            };

                            lineVectors.VectorResults.Add(pointVector);

                            // Exclude data points that are not in the wafer circle + a security margin
                            if (EuclideanDistance(x, y, 0.0, 0.0) > (_waferRadius + 0.25))
                            {
                                pointVector.Length = double.NaN;
                                pointVector.Angle = double.NaN;
                                continue;
                            }

                            // wafer to motor referential
                            var motorPosition = ConvertWaferToMotor(new XYPosition(new WaferReferential(), x, y), applyStageCorrection);
                            var Shifts = XYCalibrationHelper.ComputeCorrection(motorPosition.X, motorPosition.Y, _xyCalibdata);
                            if (SelectedCorrectionType != CorrectionType.XY)
                            {
                                bool useXOnly = SelectedCorrectionType == CorrectionType.X;
                                Shifts = new Tuple<Length, Length>(useXOnly ? Shifts.Item1 : 0.Micrometers(), useXOnly ? 0.Micrometers() : Shifts.Item2);
                            }
                            // case du vector on calcul la norme
                            double dcX = Shifts.Item1.GetValueAs(XYCalibrationData.CorrectionUnit);
                            double dcY = Shifts.Item2.GetValueAs(XYCalibrationData.CorrectionUnit);
                            double value = EuclideanDistance(dcX, dcY, 0.0, 0.0);

                            pointVector.Length = vFactor * value;
                            pointVector.Angle = Math.Atan2(dcY, dcX);

                            if (value < CurrentMinValue) CurrentMinValue = value;
                            if (value > CurrentMaxValue) CurrentMaxValue = value;
                        }

                        return lineVectors;
                    });

                    vectorsTasks.Add(task);
                    task.Start(TaskScheduler.Current);
                }
            }
                
            // Wait for all the Heat Map tasks  to finish.
            Task.WaitAll(heatMapTasks.Cast<Task>().ToArray());
            _intensityResults = heatMapTasks.Select(task => task.Result).ToList();

            // Wait for all the Vector tasks  to finish.
            Task.WaitAll(vectorsTasks.Cast<Task>().ToArray());
            _vectorResults = vectorsTasks.Select(task => task.Result).ToList();

            AdvancedMax = CurrentMaxValue; 
        }


        private void OnFeedDataDone()
        {
            CurrentMaxValue = SpecValueMax;
            UpdatePalette();
            ApplyHeatMapIntensities();
            ApplyVectorAnnotations();
            IsBusy = false;
        }

        private void ApplyHeatMapIntensities()
        {
            if (_intensityResults == null) return;

            foreach (var line in _intensityResults)
            {
                foreach (var point in line.IntensityResults)
                {
                    HeatMap.Data[line.IntensityGridX, point.IntensityGridY].X = point.X;
                    HeatMap.Data[line.IntensityGridX, point.IntensityGridY].Y = point.Y;
                    HeatMap.Data[line.IntensityGridX, point.IntensityGridY].Value = point.Value;
                }
            }

            // Notify chart about updated data.
            HeatMap.InvalidateValuesDataOnly();

            _intensityResults = null; 
        
        }

        private void ApplyVectorAnnotations()
        {
            if (DisplayAllMeasureAsVector)
            {
                // affichage des vector de mesure sur les point de correction
                double vectorsToWaferRatio = _waferRadius * 2.0 / GridVectorSize;
                double vMargin = vectorsToWaferRatio * 0.5; // to center teh grid , if gris is an odd number, an arrow will be placced at wafer center
                double vFactor = Math.Sqrt(2) * _waferRadius * 0.1 / SpecValueMax;
                _vectorResults = new List<LineVectorResult>
                {
                    new LineVectorResult()
                };
                var lvr = _vectorResults.Last();

                bool applyStageCorrection = StageCorrectionApplied != null;

                foreach (var corr in _xyCalibdata.Corrections)
                {
                    var motorPosition = new XYPosition(new MotorReferential(), corr.XTheoricalPosition.Millimeters, corr.YTheoricalPosition.Millimeters);
                    var WaferPosition = ConvertMotorToWafer(motorPosition, applyStageCorrection);

                    var pointVector = new PointVectorResult
                    {
                        X = WaferPosition.X,
                        Y = WaferPosition.Y
                    };

                    lvr.VectorResults.Add(pointVector);

                    // Exclude data points that are not in the wafer circle + a security margin
                    if (EuclideanDistance(WaferPosition.X, WaferPosition.Y, 0.0, 0.0) > (_waferRadius + 0.25))
                    {
                        pointVector.Length = double.NaN;
                        pointVector.Angle = double.NaN;
                        continue;
                    }

                    // wafer to motor referential
                    var Shifts = XYCalibrationHelper.ComputeCorrection(motorPosition.X, motorPosition.Y, _xyCalibdata);
                    if (SelectedCorrectionType != CorrectionType.XY)
                    {
                        bool useXOnly = SelectedCorrectionType == CorrectionType.X;
                        Shifts = new Tuple<Length, Length>(useXOnly ? Shifts.Item1 : 0.Micrometers(), useXOnly ? 0.Micrometers() : Shifts.Item2);
                    }
                    // case du vector on calcul la norme
                    double dcX = Shifts.Item1.GetValueAs(XYCalibrationData.CorrectionUnit);
                    double dcY = Shifts.Item2.GetValueAs(XYCalibrationData.CorrectionUnit);
                    double value = EuclideanDistance(dcX, dcY, 0.0, 0.0);

                    pointVector.Length = vFactor * value;
                    pointVector.Angle = Math.Atan2(dcY, dcX);

                }
            }

            UpdateChart(() =>
            {
                Chart.ViewXY.Annotations.Clear();
                foreach (var line in _vectorResults)
                {
                    foreach (var point in line.VectorResults)
                    {
                        if (double.IsNaN(point.Length))
                            continue;

                        var vector = new LightningChartLib.WPF.Charting.Annotations.AnnotationXY(Chart.ViewXY, Chart.ViewXY.XAxes[0], Chart.ViewXY.YAxes[0])
                        {
                            Style = AnnotationStyle.Arrow,
                            AllowUserInteraction = false
                        };
                        vector.TextStyle.Visible = false;
                        vector.ArrowStyleBegin = ArrowStyle.None;
                        vector.ArrowStyleEnd = ArrowStyle.Caliper;
                        vector.LocationCoordinateSystem = CoordinateSystem.AxisValues;

                        double beginX = point.X;
                        double beginY = point.Y;
                        double endX = beginX + Math.Cos(point.Angle) * point.Length;
                        double endY = beginY + Math.Sin(point.Angle) * point.Length;
                        // Change color.
                        //vector.ArrowLineStyle.Color = Color.FromRgb(255,242,0);
                        vector.ArrowLineStyle.Color = Color.FromRgb(15, 15, 100);
                        // Change width.
                        //vector.ArrowLineStyle.Width = (float)arrowLineWidth;
                        // Change location as axis values.
                        vector.LocationAxisValues.SetValues(beginX, beginY);
                        // Change target as axis values.
                        vector.TargetAxisValues.SetValues(endX, endY);

                        if (point.Length == 0)
                        {
                            vector.Style = AnnotationStyle.Ellipse;
                            vector.Shadow.Visible= false;
                            vector.Sizing = AnnotationXYSizing.ScreenCoordinates;
                            vector.SizeScreenCoords.Width = 3;
                            vector.SizeScreenCoords.Height = 3;
                            vector.Fill.GradientFill = GradientFill.Solid; 
                            vector.Fill.Color = Color.FromRgb(15, 15, 100);
                            vector.BorderLineStyle.Color = Color.FromRgb(15, 15, 100);
                        }

                        Chart.ViewXY.Annotations.Add(vector);
                    }
                }
            });

        }

        protected override void OnMarkerClicked(object sender, MouseEventArgs e)
        {
            // nothing to do yet
        }
    }
}
