using System;

using ConfigurationManager.Configuration;

namespace ConfigurationManager.ViewModel.Setting
{
    public class WcfAddressSettingViewModel : SettingBaseViewModel
    {
        public WcfAddressSettingViewModel(Configuration.Setting setting) : base(setting)
        {
        }

        public override void Validate()
        {
            try
            {
                Uri uri = new Uri(Value);
                State = SettingState.Valid;
            }
            catch
            {
                State = SettingState.Error;
                Error = "Invalid URI";
            }
        }
    }
}
