using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.Shared.Hardware.ClientProxy.Axes.Models;

namespace UnitySC.PM.ANA.Client.Proxy.Axes.Models
{
    public class Status : AxisStatus
    {
        #region Fields

        private bool _isLanded;
        private bool _isWaferClamped;
        #endregion

        #region Public methods
        

        public bool IsLanded
        {
            get
            {
                return _isLanded;
            }
            set
            {
                if (_isLanded == value)
                    return;
                _isLanded = value;
                OnPropertyChanged();
            }
        }

        public bool IsWaferClamped
        {
            get
            {
                return _isWaferClamped;
            }
            set
            {
                if (_isWaferClamped == value)
                    return;
                _isWaferClamped = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructors
        public Status()
        {
            IsLanded = false;
            IsWaferClamped = false;
        }
        #endregion
    }
}
