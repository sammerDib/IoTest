using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace UnitySC.Shared.Tools
{
    public static class DirectoryTools
    {
        public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            CopyDirectory(sourceDir, destinationDir, recursive, null);
        }

        public static void CopyDirectoryAndSendProgressMessage(string sourceDir, string destinationDir, bool recursive, Action sendProgressMessage)
        {
            CopyDirectory(sourceDir, destinationDir, recursive, sendProgressMessage);
        }

        private static void CopyDirectory(string sourceDir, string destinationDir, bool recursive, Action sendProgressMessage = null)
        {
            var dir = new DirectoryInfo(sourceDir);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
            }

            Directory.CreateDirectory(destinationDir);

            var files = dir.GetFiles();
            var subDirs = dir.GetDirectories();

            Parallel.ForEach(files, file =>
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, true);
                sendProgressMessage?.Invoke();
            });

            if (recursive)
            {
                Parallel.ForEach(subDirs, subDir =>
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true, sendProgressMessage);
                });
            }
        }

        public static int CountFilesInDirectoryAndSendProgressMessage(string directoryPath, Action<int> sendProgressMessage)
        {
            int fileCount = 0;
            var subDirectoryFileCounts = new ConcurrentBag<int>();

            // Use a loop instead of .Count() which would involve a double course of the collection
            foreach (string directory in Directory.EnumerateFiles(directoryPath))
            {
                fileCount++;
                sendProgressMessage.Invoke(fileCount);
            }

            var subDirectories = Directory.EnumerateDirectories(directoryPath).ToList();
            Parallel.ForEach(subDirectories, subDirectory =>
            {
                int count = CountFilesInDirectoryAndSendProgressMessage(subDirectory, sendProgressMessage);
                subDirectoryFileCounts.Add(count);
            });

            fileCount += subDirectoryFileCounts.Sum();

            return fileCount;
        }

        public static void CompareDirectoriesRecursive(string dir1, string dir2, List<string> differences)
        {
            if (!Directory.Exists(dir1))
            {
                differences.Add($"Directory {dir1} does not exist");
                return;
            }
            if (!Directory.Exists(dir2))
            {
                differences.Add($"Directory {dir2} does not exist");
                return;
            }

            var files1 = new HashSet<string>(Directory.GetFiles(dir1));
            var files2 = new HashSet<string>(Directory.GetFiles(dir2));
            var dirs1 = new HashSet<string>(Directory.GetDirectories(dir1));
            var dirs2 = new HashSet<string>(Directory.GetDirectories(dir2));

            CompareFileLists(files1, files2, differences, dir1, dir2);
            CompareDirectoryLists(dirs1, dirs2, differences, dir1, dir2);
        }

        private static void CompareFileLists(HashSet<string> files1, HashSet<string> files2, List<string> differences, string dir1, string dir2)
        {
            foreach (string file1 in files1)
            {
                string fileName = Path.GetFileName(file1);
                string file2 = Path.Combine(dir2, fileName);

                if (!files2.Contains(file2))
                {
                    differences.Add($"Missing file in {dir2}: {fileName}");
                }
                else
                {
                    var date1 = File.GetLastWriteTime(file1);
                    var date2 = File.GetLastWriteTime(file2);
                    if (date1 != date2)
                    {
                        differences.Add($"Date difference : {fileName} ({date1} != {date2})");
                    }
                }
            }

            foreach (string file2 in files2)
            {
                string fileName = Path.GetFileName(file2);
                if (!files1.Contains(Path.Combine(dir1, fileName)))
                {
                    differences.Add($"Missing file in {dir1}: {fileName}");
                }
            }
        }

        private static void CompareDirectoryLists(HashSet<string> dirs1, HashSet<string> dirs2, List<string> differences, string dir1, string dir2)
        {
            foreach (string subDir1 in dirs1)
            {
                string subDirName = Path.GetFileName(subDir1);
                string subDir2 = Path.Combine(dir2, subDirName);

                if (!dirs2.Contains(subDir2))
                {
                    differences.Add($"Missing directory in {dir2}: {subDirName}");
                }
                else
                {
                    CompareDirectoriesRecursive(subDir1, subDir2, differences);
                }
            }

            foreach (string subDir2 in dirs2)
            {
                string subDirName = Path.GetFileName(subDir2);
                if (!dirs1.Contains(Path.Combine(dir1, subDirName)))
                {
                    differences.Add($"Missing directory in {dir1}: {subDirName}");
                }
            }
        }
    }
}
