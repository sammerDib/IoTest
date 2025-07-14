using System.Collections.Generic;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Stage.ViewModel
{
    public class SettingGroup
    {
        public SettingGroup(string settingGroupName)
        {
            SettingGroupName = settingGroupName;
        }

        public string SettingGroupName { get; set; }
        public IList<SettingParameter> Settings { get; set; }
    }
}