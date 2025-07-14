namespace UnitySC.PM.Shared.Hardware.ClientProxy.Stage.ViewModel
{
    public class SettingParameter
    {
        public SettingParameter(string settingName, string settingValue)
        {
            SettingName = settingName;
            SettingValue = settingValue;
        }

        public string SettingName { get; set; }
        public string SettingValue { get; set; }
    }
}