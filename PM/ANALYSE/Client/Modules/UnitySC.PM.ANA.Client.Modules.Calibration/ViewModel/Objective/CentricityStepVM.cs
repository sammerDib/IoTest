using System;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Axes;
using UnitySC.PM.ANA.Client.Proxy.Calibration;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Controls;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.ANA.Client.Modules.Calibration.ViewModel.Objective
{
    public class CentricityStepVM : ObjectiveStepBaseVM
    {
        private AxesSupervisor _axeSupervisor;
        private CalibrationSupervisor _calibrationSupervisor;
        private CamerasSupervisor _camerasSupervisor;

        public CentricityStepVM(ObjectiveToCalibrateVM objective) : base(objective)
        {
            _axeSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();
            _calibrationSupervisor = ClassLocator.Default.GetInstance<CalibrationSupervisor>();
            _camerasSupervisor = ClassLocator.Default.GetInstance<CamerasSupervisor>();
            RefPos = ObjectiveVM?.Image?.CentricitiesRefPosition;
        }

        private bool _isEditing = false;

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                if (_isEditing != value)
                {
                    if (value)
                    {
                        try
                        {
                            SaveResultAndState();
                            _axeSupervisor.AxesVM.Position.PropertyChanged -= Position_PropertyChanged;
                            GetRefPos();
                            MoveToCorrectedPos();
                            _axeSupervisor.AxesVM.Position.PropertyChanged += Position_PropertyChanged;
                            StepState = StepStates.InProgress;
                        }
                        catch
                        {
                            ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Please set initial position for main objective centricity.");
                            _isEditing = false;
                            return;
                        }
                    }
                    else
                    {
                        _axeSupervisor.AxesVM.Position.PropertyChanged -= Position_PropertyChanged;
                    }
                    _isEditing = value;
                    OnPropertyChanged();
                }
            }
        }

        private void GetRefPos()
        {
            if (_refPos == null)
            {
                if (ObjectiveVM.IsMain)
                {
                    if (ObjectiveVM?.Image?.CentricitiesRefPosition != null)
                    {
                        RefPos = ObjectiveVM.Image.CentricitiesRefPosition;
                    }
                    else
                    {
                        TakeRefPostion();
                    }
                }
                else
                {
                    throw new Exception("Objective centricities ref pos could not be null");
                }
            }
        }

        private void MoveToCorrectedPos()
        {
            var posWithOffset = (XYPosition)_refPos.Clone();
            posWithOffset.X += ObjectiveVM?.Image?.XOffset?.Length?.Millimeters ?? 0;
            posWithOffset.Y += ObjectiveVM?.Image?.YOffset?.Length?.Millimeters ?? 0;
            _axeSupervisor.GotoPosition(posWithOffset, AxisSpeed.Normal);
            _axeSupervisor.WaitMotionEnd(20000);
        }

        private void Position_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateOffset();
        }

        private void UpdateOffset()
        {
            if (_refPos != null)
            {
                var xOffset = _axeSupervisor.AxesVM.Position.X - _refPos.X;
                var yOffset = _axeSupervisor.AxesVM.Position.Y - _refPos.Y;
                ObjectiveVM.Image.XOffset = xOffset.Millimeters().ToVM();
                ObjectiveVM.Image.YOffset = yOffset.Millimeters().ToVM();
            }
        }

        private void TakeRefPostion()
        {
            RefPos = _axeSupervisor.GetXYZTopZBottomPosition().ToXYPosition();
        }

        private AutoRelayCommand _edit;

        public AutoRelayCommand Edit
        {
            get
            {
                return _edit ?? (_edit = new AutoRelayCommand(
              () =>
              {
                  IsEditing = true;
              },
              () => { return ObjectiveVM.PixelSizeStep.StepState == StepStates.Done && StepState != StepStates.InProgress; }));
            }
        }

        private AutoRelayCommand _submit;

        public AutoRelayCommand Submit
        {
            get
            {
                return _submit ?? (_submit = new AutoRelayCommand(
              () =>
              {
                  if (ObjectiveVM.Image.XOffset != null && ObjectiveVM.Image.YOffset != null)
                  {
                      ObjectiveVM.Image.CentricitiesRefPosition = RefPos;
                      StepState = StepStates.Done;
                  }
                  else
                  {
                      StepState = StepStates.NotDone;
                  }
                  IsEditing = false;
                  ObjectiveVM.Update();
              },

              () => { return true; }));
            }
        }

        private AutoRelayCommand _cancel;

        public AutoRelayCommand Cancel
        {
            get
            {
                return _cancel ?? (_cancel = new AutoRelayCommand(
              () =>
              {
                  RestoreResultAndState();
                  IsEditing = false;
              },

              () => { return true; }));
            }
        }

        private XYPosition _refPos;

        public XYPosition RefPos
        {
            get { return _refPos; }
            set
            {
                if (value != RefPos)
                {
                    _refPos = value;
                    if (_refPos != null)
                    {
                        RefPosX = new LengthVM(_refPos.X, LengthUnit.Millimeter);
                        RefPosY = new LengthVM(_refPos.Y, LengthUnit.Millimeter);
                    }
                    OnPropertyChanged();
                }
            }
        }

        private LengthVM _refPosX;

        public LengthVM RefPosX
        {
            get => _refPosX; set { if (_refPosX != value) { _refPosX = value; OnPropertyChanged(); } }
        }

        private LengthVM _refPosY;

        public LengthVM RefPosY
        {
            get => _refPosY; set { if (_refPosY != value) { _refPosY = value; OnPropertyChanged(); } }
        }

        private AutoRelayCommand _moveToRefPosition;

        public AutoRelayCommand MoveToRefPosition
        {
            get
            {
                return _moveToRefPosition ?? (_moveToRefPosition = new AutoRelayCommand(
              () =>
              {
                  _axeSupervisor.GotoPosition(_refPos, AxisSpeed.Normal);
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _setRefPosition;

        public AutoRelayCommand SetRefPosition
        {
            get
            {
                return _setRefPosition ?? (_setRefPosition = new AutoRelayCommand(
              () =>
              {
                  if (ObjectiveVM.IsMain)
                  {
                      TakeRefPostion();
                      UpdateOffset();
                      OnPropertyChanged();
                  }
              },
              () => { return true; }));
            }
        }
    }
}
