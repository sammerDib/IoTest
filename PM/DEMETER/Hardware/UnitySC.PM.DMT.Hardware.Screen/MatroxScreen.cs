using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;

using CommunityToolkit.Mvvm.Messaging;

using Matrox.MatroxImagingLibrary;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Screens;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.PlcScreen;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.DMT.Hardware.Screen
{
    public class MatroxScreen : ScreenBase
    {
        protected static IMessenger Messenger => ClassLocator.Default.GetInstance<IMessenger>();

        private MilDisplay _milDisplay;
        private MilImage _milDisplayImage;

        private DensitronDM430GNScreenController _controller;
        private DMTScreenConfig _screenConfig;

        private bool _monitorTemperatureTaskRunning;
        private bool _screenOverheat;

        public override void Init(DMTScreenConfig config, IGlobalStatusServer globalStatusServer, ScreenDensitronDM430GNControllerConfig screenDensitronConfig, ScreenController controller)
        {
            base.Init(config, globalStatusServer, screenDensitronConfig, controller);
            _controller = (DensitronDM430GNScreenController)controller;

            ParseConfig(config);
            _screenConfig = config;

            InitSettingsScreen();

            _milDisplay = new MilDisplay();
            _milDisplayImage = new MilImage();
            _milDisplayImage.AllocColor(Mil.Instance.HostSystem, /*SizeBand:*/ 3, Width, Height, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC + MIL.M_DISP);
            if (config.IsSimulated)
            {
                _milDisplay.Alloc(Mil.Instance.HostSystem, MIL.M_DEFAULT, "M_DEFAULT", MIL.M_WINDOWED);
                _milDisplay.ZoomFactorX = 0.4;
                _milDisplay.ZoomFactorY = 0.4;
                _milDisplay.Title = config.Name;
            }
            else
            {
                //_milDisplay.Alloc(Mil.Instance.HostSystem, DisplayID - 1, config.Resolution, MIL.M_EXCLUSIVE);
                _milDisplay.Alloc(Mil.Instance.HostSystem, (int)DisplayPosition, config.Resolution, MIL.M_EXCLUSIVE);
            }

            _milDisplay.Select(_milDisplayImage);
            State = new DeviceState(DeviceStatus.Ready);

            MonitorTemperature();
        }

        public override void Shutdown()
        {
            if (_milDisplay != null)
            {
                _milDisplay.Select(null);
                _milDisplay.Dispose();
                _milDisplay = null;
            }
            if (_milDisplayImage != null)
            {
                _milDisplayImage.Dispose();
                _milDisplayImage = null;
            }

            _monitorTemperatureTaskRunning = false;
        }

        public override void DisplayImage(USPImageMil procimg)
        {
            ClearOtherScreens();
            if (_screenOverheat)
            {
                throw new Exception("Screen is overheating");
            }

            var milImage = procimg.GetMilImage();
            MilImage.Copy(milImage, _milDisplayImage);
            Messenger.Send(new ScreenMessage() { Screen = this, Image = procimg });
            System.Threading.Thread.Sleep((int)(ScreenStabilizationTimeSec * 1000));
        }

        public override async Task DisplayImageAsync(USPImageMil procimg)
        {
            ClearOtherScreens();
            if (_screenOverheat)
            {
                throw new Exception("Screen is overheating");
            }

            var milImage = procimg.GetMilImage();
            MilImage.Copy(milImage, _milDisplayImage);
            Messenger.Send(new ScreenMessage() { Screen = this, Image = procimg });
            await Task.Delay((int)(ScreenStabilizationTimeSec * 1000));
        }

        public override void Clear()
        {
            _milDisplayImage.Clear(MIL.M_RGB888(Colors.Black.R, Colors.Black.G, Colors.Black.B));
            Messenger.Send(new ScreenMessage() { Screen = this, Color = Colors.Black });
        }

        public override async Task ClearAsync(Color color)
        {
            _milDisplayImage.Clear(MIL.M_RGB888(color.R, color.G, color.B));
            Messenger.Send(new ScreenMessage() { Screen = this, Color = color });

            await Task.Delay((int)(ScreenStabilizationTimeSec * 1000));
        }

        public Task MonitorTemperature()
        {
            return Task.Run(async () =>
             {
                 _monitorTemperatureTaskRunning = true;


                 while (_monitorTemperatureTaskRunning)
                 {
                     if (_controller.TemperatureValue > _screenConfig.ScreenTemperatureLimit)
                     {
                         _screenOverheat = true;
                         Clear();
                         string message = $"Screen {_screenConfig.DisplayPosition} is overheating. Temperature value {_controller.TemperatureValue} ";
                         Logger.Error(message);
                         GlobalStatusServer.SetGlobalStatus(new GlobalStatus(new Message(MessageLevel.Error, message)));

                         await Task.Delay(_screenConfig.BlackoutDurationMs);
                     }
                     else
                     {
                         //Comment or uncomment if you want to log the screen temperature values. 
                         //LogTemperatureData();

                         _screenOverheat = false;
                     }
                     await Task.Delay(2000);
                 }
             });
        }

        /// Logs the current temperature data to a specified CSV file.
        public void LogTemperatureData()
        {
            var path = "temperature_log.csv";
            if (!File.Exists(path))
            {
                using (var writer = new StreamWriter(path, false))
                {
                    writer.WriteLine("Timestamp,TemperatureValue");
                }

            }
            var timestamp = DateTime.UtcNow.ToString("dd-MM-yyyy_HH:mm:ss.ff");
            var temperature = _controller.TemperatureValue;

            using (var writer = new StreamWriter(path, true))
            {
                writer.WriteLine($"{timestamp},{temperature}");
            }
        }

        public async Task InitSettingsScreen()
        {
            await SwitchScreenOnOff(true);

            await SetBacklightAsync(_screenConfig.BacklightPercentage);

            await SetBrightnessAsync(_screenConfig.BrightnessPercentage);

            await SetContrastAsync(_screenConfig.ContrastPercentage);

            await SetSharpnessAsync(_screenConfig.Sharpness);

            await FanAutoOn(_screenConfig.FanAutoOn);
        }

        public async Task SwitchScreenOnOff(bool turnOn)
        {
            if (turnOn) { await _controller.PowerOnAsync(); }
            else
            {
                await _controller.PowerOffAsync();
            }
        }

        public async Task SetBacklightAsync(short value_InPercent)
        {
            await _controller.SetBacklightAsync(value_InPercent);
        }

        public async Task SetBrightnessAsync(short value_InPercent)
        {
            await _controller.SetBrightnessAsync(value_InPercent);
        }

        public async Task SetContrastAsync(short value_InPercent)
        {
            await _controller.SetContrastAsync(value_InPercent);
        }

        public async Task SetSharpnessAsync(int step)
        {
            var displayControlStep = (DisplayControlStep)step;
            await _controller.SetSharpnessAsync(displayControlStep);
        }

        public async Task SetFanSpeedAsync(int step)
        {
            var displayControlStep = (DisplayControlStep)step;
            await _controller.SetFanStepAsync(displayControlStep);
        }

        public short GetBacklight()
        {
            return _controller.BacklightValue;
        }

        public short GetBrightness()
        {
            return _controller.BrightnessValue;
        }

        public short GetContrast()
        {
            return _controller.ContrastValue;
        }

        public double GetSharpness()
        {
            return _controller.SharpnessValue;
        }

        public double GetTemperature()
        {
            return _controller.TemperatureValue;
        }

        public int GetFanRPM()
        {
            return _controller.FanRPMValue;
        }

        public Dictionary<string, short> GetDefaultScreenValues()
        {
            var screenDefaultValues = new Dictionary<string, short>
            {
                { "backlight", _screenConfig.BacklightPercentage },
                { "brightness", _screenConfig.BrightnessPercentage },
                { "contrast", _screenConfig.ContrastPercentage }
            };
            return screenDefaultValues;
        }

        public async Task FanAutoOn(bool autOn)
        {
            await _controller.FanAutoOn(autOn);
        }

        public void TriggerUpdateEvent()
        {
            _controller.TriggerUpdateEvent();
        }

        public async Task RestoreParametersAsync()
        {
            InitSettingsScreen();
            /*
             * TODO GVA : The opc reset does not work. To be fixed by Gael ;)
            IOpcMultiParams opcMultiParams = new DensitronDM430GNScreenOpcMultiParams(true, _screenConfig.BacklightPercentage,
                _screenConfig.BrightnessPercentage, _screenConfig.ContrastPercentage, _screenConfig.Sharpness);

            _controller.RestoreParameters(opcMultiParams);

            For now, all the buttons are independent. The function is not used.
            */
        }
    }
}
