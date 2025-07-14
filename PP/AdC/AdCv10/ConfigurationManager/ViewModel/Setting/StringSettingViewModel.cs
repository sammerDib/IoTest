using System;
using System.Threading.Tasks;

using ConfigurationManager.Configuration;

namespace ConfigurationManager.ViewModel.Setting
{
    public class StringSettingViewModel : SettingBaseViewModel
    {
        public StringSettingViewModel(Configuration.Setting setting) : base(setting)
        {
        }

        public override void Validate()
        {
            State = SettingState.InProgress;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((() => { State = SettingState.Valid; }));
                }
                catch (Exception ex)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((() => { State = SettingState.Error; Error = ex.Message; }));
                }
            });
        }
    }
}
