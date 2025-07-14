using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.DMT.CommonUI.ViewModel;
using UnitySC.PM.DMT.Hardware.ClientProxy.Screen;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.PM.DMT.Client.Modules.TestHardware.ViewModel
{
    public class TestScreenVM : ObservableRecipient, ITabManager, IRecipient<BacklightChangedMessage>, IRecipient<BrightnessChangedMessage>, IRecipient<ContrastChangedMessage>,
        IRecipient<TemperatureChangedMessage>, IRecipient<SharpnessChangedMessage>, IRecipient<FanChangedMessage>,
        IRecipient<FanAutoChangedMessage>, IRecipient<PowerStateChangedMessage>
    {
        private readonly Dictionary<string, short> _screenConfigValues;
        private readonly ScreenSupervisor _screenSupervisor;

        public TestScreenVM(Side waferSide, ScreenSupervisor screenSupervisor)
        {
            _screenSupervisor = screenSupervisor;
            WaferSide = waferSide;
            _screenConfigValues = _screenSupervisor.GetDefaultScreenValues(WaferSide);
        }

        public TestScreenVM(Side waferSide, ScreenSupervisor screenSupervisor, IMessenger messenger) : base(messenger)
        {
            _screenSupervisor = screenSupervisor;
            WaferSide = waferSide;
            _screenConfigValues = _screenSupervisor.GetDefaultScreenValues(WaferSide);
        }

        private double _progressBarValue = 0;

        public double ProgressBarValue
        {
            get { return _progressBarValue; }
            set
            {
                _progressBarValue = value;
                OnPropertyChanged();
            }
        }

        private int _progressMaximum = 100;

        public int ProgressMaximum
        {
            get { return _progressMaximum; }
        }

        private bool _isWhiteDisplayedOnScreen;

        public bool IsWhiteDisplayedOnScreen
        {
            get => _isWhiteDisplayedOnScreen;
            set
            {
                if (SetProperty(ref _isWhiteDisplayedOnScreen, value))
                {
                    SetScreenWhiteColorCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private int _screenWidth;

        public int ScreenWidth
        {
            get => _screenWidth;
            set => SetProperty(ref _screenWidth, value);
        }

        private int _screenHeight;

        public int ScreenHeight
        {
            get => _screenHeight;
            set => SetProperty(ref _screenHeight, value);
        }

        private double _screenWhiteDisplayTimeSec;

        public double ScreenWhiteDisplayTimeSec
        {
            get => _screenWhiteDisplayTimeSec;
            set => SetProperty(ref _screenWhiteDisplayTimeSec, value);
        }

        private double _screenTemperature;

        public double ScreenTemperature
        {
            get => _screenTemperature;
            set => SetProperty(ref _screenTemperature, value);
        }

        private Side _waferSide;

        public Side WaferSide
        {
            get => _waferSide;
            set => SetProperty(ref _waferSide, value);
        }

        private bool _isFanAuto;

        public bool IsFanAuto
        {
            get => _isFanAuto;
            set => SetProperty(ref _isFanAuto, value);
        }

        private bool _isScreenOn = true;

        public bool IsScreenOn
        {
            get => _isScreenOn;
            set => SetProperty(ref _isScreenOn, value);
        }

        #region Backlight

        private short _backlight;

        public short Backlight
        {
            get => _backlight;
            set
            {
                if (SetProperty(ref _backlight, value))
                {
                    HasBacklightChanged = _backlight != GetDefaultScreenValue("backlight") || HasBacklightChangedFromDefault;
                }
            }
        }

        private bool _hasBacklightChanged;

        public bool HasBacklightChanged
        {
            get { return _hasBacklightChanged; }
            set
            {
                if (SetProperty(ref _hasBacklightChanged, value))
                {
                    UpdateBacklightCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private bool _hasBacklightChangedFromFromDefault;

        public bool HasBacklightChangedFromDefault
        {
            get => _hasBacklightChangedFromFromDefault;
            set
            {
                if (SetProperty(ref _hasBacklightChangedFromFromDefault, value))
                {
                    IsBacklightChanging = false;
                    ResetBacklight.NotifyCanExecuteChanged();
                }
            }
        }

        private bool _isBacklightChanging;

        public bool IsBacklightChanging
        {
            get { return _isBacklightChanging; }
            set
            {
                if (SetProperty(ref _isBacklightChanging, value))
                {
                    ResetBacklight.NotifyCanExecuteChanged();
                }
            }
        }

        private AsyncRelayCommand _updateBacklightCommand;

        public AsyncRelayCommand UpdateBacklightCommand
        {
            get
            {
                return _updateBacklightCommand ?? (_updateBacklightCommand = new AsyncRelayCommand(
              async () =>
              {
                  HasBacklightChangedFromDefault = _backlight != GetDefaultScreenValue("backlight");

                  HasBacklightChanged = false;
                  await _screenSupervisor.SetBacklightAsync(WaferSide, Backlight);
                  IsBacklightChanging = true;
              }, () => HasBacklightChanged));
            }
        }

        private AsyncRelayCommand _resetBacklight;

        public AsyncRelayCommand ResetBacklight
        {
            get
            {
                return _resetBacklight ?? (_resetBacklight = new AsyncRelayCommand(

                    async () =>
                    {
                        short defaultValue = GetDefaultScreenValue("backlight");
                        await _screenSupervisor.SetBacklightAsync(WaferSide, defaultValue);
                        HasBacklightChangedFromDefault = false;
                        IsBacklightChanging = true;
                    }, () => !IsBacklightChanging && HasBacklightChangedFromDefault));
            }
        }

        #endregion Backlight

        #region Brightness

        private short _brightness;

        public short Brightness
        {
            get => _brightness;
            set
            {
                if (SetProperty(ref _brightness, value))
                {
                    HasBrightnessChanged = _brightness != GetDefaultScreenValue("brightness") || HasBrightnessChangedFromDefault;
                }
            }
        }

        private bool _hasBrightnessChanged;

        public bool HasBrightnessChanged
        {
            get => _hasBrightnessChanged;
            set
            {
                if (SetProperty(ref _hasBrightnessChanged, value))
                {
                    IsBrightnessChanging = false;
                    UpdateBrightnessCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private bool _hasBrightnessChangedFromDefault;

        public bool HasBrightnessChangedFromDefault
        {
            get => _hasBrightnessChangedFromDefault;
            set
            {
                if (SetProperty(ref _hasBrightnessChangedFromDefault, value))
                {
                    ResetBrightness.NotifyCanExecuteChanged();
                }
            }
        }

        private bool _isBrightnessChanging;

        public bool IsBrightnessChanging
        {
            get => _isBrightnessChanging;
            set
            {
                if (SetProperty(ref _isBrightnessChanging, value))
                {
                    ResetBrightness.NotifyCanExecuteChanged();
                }
            }
        }

        private AsyncRelayCommand _updateBrightnessCommand;

        public AsyncRelayCommand UpdateBrightnessCommand
        {
            get
            {
                return _updateBrightnessCommand ?? (_updateBrightnessCommand = new AsyncRelayCommand(
              async () =>
              {
                  HasBrightnessChangedFromDefault = _brightness != GetDefaultScreenValue("brightness");
                  HasBrightnessChanged = false;
                  await _screenSupervisor.SetBrightnessAsync(WaferSide, Brightness);
                  IsBrightnessChanging = true;
              }, () => HasBrightnessChanged
              ));
            }
        }

        private AsyncRelayCommand _resetBrightness;

        public AsyncRelayCommand ResetBrightness
        {
            get
            {
                return _resetBrightness ?? (_resetBrightness = new AsyncRelayCommand(

                    async () =>
                    {
                        short defaultValue = GetDefaultScreenValue("brightness");
                        await _screenSupervisor.SetBrightnessAsync(WaferSide, defaultValue);
                        HasBrightnessChangedFromDefault = false;
                        IsBrightnessChanging = true;
                    }, () => !IsBrightnessChanging && HasBrightnessChangedFromDefault
                    ));
            }
        }

        #endregion Brightness

        #region Contrast

        private short _contrast;

        public short Contrast
        {
            get => _contrast;
            set
            {
                if (SetProperty(ref _contrast, value))
                {
                    HasContrastChanged = _contrast != GetDefaultScreenValue("contrast") || HasContrastChangedFromDefault;
                }
            }
        }

        private bool _hasContrastChanged;

        public bool HasContrastChanged
        {
            get => _hasContrastChanged;
            set
            {
                if (SetProperty(ref _hasContrastChanged, value))
                {
                    UpdateContrastCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private bool _hasContrastChangedFromDefault;

        public bool HasContrastChangedFromDefault
        {
            get => _hasContrastChangedFromDefault;
            set
            {
                if (SetProperty(ref _hasContrastChangedFromDefault, value))
                {
                    IsContrastChanging = false;
                    ResetContrastCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private bool _isContrastChanging;

        public bool IsContrastChanging
        {
            get => _isContrastChanging;
            set
            {
                if (SetProperty(ref _isContrastChanging, value))
                {
                    ResetContrastCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private AsyncRelayCommand _updateContrastCommand;

        public AsyncRelayCommand UpdateContrastCommand
        {
            get
            {
                return _updateContrastCommand ?? (_updateContrastCommand = new AsyncRelayCommand(
                async () =>
                {
                    HasContrastChangedFromDefault = _contrast != GetDefaultScreenValue("contrast");
                    HasContrastChanged = false;
                    await _screenSupervisor.SetContrastAsync(WaferSide, Contrast);
                    IsContrastChanging = true;
                }, () => HasContrastChanged));
            }
        }

        private AsyncRelayCommand _resetContrastCommand;

        public AsyncRelayCommand ResetContrastCommand
        {
            get
            {
                return _resetContrastCommand ?? (_resetContrastCommand = new AsyncRelayCommand(

                    async () =>
                    {
                        short defaultValue = GetDefaultScreenValue("contrast");
                        await _screenSupervisor.SetContrastAsync(WaferSide, defaultValue);
                        HasContrastChangedFromDefault = false;
                        IsContrastChanging = true;
                    }, () => !IsContrastChanging && HasContrastChangedFromDefault
                    ));
            }
        }

        #endregion Contrast

        #region Sharpness

        private int _sharpness;

        public int Sharpness
        {
            get => _sharpness;
            set => SetProperty(ref _sharpness, value);
        }

        private AsyncRelayCommand _updateSharpnessCommand;

        public AsyncRelayCommand UpdateSharpnessCommand
        {
            get
            {
                return _updateSharpnessCommand ?? (_updateSharpnessCommand = new AsyncRelayCommand(
                async () =>
                {
                    await _screenSupervisor.SetSharpnessAsync(WaferSide, Sharpness);
                }));
            }
        }

        #endregion Sharpness

        #region FanSpeed

        private bool _hasFanSpeedChanged;

        public bool HasFanSpeedChanged
        {
            get => _hasFanSpeedChanged;
            set
            {
                if (SetProperty(ref _hasFanSpeedChanged, value))
                {
                    UpdateFanSpeedCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private int _fanSpeed;

        public int FanSpeed
        {
            get => _fanSpeed;
            set
            {
                if (SetProperty(ref _fanSpeed, value))
                {
                    HasFanSpeedChanged = true;
                }
            }
        }

        private AsyncRelayCommand _updateFanSpeedCommand;

        public AsyncRelayCommand UpdateFanSpeedCommand
        {
            get
            {
                return _updateFanSpeedCommand ?? (_updateFanSpeedCommand = new AsyncRelayCommand(
              async () =>
              {
                  HasFanSpeedChanged = false;
                  await _screenSupervisor.SetFanSpeedAsync(WaferSide, FanSpeed);
              }, () => HasFanSpeedChanged));
            }
        }

        #endregion FanSpeed

        private AsyncRelayCommand _switchScreenOnOff;

        public AsyncRelayCommand SwitchScreenOnOff
        {
            get
            {
                return _switchScreenOnOff ?? (_switchScreenOnOff = new AsyncRelayCommand(
                    async () =>
                    {
                        await _screenSupervisor.SwitchScreenOnOffAsync(WaferSide, IsScreenOn);
                    }));
            }
        }

        private short GetDefaultScreenValue(string value)
        {
            return _screenConfigValues.TryGetValue(value, out short result) ? result : (short)0;
        }

        private AsyncRelayCommand _turnFanAutoOn;

        public AsyncRelayCommand TurnFanAutoOn
        {
            get
            {
                return _turnFanAutoOn ?? (_turnFanAutoOn = new AsyncRelayCommand(
                    async () =>
                    {
                        await _screenSupervisor.TurnFanAutoOnAsync(WaferSide, IsFanAuto);
                    }));
            }
        }

        private double _temperature;

        public double Temperature
        {
            get => _temperature;
            set => SetProperty(ref _temperature, value);
        }

        private AutoRelayCommand _setScreenBlackColorCommand;

        public AutoRelayCommand SetScreenBlackColorCommand
        {
            get
            {
                return _setScreenBlackColorCommand ?? (_setScreenBlackColorCommand = new AutoRelayCommand(() =>
                {
                    if (SetScreenWhiteColorCommand.IsRunning && SetScreenWhiteColorCommand.CanBeCanceled)
                    {
                        SetScreenWhiteColorCommand.Cancel();
                    }
                }, () => SetScreenWhiteColorCommand.IsRunning));
            }
        }

        private AsyncRelayCommand _setScreenWhiteColorCommand;

        public AsyncRelayCommand SetScreenWhiteColorCommand
        {
            get
            {
                return _setScreenWhiteColorCommand ?? (_setScreenWhiteColorCommand = new AsyncRelayCommand(async (cancellationToken) =>
                {
                    SetScreenWhiteColorCommand.NotifyCanExecuteChanged();
                    SetScreenBlackColorCommand.NotifyCanExecuteChanged();
                    try
                    {
                        _progressMaximum = (int)ScreenWhiteDisplayTimeSec;
                        OnPropertyChanged(nameof(ProgressMaximum));
                        ProgressBarValue = ProgressMaximum;
                        _screenSupervisor.SetScreenColor(WaferSide, Colors.White, false);
                        await Task.Run(WaitAndDecreaseProgress(cancellationToken), cancellationToken);
                    }
                    catch (OperationCanceledException _)
                    {
                        //Nothing to do as the cancellation has been requested
                    }
                    finally
                    {
                        _screenSupervisor.SetScreenColor(WaferSide, Colors.Black, false);
                        ProgressBarValue = ProgressMaximum;
                    }
                }, () => !SetScreenWhiteColorCommand.IsRunning && !IsWhiteDisplayedOnScreen));
            }
        }

        private Func<Task> WaitAndDecreaseProgress(CancellationToken cancellationToken)
        {
            return async () =>
            {
                while (0 < ProgressBarValue)
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                        ProgressBarValue--;
                    }
                    catch (OperationCanceledException _)
                    {
                        break;
                    }
                }
            };
        }

        private void GetScreenPropertyValues()
        {
            Backlight = _screenSupervisor.GetBacklight(_waferSide);
            Brightness = _screenSupervisor.GetBrightness(_waferSide);
            Contrast = _screenSupervisor.GetContrast(_waferSide);
            Temperature = _screenSupervisor.GetTemperature(_waferSide);
            FanSpeed = _screenSupervisor.GetFanRPM(_waferSide);
            var screenInfo = _screenSupervisor.GetScreenInfo(_waferSide);

            if (screenInfo != null)
            {
                ScreenWhiteDisplayTimeSec = screenInfo.ScreenWhiteDisplayTimeSec;
                ScreenHeight = screenInfo.Height;
                ScreenWidth = screenInfo.Width;
            }

            _screenSupervisor.TriggerUpdateEvent(_waferSide);
        }

        public void Display()
        {
            IsActive = true;
            GetScreenPropertyValues();
        }

        public bool CanHide()
        {
            return true;
        }

        public void Hide()
        {
            IsActive = false;
            if (SetScreenWhiteColorCommand.CanBeCanceled && SetScreenWhiteColorCommand.IsRunning)
            {
                SetScreenWhiteColorCommand.Cancel();
            }
        }

        public void Receive(BacklightChangedMessage message)
        {
            if (message.Side == WaferSide)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Backlight = message.Backlight;
                    HasBacklightChanged = false;
                    IsBacklightChanging = false;
                });
            }
        }

        public void Receive(BrightnessChangedMessage message)
        {
            if (message.Side == WaferSide)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Brightness = message.Brightness;
                    HasBrightnessChanged = false;
                    IsBrightnessChanging = false;
                });
            }
        }

        public void Receive(ContrastChangedMessage message)
        {
            if (message.Side == WaferSide)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Contrast = message.Contrast;
                    HasContrastChanged = false;
                    IsContrastChanging = false;
                });
            }
        }

        public void Receive(TemperatureChangedMessage message)
        {
            if (message.Side == WaferSide)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Temperature = message.Temperature;
                });
            }
        }

        public void Receive(SharpnessChangedMessage message)
        {
            if (message.Side == WaferSide)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Sharpness = message.Sharpness;
                });
            }
        }

        public void Receive(FanChangedMessage message)
        {
            if (message.Side == WaferSide)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    FanSpeed = message.Fan;
                    HasFanSpeedChanged = false;
                });
            }
        }

        public void Receive(FanAutoChangedMessage message)
        {
            if (message.Side == WaferSide)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    IsFanAuto = message.FanAuto;
                });
            }
        }

        public void Receive(PowerStateChangedMessage message)
        {
            if (message.Side == WaferSide)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    IsScreenOn = message.PowerState;
                });
            }
        }
    }
}
