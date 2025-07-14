using System.Collections.Generic;

namespace ConfigurationManager.Configuration
{
    public class Setting
    {
        public Setting()
        {
            Values = new Dictionary<ApplicationType, string>();
        }

        public string Key { get; set; }
        public string Help { get; set; }
        public ConfigurationType ConfigurationType { get; set; }
        public List<ApplicationType> UsedIn { get; set; }
        public Dictionary<ApplicationType, string> Values { get; set; }
        public bool Expert { get; set; }
        public bool Custom { get; set; }
    }
}
