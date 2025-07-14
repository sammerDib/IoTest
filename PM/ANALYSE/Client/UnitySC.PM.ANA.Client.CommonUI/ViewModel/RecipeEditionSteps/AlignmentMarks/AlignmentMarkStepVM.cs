using System;
using System.Linq;
using System.Windows;

using MvvmDialogs;

using UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps;
using UnitySC.PM.ANA.Client.Shared.Helpers;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Controls;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class AlignmentMarkStepVM : StepBaseVM, IModalDialogViewModel
    {
        private AlignmentMarkSiteStepVM _alignmentMarkSite;

        public AlignmentMarkStepVM(AlignmentMarkSiteStepVM alignmentMarkSite, bool isMainAlignmentMark)
        {
            _alignmentMarkSite = alignmentMarkSite;
            _isMain = isMainAlignmentMark;
        }

        public AlignmentMarkStepVM(AlignmentMarkSiteStepVM alignmentMarkSite, PositionWithPatternRec patternRecImage, bool isMainAlignmentMark) : this(alignmentMarkSite, isMainAlignmentMark)
        {
            _currentPatternRecImage = patternRecImage;
        }

        internal void Reset()
        {
            StepState = StepStates.NotDone;
            CurrentPatternRecImage = null;
        }

        #region Properties

        private bool _isMain;

        public bool IsMain
        {
            get => _isMain; set { if (_isMain != value) { _isMain = value; OnPropertyChanged(); } }
        }

        private bool _isEditing = false;

        public bool IsEditing
        {
            get => _isEditing; set { if (_isEditing != value) { _isEditing = value; OnPropertyChanged(); } }
        }

        public bool? DialogResult { get; }

        private PositionWithPatternRec _currentPatternRecImage;

        public PositionWithPatternRec CurrentPatternRecImage
        {
            get => _currentPatternRecImage;
            set
            {
                if (_currentPatternRecImage != value)
                {
                    _currentPatternRecImage = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CurrentPositionX));
                    OnPropertyChanged(nameof(CurrentPositionY));
                }
            }
        }

        internal void UpdateIsLast()
        {
            OnPropertyChanged(nameof(IsLast));
            OnPropertyChanged(nameof(IsLastValid));
        }

        // in mm
        public double CurrentPositionX => CurrentPatternRecImage?.Position?.X ?? double.NaN;

        // in mm
        public double CurrentPositionY => CurrentPatternRecImage?.Position?.Y ?? double.NaN;

        public bool IsLast
        {
            get
            {
                return ((_alignmentMarkSite.AlignmentMarks.Last() == this));
            }
        }

        public bool IsLastValid
        {
            get
            {
                return (this.CurrentPatternRecImage != null && ((_alignmentMarkSite.AlignmentMarks.Last() == this)));
            }
        }

        #endregion Properties

        #region RelayCommands

        private AutoRelayCommand _edit;

        public AutoRelayCommand Edit
        {
            get
            {
                return _edit ?? (_edit = new AutoRelayCommand(
                    () =>
                    {
                        IsEditing = true;
                        StepState = StepStates.InProgress;
                        ApplyCurrentSettings();
                        CurrentPatternRecImage = null;
                    },
                    () => { return _alignmentMarkSite.RecipeAlignmentMarks.CanEdit(); }
                ));
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
                        try
                        {
                            Rect rectRoi;
                            if (_alignmentMarkSite.RecipeAlignmentMarks.IsCenteredROI)
                            {
                                rectRoi = new Rect(_alignmentMarkSite.RecipeAlignmentMarks.RoiSize);
                            }
                            else
                            {
                                rectRoi = _alignmentMarkSite.RecipeAlignmentMarks.RoiRectAlignmentMarks;
                            }
                            CurrentPatternRecImage = PatternRecHelpers.CreatePositionWithTopPatternRec(rectRoi, _alignmentMarkSite.RecipeAlignmentMarks.IsCenteredROI);

                            IsEditing = false;

                            _alignmentMarkSite.RecipeAlignmentMarks.IsModified = true;

                            StepState = StepStates.Done;
                            _alignmentMarkSite.UpdateAllIsLast();
                        }
                        catch (Exception ex)
                        {
                            var dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
                            dialogService.ShowMessageBox("Failed to submit alignment mark", ex.Message, MessageBoxButton.OK);
                        }
                    },
                    () => { return true; }
                ));
            }
        }

        private AutoRelayCommand _displayImage;

        public AutoRelayCommand DisplayImage
        {
            get
            {
                return _displayImage ?? (_displayImage = new AutoRelayCommand(
                    () =>
                    {
                        var patternRecDisplayVM = new PatternRecDisplayVM() { PatternRec = CurrentPatternRecImage.PatternRec };

                        ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowDialog<PatternRecDisplay>(patternRecDisplayVM);
                    },
                    () => { return (!(CurrentPatternRecImage is null)); }
                ));
            }
        }

        private AutoRelayCommand _displayParameters;

        public AutoRelayCommand DisplayParameters
        {
            get
            {
                return _displayParameters ?? (_displayParameters = new AutoRelayCommand(
                    () =>
                    {
                        var patternRecVM = new PatternRecParametersVM(CurrentPatternRecImage);

                        ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowDialog<PatternRecParametersView>(patternRecVM);
                    },
                    () => { return (!(CurrentPatternRecImage is null)); }
                ));
            }
        }

        private AutoRelayCommand _addAlignmentMark;

        public AutoRelayCommand AddAlignmentMark
        {
            get
            {
                return _addAlignmentMark ?? (_addAlignmentMark = new AutoRelayCommand(
                    () =>
                    {
                        _alignmentMarkSite.AddNewAlignmentMark();
                    },
                    () => { return _alignmentMarkSite.RecipeAlignmentMarks.CanEdit() && (this.StepState == StepStates.Done); }
                ));
            }
        }

        private AutoRelayCommand _removeAlignmentMark;

        public AutoRelayCommand RemoveAlignmentMark
        {
            get
            {
                return _removeAlignmentMark ?? (_removeAlignmentMark = new AutoRelayCommand(
                    () =>
                    {
                        _alignmentMarkSite.RemoveAlignmentMark(this);
                    },
                    () => { return true; }
                ));
            }
        }

        private AutoRelayCommand _gotoCurrentPosition;

        public AutoRelayCommand GotoCurrentPosition
        {
            get
            {
                return _gotoCurrentPosition ?? (_gotoCurrentPosition = new AutoRelayCommand(
                    () =>
                    {
                        ApplyCurrentSettings();
                    },
                    () => { return true; }
                ));
            }
        }

        private void ApplyCurrentSettings()
        {
            if (CurrentPatternRecImage is null)
            {
                return;
            }
            ServiceLocator.AxesSupervisor.GotoPosition(CurrentPatternRecImage.Position, AxisSpeed.Fast);
            var pixelSizemm = ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(ServiceLocator.CamerasSupervisor.Objective.DeviceID).Image.PixelSizeX.Millimeters;

            // We restore the ROI size
            if (!(CurrentPatternRecImage is null) && !(CurrentPatternRecImage.PatternRec.RegionOfInterest is null))
                _alignmentMarkSite.RecipeAlignmentMarks.RoiRectAlignmentMarks = new Rect(CurrentPatternRecImage.PatternRec.RegionOfInterest.X.ToPixels(pixelSizemm.Millimeters()),
                                                                            CurrentPatternRecImage.PatternRec.RegionOfInterest.Y.ToPixels(pixelSizemm.Millimeters()),
                                                                            CurrentPatternRecImage.PatternRec.RegionOfInterest.Width.ToPixels(pixelSizemm.Millimeters()),
                                                                            CurrentPatternRecImage.PatternRec.RegionOfInterest.Height.ToPixels(pixelSizemm.Millimeters()));
            else
                _alignmentMarkSite.RecipeAlignmentMarks.RoiRectAlignmentMarks = new Rect(0, 0, ServiceLocator.CamerasSupervisor.Camera.CameraInfo.Width, ServiceLocator.CamerasSupervisor.Camera.CameraInfo.Height);

            // We apply the context
            ServiceLocator.ContextSupervisor.Apply(CurrentPatternRecImage.Context);
        }

        #endregion RelayCommands
    }
}
