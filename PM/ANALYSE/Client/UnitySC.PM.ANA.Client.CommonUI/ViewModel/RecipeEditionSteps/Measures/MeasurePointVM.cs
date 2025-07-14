using System;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps;
using UnitySC.PM.ANA.Client.Controls.Camera;
using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Axes;
using UnitySC.PM.ANA.Client.Shared.Helpers;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Recipe.Context;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures
{
    public class MeasurePointVM : ObservableObject, ICameraDisplayPoint, IDisposable
    {
        private MeasurePointsVM _pointsManager;

        // Position in wafer coordinates for the display in the camera view
        private Point _displayPosition;

        private Point? _displayPositionOnDie;

        public bool IsDiePosition { get; set; }

        private MeasurePointVM(MeasurePointsVM pointsManager, int siteId, bool isDiePosition)
        {
            _pointsManager = pointsManager;
            ServiceLocator.AxesSupervisor.AxesVM.PropertyChanged += AxesVM_PropertyChanged;
            IsDiePosition = isDiePosition;
            Id = siteId;
            IsSubMeasurePoint = pointsManager.RecipeMeasure.MeasureSettings.IsMeasureWithSubMeasurePoints;
        }

        public MeasurePointVM(MeasurePointsVM pointsManager, XYZTopZBottomPosition position, int siteId, bool isDiePosition) : this(pointsManager, siteId, isDiePosition)
        {
            UpdatePosition(position);
        }

        private void UpdatePosition(XYZTopZBottomPosition position)
        {
            PointPosition = position;
        }

        public MeasurePointVM(MeasurePointsVM pointsManager, PatternRecognitionDataWithContext patternRec, XYZTopZBottomPosition position, int siteId, bool isPositionOnDie) : this(pointsManager, position, siteId, isPositionOnDie)
        {
            PointPatternRec = patternRec;
        }

        public static MeasurePointVM FromMeasurePoint(MeasurePointsVM pointsManager, MeasurePoint measurePoint, bool isPositionOnDie)
        {
            MeasurePointVM measurePointVM;
            if (measurePoint.PatternRec is null)
                measurePointVM = new MeasurePointVM(pointsManager, measurePoint.Position.ToXYZTopZBottomPosition(GetWaferOrCurrentDieReferential(pointsManager, isPositionOnDie)), measurePoint.Id, isPositionOnDie);
            else
                measurePointVM = new MeasurePointVM(pointsManager, measurePoint.PatternRec, measurePoint.Position.ToXYZTopZBottomPosition(GetWaferOrCurrentDieReferential(pointsManager, isPositionOnDie)), measurePoint.Id, isPositionOnDie);
            measurePointVM.Id = measurePoint.Id;
            return measurePointVM;
        }

        public static MeasurePointVM NewFromMeasurePoint(MeasurePointsVM pointsManager, MeasurePoint measurePoint, bool isPositionOnDie, int newSiteId)
        {
            MeasurePointVM measurePointVM;
            if (measurePoint.PatternRec is null)
            {
                measurePointVM = new MeasurePointVM(pointsManager, measurePoint.Position.ToXYZTopZBottomPosition(GetWaferOrCurrentDieReferential(pointsManager, isPositionOnDie)), newSiteId, isPositionOnDie);
            }
            else
            {
                var patternRecClone = (PatternRecognitionDataWithContext)measurePoint.PatternRec.Clone();
                measurePointVM = new MeasurePointVM(pointsManager, patternRecClone, measurePoint.Position.ToXYZTopZBottomPosition(GetWaferOrCurrentDieReferential(pointsManager, isPositionOnDie)), newSiteId, isPositionOnDie);
            }
            return measurePointVM;
        }

        private void AxesVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AxesVM.CurrentDieIndex))
            {
                UpdateDisplayPosition();
            }
        }

        private void UpdateDisplayPosition()
        {
            var positionOnWafer = ServiceLocator.ReferentialSupervisor.ConvertTo(GetPositionOnWaferOrCurrentDie(), ReferentialTag.Wafer)?.Result.ToXYPosition();
            if (positionOnWafer != null)
            {
                DisplayPosition = new Point(positionOnWafer.X, positionOnWafer.Y);
            }
        }

        private void UpdateDisplayPositionOnDie()
        {
            // If we don't have a wafer map we don't have a display position on die
            if (!IsDiePosition)
                return;
            var positionOnDie = ServiceLocator.ReferentialSupervisor.ConvertTo(GetPositionOnWaferOrCurrentDie(), ReferentialTag.Die)?.Result.ToXYPosition();
            if (positionOnDie != null)
            {
                _displayPositionOnDie = new Point(positionOnDie.X, positionOnDie.Y);
            }
        }

        private bool _isSelected = false;

        public bool IsSelected
        {
            get => _isSelected; set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(); } }
        }

        private int _index = 0;

        public int Index
        {
            get
            {
                return _index;
            }

            set
            {
                if (_index == value)
                {
                    return;
                }

                _index = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Name));
            }
        }

        #region ICameraDisplayPoint

        public string Name
        {
            get { return $"{Id}"; }
        }

        // Position used for the display on the camera in wafer coordinates
        public Point DisplayPosition
        {
            set { _displayPosition = value; _pointsManager.UpdatePoint(this); }
            get { return _displayPosition; }
        }

        // Position used for the display on the navigation Die
        public Point? DisplayPositionOnDie
        {
            get { return _displayPositionOnDie; }
        }

        #endregion ICameraDisplayPoint

        private int? _id = null;

        public int? Id
        {
            get => _id;
            internal set => SetProperty(ref _id, value);
        }

        private bool _isModified = false;

        public bool IsModified
        {
            get => _isModified;
            internal set => SetProperty(ref _isModified, value);
        }

        private PatternRecognitionDataWithContext _pointPatternRec = null;

        public PatternRecognitionDataWithContext PointPatternRec
        {
            get => _pointPatternRec; set { if (_pointPatternRec != value) { _pointPatternRec = value; OnPropertyChanged(); } }
        }

        // Position of the point in die referential if there is a wafer map, in wafer referential if there is no wafer map.

        private XYZTopZBottomPosition _pointPosition = null;

        public XYZTopZBottomPosition PointPosition
        {
            get => _pointPosition;
            set
            {
                if (_pointPosition != value)
                {
                    _pointPosition = value;
                    // Do not change the order of the updates below 
                    UpdateDisplayPositionOnDie();
                    UpdateDisplayPosition();
                    OnPropertyChanged();
                }
            }
        }

        private bool _isSubMeasurePoint = false;

        public bool IsSubMeasurePoint
        {
            get => _isSubMeasurePoint;
            set => SetProperty(ref _isSubMeasurePoint, value);
        }

        #region RelayCommands

        private AutoRelayCommand _displayImage;

        public AutoRelayCommand DisplayImage
        {
            get
            {
                return _displayImage ?? (_displayImage = new AutoRelayCommand(
                    () =>
                    {
                        var patternRecDisplayVM = new PatternRecDisplayVM() { PatternRec = PointPatternRec };

                        ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowDialog<PatternRecDisplay>(patternRecDisplayVM);
                    },
                    () => { return (!(PointPatternRec is null)); }
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
                        var patternRecVM = new PatternRecParametersVM(new PositionWithPatternRec(PointPosition, PointPatternRec));

                        ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowDialog<PatternRecParametersView>(patternRecVM);
                    },
                    () => { return (!(PointPatternRec is null)); }
                ));
            }
        }

        private AutoRelayCommand _deletePoint;

        public AutoRelayCommand DeletePoint
        {
            get
            {
                return _deletePoint ?? (_deletePoint = new AutoRelayCommand(
                    () =>
                    {
                        _pointsManager.DeletePoint(this);
                    },
                    () => { return true; }
                ));
            }
        }

        private AsyncRelayCommand _gotoPointPosition;

        public AsyncRelayCommand GotoPointPosition
        {
            get
            {
                return _gotoPointPosition ?? (_gotoPointPosition = new AsyncRelayCommand(
                    async () =>
                    {
                        // TODO : voir pourquoi destPosition était affecté différemment selon la présence de PatternRec ou non
                        // FDS + RBU : GetPositionOnWaferOrCurrentDie() utilisé dans les 2 cas désormais
                        var destPosition = GetPositionOnWaferOrCurrentDie();
                        string selectedObjectiveId;
                        if (!(PointPatternRec is null))
                        {
                            var patternRecContext = PointPatternRec.Context as TopImageAcquisitionContext;
                            patternRecContext.Lights.Lights.ForEach(light =>
                            {
                                ServiceLocator.LightsSupervisor.SetLightIntensity(light.DeviceID, light.Intensity);
                            });
                            selectedObjectiveId = patternRecContext.TopObjectiveContext.ObjectiveId;
                            _pointsManager.SelectedObjective = CamerasSupervisor.GetObjectiveFromId(selectedObjectiveId);
                            await ServiceLocator.CamerasSupervisor.WaitObjectiveChanged(selectedObjectiveId, true);
                        }
                        else
                        {
                            selectedObjectiveId = ServiceLocator.CamerasSupervisor.Objective.DeviceID;
                        }

                        ServiceLocator.AxesSupervisor.GotoPosition(destPosition, AxisSpeed.Fast);
                        double pixelSizemm = ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(selectedObjectiveId).Image.PixelSizeX.Millimeters;

                        if (!(PointPatternRec is null))
                        {
                            if (PointPatternRec.RegionOfInterest is null)
                            {
                                _pointsManager.RecipeMeasure.RoiRect = Rect.Empty;
                            }
                            else
                            {
                                _pointsManager.RecipeMeasure.RoiRect = new Rect(PointPatternRec.RegionOfInterest.X.ToPixels(pixelSizemm.Millimeters()),
                                                                                PointPatternRec.RegionOfInterest.Y.ToPixels(pixelSizemm.Millimeters()),
                                                                                PointPatternRec.RegionOfInterest.Width.ToPixels(pixelSizemm.Millimeters()),
                                                                                PointPatternRec.RegionOfInterest.Height.ToPixels(pixelSizemm.Millimeters()));
                            }
                        }
                        else
                        {
                            _pointsManager.RecipeMeasure.ResetRoiRect();
                        }
                    },

                    () => { return true; }
                ));
            }
        }

        private AutoRelayCommand _testMeasureOnPoint;

        public AutoRelayCommand TestMeasureOnPoint
        {
            get
            {
                return _testMeasureOnPoint ?? (_testMeasureOnPoint = new AutoRelayCommand(
                    () =>
                    {
                        _pointsManager.RecipeMeasure.TestMeasureOnPoint(this);
                    },
                    () => { return _pointsManager.RecipeMeasure.MeasureSettings.AreSettingsValid(null, true); }
                ));
            }
        }

        private AutoRelayCommand _updatePointPosition;

        public AutoRelayCommand UpdatePointPosition
        {
            get
            {
                return _updatePointPosition ?? (_updatePointPosition = new AutoRelayCommand(
                    () =>
                    {
                        try
                        {
                            if (!(PointPatternRec is null))
                            {
                                Rect rectRoi;
                                if (_pointsManager.RecipeMeasure.IsCenteredRoi)
                                {
                                    rectRoi = new Rect(_pointsManager.RecipeMeasure.RoiSize);
                                }
                                else
                                {
                                    rectRoi = _pointsManager.RecipeMeasure.RoiRect;
                                }
                                // It is a point with image, we update also the pattern rec
                                var newPatternRecImage = PatternRecHelpers.CreatePatternRectWithContext(rectRoi, _pointsManager.RecipeMeasure.IsCenteredRoi);
                                PointPatternRec = newPatternRecImage;
                            }

                            var newPointPosition = _pointsManager.GetCurrentPositionOnRelevantReferential();
                            PointPosition = newPointPosition;
                            IsModified = true;
                        }
                        catch (Exception ex)
                        {
                            ClassLocator.Default.GetInstance<ILogger>().Error(ex, "UpdatePointPosition: Error during update point position");
                        }
                    },
                    () => { return true; }
                ));
            }
        }

        #endregion RelayCommands

        private static ReferentialBase GetWaferOrCurrentDieReferential(MeasurePointsVM pointsManager, bool isPositionOnDie)
        {
            if (!isPositionOnDie)
                return new WaferReferential();

            // AxesVM.CurrentDieIndex can be null when we load a Recipe from DB for example. When that happen,
            // just set the current die index to the default one.
            DieIndex currentDieIndex = ServiceLocator.AxesSupervisor.AxesVM.CurrentDieIndex ?? new DieIndex();
            return new DieReferential(currentDieIndex.Column, currentDieIndex.Row);
        }

        private PositionBase GetPositionOnWaferOrCurrentDie()
        {
            return PointPosition.PositionInReferential(GetWaferOrCurrentDieReferential(_pointsManager, IsDiePosition));
        }

        public void Dispose()
        {
            ServiceLocator.AxesSupervisor.AxesVM.PropertyChanged -= AxesVM_PropertyChanged;
        }
    }
}
