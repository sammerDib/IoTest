using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.Command
{
    public class DeviceCommandContextParametersViewModel : Notifier
    {
        private bool _enableVerification = true;

        public bool EnableVerification
        {
            get => _enableVerification;
            set => SetAndRaiseIfChanged(ref _enableVerification, value);
        }

        private long _timeOut;

        public long TimeOut
        {
            get => _timeOut;
            set => SetAndRaiseIfChanged(ref _timeOut, value);
        }

        private bool _useTimeout = true;

        public bool UseTimeout
        {
            get => _useTimeout;
            set => SetAndRaiseIfChanged(ref _useTimeout, value);
        }
    }
}
