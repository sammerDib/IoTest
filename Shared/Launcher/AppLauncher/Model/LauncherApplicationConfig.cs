using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace AppLauncher
{
    [Serializable]
    public class LauncherApplicationConfig
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }

        public string Arguments { get; set; }
        public List<string> ServiceDependencies { get; set; }
    }
}
