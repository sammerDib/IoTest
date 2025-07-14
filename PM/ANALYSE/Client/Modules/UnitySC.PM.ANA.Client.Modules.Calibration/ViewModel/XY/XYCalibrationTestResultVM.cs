using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Collection;

namespace UnitySC.PM.ANA.Client.Modules.Calibration.ViewModel.XY
{
    public class XYCalibrationTestResultVM : ObservableObject
    {
        public enum TestState { InProgess, Valid, OutOfSpec}

        public XYCalibrationTestResultVM(int id)
        {
            Id = id;
            State = TestState.InProgess;
            DisplayTestLabel = $"Stage error Test #{Id}";
        }

        private TestState _state;
        public TestState State
        {
            get => _state; set { if (_state != value) { _state = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsAvailable)); } }
        }

        public bool IsAvailable => _state != TestState.InProgess;

        private string _info;
        public string Info
        {
            get => _info; set { if (_info != value) { _info = value; OnPropertyChanged(); } }
        }

        private string _displayTestLabel;
        public string DisplayTestLabel
        {
            get => _displayTestLabel; set { if (_displayTestLabel != value) { _displayTestLabel = value; OnPropertyChanged(); } }
        }

        public void SetResult(XYCalibrationTest xYCalibrationTest, XYCalibrationData xyCalibrationDataApplied)
        {
            TestCalibResult = xYCalibrationTest;
            XYVectorHeatMapVMTest.StageCorrectionApplied = xyCalibrationDataApplied;
            BadPoints = xYCalibrationTest.BadPoints;
            if (BadPoints is null || BadPoints.IsEmpty())
            {
                State = TestState.Valid;
            }
            else
            {
                State = TestState.OutOfSpec;
            }
        }

        private XYCalibrationTest _testCalibResult;
        public XYCalibrationTest TestCalibResult
        {
            get => _testCalibResult; set { if (_testCalibResult != value) { _testCalibResult = value; OnPropertyChanged(); }  }
        }

        private List<Correction> _badPoints;
        public List<Correction> BadPoints
        {
            get => _badPoints; set { if (_badPoints != value) { _badPoints = value; OnPropertyChanged(); } }
        }

        private List<XYPosition> _umcomputableCorrections;
        public List<XYPosition> UmcomputableCorrections
        {
            get => _umcomputableCorrections; set { if (_umcomputableCorrections != value) { _umcomputableCorrections = value; OnPropertyChanged(); } }
        }

        private int _id;
        public int Id
        {
            get => _id; set { if (_id != value) { _id = value; OnPropertyChanged(); } }
        }

        private XYCalibResultVectorHeatMapVM _xyVectorHeatMapVMTest;
        public XYCalibResultVectorHeatMapVM XYVectorHeatMapVMTest
        {
            get => _xyVectorHeatMapVMTest; set { if (_xyVectorHeatMapVMTest != value) { _xyVectorHeatMapVMTest = value; OnPropertyChanged(); } }
        }

    }
}
