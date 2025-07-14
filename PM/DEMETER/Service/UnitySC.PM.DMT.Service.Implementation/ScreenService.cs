using System.ServiceModel;
using System.Windows.Media;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Hardware.Screen;
using UnitySC.PM.DMT.Hardware.Service.Interface.Screen;
using UnitySC.PM.Shared.Data.Image;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Hardware.Service.Interface.Screen;

namespace UnitySC.PM.DMT.Hardware.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ScreenService : DuplexServiceBase<IScreenServiceCallback>, IScreenService
    {
        private DMTHardwareManager _hardwareManager;

        public ScreenService(ILogger logger) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = ClassLocator.Default.GetInstance<DMTHardwareManager>();

            var messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Register<ScreenMessage>(this, (r, m) => { ScreenImageChanged(m.Screen, m.Image, m.Color); });
        }

        public override void Init()
        {
            base.Init();
        }

        private void ScreenImageChanged(ScreenBase screen, USPImageMil procimage, Color color)
        {
            _logger.Information("ScreenService : Screen image changed");
            var svcimg = procimage == null ? null : procimage.ToServiceImage();
            InvokeCallback(x => x.ScreenImageChangedCallback(screen.DeviceID, svcimg, color));
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                Subscribe();
                messageContainer.Add(new Message(MessageLevel.Information, "Subscribed to hardware change"));
            });
        }

        Response<VoidResult> IScreenService.Unsubscribe()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                base.Unsubscribe();
                messageContainer.Add(new Message(MessageLevel.Information, "Unsubscribed to hardware change"));
            });
        }

        Response<ScreenInfo> IScreenService.GetScreenInfo(string screenId)
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
                    ScreenWhiteDisplayTime = scr.ScreenWhiteDisplayTime
                };
            });
        }

        Response<VoidResult> IScreenService.SetScreenColor(string screenId, Color color)
        {
            return InvokeVoidResponse(messagesContainer =>
            {
                _hardwareManager.Screens[screenId].Clear(color);
            });
        }

        Response<VoidResult> IScreenService.DisplayImage(string screenId, ServiceImage image)
        {
            return InvokeVoidResponse(messagesContainer =>
            {
                var procimg = new USPImageMil(image);
                _hardwareManager.Screens[screenId].DisplayImage(procimg);
            });
        }
    }
}
