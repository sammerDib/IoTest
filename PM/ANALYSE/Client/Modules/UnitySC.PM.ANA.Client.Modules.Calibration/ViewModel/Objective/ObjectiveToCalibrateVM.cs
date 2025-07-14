using System;

using AutoMapper;

using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Controls;
using UnitySC.Shared.UI.ViewModel;

using static UnitySC.PM.ANA.Service.Interface.ObjectiveConfig;

namespace UnitySC.PM.ANA.Client.Modules.Calibration.ViewModel.Objective
{
    public class ObjectiveToCalibrateVM : ViewModelBaseExt, IDisposable
    {
        public delegate void ResultChangedHandler(ObjectiveToCalibrateVM objectiveToCalibrate);

        public delegate void CentricitiesRefPosChangedHandler(ObjectiveToCalibrateVM objectiveToCalibrate);

        public event ResultChangedHandler ResultChangedEvent;

        public event CentricitiesRefPosChangedHandler CentricitiesRefPosChangedEvent;

        private bool _isValidated;

        public bool IsValidated
        {
            get => _isValidated; set { if (_isValidated != value) { _isValidated = value; OnPropertyChanged(); } }
        }

        private bool _isMain;

        public bool IsMain
        {
            get => _isMain; set { if (_isMain != value) { _isMain = value; OnPropertyChanged(); } }
        }

        private bool _isBottom;

        public bool IsBottom
        {
            get => _isBottom; set { if (_isBottom != value) { _isBottom = value; OnPropertyChanged(); } }
        }

        public bool IsEditing => FocusPositionStep.IsEditing || PixelSizeStep.IsEditing || CentricityStep.IsEditing;
        public ObjectiveCalibration CalibrationData { get; private set; }

        public ObjectiveType ObjType { get; private set; }

        public ObjectiveToCalibrateVM(ObjectiveCalibration calibrationData, ModulePositions position, bool isMain, ObjectiveType objType)
        {
            CalibrationData = calibrationData;
            CalibrationDataToVM();
            Position = position;
            FocusPositionStep = new FocusPositionStepVM(this);
            PixelSizeStep = new PixelSizeStepVM(this);
            CentricityStep = new CentricityStepVM(this);
            AdvancedSettingsStep = new AdvancedSettingsStepVM(this);
            ObjType = objType;
            InitState();
            PixelSizeStep.PropertyChanged += PixelSizeStep_PropertyChanged;
            CentricityStep.PropertyChanged += CentricityStep_PropertyChanged;
            CentricityStep.PropertyChanged += CentricitiesRefPos_PropertyChanged;
            IsMain = isMain;
            IsBottom = position == ModulePositions.Down;
            if (IsBottom)
            {
                CentricityStep.StepState = StepStates.Done;
            }
            UpdateStatus();
        }

        private ObjectiveCalibrationResult _flowResult;

        public ObjectiveCalibrationResult FlowResult
        {
            get => _flowResult;
            set
            {
                if (_flowResult != value)
                {
                    _flowResult = value;
                    FlowResultToVM();
                    Update();
                    OnPropertyChanged();
                }
                ResultChangedEvent?.Invoke(this);
            }
        }

        public FocusPositionStepVM FocusPositionStep { get; set; }
        public AdvancedSettingsStepVM AdvancedSettingsStep { get; set; }
        public PixelSizeStepVM PixelSizeStep { get; set; }
        public CentricityStepVM CentricityStep { get; set; }

        public string Id => CalibrationData.DeviceId;
        public string Name => CalibrationData.ObjectiveName;

        public Length ZOffsetWithMainObjective
        {
            get
            {
                return CalibrationData.ZOffsetWithMainObjective;
            }
            set
            {
                CalibrationData.ZOffsetWithMainObjective = value;
                OnPropertyChanged();
            }
        }

        public Length OpticalReferenceElevationFromStandardWafer
        {
            get
            {
                return CalibrationData.OpticalReferenceElevationFromStandardWafer;
            }
            set
            {
                CalibrationData.OpticalReferenceElevationFromStandardWafer = value;
                OnPropertyChanged();
            }
        }

        private AutofocusParametersVM _autofocus;

        public AutofocusParametersVM Autofocus
        {
            get => _autofocus; set { if (_autofocus != value) { _autofocus = value; OnPropertyChanged(); } }
        }

        private ImageParametersVM _image;

        public ImageParametersVM Image
        {
            get => _image; set { if (_image != value) { _image = value; OnPropertyChanged(); } }
        }

        public ModulePositions Position { get; private set; }

        private void UpdateStatus()
        {
            IsValidated = CalibrationData.AutoFocus != null
                && CalibrationData.Image != null;
        }

        public void Update()
        {
            IsValidated = FocusPositionStep.StepState == StepStates.Done
                && PixelSizeStep.StepState == StepStates.Done
                && CentricityStep.StepState == StepStates.Done
                && AdvancedSettingsStep.StepState == StepStates.Done;
        }

