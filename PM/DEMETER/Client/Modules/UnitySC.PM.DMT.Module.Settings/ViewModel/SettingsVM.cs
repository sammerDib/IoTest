using System;
using System.Collections.Generic;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.DMT.CommonUI;
using UnitySC.PM.DMT.CommonUI.ViewModel;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.PM.DMT.Shared.UI.ViewModel;
using UnitySC.PM.Shared.Hardware.ClientProxy.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.DMT.Modules.Settings.ViewModel
{
    public class SettingsVM : ObservableObject, IMenuContentViewModel, IDisposable
    {
        private const int MotionEndTimeout = 5000;
        private const string LinearAxisID = "Linear";

        private readonly CameraSupervisor _cameraSupervisor;
        private readonly ScreenSupervisor _screenSupervisor;
        private readonly CalibrationSupervisor _calibrationSupervisor;
        private readonly AlgorithmsSupervisor _algorithmsSupervisor;
        private readonly MotionAxesSupervisor _motionAxesSupervisor;
        private readonly IDialogOwnerService _dialogService;
        private readonly ILogger _logger;

        private DMTChuckPosition _chuckPosition;

        private Side _currentVisibleSide = Side.Front;

        private AutoRelayCommand _displayBackSettings;

        private AutoRelayCommand _displayFrontSettings;

        private bool _isVisible;

        private RelayCommand _moveToLoadingPositionCommand;

        private RelayCommand _moveToMeasurementPositionCommand;

        private ITabManager _selectedSettingBack;

        private ITabManager _selectedSettingFront;

        private List<ISettingVM> _settingsBack;

        private List<ISettingVM> _settingsFront;

        public SettingsVM(CameraSupervisor cameraSupervisor, ScreenSupervisor screenSupervisor, CalibrationSupervisor calibrationSupervisor,
            MotionAxesSupervisor motionAxesSupervisor, AlgorithmsSupervisor algorithmsSupervisor,
            IDialogOwnerService dialogService, ILogger logger, IMessenger messenger)
        {
            _cameraSupervisor = cameraSupervisor;
            _screenSupervisor = screenSupervisor;
            _calibrationSupervisor = calibrationSupervisor;
            _algorithmsSupervisor = algorithmsSupervisor;
            _motionAxesSupervisor = motionAxesSupervisor;
            _dialogService = dialogService;
            _logger = logger;
            _settingsFront = new List<ISettingVM>();
            _settingsBack = new List<ISettingVM>();

            if (IsFrontCameraAvailable)
            {
                var side = Side.Front;
                var exposureSettings = CreateExposureSettings(side, 80, MeasureType.BrightFieldMeasure);

                CreateAndAddSettings(SettingsFront, side, exposureSettings, messenger);
            }

            if (IsBackCameraAvailable)
            {
                var side = Side.Back;
                var exposureSettings = CreateExposureSettings(side, 80, MeasureType.BrightFieldMeasure);

                CreateAndAddSettings(SettingsBack, side, exposureSettings, messenger);
            }

            SelectedSettingFront = _settingsFront.First() as ITabManager;
        }

        public DMTChuckPosition ChuckPosition
        {
            get => _chuckPosition;
            set
            {
                if (_chuckPosition != value)
                {
                    _chuckPosition = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanMoveToMeasurementPosition));
                    MoveToMeasurementPositionCommand.NotifyCanExecuteChanged();
                    MoveToLoadingPositionCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public bool CanMoveToMeasurementPosition => ChuckPosition != DMTChuckPosition.Process;

        public RelayCommand MoveToMeasurementPositionCommand =>
            _moveToMeasurementPositionCommand ?? (_moveToMeasurementPositionCommand = new RelayCommand(() =>
                {
                    MoveToPosition(DMTChuckPosition.Process);
                },
                () => CanMoveToMeasurementPosition));

        public RelayCommand MoveToLoadingPositionCommand =>
            _moveToLoadingPositionCommand ?? (_moveToLoadingPositionCommand = new RelayCommand(() =>
                {
                    MoveToPosition(DMTChuckPosition.Loading);
                },
                () => !CanMoveToMeasurementPosition));

        public List<ISettingVM> SettingsFront
        {
            get => _settingsFront;
            set
            {
                if (_settingsFront != value)
                {
                    _settingsFront = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<ISettingVM> SettingsBack
        {
            get => _settingsBack;
            set
            {
                if (_settingsBack != value)
                {
                    _settingsBack = value;
                    OnPropertyChanged();
                }
            }
        }

        public Side CurrentVisibleSide
        {
            get => _currentVisibleSide;
            set
            {
                if (_currentVisibleSide != value)
                {
                    HideSelectedSetting(_currentVisibleSide, null);
                    _currentVisibleSide = value;
                    DisplaySelectedSetting(_currentVisibleSide);
                    OnPropertyChanged();
                }
            }
        }

        public ITabManager SelectedSettingFront
        {
            get => _selectedSettingFront;

            set
            {
                if (_selectedSettingFront == value)
                {
                    return;
                }

                HideSelectedSetting(Side.Front, value);
                _selectedSettingFront = value;
                if (_isVisible)
                    DisplaySelectedSetting(Side.Front);
                OnPropertyChanged();
            }
        }

        public ITabManager SelectedSettingBack
        {
            get => _selectedSettingBack;

            set
            {
                if (_selectedSettingBack == value)
                {
                    return;
                }

                HideSelectedSetting(Side.Back, value);

                _selectedSettingBack = value;
                if (_isVisible)
                    DisplaySelectedSetting(Side.Back);
                OnPropertyChanged();
            }
        }

        public bool IsFrontCameraAvailable
        {
            get
            {
                var camerasIds = _cameraSupervisor.GetCameraSides();
                return camerasIds.Contains(Side.Front);
            }
        }

        public bool IsBackCameraAvailable
        {
            get
            {
                var camerasIds = _cameraSupervisor.GetCameraSides();
                return camerasIds.Contains(Side.Back);
            }
        }

        public bool IsVisible
        {
            get => _isVisible;

            set
            {
                if (_isVisible == value)
                {
                    return;
                }

                _isVisible = value;

                if (_isVisible)
                {
                    DisplaySelectedSetting(CurrentVisibleSide);
                    var currentPosition = (XTPosition)_motionAxesSupervisor.GetCurrentPosition().Result;
                    if (currentPosition != null)
                    {
                        ChuckPosition = (DMTChuckPosition)(int)currentPosition.X;
                    }
                }
                else
                {
                    HideSelectedSetting(CurrentVisibleSide, null);
                }

                OnPropertyChanged();
            }
        }

        public AutoRelayCommand DisplayFrontSettings =>
            _displayFrontSettings ?? (_displayFrontSettings = new AutoRelayCommand(
                () =>
                {
                    CurrentVisibleSide = Side.Front;
                },
                () => { return true; }
            ));

        public AutoRelayCommand DisplayBackSettings =>
            _displayBackSettings ?? (_displayBackSettings = new AutoRelayCommand(
                () =>
                {
                    CurrentVisibleSide = Side.Back;
                },
                () => { return true; }
            ));

        public void Dispose()
        {
            foreach (var setting in SettingsFront)
            {
                if (setting is IDisposable)
                {
                    (setting as IDisposable).Dispose();
                }
            }

            foreach (var setting in SettingsBack)
            {
                if (setting is IDisposable)
                {
                    (setting as IDisposable).Dispose();
                }
            }
        }

        public void Refresh()
        {
            IsVisible = true;
        }

        public bool CanClose()
        {
            IsVisible = false;
            return true;
        }

        public bool IsEnabled => true;

        private void MoveToPosition(DMTChuckPosition destination)
        {
            if (ChuckPosition != destination)
            {
                var position = new Length((double)destination, LengthUnit.Millimeter);
                var move = new PMAxisMove(LinearAxisID, position);
                _motionAxesSupervisor.Move(move);
                _motionAxesSupervisor.WaitMotionEnd(MotionEndTimeout);
                ChuckPosition = destination;
            }
        }

        private void CreateAndAddSettings(List<ISettingVM> settings, Side side,
            ExposureSettingsWithAutoVM exposureSettings, IMessenger messenger)
        {
            var highAngleDarkFieldExposureSettings = CreateExposureSettings(side, 50, MeasureType.HighAngleDarkFieldMeasure);
            settings.Add(new DeadPixelsVM(side, _cameraSupervisor, _screenSupervisor, _calibrationSupervisor, _dialogService, _logger));
            settings.Add(new CameraFocusVM(side, exposureSettings, _cameraSupervisor, _screenSupervisor, _dialogService, _logger, messenger));
            settings.Add(new AlignmentVM(side, exposureSettings, _cameraSupervisor, _screenSupervisor, _calibrationSupervisor, _dialogService, _logger, messenger));
            settings.Add(new PerspectiveVM(side, exposureSettings, _cameraSupervisor, _screenSupervisor, _calibrationSupervisor, _algorithmsSupervisor, _dialogService, _logger));
            settings.Add(new ExposureVM(side, _cameraSupervisor, _screenSupervisor, _calibrationSupervisor, _dialogService, _logger));
            settings.Add(new CurvatureDynamicsVM(side, exposureSettings, _cameraSupervisor, _screenSupervisor, _calibrationSupervisor, _dialogService, _logger, messenger));
            settings.Add(new HighAngleDarkFieldMaskVM(side, highAngleDarkFieldExposureSettings, _cameraSupervisor, _screenSupervisor, _calibrationSupervisor, _dialogService, _logger, messenger));
            settings.Add(new GlobalTopoVM(side, exposureSettings, _cameraSupervisor, _screenSupervisor, _calibrationSupervisor, _dialogService, _logger));
            settings.Add(new SystemUniformityVM(side, exposureSettings, _cameraSupervisor, _screenSupervisor, _calibrationSupervisor, _dialogService, _logger));
        }

        private ExposureSettingsWithAutoVM CreateExposureSettings(Side side, double exposureTimeMs,
            MeasureType measure)
        {
            return new ExposureSettingsWithAutoVM(side, measure, _cameraSupervisor, _screenSupervisor, _algorithmsSupervisor, _dialogService)
            {
                ExposureTimeMs = exposureTimeMs,
                EditExposureTime = exposureTimeMs,
                ExposureTimeStatus = ExposureTimeStatus.Valid
            };
        }

        private void HideSelectedSetting(Side waferSide, ITabManager nextSetting)
        {
            var selectedSetting = waferSide == Side.Front ? _selectedSettingFront : _selectedSettingBack;
            if (!(selectedSetting is null))
            {
                if (!selectedSetting.CanHide())
                {
                    return;
                }
                if (selectedSetting is SettingWithVideoStreamVM selectedSettingWithStream && nextSetting is SettingWithVideoStreamVM nextSetttingWithStream)
                {
                    selectedSettingWithStream.NeedsToStopGrabOnHiding = false;
                    nextSetttingWithStream.NeedsToStartGrabOnDisplay = false;
                }
                else if (selectedSetting is SettingWithVideoStreamVM sSettingWithStream && nextSetting is SettingVM)
                {
                    sSettingWithStream.NeedsToStopGrabOnHiding = true;
                }
                else if (selectedSetting is SettingVM && nextSetting is SettingWithVideoStreamVM nSetttingWithStream)
                {
                    nSetttingWithStream.NeedsToStartGrabOnDisplay = true;
                }
                selectedSetting.Hide();
            }
        }

        private void DisplaySelectedSetting(Side waferSide)
        {
            var selectedSetting = waferSide == Side.Front ? _selectedSettingFront : _selectedSettingBack;

            if (!(selectedSetting is null))
            {
                selectedSetting.Display();
            }

            switch (waferSide)
            {
                case Side.Front:
                    if (_selectedSettingFront != null)
                    {
                        _selectedSettingFront.Display();
                    }

                    break;

                case Side.Back:
                    if (_selectedSettingBack != null)
                    {
                        _selectedSettingBack.Display();
                    }

                    break;
            }
        }
    }
}
