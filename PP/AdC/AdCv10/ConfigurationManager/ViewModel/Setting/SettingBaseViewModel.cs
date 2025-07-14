using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using ConfigurationManager.Configuration;

namespace ConfigurationManager.ViewModel.Setting
{
    public abstract class SettingBaseViewModel : ObservableRecipient
    {
        public SettingBaseViewModel(Configuration.Setting setting)
        {
            _setting = setting;
            _useThisValueForAllApplication = AllValuesAreEqual;
        }

        private string _error;
        public string Error
        {
            get => _error; set { if (_error != value) { _error = value; OnPropertyChanged(); } }
        }

        public string Help
        {
            get => _setting.Help; set { if (_setting.Help != value) { _setting.Help = value; OnPropertyChanged(); } }
        }

        public string Key
        {
            get => _setting.Key; set { if (_setting.Key != value) { _setting.Key = value; OnPropertyChanged(); } }
        }

        private SettingState _state;
        public SettingState State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged();
                    if (_state != SettingState.Error)
                        Error = string.Empty;
                }
            }
        }

        private ConfigurationManager.Configuration.Setting _setting;
        private ApplicationType _selectedApplication;
        public ApplicationType SelectedApplication
        {
            get => _selectedApplication;
            set
            {
                if (_selectedApplication != value)
                {
                    _selectedApplication = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Value));
                    Validate();
                }
            }
        }

        private bool _useThisValueForAllApplication;
        public bool UseThisValueForAllApplication
        {
            get => _useThisValueForAllApplication;
            set
            {
                if (_useThisValueForAllApplication != value)
                {
                    _useThisValueForAllApplication = value;

                    if (_useThisValueForAllApplication)
                        Value = _setting.Values[_selectedApplication];

                    OnPropertyChanged();
                }
            }
        }

        public string Value
        {
            get
            {
                return _setting.Values.ContainsKey(_selectedApplication) ? _setting.Values[_selectedApplication] : null;
            }
            set
            {
                if ((_setting.Values.Count > 0) && !_setting.Values.ContainsKey(_selectedApplication))
                    return;

                string newvalue = CheckFormat(value);
                if (_useThisValueForAllApplication)
                {
                    foreach (var key in _setting.Values.Select(x => x.Key).ToList())
                    {
                        _setting.Values[key] = newvalue;
                    }
                }
                else
                    _setting.Values[_selectedApplication] = newvalue;

                OnPropertyChanged();
                Validate();
                OnPropertyChanged(nameof(AllValuesAreEqual));
            }
        }

        public bool AllValuesAreEqual => _setting.Values.Select(x => x.Value).Distinct().Count() == 1;

        public abstract void Validate();

        public virtual string CheckFormat(string newvalue) { return newvalue.Trim(); }
    }
}
