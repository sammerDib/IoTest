using System.Collections.Generic;
using System.Windows.Media.Imaging;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using MvvmDialogs;

using UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Controls;
using UnitySC.Shared.UI.Dialog;
using System;
using UnitySC.Shared.Image;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class EdgeDetectionVM : VMSharedBase, IModalDialogViewModel
    {
        private RecipeAlignmentVM _recipeAlignmentVM;
        private WaferEdgePositions _edgePosition;

        public EdgeDetectionVM(RecipeAlignmentVM recipeAlignmentVM, WaferEdgePositions edgePosition)
        {
            _recipeAlignmentVM = recipeAlignmentVM;
            _edgePosition = edgePosition;
        }

        private StepStates _stepState = StepStates.NotDone;

        public StepStates StepState
        {
            get => _stepState;
            set
            {
                if (_stepState != value)
                {
                    _stepState = value;
                    if (_stepState == StepStates.NotDone)
                    {
                        Image = null;
                        ServiceImage = null;
                        ErrorMessage = null;
                    }
                    UpdateAllCanExecutes();
                    OnPropertyChanged();
                }
            }
        }

        private string _errorMessage = null;

        public string ErrorMessage
        {
            get => _errorMessage; set { if (_errorMessage != value) { _errorMessage = value; OnPropertyChanged(); } }
        }

        private bool _isEditing = false;

        public bool IsEditing
        {
            get => _isEditing; set { if (_isEditing != value) { _isEditing = value; OnPropertyChanged(); } }
        }

        private bool _isManuallyEdited = false;

        public bool IsManuallyEdited
        {
            get => _isManuallyEdited; set { if (_isManuallyEdited != value) { _isManuallyEdited = value; OnPropertyChanged(); } }
        }

        private BitmapSource _image;

        public BitmapSource Image
        {
            get => _image; set { if (_image != value) { _image = value; OnPropertyChanged(); } }
        }

        private ServiceImage _serviceImage = null;

        public ServiceImage ServiceImage
        {
            get => _serviceImage; set { if (_serviceImage != value) { _serviceImage = value; OnPropertyChanged(); } }
        }

        private List<ServicePoint> _edgePoints = null;

        public List<ServicePoint> EdgePoints
        {
            get => _edgePoints; set { if (_edgePoints != value) { _edgePoints = value; OnPropertyChanged(); } }
        }

        private List<(ServicePoint pt1, ServicePoint pt2)> _notchLines = null;

        public List<(ServicePoint pt1, ServicePoint pt2)> NotchLines
        {
            get => _notchLines; set { if (_notchLines != value) { _notchLines = value; OnPropertyChanged(); } }
        }

        // Position when set manually
        private PositionBase _manualPosition = null;

        public PositionBase ManualPosition
        {
            get => _manualPosition; set { if (_manualPosition != value) { _manualPosition = value; OnPropertyChanged(); } }
        }

        // it is valid only if the position has been set manually
        public ServiceImageWithPosition GetManualServiceImageWithPosition()
        {
            if (ManualPosition is null)
                return null;
            var serviceImageWithPosition = new ServiceImageWithPosition(ServiceImage, ManualPosition);
            return serviceImageWithPosition;
        }

        private AutoRelayCommand _zoomImage;

        public AutoRelayCommand ZoomImage
        {
            get
            {
                return _zoomImage ?? (_zoomImage = new AutoRelayCommand(
                    () =>
                    {
                        ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowDialog<EdgeImageView>(this);
                    },
                    () => { return Image != null; }
                ));
            }
        }

        private AutoRelayCommand _zoomImageNotch;

        public AutoRelayCommand ZoomImageNotch
        {
            get
            {
                return _zoomImageNotch ?? (_zoomImageNotch = new AutoRelayCommand(
                    () =>
                    {
                        ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowDialog<NotchImageView>(this);
                    },
                    () => { return Image != null; }
                ));
            }
        }

        private AutoRelayCommand _edit;

        public AutoRelayCommand Edit
        {
            get
            {
                return _edit ?? (_edit = new AutoRelayCommand(
                    () =>
                    {
                        ManualPosition = null;
                        StepState = StepStates.InProgress;
                        // The global state of the step must be InProgress
                        _recipeAlignmentVM.EdgesStep.StepState = StepStates.InProgress;

                        PositionBase imagePosition = null;
                        switch (_edgePosition)
                        {
                            case WaferEdgePositions.Top:
                                imagePosition = _recipeAlignmentVM.EdgesStep.Settings?.ImagePositionTop;
                                break;

                            case WaferEdgePositions.Right:
                                imagePosition = _recipeAlignmentVM.EdgesStep.Settings?.ImagePositionRight;
                                break;

                            case WaferEdgePositions.Bottom:
                                imagePosition = _recipeAlignmentVM.EdgesStep.Settings?.ImagePositionBottom;
                                break;

                            case WaferEdgePositions.Left:
                                imagePosition = _recipeAlignmentVM.EdgesStep.Settings?.ImagePositionLeft;
                                break;
                        }
                        // We move to the theoretical position
                        ServiceLocator.AxesSupervisor.GotoPosition(imagePosition, AxisSpeed.Fast);

                        IsEditing = true;
                        Image = null;
                    },
                    () => { return !_recipeAlignmentVM.IsAutomaticAlignmentInProgress; }
                ));
            }
        }

        private AutoRelayCommand _submit;

        public AutoRelayCommand Submit
        {
            get
            {
                return _submit ?? (_submit = new AutoRelayCommand(
                    async () =>
                    {
                        try
                        {
                            IsEditing = false;
                            IsManuallyEdited = true;
                            var curPosition = ServiceLocator.AxesSupervisor.GetCurrentPosition()?.Result;
                            //Converting from motor referential to stage referential because the BWA needs a stage referential position for the image's centroid
                            //Without the conversion, the position and the BWA result are off by ~200 microns
                            var curPositionInStageReferential = ServiceLocator.ReferentialSupervisor.ConvertTo(curPosition, ReferentialTag.Stage)?.Result;
                            await _recipeAlignmentVM.EdgesStep.ExecuteBWAImageAsync(ServiceLocator.ChuckSupervisor.ChuckVM.SelectedWaferCategory, curPositionInStageReferential, _edgePosition);
                            ManualPosition = curPositionInStageReferential;
                            StepState = StepStates.Done;
                        }
                        catch (Exception ex)
                        {
                            Logger.Information(ex.Message);
                            StepState = StepStates.Error;
                        }
                    },
                    () => { return true; }
                ));
            }
        }

        public bool? DialogResult { get; }
    }
}
