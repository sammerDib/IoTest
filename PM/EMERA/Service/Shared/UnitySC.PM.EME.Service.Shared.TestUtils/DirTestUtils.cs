using System;
using System.IO;

namespace UnitySC.PM.EME.Service.Shared.TestUtils
{
    public static class DirTestUtils
    {
        public static string GetWorkingDirectoryDataPath(string localDataPath)
        {
            // get the current WORKING directory (i.e. \bin\Debug)
            string workingDirectory = AppContext.BaseDirectory;

            // get the current PROJECT directory
            string projectDirectory = Directory.GetParent(workingDirectory).Parent?.Parent?.FullName;
            return projectDirectory + localDataPath;
        }
    }
}
