using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.PM.ANA.Client.Proxy.Axes.Models
{
    public class ChuckStatus : ObservableObject
    {
        #region Fields
        private bool _isWaferClamped;
        private bool _isWaferPresent;
        #endregion

        #region Public methods
        public bool IsWaferPresent
        {
            get
            {
                return _isWaferPresent;
            }
            set
            {
                if (_isWaferPresent == value)
                    return;
                _isWaferPresent = value;
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
        public ChuckStatus()
        {
            IsWaferClamped = false;
            IsWaferPresent = false;
        }
        #endregion
    }
}
