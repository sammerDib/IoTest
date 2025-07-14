using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Hardware.Screen;
using UnitySC.PM.DMT.Hardware.Service.Interface.Screen;
using UnitySC.PM.DMT.Service.Implementation.Calibration;
using UnitySC.PM.DMT.Service.Implementation.Extensions;
using UnitySC.PM.DMT.Service.Implementation.Fringes;
using UnitySC.PM.DMT.Service.Interface.Calibration;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Interface.Measure.Configuration;
using UnitySC.PM.DMT.Service.Interface.Screen;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Extensions;
using UnitySC.Shared.Data.Extensions;
using UnitySC.PM.Shared.Hardware.Service.Interface.PlcScreen;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.DMT.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class DMTScreenService : DuplexServiceBase<IScreenServiceCallback>, IDMTScreenService
    {
        private readonly DMTHardwareManager _hardwareManager;
        private readonly CalibrationManager _calibrationManager;
        private readonly MeasuresConfiguration _measuresConfiguration;
        private readonly FringeManager _fringeManager;
        private readonly IMessenger _messenger;

        public DMTScreenService(ILogger logger, DMTHardwareManager dmtHardwareManager, CalibrationManager calibrationManager, FringeManager fringeManager, MeasuresConfiguration measuresConfiguration, IMessenger messenger) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = dmtHardwareManager;
            _calibrationManager = calibrationManager;
            _fringeManager = fringeManager;
            _measuresConfiguration = measuresConfiguration;

            _messenger = messenger;

            _messenger.Register<BacklightMessage>(this, (r, m) => { OnBacklightChanged(m.Side, m.Backlight); });
            _messenger.Register<BrightnessMessage>(this, (r, m) => { OnBrightnessChanged(m.Side, m.Brightness); });
            _messenger.Register<ContrastMessage>(this, (r, m) => { OnContrastChanged(m.Side, m.Contrast); });
            _messenger.Register<TemperatureMessage>(this, (r, m) => { OnTemperatureChanged(m.Side, m.Temperature); });
            _messenger.Register<FanStepMessage>(this, (r, m) => { OnFanChanged(m.Side, (int)m.FanStep); });

            _messenger.Register<FanAutoMessage>(this, (r, m) => { OnFanAutoChanged(m.Side, m.FanAuto); });
            _messenger.Register<PowerStateMessage>(this, (r, m) => { OnPowerStateChanged(m.Side, m.PowerState); });

            _messenger.Register<ScreenMessage>(this, (r, m) => { ScreenImageChanged(m.Screen, m.Image, m.Color); });
        }

        public override void Init()
        {
            base.Init();
        }

        Response<VoidResult> IDMTScreenService.DisplayImageOnSide(Side side, ServiceImage image, bool? applyEllipseMask)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                var ellipseMaskInput = _calibrationManager.GetEllipseMasksBySide(side);
                if (applyEllipseMask.GetValueOrDefault(false) && !(ellipseMaskInput is null))
                {
                    image = CreateEllipseMask(side, ellipseMaskInput, null, image);
                }
                using (USPImageMil procImg = new USPImageMil(image))
                {
                    _hardwareManager.ScreensBySide[side].DisplayImage(procImg);
                }
            });
        }

        Response<ScreenInfo> IDMTScreenService.GetScreenInfoForSide(Side side)
        {
            return InvokeDataResponse(messageContainer =>
            {
                ScreenBase scr = _hardwareManager.ScreensBySide[side];
                return new ScreenInfo()
                {
                    Model = scr.Model,
                    SerialNumber = scr.SerialNumber,
                    Version = scr.Version,
                    Width = scr.Width,
                    Height = scr.Height,
                    PixelPitchHorizontal = scr.PixelPitchHorizontal,
                    PixelPitchVertical = scr.PixelPitchVertical,
                    ScreenWhiteDisplayTimeSec = scr.ScreenWhiteDisplayTimeSec,
                };
            });
        }

        Response<VoidResult> IDMTScreenService.SetScreenColorOnSide(Side side, Color color, bool? applyEllipseMask)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                var ellipseMaskInput = _calibrationManager.GetEllipseMasksBySide(side);

                if (applyEllipseMask.GetValueOrDefault(false) && !(ellipseMaskInput is null))
                {
                    ServiceImage serviceImage = CreateEllipseMask(side, ellipseMaskInput, color);

                    using (USPImageMil displayImage = new USPImageMil(serviceImage))
                    {
                        _hardwareManager.ScreensBySide[side].DisplayImage(displayImage);
                    }
                }
                else
                {
                    if (_hardwareManager.ScreensBySide.ContainsKey(side))
                    {
                        _hardwareManager.ScreensBySide[side].ClearAsync(color);
                    }
                }
            });
        }

        Response<VoidResult> IDMTScreenService.DisplayCrossOnSide(Side side, Color backgroundColor, Color crossColor, int thickness, double centerX, double centerY)
        {
            return InvokeVoidResponse(messagesContainer =>
            {
                ScreenBase screen = _hardwareManager.ScreensBySide[side];
                using (USPImageMil screenImage = new USPImageMil())
                {
                    screenImage.CreateCrossImage(screen.Width, screen.Height, backgroundColor, crossColor, thickness, centerX, centerY);
                    screen.DisplayImage(screenImage);
                }
            });
        }

        Response<VoidResult> IDMTScreenService.DisplayFringeOnSide(Side side, Fringe fringe, int imageIndex, Color color)
        {
            return InvokeVoidResponse(messagesContainer =>
            {
                List<USPImageMil> fringeImages = _fringeManager.GetFringeImages(side, fringe);
                USPImageMil procimg = fringeImages[imageIndex];

                using (USPImageMil colorimg = procimg.ColorizeImage(color))
                {
                    _hardwareManager.ScreensBySide[side].DisplayImage(colorimg);
                }
            });
        }

        Response<VoidResult> IDMTScreenService.DisplayHighAngleDarkFielMaskdOnSide(Side side, Color color)
        {
            return InvokeVoidResponse(messagesContainer =>
            {
                USPImageMil procimg = _calibrationManager.GetHighAngleDarkFieldMaskForSide(side);
                if (procimg is null)
                {
                    return;
                }
                using (USPImageMil colorimg = procimg.ColorizeImage(color))
                {
                    _hardwareManager.ScreensBySide[side].DisplayImage(colorimg);
                }
            });
        }

        /// <summary>
        /// Get fringes
        /// </summary>
        /// <returns></returns>
        Response<List<Fringe>> IDMTScreenService.GetAvailableFringes()
        {
            return InvokeDataResponse(messagesContainer =>
            {
                return _fringeManager.Fringes;
            });
        }

        Response<List<Color>> IDMTScreenService.GetAvailableColors()
        {
            return InvokeDataResponse(messagesContainer =>
            {
                return _measuresConfiguration.GetConfiguration<BrightFieldMeasureConfiguration>().Colors;
            });
        }

        private ServiceImage CreateEllipseMask(Side side, EllipseMaskInput ellipseMaskInput, Color? color, ServiceImage sourceImage = null)
        {
            ScreenBase screen = _hardwareManager.ScreensBySide[side];
            double ellipseRadiusX = ellipseMaskInput.ScreenWidthRatio / 2 * screen.Width;
            double ellipseRadiusY = ellipseMaskInput.ScreenHeightRatio / 2 * screen.Height;
            Point ellipseCenter = new Point(screen.Width / 2 + ellipseMaskInput.XShiftFromCenterInPixels, screen.Height - ellipseRadiusY);
            Rect screenRect = new Rect(0, 0, screen.Width, screen.Height);
            SolidColorBrush ellipseBrush;

            if (sourceImage != null)
            {
                ellipseBrush = Brushes.White;
            }
            else
            {
                ellipseBrush = color.HasValue && color.Value != null ? new SolidColorBrush(color.Value) : Brushes.White;
            }

            DrawingVisual ellipseVisual = new DrawingVisual();
            DrawingContext ellispseContext = ellipseVisual.RenderOpen();

            ellispseContext.DrawRectangle(Brushes.Black, null, screenRect);
            ellispseContext.DrawEllipse(ellipseBrush, null, ellipseCenter, ellipseRadiusX, ellipseRadiusY);

            ellispseContext.Close();

            RenderTargetBitmap bmp24 = new RenderTargetBitmap(screen.Width, screen.Height, 96, 96, PixelFormats.Pbgra32);
            if (sourceImage != null)
            {
                DrawingVisual imageVisual = new DrawingVisual();
                DrawingContext drawingContext = imageVisual.RenderOpen();

                drawingContext.DrawImage(sourceImage.ConvertToWpfBitmapSource(), screenRect);
                drawingContext.Close();

                imageVisual.OpacityMask = new DrawingBrush(ellipseVisual.Drawing);
                bmp24.Render(imageVisual);
            }
            else
            {
                bmp24.Render(ellipseVisual);
            }
            ServiceImage destImage = new ServiceImage();
            destImage.CreateFromBitmap(bmp24.ConvertToRGB24());
            return destImage;
        }

        async Task<Response<VoidResult>> IDMTScreenService.SwitchScreenOnOffAsync(Side side, bool on)
        {
            return await InvokeVoidResponseAsync(async messagesContainer =>
            {
                var screen = _hardwareManager.ScreensBySide[side];
                if (screen is MatroxScreen matroxScreen)
                {
                    await matroxScreen.SwitchScreenOnOff(on);
                }
            });
        }

        async Task<Response<VoidResult>> IDMTScreenService.RestoreParametersAsync(Side side)
        {
            return await InvokeVoidResponseAsync(async messagesContainer =>
            {
                var screen = _hardwareManager.ScreensBySide[side];
                if (screen is MatroxScreen matroxScreen)
                {
                    await matroxScreen.RestoreParametersAsync();
                }
            });
        }

        Response<VoidResult> IDMTScreenService.TriggerUpdateEvent(Side side)
        {
            return InvokeVoidResponse(messagesContainer =>
            {
                var screen = _hardwareManager.ScreensBySide[side];
                if (screen is MatroxScreen matroxScreen)
                {
                    matroxScreen.TriggerUpdateEvent();
                }
            });
        }

        async Task<Response<VoidResult>> IDMTScreenService.TurnFanAutoOnAsync(Side screenSide, bool isFanAuto)
        {
            return await InvokeVoidResponseAsync(async messagesContainer =>
            {
                var screen = _hardwareManager.ScreensBySide[screenSide];
                if (screen is MatroxScreen matroxScreen)
                {
                    await matroxScreen.FanAutoOn(isFanAuto);
                }
            });
        }

        async Task<Response<VoidResult>> IDMTScreenService.SetBrightnessAsync(Side screenSide, short brightness)
        {
            return await InvokeVoidResponseAsync(async messagesContainer =>
            {
                var screen = _hardwareManager.ScreensBySide[screenSide];
                if (screen is MatroxScreen matroxScreen)
                {
                    await matroxScreen.SetBrightnessAsync(brightness);
                }
            });
        }

        async Task<Response<VoidResult>> IDMTScreenService.SetBacklightAsync(Side screenSide, short backlight)
        {
            return await InvokeVoidResponseAsync(async messagesContainer =>
            {
                var screen = _hardwareManager.ScreensBySide[screenSide];
                if (screen is MatroxScreen matroxScreen)
                {
                    await matroxScreen.SetBacklightAsync(backlight);
                }
            });
        }

        async Task<Response<VoidResult>> IDMTScreenService.SetContrastAsync(Side screenSide, short contrast)
        {
            return await InvokeVoidResponseAsync(async messagesContainer =>

            {
                var screen = _hardwareManager.ScreensBySide[screenSide];
                if (screen is MatroxScreen matroxScreen)
                {
                    await matroxScreen.SetContrastAsync(contrast);
                }
            });
        }

        async Task<Response<VoidResult>> IDMTScreenService.SetSharpnessAsync(Side screenSide, int sharpness)
        {
            return await InvokeVoidResponseAsync(async messagesContainer =>

            {
                var screen = _hardwareManager.ScreensBySide[screenSide];
                if (screen is MatroxScreen matroxScreen)
                {
                    await matroxScreen.SetSharpnessAsync(sharpness);
                }
            });
        }

        async Task<Response<VoidResult>> IDMTScreenService.SetFanSpeedAsync(Side screenSide, int fanSpeed)
        {
            return await InvokeVoidResponseAsync(async messagesContainer =>
            {
                var screen = _hardwareManager.ScreensBySide[screenSide];
                if (screen is MatroxScreen matroxScreen)
                {
                    await matroxScreen.SetFanSpeedAsync(fanSpeed);
                }
            });
        }

        Response<short> IDMTScreenService.GetBacklight(Side screenSide)
        {
            short defaultValue = 0;
            return InvokeDataResponse(messagesContainer =>
            {
                var screen = _hardwareManager.ScreensBySide[screenSide];
                if (screen is MatroxScreen matroxScreen)
                {
                    return matroxScreen.GetBacklight();
                }
                else if (screen is DummyMatroxScreen)
                {
                    return (short)100;
                }
                return defaultValue;
            });
        }

        Response<short> IDMTScreenService.GetBrightness(Side screenSide)
        {
            short defaultValue = 0;
            return InvokeDataResponse(messagesContainer =>
            {
                var screen = _hardwareManager.ScreensBySide[screenSide];
                if (screen is MatroxScreen matroxScreen)
                {
                    return matroxScreen.GetBrightness();
                }
                else if (screen is DummyMatroxScreen)
                {
                    return (short)50;
                }
                return defaultValue;
            });
        }

        Response<short> IDMTScreenService.GetContrast(Side screenSide)
        {
            short defaultValue = 0;
            return InvokeDataResponse(messagesContainer =>
            {
                var screen = _hardwareManager.ScreensBySide[screenSide];
                if (screen is MatroxScreen matroxScreen)
                {
                    return matroxScreen.GetContrast();
                }
                else if (screen is DummyMatroxScreen)
                {
                    return (short)50;
                }
                return defaultValue;
            });
        }

        Response<double> IDMTScreenService.GetTemperature(Side screenSide)
        {
            return InvokeDataResponse(messagesContainer =>
            {
                var screen = _hardwareManager.ScreensBySide[screenSide];
                if (screen is MatroxScreen matroxScreen)
                {
                    return matroxScreen.GetTemperature();
                }
                return 0.0;
            });
        }

        Response<int> IDMTScreenService.GetFanRPM(Side screenSide)
        {
            return InvokeDataResponse(messagesContainer =>
            {
                var screen = _hardwareManager.ScreensBySide[screenSide];
                if (screen is MatroxScreen matroxScreen)
                {
                    return matroxScreen.GetFanRPM();
                }
                return 0;
            });
        }

        Response<Dictionary<string, short>> IDMTScreenService.GetDefaultScreenValues(Side screenSide)
        {
            return InvokeDataResponse(messagesContainer =>
            {
                var screen = _hardwareManager.ScreensBySide[screenSide];
                if (screen is MatroxScreen matroxScreen)
                {
                    return matroxScreen.GetDefaultScreenValues();
                }
                else if (screen is DummyMatroxScreen)
                {
                    return new Dictionary<string, short> { { "backlight", 50 }, { "brightness", 50 }, { "contrast", 100 } };
                }
                return null;
            });
        }

        private void OnBacklightChanged(Side side, double value)
        {
            InvokeCallback(i => i.OnBacklightChangedCallback(side, (short)value));
        }

        private void OnBrightnessChanged(Side side, double value)
        {
            InvokeCallback(i => i.OnBrightnessChangedCallback(side, (short)value));
        }

        private void OnContrastChanged(Side side, double value)
        {
            InvokeCallback(i => i.OnContrastChangedCallback(side, (short)value));
        }

        private void OnSharpnessChanged(Side side, int value)
        {
            InvokeCallback(i => i.OnSharpnessChangedCallback(side, value));
        }

        private void OnTemperatureChanged(Side side, double value)
        {
            InvokeCallback(i => i.OnTemperatureChangedCallback(side, value));
        }

        private void OnFanChanged(Side side, int value)
        {
            InvokeCallback(i => i.OnFanChangedCallback(side, value));
        }

        private void OnFanAutoChanged(Side side, bool value)
        {
            InvokeCallback(i => i.OnFanAutoChangedCallback(side, value));
        }

        private void OnPowerStateChanged(Side side, bool value)
        {
            InvokeCallback(i => i.OnPowerStateChangedCallback(side, value));
        }

        private void ScreenImageChanged(ScreenBase screen, USPImageMil procimage, Color color)
        {
            var svcimg = procimage == null ? null : procimage.ToServiceImage();
            InvokeCallback(x => x.ScreenImageChangedCallback(screen.DeviceID, svcimg, color));
        }

        Response<ScreenInfo> IDMTScreenService.GetScreenInfo(string screenId)
        {
            return InvokeDataResponse(messageContainer =>
            {
                ScreenBase scr = _hardwareManager.Screens[screenId];
                return new ScreenInfo()
                {
                    Model = scr.Model,
                    SerialNumber = scr.SerialNumber,
                    Version = scr.Version,
                    Width = scr.Width,
                    Height = scr.Height,
                    PixelPitchHorizontal = scr.PixelPitchHorizontal,
                    PixelPitchVertical = scr.PixelPitchVertical,
                    ScreenWhiteDisplayTimeSec = scr.ScreenWhiteDisplayTimeSec
                };
            });
        }

        async Task<Response<VoidResult>> IDMTScreenService.SetScreenColorAsync(string screenId, Color color)
        {
            return await InvokeVoidResponseAsync(async messagesContainer =>
            {
                await _hardwareManager.Screens[screenId].ClearAsync(color);
            });
        }

        async Task<Response<VoidResult>> IDMTScreenService.DisplayImageAsync(string screenId, ServiceImage image)
        {
            return await InvokeVoidResponseAsync(async messagesContainer =>
            {
                var procimg = new USPImageMil(image);
                await _hardwareManager.Screens[screenId].DisplayImageAsync(procimg);
            });
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                Subscribe();
                messageContainer.Add(new Message(MessageLevel.Information, "Subscribed to hardware change"));
            });
        }

        public Response<VoidResult> UnsubscribeFromChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                Unsubscribe();
                messageContainer.Add(new Message(MessageLevel.Information, "Unsubscribed to hardware change"));
            });
        }
    }
}
