using System;

namespace AppLauncher
{
    [Serializable]
    public class LauncherServiceConfig
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }

        public string Arguments { get; set; }
        public string ServiceName { get; set; }
        public bool DisplayInLauncher { get; set; }
 
        public bool IsConsoleMode { get; set; }
        public bool ShowConsoleWindow { get; set; }
        public int DelayBeforeLaunchingNextService { get; set; }

    }
}
