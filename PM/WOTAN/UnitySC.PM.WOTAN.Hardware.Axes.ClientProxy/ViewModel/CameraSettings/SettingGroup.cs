using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Axes.ViewModel
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
