using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using UnitySC.PM.ANA.Client.Proxy.Axes;
using UnitySC.PM.ANA.Client.Proxy.Chuck;
using UnitySC.PM.Shared.Hardware.ClientProxy.Chamber;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using Application = System.Windows.Application;
using RichTextBox = System.Windows.Controls.RichTextBox;

namespace UnitySC.PM.ANA.Client.Modules.HardwareReset
{
    public class HardwareResetVM : ObservableObject, IMenuContentViewModel
    {
        public bool IsEnabled => true;

        public bool IsProcessChecked { get; set; } = true;
        public bool IsAirbearingChecked { get; set; } = true;
        public bool IsZTopFocusChecked { get; set; } = true;
        public bool IsZBottomFocusChecked { get; set; } = true;
        public bool IsWaferStageChecked { get; set; } = true;

        private ImageSource _validImage = (ImageSource)Application.Current.FindResource("Valid");
        private ImageSource _waitingImage = (ImageSource)Application.Current.FindResource("RunningWithAnimation");
        private ImageSource _errorImage = (ImageSource)Application.Current.FindResource("Error");

        private CancellationTokenSource _cancellationToken;

        private bool _isResetRunning = false;

        public bool CanClose()
        {
            if (_isResetRunning)
            {
                return false;
            }
            _cancellationToken?.Cancel();

            StatusProcess = null;
            StatusAirbearing = null;
            StatusZTop = null;
            StatusZBottom = null;
            StatusWaferStage = null;

            return true;
        }

        public void Refresh()
        {
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();
            Task.Run(StartPressureMonitoring, _cancellationToken.Token);
        }

        private AsyncRelayCommand _initializeAll;

        public AsyncRelayCommand InitializeAllCommand
        {
            get
            {
                return _initializeAll ?? (_initializeAll = new AsyncRelayCommand(
                    async () =>
                    {
                        _isResetRunning = true;
                        StatusProcess = new BitmapImage();
                        StatusAirbearing = new BitmapImage();
                        StatusZTop = new BitmapImage();
                        StatusZBottom = new BitmapImage();
                        StatusWaferStage = new BitmapImage();
                        ConsoleOutput += $"{DateTime.Now} \r";

                        bool process = IsProcessChecked;
                        bool airbearing = IsAirbearingChecked;
                        bool zTop = IsZTopFocusChecked;
                        bool zBottom = IsZBottomFocusChecked;
                        bool waferStage = IsWaferStageChecked;
                        
                        if (process)
                        {
                            await ExecuteProcessResetAsync();
                        }

                        if (airbearing)
                        {
                            await ExecuteAirbearingResetAsync();
                        }

                        if (zTop)
                        {
                            await ExecuteZTopFocusResetAsync();
                        }

                        if (zBottom)
                        {
                            await ExecuteZBottomFocusResetAsync();
                        }

                        if (waferStage)
                        {
                            await ExecuteWaferStageResetAsync();
                        }

                        _isResetRunning = false;
                    },
                    () => !_isResetRunning));
            }
        }

        private async Task ExecuteProcessResetAsync()
        {
            try
            {
                await ClassLocator.Default.GetInstance<ChamberSupervisor>().ResetProcess();
                ConsoleOutput += "Successful reset Process. \r";
                StatusProcess = _validImage;
            }
            catch (Exception ex)
            {
                ConsoleOutput += "Error during reset Process : " + ex.Message + "\r";
                StatusProcess = _errorImage;
            }
        }

        private async Task ExecuteWaferStageResetAsync()
        {
            StatusWaferStage = _waitingImage;
            try
            {
                await ClassLocator.Default.GetInstance<ChuckSupervisor>().ResetWaferStage();
                ConsoleOutput += "Successful reset WaferStage. \r";
                StatusWaferStage = _validImage;
            }
            catch (Exception ex)
            {
                ConsoleOutput += "Error during reset WaferStage : " + ex.Message + "\r";
                StatusWaferStage = _errorImage;
            }
        }

        private async Task ExecuteAirbearingResetAsync()
        {
            StatusAirbearing = _waitingImage;
            try
            {
                await ClassLocator.Default.GetInstance<ChuckSupervisor>().ResetAirbearing();
                ConsoleOutput += "Successful reset Airbearing. \r";
                StatusAirbearing = _validImage;
            }
            catch (Exception ex)
            {
                ConsoleOutput += "Error during reset Airbearing : " + ex.Message + "\r";
                StatusAirbearing = _errorImage;
            }
        }

