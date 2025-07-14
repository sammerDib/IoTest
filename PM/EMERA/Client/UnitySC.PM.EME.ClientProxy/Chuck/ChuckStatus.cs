using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.PM.EME.Client.Proxy.Chuck
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
            get => _isWaferPresent;
            set => SetProperty(ref _isWaferPresent, value);
        }

        public bool IsWaferClamped
        {
            get => _isWaferClamped;
            set => SetProperty(ref _isWaferClamped, value);
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
