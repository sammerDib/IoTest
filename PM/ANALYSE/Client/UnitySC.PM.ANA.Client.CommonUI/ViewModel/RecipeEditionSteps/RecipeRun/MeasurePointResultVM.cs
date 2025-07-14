using System.Collections.Generic;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Format.Metro;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.RecipeRun
{
    public class MeasurePointResultVM : ObservableObject
    {
        public MeasurePointResultVM(string measureName, int pointIndex, DieIndex dieIndex, Point position, int? repeatIndex, int? nbRepeat, bool isInProgress = false, bool isSubMeasurePoint = false)
        {
            MeasureName = measureName;
            PointIndex = pointIndex;
            Position = position;
            DieIndex = dieIndex;
            RepeatIndex = repeatIndex;
            NbRepeat = nbRepeat;
            IsInProgress = isInProgress;
            IsSubMeasurePoint = isSubMeasurePoint;
        }

        private string _measureName;

        public string MeasureName
        {
            get => _measureName; set { if (_measureName != value) { _measureName = value; OnPropertyChanged(); } }
        }

        private int _pointIndex;

        public int PointIndex
        {
            get => _pointIndex; set { if (_pointIndex != value) { _pointIndex = value; OnPropertyChanged(); } }
        }

        private Point _position;

        public Point Position
        {
            get => _position; set { if (_position != value) { _position = value; OnPropertyChanged(); } }
        }

        private DieIndex _dieIndex;

        public DieIndex DieIndex
        {
            get => _dieIndex; set { if (_dieIndex != value) { _dieIndex = value; OnPropertyChanged(); } }
        }

        private int? _repeatIndex = null;

        public int? RepeatIndex
        {
            get => _repeatIndex; set { if (_repeatIndex != value) { _repeatIndex = value; OnPropertyChanged(); } }
        }

        private int? _nbRepeat;

        public int? NbRepeat
        {
            get => _nbRepeat;
            set { if (_nbRepeat != value) { _nbRepeat = value; OnPropertyChanged(); } }
        }

        private string _resultFolderPath;

        public string ResultFolderPath
        {
            get => _resultFolderPath; set { if (_resultFolderPath != value) { _resultFolderPath = value; OnPropertyChanged(); } }
        }

        private List<ResultValue> _resValues;

        public List<ResultValue> ResValues
        {
            get => _resValues; set { if (_resValues != value) { _resValues = value; OnPropertyChanged(); } }
        }

        private MeasurePointResult _result;

        public MeasurePointResult Result
        {
            get => _result; set { if (_result != value) { _result = value; ResValues = _result.GetResultValues(); ; OnPropertyChanged(); } }
        }

        private bool _isInProgress = false;

        public bool IsInProgress
        {
            get => _isInProgress; set { if (_isInProgress != value) { _isInProgress = value; OnPropertyChanged(); } }
        }

        private bool _isSubMeasurePoint = false;

        public bool IsSubMeasurePoint
        {
            get => _isSubMeasurePoint;
            set => SetProperty(ref _isSubMeasurePoint, value);
        }
    }
}
