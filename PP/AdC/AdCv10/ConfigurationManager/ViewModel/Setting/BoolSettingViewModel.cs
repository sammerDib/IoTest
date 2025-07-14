using System;

using ConfigurationManager.Configuration;

namespace ConfigurationManager.ViewModel.Setting
{
    public class BoolSettingViewModel : SettingBaseViewModel
    {
        public BoolSettingViewModel(Configuration.Setting setting) : base(setting)
        {
        }

        public bool IsChecked
        {
            get
            {
                bool res;
                if (Boolean.TryParse(Value, out res))
                    return res;
                else
                    return false;
            }
            set
            {
                Value = value.ToString();
                OnPropertyChanged();
            }
        }

        public override void Validate()
        {
            bool res;
            System.Windows.Application.Current.Dispatcher.Invoke((() => { State = Boolean.TryParse(Value, out res) ? SettingState.Valid : SettingState.Error; }));
        }
    }
}
