using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosCppWrapper;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.Common.Interpolation
{
    public struct IntPoint
    {
        public int X { get; }

        public int Y { get; }

        public IntPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(IntPoint point1, IntPoint point2) => point1.X == point2.X && point1.Y == point2.Y;

        public static bool operator !=(IntPoint point1, IntPoint point2) => !(point1 == point2);

        public static bool Equals(IntPoint point1, IntPoint point2) => point1.X.Equals(point2.X) && point1.Y.Equals(point2.Y);

        public override bool Equals(object o) => o is IntPoint point2 && IntPoint.Equals(this, point2);

        public bool Equals(IntPoint value) => IntPoint.Equals(this, value);

        public override int GetHashCode()
        {
            int hashCode1 = X.GetHashCode();
            int hashCode2 = Y.GetHashCode();
            return hashCode1 ^ hashCode2;
        }
    }

    public class PointInterpolationResult
    {
        public int IntensityGridY { get; set; }
        public int IntensityGridX { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Value { get; set; }
    }

    public enum InterpolatorState
    {
        NotStarted = 0,
        InProgress = 1,
        IsCancellationRequested = 2,
        EmptyValidMeasures = 3,
        SetInputsPointsFailure = 4,
        ComputeDataFailure = 5,
        InterpolationFailure = 6,
        InterpolationSuccess = 7
    }
    
    public class InterpolationEngine<T>
    {
        #region Constants
        public const int DefaultHeatMapSide = 256;
        public const int DefaultHeatMapSide_Square = 50;
        public const int DefaultHeatMapSide_Circle = 256;
        #endregion

        #region Fields

        private CancellationTokenSource _tokenSource;
        private readonly Func<T, double> _getXPosition;
        private readonly Func<T, double> _getYPosition;
        private readonly Func<T, double?> _getValue;

        #endregion

        #region Properties
        
        public InterpolatorState State { get; private set; } = InterpolatorState.NotStarted;

        public double CurrentMinValue { get; private set; }

        public double CurrentMaxValue { get; private set; }

        public double? TargetValue { get; private set; }

        public int HeatMapSide { get; private set; } // The heat map is a square which dimension is: HeatMapSide * HeatMapSize

        public Dictionary<IntPoint, PointInterpolationResult> InterpolationResults { get; private set; }

        public LengthUnit Unit { get; }

        #endregion

        #region Events

        public event EventHandler InterpolationDone;

        #endregion

        public InterpolationEngine(Func<T, double> getXPosition, Func<T, double> getYPosition, Func<T, double?> getValue, LengthUnit unit, double? targetValue = null, int? heatmapsize = null)
        {
            _getXPosition = getXPosition;
            _getYPosition = getYPosition;
            _getValue = getValue;
            Unit = unit;
            TargetValue = targetValue;
            HeatMapSide = heatmapsize ?? DefaultHeatMapSide;
        }

        #region Methods

        public void Cancel()
        {
            _tokenSource?.Cancel();
        }
        public void ResetState()
        { 
             if(State != InterpolatorState.InProgress)
                State = InterpolatorState.NotStarted;
        }

        public async void InterpolateSquare(ICollection<T> points, double squareHeight, double squareWidth)
        {
            _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            Debug.WriteLine($"Interpolation S start time (HeatMap side: {HeatMapSide} for {points.Count} measures) - State = {State}");

            if (State != InterpolatorState.NotStarted)
            {
                stopWatch.Stop();
                Debug.WriteLine($"Interpolation S Previously done - resend results");
                InterpolationDone?.Invoke(this, EventArgs.Empty);
                return;
            }

            State = InterpolatorState.InProgress;

            double[] heatMapValues = new double[HeatMapSide * HeatMapSide];

            int gridW = HeatMapSide;
            int gridH = HeatMapSide;
            double xStart = 0;
            double yStart = 0;
            double xEnd = squareWidth;
            double yEnd = squareHeight;
            double xStep = (xEnd - xStart) / (gridW - 1);
            double yStep = (yEnd - yStart) / (gridH - 1);

            // NOTE RTI : ici l'engine ne store pas les interpolator2D ce qui est dommage
            // on peux imaginer faire le compute sur le circle et utiliser ce meme interpolator sur les die ce qui garantirai une cohérence visuel entre la vue wafer et la vue die
            // or ici on a une instance engine pour le wafer et aune autre pour le die
            // à defaut on pourrait allimentaire l'engine du die avec l'interpolator 2D du wafer une fois celui ci computed
            // => il faut reprendre aussi le circle il n'ai pas nécéssaire de recomputé un interpolator à qui on a dejà passé les inputs on l'utilise tt simplement
            
            // for die because we have normally few points and it is computed more often it is more effecient to use IDW alogrithm 
            var interpolationType = InterpolateAlgoType.IDW;
            //var interpolationType = InterpolateAlgoType.MBA;
            var interpolator = new Interpolator2D(interpolationType);

            if (interpolationType == InterpolateAlgoType.MBA)
            {     
                List<double> settings = new List<double>()
                {
                    xStart - 0.1,
                    yStart - 0.1,
                    xEnd + 0.1,
                    yEnd + 0.1,
                    3.0,
                    3.0
                };
                if (TargetValue.HasValue)
                    settings.Add(TargetValue.Value);

                if (!interpolator.InitSettings(settings.ToArray()))
                {
                    throw new Exception("Interpolator S MBA Init Settings Failure");
                }
            }

            var taskInterpGrid = TaskInterpGrid(points, token, interpolator, heatMapValues, gridW, gridH, xStart, xStep, yStart, yStep);

            taskInterpGrid.Start(TaskScheduler.Current);
            Task.WaitAll(taskInterpGrid);

            var resultState = taskInterpGrid.Result;
            if (resultState != InterpolatorState.InterpolationSuccess)
            {
                State = resultState;
                stopWatch.Stop();
                var elapsed = stopWatch.Elapsed;
                string elapsedMessage = $"{elapsed.Minutes:00}\':{elapsed.Seconds:00}\".{elapsed.Milliseconds / 10:00}";
                Debug.WriteLine($"Interpolation S Grid NOK within {elapsedMessage}");
                return;
            }

            await Task.Run(() =>
            {
                CurrentMinValue = double.MaxValue;
                CurrentMaxValue = double.MinValue;

                var tasks = new List<Task<Dictionary<IntPoint, PointInterpolationResult>>>();

                for (int i = 0; i < HeatMapSide; ++i)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    int gridX = i;

                    var task = new Task<Dictionary<IntPoint, PointInterpolationResult>>(() =>
                    {
                        if (token.IsCancellationRequested) return null;

                        var lineInterpolation = new Dictionary<IntPoint, PointInterpolationResult>();

                        for (int gridY = 0; gridY < HeatMapSide; ++gridY)
                        {
                            if (token.IsCancellationRequested) return null;

                            double x = gridX * xStep + xStart;
                            double y = gridY * yStep + yStart;

                            var pointInterpolation = new PointInterpolationResult
                            {
                                IntensityGridX = gridX,
                                IntensityGridY = gridY,
                                X = x,
                                Y = y,
                            };

                            lineInterpolation.Add(new IntPoint(gridX, gridY), pointInterpolation);

                            double value = heatMapValues[gridX + gridY * HeatMapSide];
                            pointInterpolation.Value = value;

                            if (value < CurrentMinValue) CurrentMinValue = value;
                            if (value > CurrentMaxValue) CurrentMaxValue = value;
                        }

                        return lineInterpolation;
                    });

                    tasks.Add(task);
                    task.Start(TaskScheduler.Current);
                }

                // Wait for all the tasks to finish.
                Task.WaitAll(tasks.Cast<Task>().ToArray());
                InterpolationResults = tasks.SelectMany(task => task.Result).ToDictionary(pair => pair.Key, pair => pair.Value);

                stopWatch.Stop();

                var ts = stopWatch.Elapsed;
                string elapsedTime = $"{ts.Minutes:00}\':{ts.Seconds:00}\".{ts.Milliseconds / 10:00}";
                Debug.WriteLine($"Interpolation S time {elapsedTime} (HeatMap side: {HeatMapSide} for {points.Count} measures [{interpolationType}])");

                State = InterpolatorState.InterpolationSuccess;
                InterpolationDone?.Invoke(this, EventArgs.Empty);
            }, token).ConfigureAwait(false);
        }

        public void InterpolateCircle(ICollection<T> points, double diameterMillimeters)
        {
            _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            Debug.WriteLine($"Interpolation C start time (HeatMap side: {HeatMapSide} for {points.Count} measures)");

            if (State != InterpolatorState.NotStarted)
            {
                stopWatch.Stop();
                Debug.WriteLine($"Interpolation C Previously done - resend results");
                InterpolationDone?.Invoke(this, EventArgs.Empty);
                return;
            }

            State = InterpolatorState.InProgress;

            double[] heatMapValues = new double[HeatMapSide * HeatMapSide];

            double radius = diameterMillimeters / 2;

            int gridW = HeatMapSide;
            int gridH = HeatMapSide;
            double xStart = -radius;
            double yStart = -radius;
            double xEnd = radius;
            double yEnd = radius;
            double xStep = (xEnd - xStart) / (gridW - 1);
            double yStep = (yEnd - yStart) / (gridH - 1);

            //var interpolationType = InterpolateAlgoType.IDW;
            var interpolationType = InterpolateAlgoType.MBA;
            var interpolator = new Interpolator2D(interpolationType);

            if (interpolationType == InterpolateAlgoType.MBA)
            {
                List<double> settings = new List<double>()
                {
                    -radius - 0.1,
                    -radius - 0.1,
                    radius + 0.1,
                    radius + 0.1,
                    3.0,
                    3.0
                };
                if (TargetValue.HasValue)
                    settings.Add(TargetValue.Value);
                var settingsarray = settings.ToArray();
                if (!interpolator.InitSettings(settingsarray))
                {
                    throw new Exception("Interpolator C MBA Init Settings Failure");
                }
            }

            var taskInterpGrid = TaskInterpGrid(points, token, interpolator, heatMapValues, gridW, gridH, xStart, xStep, yStart, yStep);

            taskInterpGrid.Start(TaskScheduler.Current);
            Task.WaitAll(taskInterpGrid);

            var resultState = taskInterpGrid.Result;
            if (resultState != InterpolatorState.InterpolationSuccess)
            {
                State = resultState;
                stopWatch.Stop();
                var elapsed = stopWatch.Elapsed;
                string elapsedMessage = $"{elapsed.Minutes:00}\':{elapsed.Seconds:00}\".{elapsed.Milliseconds / 10:00}";
                Debug.WriteLine($"Interpolation C Grid NOK within {elapsedMessage} with state: {resultState}");
                return;
            }

            Task.Factory.StartNew(() =>
            {
                CurrentMinValue = double.MaxValue;
                CurrentMaxValue = double.MinValue;
                
                var tasks = new List<Task<Dictionary<IntPoint, PointInterpolationResult>>>();

                for (int i = 0; i < HeatMapSide; ++i)
                {
                    if (token.IsCancellationRequested) return;

                    int gridX = i;

                    var task = new Task<Dictionary<IntPoint, PointInterpolationResult>>(() =>
                    {
                        if (token.IsCancellationRequested) return null;

                        var lineInterpolation = new Dictionary<IntPoint, PointInterpolationResult>();

                        for (int gridY = 0; gridY < HeatMapSide; ++gridY)
                        {
                            if (token.IsCancellationRequested) return null;

                            double x = gridX * xStep + xStart;
                            double y = gridY * yStep + yStart;

                            var pointInterpolation = new PointInterpolationResult
                            {
                                IntensityGridX = gridX,
                                IntensityGridY = gridY, 
                                X = x, 
                                Y = y,
                            };

                            lineInterpolation.Add(new IntPoint(gridX, gridY), pointInterpolation);
                            
                            double value = heatMapValues[gridX + gridY * HeatMapSide];
                            pointInterpolation.Value = value;

                            if (value < CurrentMinValue) CurrentMinValue = value;
                            if (value > CurrentMaxValue) CurrentMaxValue = value;
                        }

                        return lineInterpolation;
                    });

                    tasks.Add(task);
                    task.Start(TaskScheduler.Current);
                }

                // Wait for all the tasks to finish.
                Task.WaitAll(tasks.Cast<Task>().ToArray());
                InterpolationResults = tasks.SelectMany(task => task.Result).ToDictionary(pair => pair.Key, pair => pair.Value);

                stopWatch.Stop();

                var ts = stopWatch.Elapsed;
                string elapsedTime = $"{ts.Minutes:00}\':{ts.Seconds:00}\".{ts.Milliseconds / 10:00}";
                Debug.WriteLine($"Interpolation C time {elapsedTime} (HeatMap side: {HeatMapSide} for {points.Count} measures [{interpolationType}])");

                State = InterpolatorState.InterpolationSuccess;
                InterpolationDone?.Invoke(this, EventArgs.Empty);
            }, token);
        }

        private Task<InterpolatorState> TaskInterpGrid(ICollection<T> points, CancellationToken token, Interpolator2D interpolator, double[] heatMapValues, int gridW, int gridH, double xStart, double xStep, double yStart, double yStep)
        {
            var taskInterpGrid = new Task<InterpolatorState>(() =>
            {
                double[] measuresX = null;
                double[] measuresY = null;
                double[] measuresValue = null;
                if (points.Count != 0)
                {
                    var validMeasures = points.Where(x => _getValue(x).HasValue).ToList();
                    if (token.IsCancellationRequested) return InterpolatorState.IsCancellationRequested;
                    if (validMeasures.Count < 1)
                    {
                        measuresX = new double[1] { 0.0 };
                        measuresY = new double[1] { 0.0 };
                        measuresValue = new double[1] { TargetValue ?? 0.0 };
                        //return InterpolatorState.EmptyValidMeasures;
                    }
                    else
                    {
                        measuresX = validMeasures.Select(x => _getXPosition(x)).ToArray();
                        if (token.IsCancellationRequested) return InterpolatorState.IsCancellationRequested;

                        measuresY = validMeasures.Select(x => _getYPosition(x)).ToArray();
                        if (token.IsCancellationRequested) return InterpolatorState.IsCancellationRequested;

                        measuresValue = validMeasures.Select(x =>
                        {
                            double? value = _getValue(x);
                            if (value.HasValue) return value.Value;
                            return 0.0;
                        }).ToArray();
                    }
                }

                if (token.IsCancellationRequested) return InterpolatorState.IsCancellationRequested;

                if (!interpolator.SetInputsPoints(measuresX, measuresY, measuresValue))
                {
                    Debug.WriteLine($"### WARNING : Interpolation SetInputsPoints failure ###");
                    return InterpolatorState.SetInputsPointsFailure;
                }

                if (token.IsCancellationRequested) return InterpolatorState.IsCancellationRequested;

                if (!interpolator.ComputeData())
                {
                    Debug.WriteLine($"### WARNING : Interpolation ComputeData failure ###");
                    return InterpolatorState.ComputeDataFailure;
                }

                if (token.IsCancellationRequested) return InterpolatorState.IsCancellationRequested;

                var pinGridMap = GCHandle.Alloc(heatMapValues, GCHandleType.Pinned);
                bool sucess = interpolator.InterpolateGrid((ulong)pinGridMap.AddrOfPinnedObject(), gridW, gridH, xStart, xStep, yStart, yStep);
                pinGridMap.Free();
                return sucess ? InterpolatorState.InterpolationSuccess : InterpolatorState.InterpolationFailure;
            });
            return taskInterpGrid;
        }

        private static double EuclideanDistance(double x1, double y1, double x2, double y2)
        {
            double xDist = x1 - x2;
            double yDist = y1 - y2;
            return Math.Sqrt(xDist * xDist + yDist * yDist);
        }

        [Obsolete]
        private void InterpolatePoint(T point, double xPointPosition, double yPointPosition, double x, double y, ref double sum, ref double sumWeights)
        {
            if (point == null) return;

            double? pointValue = _getValue(point);
            if (!pointValue.HasValue) return;

            double weight = GetWeight(xPointPosition, yPointPosition, x, y);

            sum += pointValue.Value * weight;
            sumWeights += weight;
        }

        [Obsolete]
        private static double GetWeight(double xPointPosition, double yPointPosition, double x, double y)
        {
            double distance = EuclideanDistance(x, y, xPointPosition, yPointPosition);
            return distance != 0 ? 1.0 / Math.Pow(distance, 3) : 1;
        }

        #endregion
    }
}
