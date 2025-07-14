using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using LightningChartLib.WPF.Charting;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using Microsoft.Win32;

using UnitySC.Shared.Format.HAZE;
using UnitySC.Shared.ResultUI.HAZE.View.WaferDetails;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.Shared.ResultUI.HAZE.ViewModel.WaferDetails
{
    public class HazeMapVM : ObservableRecipient
    {
        private readonly Func<HazeMap> _getCurrentHazeMap;
        private readonly HazeProfileChartVM _hazeProfileChartVm;

        public HazeMapVM(Func<HazeMap> getCurrentHazeMap, HazeProfileChartVM hazeProfileChartVm)
        {
            _getCurrentHazeMap = getCurrentHazeMap;
            _hazeProfileChartVm = hazeProfileChartVm;
        }

        #region Properties
        
        private ImageSource _img;

        public ImageSource HazeImage
        {
            get { return _img; }
            set { SetProperty(ref _img, value); }
        }

        public string ExportFileName { get; set; }

        private HazeMapTool _currentTool = HazeMapTool.Move;

        public HazeMapTool CurrentTool
        {
            get { return _currentTool; }
            set
            {
                if (SetProperty(ref _currentTool, value) && _currentTool != HazeMapTool.Move)
                {
                    CurrentProfileTool = _currentTool;
                }
            }
        }

        private HazeMapTool _currentProfileTool = HazeMapTool.LineProfile;

        public HazeMapTool CurrentProfileTool
        {
            get { return _currentProfileTool; }
            private set 
            {
                if (SetProperty(ref _currentProfileTool, value))
                {
                    if (_currentProfileTool == HazeMapTool.LineProfile)
                    {
                        LineProfileVisibility = Visibility.Visible;
                        CrossProfileVisibility = Visibility.Collapsed;
                        UpdateProfile();
                    }
                    else if (_currentProfileTool == HazeMapTool.CrossProfile)
                    {
                        LineProfileVisibility = Visibility.Collapsed;
                        CrossProfileVisibility = Visibility.Visible;
                        UpdateCrossProfile();
                    }
                    ExportCsvProfileCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private Visibility _lineProfileVisibility = Visibility.Visible;

        public Visibility LineProfileVisibility
        {
            get { return _lineProfileVisibility; }
            private set { SetProperty(ref _lineProfileVisibility, value); }
        }

        private Visibility _crossProfileVisibility = Visibility.Collapsed;

        public Visibility CrossProfileVisibility
        {
            get { return _crossProfileVisibility; }
            private set { SetProperty(ref _crossProfileVisibility, value); }
        }

        #region Coordinates

        private int? _mouseOverX;

        public int? MouseOverX
        {
            get { return _mouseOverX; }
            set
            {
                if (SetProperty(ref _mouseOverX, value))
                {
                    OnPropertyChanged(nameof(MouseOverValue));
                }
            }
        }

        private int? _mouseOverY;

        public int? MouseOverY
        {
            get { return _mouseOverY; }
            set
            {
                if (SetProperty(ref _mouseOverY, value))
                {
                    OnPropertyChanged(nameof(MouseOverValue));
                }
            }
        }

        public float? MouseOverValue
        {
            get
            {
                var hazeMap = _getCurrentHazeMap();
                if (hazeMap == null) return null;
                if (MouseOverX == null || MouseOverY == null) return null;
                return GetValue(MouseOverX.Value, MouseOverY.Value);
            }
        }

        private int? _startPointX;

        public int? StartPointX
        {
            get { return _startPointX; }
            set
            {
                if (SetProperty(ref _startPointX, value))
                {
                    UpdateProfile();
                }
            }
        }

        private int? _startPointY;

        public int? StartPointY
        {
            get { return _startPointY; }
            set
            {
                if (SetProperty(ref _startPointY, value))
                {
                    UpdateProfile();
                }
            }
        }

        private int? _endPointX;

        public int? EndPointX
        {
            get { return _endPointX; }
            set
            {
                if (SetProperty(ref _endPointX, value))
                {
                    UpdateProfile();
                }
            }
        }

        private int? _endPointY;

        public int? EndPointY
        {
            get { return _endPointY; }
            set
            {
                if (SetProperty(ref _endPointY, value))
                {
                    UpdateProfile();
                }
            }
        }

        private int? _crossProfileX;

        public int? CrossProfileX
        {
            get { return _crossProfileX; }
            set
            {
                if (SetProperty(ref _crossProfileX, value))
                {
                    UpdateCrossProfile();
                }
            }
        }

        private int? _crossProfileY;

        public int? CrossProfileY
        {
            get { return _crossProfileY; }
            set
            {
                if (SetProperty(ref _crossProfileY, value))
                {
                    UpdateCrossProfile();
                }
            }
        }

        private double _markerX;

        public double MarkerX
        {
            get { return _markerX; }
            private set { SetProperty(ref _markerX, value); }
        }

        private double _markerY;

        public double MarkerY
        {
            get { return _markerY; }
            private set { SetProperty(ref _markerY, value); }
        }

        private double _horizontalMarkerX;

        public double HorizontalMarkerX
        {
            get { return _horizontalMarkerX; }
            set { SetProperty(ref _horizontalMarkerX, value); }
        }

        private double _verticalMarkerY;

        public double VerticalMarkerY
        {
            get { return _verticalMarkerY; }
            set { SetProperty(ref _verticalMarkerY, value); }
        }

        #endregion

        #region Stats

        private float? _minProfile;

        public float? MinProfile
        {
            get { return _minProfile; }
            private set { SetProperty(ref _minProfile, value); }
        }

        private float? _maxProfile;

        public float? MaxProfile
        {
            get { return _maxProfile; }
            private set { SetProperty(ref _maxProfile, value); }
        }

        private float? _meanProfile;

        public float? MeanProfile
        {
            get { return _meanProfile; }
            private set { SetProperty(ref _meanProfile, value); }
        }

        private float? _stdDevProfile;

        public float? StdDevProfile
        {
            get { return _stdDevProfile; }
            private set { SetProperty(ref _stdDevProfile, value); }
        }

        #endregion
        
        /// <summary>
        /// Point attached to profile distance map
        /// </summary>
        private readonly List<Point> _profilePoints = new List<Point>();

        private readonly List<Point> _horizontalCrossProfilePoints = new List<Point>();
        private readonly List<Point> _verticalCrossProfilePoints = new List<Point>();

        #endregion

        #region Events

        public event EventHandler OnDataChanged;

        public void RaiseDataChanged()
        {
            OnDataChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Commands

        private AutoRelayCommand _resetProfileCommand;

        public AutoRelayCommand ResetProfileCommand => _resetProfileCommand ?? (_resetProfileCommand = new AutoRelayCommand(ClearProfile));

        private AutoRelayCommand _exportCsvProfileCommand;

        public AutoRelayCommand ExportCsvProfileCommand => _exportCsvProfileCommand ?? (_exportCsvProfileCommand = new AutoRelayCommand(ExportCsvProfileCommandExecute, ExportCsvProfileCommandCanExecute));

        private bool ExportCsvProfileCommandCanExecute()
        {
            switch (CurrentProfileTool)
            {
                case HazeMapTool.LineProfile:
                    return _hazeProfileChartVm?.Points != null && _hazeProfileChartVm.Points.Any();
                case HazeMapTool.CrossProfile:
                    return _hazeProfileChartVm?.Points != null && _hazeProfileChartVm.Points.Any() &&
                           _hazeProfileChartVm?.Points2 != null && _hazeProfileChartVm.Points2.Any();
            }
            return false;
        }

        private void ExportCsvProfileCommandExecute()
        {
            var notifierVm = ClassLocator.Default.GetInstance<NotifierVM>();
            string filePath = string.Empty;

            try
            {
                // TODO FileName
                var dialog = new SaveFileDialog
                {
                    Filter = "CSV file (*.csv)|*.csv| All Files (*.*)|*.*",
                    FileName = $"{ExportFileName}_{CurrentProfileTool}.csv"
                };

                bool? dialogResult = dialog.ShowDialog();
                if (!dialogResult.HasValue || !dialogResult.Value) return;

                filePath = dialog.FileName;

                using (var writer = new StreamWriter(filePath, false))
                {
                    switch (CurrentProfileTool)
                    {
                        case HazeMapTool.LineProfile:
                            WriteLineProfile(writer);
                            break;
                        case HazeMapTool.CrossProfile:
                            WriteCrossProfile(writer);
                            break;
                    }
                }

                notifierVm.AddMessage(new Message(MessageLevel.Information, "Export profile : " + filePath + " was saved with success"));
            }
            catch (Exception)
            {
                notifierVm.AddMessage(new Message(MessageLevel.Error, "Unable to Export profile to " + filePath));
            }
        }

        private void WriteLineProfile(StreamWriter writer)
        {
            writer.WriteLine($"Profile start (px in image) : [{StartPointX} , {StartPointY}];");
            writer.WriteLine($"Profile end (px in image) : [{EndPointX} , {EndPointY}];");
            writer.WriteLine("------------");
            writer.WriteLine("");
            writer.Flush();

            writer.WriteLine("Distance (px); zValue (height unit);");

            foreach (var point in _hazeProfileChartVm.Points)
            {
                writer.WriteLine(point.X.ToString(CultureInfo.InvariantCulture) + ";" + point.Y.ToString(CultureInfo.InvariantCulture) + ";");
            }

            writer.Close();
        }

        private void WriteCrossProfile(StreamWriter writer)
        {
            writer.WriteLine($"Profile Cross center (px in image) : [{CrossProfileX} , {CrossProfileY}];");
            writer.WriteLine("------------");

            writer.WriteLine("Horizontal Profile;");
            writer.WriteLine("");
            writer.WriteLine("Distance (px); zValue (height unit);");
            foreach (var point in _hazeProfileChartVm.Points)
            {
                writer.WriteLine(point.X.ToString(CultureInfo.InvariantCulture) + ";" + point.Y.ToString(CultureInfo.InvariantCulture) + ";");
            }

            writer.WriteLine("");
            writer.WriteLine("------------");

            writer.WriteLine("Vertical Profile;");
            writer.WriteLine("");
            writer.WriteLine("Distance (px); zValue (height unit);");
            foreach (var point in _hazeProfileChartVm.Points2)
            {
                writer.WriteLine(point.X.ToString(CultureInfo.InvariantCulture) + ";" + point.Y.ToString(CultureInfo.InvariantCulture) + ";");
            }

            writer.Close();
        }

        #endregion

        #region Public Methods

        public void ClearProfile()
        {
            SetProperty(ref _startPointX, null, nameof(StartPointX));
            SetProperty(ref _startPointY, null, nameof(StartPointY));
            SetProperty(ref _endPointX, null, nameof(EndPointX));
            SetProperty(ref _endPointY, null, nameof(EndPointY));
            SetProperty(ref _crossProfileX, null, nameof(CrossProfileX));
            SetProperty(ref _crossProfileY, null, nameof(CrossProfileY));
            SetProperty(ref _markerX, double.NaN, nameof(MarkerX));
            SetProperty(ref _markerY, double.NaN, nameof(MarkerY));

            MinProfile = null;
            MaxProfile = null;
            MeanProfile = null;
            StdDevProfile = null;

            _profilePoints.Clear();
            _horizontalCrossProfilePoints.Clear();
            _verticalCrossProfilePoints.Clear();

            _hazeProfileChartVm.Clear();

            ExportCsvProfileCommand.NotifyCanExecuteChanged();
        }

        public void OnSelectedHazeMapChanged()
        {
            OnPropertyChanged(nameof(MouseOverValue));
            UpdateProfile();
        }

        public void UpdateMarkerPosition(int lineProfileMarkerIndex, int crossProfileHorizontalMarkerIndex, int crossProfileVerticalMarkerIndex)
        {
            UpdateLineProfileMarkerPosition(lineProfileMarkerIndex);
            UpdateCrossProfileHorizontalMarkerPosition(crossProfileHorizontalMarkerIndex);
            UpdateCrossProfileVerticalMarkerPosition(crossProfileVerticalMarkerIndex);
        }

        #endregion

        #region Private Methods

        private void UpdateLineProfileMarkerPosition(int markerIndex)
        {
            Point? point = null;
            if (markerIndex >= 0 && markerIndex < _profilePoints.Count)
            {
                point = _profilePoints[markerIndex];
            }

            if (point == null)
            {
                MarkerX = double.NaN;
                MarkerY = double.NaN;
            }
            else
            {
                MarkerX = point.Value.X;
                MarkerY = point.Value.Y;
            }
        }

        private void UpdateCrossProfileHorizontalMarkerPosition(int markerIndex)
        {
            Point? point = null;
            if (markerIndex >= 0 && markerIndex < _horizontalCrossProfilePoints.Count)
            {
                point = _horizontalCrossProfilePoints[markerIndex];
            }

            HorizontalMarkerX = point?.X ?? double.NaN;
        }

        private void UpdateCrossProfileVerticalMarkerPosition(int markerIndex)
        {
            Point? point = null;
            if (markerIndex >= 0 && markerIndex < _verticalCrossProfilePoints.Count)
            {
                point = _verticalCrossProfilePoints[markerIndex];
            }

            VerticalMarkerY = point?.Y ?? double.NaN;
        }

        private void HideProfile()
        {
            MinProfile = null;
            MaxProfile = null;
            MeanProfile = null;
            StdDevProfile = null;

            _hazeProfileChartVm.Clear();
        }
        
        private float GetValue(int x, int y)
        {
            var currentHazeMap = _getCurrentHazeMap();
            if (currentHazeMap == null) return float.NaN;
            return HazeUtils.GetValue(currentHazeMap.HazeMeasures, currentHazeMap.Width, currentHazeMap.Heigth, x, y);
        }

        private float GetValue(double x, double y)
        {
            int ix = (int)Math.Round(x, 0);
            int iy = (int)Math.Round(y, 0);
            return GetValue(ix, iy);
        }

        private void UpdateProfile()
        {
            bool calculatableLine = StartPointX.HasValue && StartPointY.HasValue && EndPointX.HasValue && EndPointY.HasValue;
            if (!calculatableLine)
            {
                HideProfile();
                return;
            }

            var line = HazeUtils.InterBresenhamLine(StartPointX.Value, StartPointY.Value, EndPointX.Value, EndPointY.Value);

            _profilePoints.Clear();

            if (line.Count <= 0)
            {
                HideProfile();
                return;
            }

            int nFirstNoNan = -1;

            float currentDistance = 0.0f;
            float minProfile = float.MaxValue;
            float maxProfile = float.MinValue;
            float meanProfile = 0.0f;

            var srcPoints = new SeriesPoint[line.Count];

            float minDist = 0;
            float maxDist = 0;
            var valueList = new List<float>();

            float zValue = GetValue(line[0].X, line[0].Y);

            _profilePoints.Add(line[0]);

            if (!float.IsNaN(zValue))
            {
                nFirstNoNan = 0;

                valueList.Add(zValue);
                meanProfile += zValue;

                if (zValue < minProfile) minProfile = zValue;
                if (zValue > maxProfile) maxProfile = zValue;
            }

            srcPoints[0] = new SeriesPoint
            {
                X = currentDistance,
                Y = nFirstNoNan != -1 ? zValue : double.NaN
            };

            for (int i = 1; i < line.Count; i++) // Loop through List with for
            {
                double distance = Math.Sqrt(Math.Pow(line[i].X - line[i - 1].X, 2) + Math.Pow(line[i].Y - line[i - 1].Y, 2));

                currentDistance += (float)distance;
                if (currentDistance < minDist) minDist = currentDistance;
                if (currentDistance > maxDist) maxDist = currentDistance;

                zValue = GetValue(line[i].X, line[i].Y);

                if (float.IsNaN(zValue) == false)
                {
                    if (nFirstNoNan == -1) nFirstNoNan = i;

                    valueList.Add(zValue);
                    meanProfile += zValue;

                    if (zValue < minProfile) minProfile = zValue;
                    if (zValue > maxProfile) maxProfile = zValue;

                    srcPoints[i] = new SeriesPoint
                    {
                        X = currentDistance,
                        Y = zValue
                    };
                }
                else
                {
                    srcPoints[i] = new SeriesPoint
                    {
                        X = currentDistance,
                        Y = double.NaN
                    };
                }

                _profilePoints.Add(line[i]);
            }

            _hazeProfileChartVm.ResetChart(srcPoints);

            // Compute Mean
            meanProfile /= valueList.Count;

            //Compute Standard Dev (en passant par la variance)
            float stdDevProfile = 0.0f;
            if (valueList.Count > 1)
            {
                stdDevProfile += valueList.Sum(fx => (fx - meanProfile) * (fx - meanProfile));
                stdDevProfile /= valueList.Count - 1; // Variance
                stdDevProfile = (float)Math.Sqrt(stdDevProfile); // Standard deviation
            }

            MinProfile = minProfile;
            MaxProfile = maxProfile;
            MeanProfile = meanProfile;
            StdDevProfile = stdDevProfile;

            ExportCsvProfileCommand.NotifyCanExecuteChanged();
        }

        private void UpdateCrossProfile()
        {
            var currentHazeMap = _getCurrentHazeMap();
            if (currentHazeMap == null) return;

            _horizontalCrossProfilePoints.Clear();
            _verticalCrossProfilePoints.Clear();

            bool calculatableLine = CrossProfileY.HasValue && CrossProfileX.HasValue;
            if (!calculatableLine)
            {
                HideProfile();
                return;
            }

            var lidxH = HazeUtils.InterBresenhamLine(0, CrossProfileY.Value, currentHazeMap.Width, CrossProfileY.Value);
            
            float fDist = 0.0f;

            float minZ = float.MaxValue;
            float maxZ = float.MinValue;
            float meanZ = 0.0f;
            float stdDevZ = 0.0f;

            SeriesPoint[] srcPoints = null;
            if (lidxH.Count > 0)
            {
                srcPoints = new SeriesPoint[lidxH.Count];

                float minDist = 0;
                float maxDist = 0;
                var fSampleList = new List<float>();

                float zValue = GetValue(lidxH[0].X, lidxH[0].Y);

                _horizontalCrossProfilePoints.Add(lidxH[0]);
                
                if (float.IsNaN(zValue) == false)
                {
                    fSampleList.Add(zValue);
                    meanZ += zValue;

                    if (zValue < minZ) minZ = zValue;
                    if (zValue > maxZ) maxZ = zValue;
                }

                srcPoints[0] = new SeriesPoint
                {
                    X = fDist,
                    Y = zValue
                };

                for (int i = 1; i < lidxH.Count; i++) // Loop through List with for
                {

                    double distance = Math.Sqrt(Math.Pow(lidxH[i].X - lidxH[i - 1].X, 2) + Math.Pow(lidxH[i].Y - lidxH[i - 1].Y, 2));
                    fDist += (float)distance;
                    
                    if (fDist < minDist) minDist = fDist;
                    if (fDist > maxDist) maxDist = fDist;

                    zValue = GetValue(lidxH[i].X, lidxH[i].Y);
                    if (float.IsNaN(zValue) == false)
                    {
                        fSampleList.Add(zValue);
                        meanZ += zValue;
                        if (zValue < minZ) minZ = zValue;
                        if (zValue > maxZ) maxZ = zValue;
                    }

                    srcPoints[i] = new SeriesPoint
                    {
                        X = fDist,
                        Y = zValue
                    };

                    _horizontalCrossProfilePoints.Add(lidxH[i]);
                }

                // Comute Mean
                meanZ /= fSampleList.Count;

                //Compute Standard Dev (en passant par la variance)
                stdDevZ = 0.0f;
                if (fSampleList.Count > 1)
                {
                    stdDevZ += fSampleList.Sum(fx => (fx - meanZ) * (fx - meanZ));
                    stdDevZ /= fSampleList.Count - 1; // variance
                    stdDevZ = (float)Math.Sqrt(stdDevZ); // standard deviation
                }
            }

            var lidxV = HazeUtils.InterBresenhamLine(CrossProfileX.Value, 0, CrossProfileX.Value, currentHazeMap.Heigth);
            fDist = 0.0f;
            SeriesPoint[] srcPoints2 = null;
            if (lidxV.Count > 0)
            {
                srcPoints2 = new SeriesPoint[lidxV.Count];

                float minDist = 0;
                float maxDist = 0;
                var fSampleList = new List<float>();

                float zValue = GetValue(lidxV[0].X, lidxV[0].Y);
                if (float.IsNaN(zValue) == false)
                {
                    fSampleList.Add(zValue);
                    meanZ += zValue;

                    if (zValue < minZ) minZ = zValue;
                    if (zValue > maxZ) maxZ = zValue;
                }

                srcPoints2[0] = new SeriesPoint
                {
                    X = fDist,
                    Y = zValue
                };

                _verticalCrossProfilePoints.Add(lidxV[0]);

                for (int i = 1; i < lidxV.Count; i++) // Loop through List with for
                {
                    double distance = Math.Sqrt(Math.Pow(lidxV[i].X - lidxV[i - 1].X, 2) + Math.Pow(lidxV[i].Y - lidxV[i - 1].Y, 2));
                    fDist += (float)distance;
                    
                    if (fDist < minDist) minDist = fDist;
                    if (fDist > maxDist) maxDist = fDist;

                    zValue = GetValue(lidxV[i].X, lidxV[i].Y);
                    if (float.IsNaN(zValue) == false)
                    {
                        fSampleList.Add(zValue);
                        meanZ += zValue;
                        if (zValue < minZ) minZ = zValue;
                        if (zValue > maxZ) maxZ = zValue;
                    }

                    srcPoints2[i] = new SeriesPoint
                    {
                        X = fDist,
                        Y = zValue
                    };

                    _verticalCrossProfilePoints.Add(lidxV[i]);
                }

                // Comute Mean
                meanZ /= fSampleList.Count;

                //Compute Standard Dev (en passant par la variance)
                stdDevZ = 0.0f;
                if (fSampleList.Count > 1)
                {
                    stdDevZ += fSampleList.Sum(fx => (fx - meanZ) * (fx - meanZ));
                    stdDevZ /= fSampleList.Count - 1; // variance
                    stdDevZ = (float)Math.Sqrt(stdDevZ); // standard deviation
                }
            }

            MinProfile = minZ;
            MaxProfile = maxZ;
            MeanProfile = meanZ;
            StdDevProfile = stdDevZ;

            _hazeProfileChartVm.ResetChart(srcPoints, srcPoints2);

            ExportCsvProfileCommand.NotifyCanExecuteChanged();
        }
        
        #endregion
    }
}