        private void CentricityStep_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CentricityStep.StepState))
            {
                if (CentricityStep.StepState == StepStates.Done
                    && FocusPositionStep.StepState == StepStates.Done
                    && PixelSizeStep.StepState == StepStates.Done)
                {
                    AdvancedSettingsStep.StepState = StepStates.Done;
                }
                else
                {
                    AdvancedSettingsStep.StepState = StepStates.NotDone;
                }

                Update();
            }
        }

        private void CentricitiesRefPos_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CentricityStep.SetRefPosition))
            {
                if (IsMain)
                {
                    CentricitiesRefPosChangedEvent?.Invoke(this);
                }
            }
        }

        private void PixelSizeStep_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PixelSizeStep.StepState))
            {
                if (PixelSizeStep.StepState == StepStates.Done && CentricityStep.StepState != StepStates.Done)
                {
                    CentricityStep.IsEditing = true;
                }

                if (CentricityStep.StepState == StepStates.Done
                    && FocusPositionStep.StepState == StepStates.Done
                    && PixelSizeStep.StepState == StepStates.Done)
                {
                    AdvancedSettingsStep.StepState = StepStates.Done;
                }
                Update();
            }
        }

        public void Dispose()
        {
            PixelSizeStep.PropertyChanged -= PixelSizeStep_PropertyChanged;
            CentricityStep.PropertyChanged -= CentricityStep_PropertyChanged;
            CentricityStep.PropertyChanged -= CentricitiesRefPos_PropertyChanged;
        }

        #region Mapping

        private IMapper _mapper;

        public IMapper AutoMap
        {
            get
            {
                if (_mapper == null)
                {
                    var configuration = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<Length, LengthVM>().ConvertUsing<LengthToVMConverter>();
                        cfg.CreateMap<LengthVM, Length>().ConvertUsing<VMToLengthConverter>();
                        cfg.CreateMap<AutofocusParametersVM, AutofocusParameters>().ReverseMap();
                        cfg.CreateMap<ImageParametersVM, ImageParameters>().ReverseMap();
                        cfg.CreateMap<LiseAutofocusParametersVM, LiseAutofocusParameters>().ReverseMap();
                    });
                    _mapper = configuration.CreateMapper();
                }
                return _mapper;
            }
        }

        public void CalibrationDataToVM()
        {
            if (CalibrationData != null)
            {
                if (CalibrationData.AutoFocus != null)
                {
                    Autofocus = AutoMap.Map<AutofocusParametersVM>(CalibrationData.AutoFocus);
                    Autofocus.Lise = AutoMap.Map<LiseAutofocusParametersVM>(CalibrationData.AutoFocus.Lise);
                }
                if (CalibrationData.Image != null)
                {
                    Image = AutoMap.Map<ImageParametersVM>(CalibrationData.Image);
                    Image.PixelSize = AutoMap.Map<LengthVM>(CalibrationData.Image.PixelSizeX);
                    Image.CentricitiesRefPosition = AutoMap.Map<XYPosition>(CalibrationData.Image.CentricitiesRefPosition);
                }

                OpticalReferenceElevationFromStandardWafer = CalibrationData.OpticalReferenceElevationFromStandardWafer;
                ZOffsetWithMainObjective = CalibrationData.ZOffsetWithMainObjective;
            }
        }

        private void InitState()
        {
            if (Autofocus != null)
            {
                FocusPositionStep.IsEditing = false;
                FocusPositionStep.StepState = StepStates.Done;
                if (Image != null)
                {
                    PixelSizeStep.IsEditing = false;
                    PixelSizeStep.StepState = StepStates.Done;
                    CentricityStep.IsEditing = false;
                    CentricityStep.StepState = StepStates.Done;
                    AdvancedSettingsStep.StepState = StepStates.Done;
                }
            }
        }

        public void FlowResultToVM()
        {
            if (_flowResult != null)
            {
                Autofocus = AutoMap.Map<AutofocusParametersVM>(_flowResult.AutoFocus);
                Autofocus.Lise = AutoMap.Map<LiseAutofocusParametersVM>(_flowResult.AutoFocus.Lise);
                Image = AutoMap.Map<ImageParametersVM>(_flowResult.Image);
                Image.PixelSize = AutoMap.Map<LengthVM>(_flowResult.Image.PixelSizeX);
                OpticalReferenceElevationFromStandardWafer = AutoMap.Map<Length>(_flowResult.OpticalReferenceElevationFromStandardWafer);
            }
        }

        public void VMToCalibrationData()
        {
            if (Autofocus != null)
            {
                CalibrationData.AutoFocus = AutoMap.Map<AutofocusParameters>(Autofocus);
                CalibrationData.AutoFocus.Lise = AutoMap.Map<LiseAutofocusParameters>(Autofocus.Lise);
            }
            if (Image != null)
            {
                CalibrationData.Image = AutoMap.Map<ImageParameters>(Image);
                CalibrationData.Image.PixelSizeX = AutoMap.Map<Length>(Image.PixelSize);
                CalibrationData.Image.PixelSizeY = AutoMap.Map<Length>(Image.PixelSize);
                CalibrationData.Image.CentricitiesRefPosition = AutoMap.Map<XYPosition>(Image.CentricitiesRefPosition);
            }

            CalibrationData.ZOffsetWithMainObjective = ZOffsetWithMainObjective;
            CalibrationData.OpticalReferenceElevationFromStandardWafer = OpticalReferenceElevationFromStandardWafer;
        }

        #endregion Mapping
    }
}
