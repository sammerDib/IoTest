using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.DMT.Hardware.ClientProxy.Screen;
using UnitySC.PM.DMT.Hardware.Service.Interface.Screen;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Interface.Screen;
using UnitySC.Shared.Image;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.DMT.Shared.UI.Proxy
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class ScreenSupervisor : IScreenServiceCallback
    {
        private readonly DuplexServiceInvoker<IDMTScreenService> _screenService;
        private readonly ILogger _logger;
        private readonly IMessenger _messenger;
        private readonly IDialogOwnerService _dialogService;
        public ScreenSupervisor(ILogger<ScreenSupervisor> logger,
           ILogger<IDMTScreenService> screenServiceLogger, IMessenger messenger, IDialogOwnerService dialogService)
        {
            var instanceContext = new InstanceContext(this);
            _screenService = new DuplexServiceInvoker<IDMTScreenService>(instanceContext, "DEMETERScreenService", screenServiceLogger, messenger, s => s.SubscribeToChanges());
            _logger = logger;
            _messenger = messenger;
            _dialogService = dialogService;
        }

        public ScreenInfo GetScreenInfo(Side side)
        {
            ScreenInfo info = _screenService.Invoke(s => s.GetScreenInfoForSide(side));
            return info;
        }

        public void SetScreenColor(Side side, Color color)
        {
            SetScreenColor(side, color, false);
        }

        public void SetScreenColor(Side side, Color color, bool? applyEllipseMask)
        {
            try
            {
                _screenService.InvokeAsync(s => s.SetScreenColorOnSide(side, color, applyEllipseMask));
            }
            catch (Exception ex)
            {
                string msg = $"Error when setting color on scren ${side}";
                _logger.Error(ex, msg);
                _dialogService.ShowException(ex, msg);
            }
        }

        public void DisplayImage(Side side, BitmapSource image)
        {
            DisplayImage(side, image, false);
        }

        public void DisplayImage(Side side, ServiceImage image)
        {
            DisplayImage(side, image, false);
        }

        public void DisplayImage(Side side, BitmapSource image, bool? applyEllipseMask)
        {
            ServiceImage svcImage = new ServiceImage();
            svcImage.CreateFromBitmap(image);
            _screenService.InvokeAsync(s => s.DisplayImageOnSide(side, svcImage, applyEllipseMask));
        }

        public void DisplayImage(Side side, ServiceImage image, bool? applyEllipseMask)
        {
            _screenService.InvokeAsync(s => s.DisplayImageOnSide(side, image, applyEllipseMask));
        }

        void IScreenServiceCallback.ScreenImageChangedCallback(string screenId, ServiceImage image, Color color)
        {
            _logger.Information("CameraSupervisor : Screen image changed");
            _messenger.Send(new ScreenImageChangedMessage() { ScreenId = screenId, ServiceImage = image, Color = color });
        }

        public void DisplayFringe(Side side, Fringe fringe, int fringeImageNumber, Color color)
        {
            _screenService.InvokeAsync(s => s.DisplayFringeOnSide(side, fringe, fringeImageNumber, color));
        }

        public void DisplayHighAngleDarkFieldMaskOnSide(Side side, Color color)
        {
            _screenService.InvokeAsync(s => s.DisplayHighAngleDarkFielMaskdOnSide(side, color));
        }

        public List<Color> GetAvailableColors()
        {
            return _screenService.Invoke(s => s.GetAvailableColors());
        }

        public List<Fringe> GetAvailableFringes()
        {
            return _screenService.Invoke(s => s.GetAvailableFringes());
        }

        public void TriggerUpdateEvent(Side screenSide)
        {
            _screenService.Invoke(s => s.TriggerUpdateEvent(screenSide));
        }

        public async Task SwitchScreenOnOffAsync(Side screenSide, bool screenOn)
        {
            await _screenService.InvokeAsync(s => s.SwitchScreenOnOffAsync(screenSide, screenOn));
        }

        public async Task RestoreParametersAsync(Side screenSide)
        {
            await _screenService.InvokeAsync(s => s.RestoreParametersAsync(screenSide));
        }

        public async Task TurnFanAutoOnAsync(Side screenSide, bool isFanAuto)
        {
            await _screenService.InvokeAsync(s => s.TurnFanAutoOnAsync(screenSide, isFanAuto));
        }

        public async Task SetBrightnessAsync(Side screenSide, short brightness)
        {
            await _screenService.InvokeAsync(s => s.SetBrightnessAsync(screenSide, brightness));
        }

        public async Task SetBacklightAsync(Side screenSide, short backlight)
        {
            await _screenService.InvokeAsync(s => s.SetBacklightAsync(screenSide, backlight));
        }

        public async Task SetContrastAsync(Side screenSide, short contrast)
        {
            await _screenService.InvokeAsync(s => s.SetContrastAsync(screenSide, contrast));
        }

        public async Task SetSharpnessAsync(Side screenSide, int sharpness)
        {
            await _screenService.InvokeAsync(s => s.SetSharpnessAsync(screenSide, sharpness));
        }

        public async Task SetFanSpeedAsync(Side screenSide, int value)
        {
            await _screenService.InvokeAsync(s => s.SetFanSpeedAsync(screenSide, value));
        }

        public short GetBacklight(Side screenSide)
        {
            return _screenService.Invoke(s => s.GetBacklight(screenSide));
        }

        public short GetBrightness(Side screenSide)
        {
            return _screenService.Invoke(s => s.GetBrightness(screenSide));
        }

        public short GetContrast(Side screenSide)
        {
            return _screenService.Invoke(s => s.GetContrast(screenSide));
        }

        public double GetTemperature(Side screenSide)
        {
            return _screenService.Invoke(s => s.GetTemperature(screenSide));
        }

        public int GetFanRPM(Side screenSide)
        {
            return _screenService.Invoke(s => s.GetFanRPM(screenSide));
        }

        public Dictionary<string, short> GetDefaultScreenValues(Side screenSide)
        {
            return _screenService.Invoke(s => s.GetDefaultScreenValues(screenSide));
        }

        void IScreenServiceCallback.OnBacklightChangedCallback(Side side, short value)
        {
            _logger.Information("ScreenSupervisor : Backlight changed");
            _messenger.Send(new BacklightChangedMessage() { Side = side, Backlight = value });
        }

        void IScreenServiceCallback.OnBrightnessChangedCallback(Side side, short value)
        {
            _logger.Information("ScreenSupervisor : Brightness changed");
            _messenger.Send(new BrightnessChangedMessage() { Side = side, Brightness = value });
        }

        void IScreenServiceCallback.OnContrastChangedCallback(Side side, short value)
        {
            _logger.Information("ScreenSupervisor : Contrast changed");
            _messenger.Send(new ContrastChangedMessage() { Side = side, Contrast = value });
        }

        void IScreenServiceCallback.OnSharpnessChangedCallback(Side side, int value)
        {
            _logger.Information("ScreenSupervisor : Sharpness changed");
            _messenger.Send(new SharpnessChangedMessage() { Side = side, Sharpness = value });
        }

        void IScreenServiceCallback.OnTemperatureChangedCallback(Side side, double value)
        {
            _logger.Information("ScreenSupervisor : Temperature changed");
            _messenger.Send(new TemperatureChangedMessage() { Side = side, Temperature = value });
        }

        void IScreenServiceCallback.OnFanChangedCallback(Side side, int value)
        {
            _logger.Information("ScreenSupervisor : Fan changed");
            _messenger.Send(new FanChangedMessage() { Side = side, Fan = value });
        }

        void IScreenServiceCallback.OnFanAutoChangedCallback(Side side, bool value)
        {
            _logger.Information("ScreenSupervisor : Fan auto changed");
            _messenger.Send(new FanAutoChangedMessage() { Side = side, FanAuto = value });
        }

        void IScreenServiceCallback.OnPowerStateChangedCallback(Side side, bool value)
        {
            _logger.Information("ScreenSupervisor : State changed");
            _messenger.Send(new PowerStateChangedMessage() { Side = side, PowerState = value });
        }

    }
}