        private async Task ExecuteZTopFocusResetAsync()
        {
            StatusZTop = _waitingImage;
            try
            {
                await ClassLocator.Default.GetInstance<AxesSupervisor>().ResetZTopFocus();
                ConsoleOutput += "Successful reset of the ZTop focus (UOH). \r";
                StatusZTop = _validImage;
            }
            catch (Exception ex)
            {
                ConsoleOutput += "Error during reset ZTopFocus : " + ex.Message + "\r";
                StatusZTop = _errorImage;
            }
        }

        private async Task ExecuteZBottomFocusResetAsync()
        {
            StatusZBottom = _waitingImage;
            try
            {
                await ClassLocator.Default.GetInstance<AxesSupervisor>().ResetZBottomFocus();
                ConsoleOutput += "Successful reset of the ZBottom focus (LOH). \r";
                StatusZBottom = _validImage;
            }
            catch (Exception ex)
            {
                ConsoleOutput += "Error during reset ZBottomFocus : " + ex.Message + "\r";
                StatusZBottom = _errorImage;
            }
        }

        private async Task StartPressureMonitoring()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000);

                var pressures = ClassLocator.Default.GetInstance<ChuckSupervisor>().GetSensorValues();
                if (pressures != null)
                {
                    foreach (var presssure in pressures.Result)
                    {
                        if (presssure.Key == "AirbearingVacuumSensor0")
                        {
                            if (AirbearingSensor0Value != presssure.Value)
                            {
                                AirbearingSensor0Value = presssure.Value;
                                OnPropertyChanged(nameof(AirbearingSensor0Value));
                            }
                        }

                        if (presssure.Key == "AirbearingVacuumSensor1")
                        {
                            AirbearingSensor1Value = presssure.Value;
                            OnPropertyChanged(nameof(AirbearingSensor1Value));
                        }

                        if (presssure.Key == "AirbearingPressureSensor")
                        {
                            AirbearingPressureValue = presssure.Value;
                            OnPropertyChanged(nameof(AirbearingPressureValue));
                        }
                    }
                }
            }
        }

        private AutoRelayCommand<string> _switchLightCommand;

        public AutoRelayCommand<string> SwitchLightCommand
        {
            get
            {
                return _switchLightCommand ?? (_switchLightCommand = new AutoRelayCommand<string>(
                    (lightState) =>
                    {
                        bool state = lightState == "On";

                        ClassLocator.Default.GetInstance<ChamberSupervisor>().SetChamberLightState(state);
                    },
                    (isLightOn) => true));
            }
        }

        private string _consoleOutput = "";

        public string ConsoleOutput
        {
            get => _consoleOutput;
            set => SetProperty(ref _consoleOutput, value);
        }

        private ImageSource _statusProcess = new BitmapImage();

        public ImageSource StatusProcess
        {
            get => _statusProcess;
            set => SetProperty(ref _statusProcess, value);
        }

        private ImageSource _statusAirbearing = new BitmapImage();

        public ImageSource StatusAirbearing
        {
            get => _statusAirbearing;
            set => SetProperty(ref _statusAirbearing, value);
        }

        private ImageSource _statusZTop = new BitmapImage();

        public ImageSource StatusZTop
        {
            get => _statusZTop;
            set => SetProperty(ref _statusZTop, value);
        }

        private ImageSource _statusZBottom = new BitmapImage();

        public ImageSource StatusZBottom
        {
            get => _statusZBottom;
            set => SetProperty(ref _statusZBottom, value);
        }

        private ImageSource _statusWaferStage = new BitmapImage();

        public ImageSource StatusWaferStage
        {
            get => _statusWaferStage;
            set => SetProperty(ref _statusWaferStage, value);
        }

        private double _airbearingSensor0 = 0.0;

        public double AirbearingSensor0Value
        {
            get => _airbearingSensor0;
            set => SetProperty(ref _airbearingSensor0, value);
        }

        private double _airbearingSensor1 = 0.0;

        public double AirbearingSensor1Value
        {
            get => _airbearingSensor1;
            set => SetProperty(ref _airbearingSensor1, value);
        }

        private double _airbearingPressure = 0.0;

        public double AirbearingPressureValue
        {
            get => _airbearingPressure;
            set => SetProperty(ref _airbearingPressure, value);
        }
    }
}
