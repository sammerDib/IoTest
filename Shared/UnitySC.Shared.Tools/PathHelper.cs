using System;
using System.IO;
using System.Reflection;

namespace UnitySC.Shared.Tools
{
    public static class PathHelper
    {
        public static string GetExecutingAssemblyPath()
        {
            var uri = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            return uri.LocalPath;
        }

        public static string GetFullPathFromAssembly(string path)
        {
            if (Path.IsPathRooted(path))
                return path;
            string currentPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).AbsolutePath);
            return Path.Combine(currentPath, path);
        }

        public static string GetExeFullPath()
        {
            return System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
        }

        public static string GetAppBaseDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static string GetCurrentDirectory()
        {
            return System.IO.Directory.GetCurrentDirectory();
        }

        public static string GetTempPath()
        {
            return Path.GetTempPath();
        }

        public static string GetAbsolutePath(string relativeOrAbsPath, string rootPath)
        {
            if (Path.IsPathRooted(relativeOrAbsPath))
                return relativeOrAbsPath;
            return Path.Combine(rootPath, relativeOrAbsPath);
        }
    }
}
