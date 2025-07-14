using System;

using ConfigurationManager.Configuration;

namespace ConfigurationManager.ViewModel.Setting
{
    public class BaseAddressSettingViewModel : SettingBaseViewModel
    {
        public BaseAddressSettingViewModel(Configuration.Setting setting) : base(setting)
        {
        }
        public override string CheckFormat(string newvalue)
        {
            string res = newvalue.Trim();
            string startpattern = "net.tcp://";
            if (res.StartsWith(startpattern))
            {
                res = res.Substring(startpattern.Length, res.Length - startpattern.Length);
            }

            var spltits = res.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (spltits.Length > 0)
                return spltits[0];
            else
                return res;

        }
        public override void Validate()
        {

            //Value should be localhost:intPort (ex : localhost:2254)
            //or
            // IpAdress:intPort (ex : 10.100.203.18:2432)

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
