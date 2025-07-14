using System;
using System.Windows;

using MvvmDialogs;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps;
using UnitySC.PM.ANA.Client.Shared.Helpers;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Controls;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public enum CornerPosition
    {
        TopLeft,
        BottomRight
    }

    public class CornerStepVM : StepBaseVM, IModalDialogViewModel
    {
        private CornerPosition _cornerPosition;
        private RecipeWaferMapVM _recipeWaferMap;

        public CornerStepVM(CornerPosition cornerPosition, RecipeWaferMapVM recipeWaferMap)
        {
            _cornerPosition = cornerPosition;
            _recipeWaferMap = recipeWaferMap;
        }

        internal void Reset()
        {
            StepState = StepStates.NotDone;
            CurrentPatternRecImage = null;
            IsEditing = false;
        }

        #region Properties

        private bool _isEditing = false;

        public bool IsEditing
        {
            get => _isEditing; set { if (_isEditing != value) { _isEditing = value; OnPropertyChanged(); } }
        }

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

        // in mm
        public double CurrentPositionX => CurrentPatternRecImage?.Position?.X ?? double.NaN;

        // in mm
        public double CurrentPositionY => CurrentPatternRecImage?.Position?.Y ?? double.NaN;

        #endregion Properties

        #region Methods

        public void GotoCurrentPosition()
        {
            if (null == CurrentPatternRecImage?.Position)
                return;
            ServiceLocator.AxesSupervisor.GotoPosition(CurrentPatternRecImage.Position, AxisSpeed.Fast);
        }

        #endregion Methods

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
                        GotoCurrentPosition();
                        CurrentPatternRecImage = null;

                        _recipeWaferMap.StartEditCorner(_cornerPosition);
                    },
                    () => { return _recipeWaferMap.CanEdit(); }
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
                            if (_recipeWaferMap.IsCenteredROI)
                            {
                                rectRoi = new Rect(_recipeWaferMap.RoiSize);
                            }
                            else
                            {
                                rectRoi = _recipeWaferMap.RoiRectWaferMap;
                            }
                            var newPatternRecImage = PatternRecHelpers.CreatePositionWithTopPatternRec(rectRoi, _recipeWaferMap.IsCenteredROI);

                            if (_recipeWaferMap.CheckCornerPositionValidity(_cornerPosition, newPatternRecImage, out string errorMesssage))
                            {
                                IsEditing = false;
                                StepState = StepStates.Done;
                                CurrentPatternRecImage = newPatternRecImage;
                                _recipeWaferMap.CornerPositionValidated(_cornerPosition);
                            }
                            else
                            {
                                // The corner position is not valid
                                var dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
                                dialogService.ShowMessageBox(errorMesssage, "Error", MessageBoxButton.OK);
                            }

                            _recipeWaferMap.IsModified = true;
                        }
                        catch (Exception ex)
                        {
                            var dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
                            dialogService.ShowMessageBox("Failed to submit the corner", ex.Message, MessageBoxButton.OK);

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
                        GotoCurrentPosition();
                        var previousGamma = CurrentPatternRecImage.PatternRec.Gamma;
                        if (ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowDialog<PatternRecParametersView>(new PatternRecParametersVM(CurrentPatternRecImage)) == true)
                        {
                            _recipeWaferMap.CornerPositionValidated(_cornerPosition);

                            if (previousGamma != CurrentPatternRecImage.PatternRec.Gamma)
                            {
                                _recipeWaferMap.ResetWaferMapAndStreetSizeSteps();
                            }
                        }
                    },
                    () => { return (!(CurrentPatternRecImage is null)); }
                ));
            }
        }

        public bool? DialogResult { get; }

        #endregion RelayCommands
    }
}
