using System;
using System.IO;

namespace UnitySC.PM.ANA.Service.Shared.TestUtils
{
    public static class DirTestUtils
    {
        public static string GetWorkingDirectoryDataPath(string localDataPath)
        {
            // get the current WORKING directory (i.e. \bin\Debug)
            string workingDirectory = AppContext.BaseDirectory;
            // get the current PROJECT directory
#if USE_ANYCPU
            string projectDirectory = Directory.GetParent(workingDirectory).Parent?.FullName;
#else
            string projectDirectory = Directory.GetParent(workingDirectory).Parent?.Parent?.FullName;
#endif

            return projectDirectory + localDataPath;
        }
    }
}
