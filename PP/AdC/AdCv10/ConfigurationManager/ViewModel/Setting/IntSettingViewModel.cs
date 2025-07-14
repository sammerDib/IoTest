using ConfigurationManager.Configuration;

namespace ConfigurationManager.ViewModel.Setting
{
    public class IntSettingViewModel : SettingBaseViewModel
    {
        public IntSettingViewModel(Configuration.Setting setting) : base(setting)
        {
        }

        public int IntValue
        {
            get
            {
                int res;
                if (int.TryParse(Value, out res))
                    return res;
                else
                    return 0;
            }
            set
            {
                Value = value.ToString();
                OnPropertyChanged();
            }
        }

        public override void Validate()
        {
            int res;
            System.Windows.Application.Current.Dispatcher.Invoke((() => { State = int.TryParse(Value, out res) ? SettingState.Valid : SettingState.Error; }));
        }
    }
}
