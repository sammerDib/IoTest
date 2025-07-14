using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Chamber
{
    public class InterlockVM : ObservableObject
    {        
        public string Description { get; set; }

        private string _stateValue;
        public string StateValue
        {
            get { return _stateValue; }
            set
            {
                if (_stateValue != value)
                {
                    _stateValue = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
