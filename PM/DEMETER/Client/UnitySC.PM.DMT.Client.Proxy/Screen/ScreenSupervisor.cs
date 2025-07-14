using System;
using System.ServiceModel;
using System.Windows.Media;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.DMT.Hardware.Service.Interface.Screen;
using UnitySC.PM.Shared.Data.Image;
using UnitySC.PM.DMT.Service.Interface.Screen;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.DMT.Hardware.ClientProxy.Screen
{
    public class ScreenSupervisor : IScreenServiceCallback
    {
        private InstanceContext _instanceContext;
        private ILogger _logger;
        private IMessenger _messenger;
        private readonly DuplexServiceInvoker<IDMTScreenService> _screenService;
        private IDialogOwnerService _dialogService;

        /// <summary>
        /// Constructor
        /// </summary>
        public ScreenSupervisor(ILogger<ScreenSupervisor> logger, IMessenger messenger, IDialogOwnerService dialogService)
        {
            _instanceContext = new InstanceContext(this);
            _screenService = new DuplexServiceInvoker<IDMTScreenService>(_instanceContext, "DMTScreenService",
                ClassLocator.Default.GetInstance<SerilogLogger<IDMTScreenService>>(), messenger, s => s.Subscribe());
            _logger = logger;
            _messenger = messenger;
            _dialogService = dialogService;
        }

        void IScreenServiceCallback.ScreenImageChangedCallback(string screenId, ServiceImage image, Color color)
        {
            _logger.Information("Screen Supervisor : Screen image changed");
            _messenger.Send(new ScreenImageChangedMessage() { ScreenId = screenId, ServiceImage = image, Color = color });
        }

        /// <summary>
        /// Subscribe to hardware changes
        /// </summary>
        public void Subscribe()
        {
            _screenService.Invoke(s => s.Subscribe());
        }

        /// <summary>
        /// Unsubscribe to hardware changes
        /// </summary>
        public void Unsubscribe()
        {
            try
            {
                _screenService.Invoke(s => s.Unsubscribe());
            }
            catch (Exception ex)
            {
                _dialogService.ShowException(ex, "Subscribe error");
            }
        }

        public void SetScreenColor(string screenId, Color color)
        {
            try
            {
                _screenService.Invoke(s => s.SetScreenColor(screenId, color));
            }
            catch (Exception ex)
            {
                string msg = $"Error when setting color on scren ${screenId}";
                _logger.Error(ex, msg);
                _dialogService.ShowException(ex, msg);
            }
        }

        public ScreenInfo GetScreenInfo(string screenId)
        {
            ScreenInfo info = _screenService.Invoke(s => s.GetScreenInfo(screenId));
            return info;
        }

        public void DisplayImage(string screenId, ServiceImage image)
        {
            _screenService.Invoke(s => s.DisplayImage(screenId, image));
        }

        public void OnBacklightChangedCallback(Side side, short value)
        {
            throw new NotImplementedException();
        }

        public void OnBrightnessChangedCallback(Side side, short value)
        {
            throw new NotImplementedException();
        }

        public void OnContrastChangedCallback(Side side, short value)
        {
            throw new NotImplementedException();
        }

        public void OnSharpnessChangedCallback(Side side, int value)
        {
            throw new NotImplementedException();
        }

        public void OnTemperatureChangedCallback(Side side, double value)
        {
            throw new NotImplementedException();
        }

        public void OnFanChangedCallback(Side side, int value)
        {
            throw new NotImplementedException();
        }
    }
}
