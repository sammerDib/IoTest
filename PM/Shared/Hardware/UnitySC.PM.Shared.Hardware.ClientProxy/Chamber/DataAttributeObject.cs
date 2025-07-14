using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Chamber
{
    public delegate void SetDigitalIoChanged(string identifier, string name, bool value);    
    public delegate void SetAnalogIoChanged(string identifier, string name, double value);

    

    public class DataAttributeObject : ObservableObject
    {
        private SetDigitalIoChanged _onDigitalValueChanged;
        private SetAnalogIoChanged _onAnalogValueChanged;
        
        private bool _value;
        private double _analogicValue;
        public string Name { get; set; }
        public string Identifier { get; set; }
        public string ControllerId { get; set; }
        public string CommandName { get; set; }

        public bool Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    if (OnDigitalValueChanged != null) OnDigitalValueChanged(Identifier, Name, _value);
                    OnPropertyChanged();
                }
            }
        }

        public double AnalogicValue
        {
            get { return _analogicValue; }
            set
            {
                if (_analogicValue != value)
                {
                    _analogicValue = value;
                    if (OnAnalogValueChanged != null) OnAnalogValueChanged(Identifier, Name, _analogicValue);
                    OnPropertyChanged();
                }
            }
        }

        public void UpdateValue(bool value)
        {
            if (_value != value)
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }
        public void UpdateAnalogicValue(double value)
        {
            if (_analogicValue != value)
            {
                _analogicValue = value;
                OnPropertyChanged(nameof(AnalogicValue));
            }
        }

        public SetDigitalIoChanged OnDigitalValueChanged { get => _onDigitalValueChanged; set => _onDigitalValueChanged = value; }
        public SetAnalogIoChanged OnAnalogValueChanged { get => _onAnalogValueChanged; set => _onAnalogValueChanged = value; }
    }
}
