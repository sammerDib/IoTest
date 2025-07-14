using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using UnitySC.Shared.Tools;

using MessageBox = System.Windows.MessageBox;

namespace VersionSwitcher.ViewModel
{
    internal class CustomVersionStringComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            var vx = new Version(x.TrimStart('v'));
            var vy = new Version(y.TrimStart('v'));
            return vx.CompareTo(vy);
        }
    }

    internal class SwitcherResult
    {
        public bool Success { get; set; }
        public Exception Exception { get; set; }
    }

    public class VersionSelectionViewModel : ObservableObject
    {
        #region Variables

        private const string DEFAULT_PATH = @"C:\UnitySC";
        private const string CONFIG_FOLDER_NAME = "Config";
        private const string BIN_FOLDER_NAME = "Bin";
        private const string APP_LAUNCHER_FILE_NAME = "AppLauncher*";
        private const string VERSION_XML_FILE_NAME = "Version_*.xml";
        private const string RELEASE_FOLDER_NAME = "Release";

        private string directoryPath;

        private SortedDictionary<string, string> versionDictionary;

        private string _snapshotPath;

        #endregion

        #region Properties

        private string _selectedVersion;

        private string _currentVersion;

        private bool _isUpgrade;

        private int _pbMaximum = 0;

        private int _pbValue = 0;

        private AsyncRelayCommand _onSwitchCommand;

        private string _pbText;

        private string _statusLabel;

        private string _pathLabel;

        private Brush _statusBrush;

        private ObservableCollection<string> _versions;

        public string CurrentVersion
        {
            get => _currentVersion;
            set => SetProperty(ref _currentVersion, value);
        }

        public string SelectedVersion
        {
            get => _selectedVersion;
            set
            {
                SetProperty(ref _selectedVersion, value);
                ComputeUpgradeOrDowngrade();
                ComputeVersionPath();
            }
        }

        public bool IsUpgrade
        {
            get => _isUpgrade;
            set => SetProperty(ref _isUpgrade, value);
        }

        public int PBMaximum
        {
            get => _pbMaximum;
            set => SetProperty(ref _pbMaximum, value);
        }

        public int PBValue
        {
            get => _pbValue;
            set => SetProperty(ref _pbValue, value);
        }

        public string PBText
        {
            get => _pbText;
            set => SetProperty(ref _pbText, value);
        }

        public string StatusLabel
        {
            get => _statusLabel;
            set => SetProperty(ref _statusLabel, value);
        }

        public string PathLabel
        {
            get => _pathLabel;
            set => SetProperty(ref _pathLabel, value);
        }

        public Brush StatusBrush
        {
            get => _statusBrush;
            set => SetProperty(ref _statusBrush, value);
        }

        public AsyncRelayCommand OnSwitchCommand
        {
            get => _onSwitchCommand ?? (_onSwitchCommand = new AsyncRelayCommand(SwitchVersion));
        }

        public ObservableCollection<string> Versions
        {
            get => _versions;
            set => SetProperty(ref _versions, value);
        }

        public string MainLabel { get; set; }

        #endregion

        #region Contructor

        public VersionSelectionViewModel()
        {
            StatusLabel = "Initializing...";
            InitializeComponent();
            StatusLabel = string.Empty;
        }

        // FOR FUTURE
        /*
        public VersionSelectionViewModel()
        {
            String message = "Default installation folder for the UnitySC programs is : " + DEFAULTPATH + "\n" +
                             "Do you want to select another folder?";
            String caption = "UnitySC USP Version Switcher - Installation folder";
            MessageBoxImage icon = MessageBoxImage.Question;
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxResult result = MessageBox.Show(message, caption, button, icon);
            if (result == MessageBoxResult.Yes){
                FolderBrowserDialog folderDialog = new FolderBrowserDialog();
                folderDialog.SelectedPath =  DEFAULTPATH;
                folderDialog.Description = "Select USP installation folder";
                folderDialog.ShowDialog();
                directoryPath = folderDialog.SelectedPath;
            }

            MainLabel =
                "Please choose the version of UnitySC programs you want to use.\n" +
                "Available versions are scanned in the folder " +
                directoryPath;
            LoadVersionsFromDirectory(directoryPath);
        }
        */

        #endregion

        #region Methods and Functions

        private void InitializeComponent()
        {
            versionDictionary = new SortedDictionary<string, string>(new CustomVersionStringComparer());
            directoryPath = DEFAULT_PATH;
            SelectedVersion = null;

            VerifyPath(directoryPath);
            LoadCurrentVersion(directoryPath);
            LoadVersionsFromDirectory(directoryPath);
        }

        /// <summary>
        /// Compute colors and visibilities for arrows in the view
        /// </summary>
        private void ComputeUpgradeOrDowngrade()
        {
            if (SelectedVersion == null || CurrentVersion == null)
            {
                return;
            }
            IComparer<string> comparer = new CustomVersionStringComparer();
            IsUpgrade = comparer.Compare(SelectedVersion, CurrentVersion) > 0;
        }

        private void ComputeVersionPath()
        {
            PathLabel = "";
            if (SelectedVersion != null && versionDictionary.ContainsKey(SelectedVersion))
            {
                PathLabel = versionDictionary[SelectedVersion];
            }
        }

        private void SetStatus(string message, bool isError)
        {
            Brush brush = isError ? Brushes.Red : Brushes.Green;
            StatusBrush = brush;
            StatusLabel = message;
        }

        private void CompleteStatus(string message)
        {
            StatusLabel += Environment.NewLine + message;
        }

        /// <summary>
        /// Verify the existence of <paramref name="path"/> and <paramref name="path"/>\Release.
        /// Exception will be thrown if one of those folders doesn't exist.
        /// </summary>
        /// <param name="path">The path to the directory to check</param>
        /// <exception cref="Exception">Exception that should not be catched, to make the app closing</exception>
        private void VerifyPath(string path)
        {
            bool isError = false;
            string errorMessage = string.Empty;
            if (!Directory.Exists(path + @"\" + RELEASE_FOLDER_NAME))
            {
                isError = true;
                errorMessage = $"Impossible to find \"{RELEASE_FOLDER_NAME}\" in \"{path}\".";
            }

            if (!Directory.Exists(path))
            {
                isError = true;
                errorMessage = $"Directory \"{path}\" doesn't exist.";
            }

            if (isError)
            {
                errorMessage += Environment.NewLine;
                errorMessage += Environment.NewLine;
                errorMessage += @"Verify your installation and try again.";
                MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// Parse and extract version number from version file in the provided directory
        /// </summary>
        /// <param name="versionDirectoryPath">Folder to search in</param>
        /// <returns>version number like 'v1.2.3'</returns>
        /// <exception cref="Exception">Exception if version file is not found or not parseable.</exception>
        private string ExtractVersionFromDirectory(string versionDirectoryPath)
        {
            string[] results = Directory.GetFiles(versionDirectoryPath, VERSION_XML_FILE_NAME, SearchOption.TopDirectoryOnly);
            if (!results.Any())
            {
                throw new Exception($"Impossible to find version file associated to folder \"{versionDirectoryPath}\".");
            }

            string versionFile = results.First();
            try
            {
                var doc = new XmlDocument();
                doc.Load(versionFile);
                return doc.DocumentElement?.SelectSingleNode("/USPVersion/Version")?.InnerText;
            }
            catch (Exception ex)
            {
                throw new Exception("Impossible to parse version file.", ex);
            }
        }

        /// <summary>
        /// Scan folders and retrieve all available versions
        /// </summary>
        /// <param name="directoryPathToScan">The root path to search in</param>
        private void LoadVersionsFromDirectory(string directoryPathToScan)
        {
            // Iterate through files in the directory and its subdirectories
            foreach (string directoryName in Directory.GetDirectories(directoryPathToScan, RELEASE_FOLDER_NAME + "_*", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    string version = ExtractVersionFromDirectory(directoryName);
                    versionDictionary.Add(version, directoryName);
                }
                catch (Exception ex)
                {
                    // Skip
                }
            }

            versionDictionary.Remove(CurrentVersion);
            Versions = new ObservableCollection<string>(versionDictionary.Keys.Reverse());
        }

        /// <summary>
        /// Retrieve current version of USP
        /// </summary>
        /// <param name="currentDirectoryPath">The root path to search in</param>
        private void LoadCurrentVersion(string currentDirectoryPath)
        {
            try
            {
                CurrentVersion = ExtractVersionFromDirectory(currentDirectoryPath + @"\" + RELEASE_FOLDER_NAME);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Impossible to parse current version.\n\nPlease check your installation and try again.\n\nException is : " +
                    ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// Method that switches versions, after doing some checks : <br/>
        /// - Verify that selected version is not null <br/>
        /// - Verify that UnitySC programs are all closed <br/>
        /// </summary>
        private async Task SwitchVersion()
        {
            SetStatus("", false);
            string selectedVersion = SelectedVersion;
            bool canCleanUpSnapshot = true;

            if (selectedVersion == null)
            {
                MessageBox.Show("Please select a version.");
                return;
            }

            if (!await CheckProcesses())
            {
                SetStatus("Switch aborted. Some Unity Processes are still running. Please close them", true);
                return;
            }

            // Prepare snapshot
            _snapshotPath = Path.Combine(directoryPath, "_Snapshots_", GetTimestampedName("Release_Snapshot"));
            var snapshotResult = await CreateSnapshot();
            if (!snapshotResult.Success)
            {
                SetStatus("Snapshot creation failed: " + snapshotResult.Exception.Message, true);
                return;
            }

            try
            {
                // Archive current version
                var archiveResult = await ArchiveCurrentVersion();
                if (!archiveResult.Success)
                {
                    throw new Exception("Archive failed: " + archiveResult.Exception.Message + Environment.NewLine +
                        $"IMPORTANT : Check 'Release_{CurrentVersion}' integrity before using it again.");
                }

                // Transfer selected version
                var transfertResult = await TransfertSelectedVersion();
                if (!transfertResult.Success)
                {
                    throw new Exception("Transfer failed: " + transfertResult.Exception.Message);
                }

                SetStatus("Switch success, you can now use the version " + selectedVersion, false);
            }
            catch (Exception ex)
            {
                // Rollback
                SetStatus("Switch failed !" + Environment.NewLine + ex.Message + Environment.NewLine, true);
                CompleteStatus("Rollback started ...");
                var rollbackResult = await RestoreSnapshot();

                if (!rollbackResult.Success)
                {
                    canCleanUpSnapshot = false;
                    CompleteStatus("Rollback failed: " + rollbackResult.Exception.Message);
                }
                else
                {
                    CompleteStatus("Rollback successful.");
                }
            }
            finally
            {
                if (canCleanUpSnapshot)
                {
                    // Clean snapshot
                    await CleanupSnapshot();
                    InitializeComponent();
                }
            }
        }

        private async Task<SwitcherResult> CreateSnapshot()
        {
            var result = new SwitcherResult { Success = true, Exception = null };

            PBText = "Creating Snapshot...";
            PBMaximum = 1;
            PBValue = 0;

            string sourcePath = Path.Combine(directoryPath, RELEASE_FOLDER_NAME);

            try
            {
                result = await CopyWholeFolder(sourcePath, _snapshotPath, true);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Exception = ex;
            }

            PBValue = 1;
            return result;
        }

        private async Task<SwitcherResult> RestoreSnapshot()
        {
            var result = new SwitcherResult { Success = true, Exception = null };

            PBText = "Restoring Snapshot...";
            PBMaximum = 1;
            PBValue = 0;

            string sourcePath = _snapshotPath;
            string targetPath = Path.Combine(directoryPath, RELEASE_FOLDER_NAME);

            try
            {
                // Remove current target directory
                if (Directory.Exists(targetPath))
                {
                    result = DeleteFolder(targetPath);
                }

                // Move snapshot to target directory
                result = await MoveFolder(sourcePath, targetPath);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Exception = ex;
            }

            PBValue = 1;
            return result;
        }

        private async Task CleanupSnapshot()
        {
            await Task.Run(() =>
            {
                try
                {
                    if (Directory.Exists(_snapshotPath))
                    {
                        DeleteFolder(_snapshotPath);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error cleaning up snapshot: " + ex.Message);
                }
            });
        }

        private async Task<SwitcherResult> TransfertSelectedVersion()
        {
            var transfertResult = new SwitcherResult { Success = true, Exception = null };

            PBText = "Transferring selected version ...";
            PBMaximum = 1;
            PBValue = 0;

            string directoryToRetrieve = string.Empty;
            if (versionDictionary.ContainsKey(SelectedVersion))
            {
                directoryToRetrieve = versionDictionary[SelectedVersion];
                string targetPath = directoryPath + @"\" + RELEASE_FOLDER_NAME;
                transfertResult = await CopyRelease(directoryToRetrieve, targetPath);
            }
            else
            {
                transfertResult.Success = false;
                transfertResult.Exception = new Exception("Impossible to retrieve target version from dictionary.");
            }

            return transfertResult;
        }

        private async Task<SwitcherResult> ArchiveCurrentVersion()
        {
            var archiveResult = new SwitcherResult { Success = true, Exception = null };

            PBText = "Archiving current version ...";
            PBMaximum = 1;
            PBValue = 0;

            // Check if current version has already been archived
            string currentVersionFolderName = RELEASE_FOLDER_NAME + "_" + CurrentVersion;
            bool currentVersionArchiveNotFound = true;
            await Task.Run(() =>
            {
                if (Directory.GetDirectories(directoryPath, currentVersionFolderName, SearchOption.TopDirectoryOnly).Any())
                {
                    currentVersionArchiveNotFound = false;
                }
            });

            string sourcePath = directoryPath + @"\" + RELEASE_FOLDER_NAME;
            string targetPath = directoryPath + @"\" + currentVersionFolderName;
            if (currentVersionArchiveNotFound)
            {
                // If current version has not already been archived, we create an archive, by copying Release folder
                PBText = "Archiving current version ...";
                PBMaximum = 1;
                PBValue = 0;

                archiveResult = await CopyRelease(sourcePath, targetPath);
            }
            else
            {
                // Check if current config and current archived version config are identical
                string sourceConfigPath = Path.Combine(sourcePath, CONFIG_FOLDER_NAME);
                string targetConfigPath = Path.Combine(targetPath, CONFIG_FOLDER_NAME);
                var compareResult = CompareConfig(sourceConfigPath, targetConfigPath);
                if (!compareResult.Success)
                {
                    // If not, we ask user if we should update archived config and do a backup of archived version config before replacing it
                    if (MessageBox.Show("Current config and its archived version have differences :" +
                        Environment.NewLine + compareResult.Exception.Message + Environment.NewLine + Environment.NewLine +
                        "Update archive config ? (a timstamped backup of config directory will be done before replacing it)",
                        "Config", 
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning,
                        MessageBoxResult.Yes) == MessageBoxResult.Yes)
                    {
                        archiveResult = await MoveFolder(targetConfigPath, GetTimestampedName(targetConfigPath + "_BackUp"));
                        if (!archiveResult.Success)
                        {
                            throw archiveResult.Exception;
                        }

                        // Copy current config into archived version and replace if exists
                        archiveResult = await CopyWholeFolder(sourceConfigPath, targetConfigPath);
                    }
                }
            }

            return archiveResult;
        }

        private SwitcherResult CompareConfig(string releasePath, string archivePath)
        {
            var compareResult = new SwitcherResult { Success = true, Exception = null };
            var differences = new List<string>();
            DirectoryTools.CompareDirectoriesRecursive(releasePath, archivePath, differences);

            if (differences.Count > 0)
            {
                var strBuilder = new StringBuilder();
                foreach (string diff in differences)
                {
                    strBuilder.AppendLine(diff);
                }

                compareResult.Success = false;
                compareResult.Exception = new Exception(strBuilder.ToString());
            }

            return compareResult;
        }

        private SwitcherResult DeleteFolder(string sourcePath)
        {
            var result = new SwitcherResult { Success = true, Exception = null };
            try
            {
                var di = new DirectoryInfo(sourcePath);
                di.Attributes &= ~FileAttributes.ReadOnly;
                Directory.Delete(sourcePath, true);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Exception = ex;
            }

            return result;
        }

        private async Task<SwitcherResult> MoveFolder(string sourcePath, string targetPath)
        {
            var result = new SwitcherResult { Success = true, Exception = null };
            await Task.Run(() =>
            {
                try
                {
                    var di = new DirectoryInfo(sourcePath);
                    di.Attributes &= ~FileAttributes.ReadOnly;
                    Directory.Move(sourcePath, targetPath);

                    PBValue++;
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Exception = ex;
                }
            });

            return result;
        }

        // Copy only four expected items of Release directory :
        // - config directory and all its content
        // - bin directory and all its content
        // - AppLauncher shorcut
        // - Version_XXX.xml file
        private async Task<SwitcherResult> CopyRelease(string sourcePath, string targetPath)
        {
            var result = new SwitcherResult { Success = true, Exception = null };

            try
            {
                // ***************************************************************************************************
                // Check if all needed items exists
                var directoryInfo = new DirectoryInfo(sourcePath);
                if (!directoryInfo.Exists)
                {
                    throw new DirectoryNotFoundException($"Impossible to find directory {sourcePath}");
                }

                string sourceConfigPath = Path.Combine(sourcePath, CONFIG_FOLDER_NAME);
                var sourceConfigDirectory = new DirectoryInfo(sourceConfigPath);
                if (!sourceConfigDirectory.Exists)
                {
                    throw new DirectoryNotFoundException($"Impossible to find directory {sourceConfigPath}");
                }

                string sourceBinPath = Path.Combine(sourcePath, BIN_FOLDER_NAME);
                var sourceBinDirectory = new DirectoryInfo(sourceBinPath);
                if (!sourceBinDirectory.Exists)
                {
                    throw new DirectoryNotFoundException($"Impossible to find directory {sourceBinPath}");
                }

                string[] sourceAppLauncherResults = Directory.GetFiles(sourcePath, APP_LAUNCHER_FILE_NAME, SearchOption.TopDirectoryOnly);
                string sourceAppLauncherPath = sourceAppLauncherResults.Length == 1 ? sourceAppLauncherResults[0] : "";
                string appLauncherFileName = Path.GetFileName(sourceAppLauncherPath);
                var sourceAppLauncherFile = new FileInfo(sourceAppLauncherPath);
                if (!sourceAppLauncherFile.Exists)
                {
                    throw new FileNotFoundException($"Impossible to find file {sourceAppLauncherPath}");
                }

                string[] sourceVersionXmlResults = Directory.GetFiles(sourcePath, VERSION_XML_FILE_NAME, SearchOption.TopDirectoryOnly);
                string sourceVersionXmlPath = sourceVersionXmlResults.Length == 1 ? sourceVersionXmlResults[0] : "";
                string versionXmlFileName = Path.GetFileName(sourceVersionXmlPath);
                var sourceVersionXmlFile = new FileInfo(sourceVersionXmlPath);
                if (!sourceVersionXmlFile.Exists)
                {
                    throw new FileNotFoundException($"Impossible to find file {sourceVersionXmlPath}");
                }

                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }
                // ***************************************************************************************************

                PBMaximum = Directory.EnumerateFiles(sourceConfigPath, "*", SearchOption.AllDirectories).Count() +
                    Directory.EnumerateFiles(sourceBinPath, "*", SearchOption.AllDirectories).Count() + 2;

                // Config folder
                result = await CopyReleaseFolder(CONFIG_FOLDER_NAME, sourcePath, targetPath);
                if (!result.Success)
                {
                    throw result.Exception;
                }

                // Bin folder
                result = await CopyReleaseFolder(BIN_FOLDER_NAME, sourcePath, targetPath);
                if (!result.Success)
                {
                    throw result.Exception;
                }

                // AppLauncher
                sourceAppLauncherFile.CopyTo(Path.Combine(targetPath, appLauncherFileName), true);
                PBValue++;

                // Version_XXX.xml
                // Remove Version_XXX.xml already existing in targetPath
                // Because it's name can be different from the source one when the copy is done from Release_XXX to Release
                string[] targetVersionXmlResults = Directory.GetFiles(targetPath, VERSION_XML_FILE_NAME, SearchOption.TopDirectoryOnly);
                string targetVersionXmlPath = targetVersionXmlResults.Length == 1 ? targetVersionXmlResults[0] : "";
                var targetXmlFileName = Path.GetFileName(targetVersionXmlPath);
                if (!string.IsNullOrEmpty(targetXmlFileName))
                {
                    File.Delete(Path.Combine(targetPath, targetXmlFileName));
                }

                sourceVersionXmlFile.CopyTo(Path.Combine(targetPath, versionXmlFileName), true);
                PBValue++;

                var di = new DirectoryInfo(targetPath);
                di.Attributes |= FileAttributes.ReadOnly;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Exception = ex;
            }

            return result;
        }

        private async Task<SwitcherResult> CopyReleaseFolder(string folderName, string sourcePath, string targetPath)
        {
            var result = new SwitcherResult { Success = true, Exception = null };

            try
            {
                string sourceFolderPath = Path.Combine(sourcePath, folderName);
                string targetFolderPath = Path.Combine(targetPath, folderName);
                string targetFolderPathTmp = targetFolderPath + "_TMP";
                if (Directory.Exists(targetPath))
                {
                    result = await MoveFolder(targetFolderPath, targetFolderPathTmp);
                    if (!result.Success)
                    {
                        throw result.Exception;
                    }
                }

                result = await CopyWholeFolder(sourceFolderPath, targetFolderPath);
                if (!result.Success)
                {
                    throw result.Exception;
                }

                if (Directory.Exists(targetFolderPathTmp))
                {
                    result = DeleteFolder(targetFolderPathTmp);
                    if (!result.Success)
                    {
                        throw result.Exception;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Exception = ex;
            }

            return result;
        }

        private async Task<SwitcherResult> CopyWholeFolder(string sourcePath, string targetPath, bool isRoot = false)
        {
            var result = new SwitcherResult { Success = true, Exception = null };

            try
            {
                var directoryInfo = new DirectoryInfo(sourcePath);
                if (!directoryInfo.Exists)
                {
                    MessageBox.Show("Impossible to find directory " + sourcePath, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    result.Success = false;
                    result.Exception = new DirectoryNotFoundException($"Impossible to find directory {sourcePath}");
                }
                else
                {
                    if (!Directory.Exists(targetPath))
                    {
                        Directory.CreateDirectory(targetPath);
                    }

                    PBMaximum = isRoot ? Directory.EnumerateFiles(sourcePath, "*.*", SearchOption.AllDirectories).Count() : PBMaximum;

                    await Task.Run(() =>
                    {
                        foreach (var file in directoryInfo.GetFiles())
                        {
                            string targetFilePath = Path.Combine(targetPath, file.Name);
                            file.CopyTo(targetFilePath, true);
                            PBValue++;
                        };
                    });

                    await Task.Run(async () =>
                    {
                        foreach (var subDir in directoryInfo.GetDirectories())
                        {
                            string newDestinationDir = Path.Combine(targetPath, subDir.Name);
                            var subCopyResult = await CopyWholeFolder(subDir.FullName, newDestinationDir);
                            if (!subCopyResult.Success)
                            {
                                throw subCopyResult.Exception;
                            }
                        }
                    });
                    if (isRoot)
                    {
                        var di = new DirectoryInfo(targetPath);
                        di.Attributes |= FileAttributes.ReadOnly;
                        di = null;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Exception = ex;
            }

            return result;
        }

        /// <summary>
        /// Verify all processes running to identify UnitySC programs
        /// </summary>
        /// <returns>true if not UnitySC programs are running. False otherwise</returns>
        private async Task<bool> CheckProcesses()
        {
            // Retrieve all processes
            var test = Process.GetProcesses();
            PBText = "Scanning processes ...";
            PBMaximum = test.Length;
            PBValue = 0;

            var unityProcesses = await Task.Run(
                () => test.Where((process) =>
                {
                    PBValue++;
                    return IsUnityProcess(process);
                }).ToList());

            // If any process is considered as UnitySC process
            if (unityProcesses.Any())
            {
                string warnMessage = unityProcesses.Aggregate("Some UnitySC software are still running : \n\n",
                    (current, process) => current + ($"\t- " + process.ProcessName + " (" + process.Id + ")\n"));

                warnMessage += "\nPlease close them all and try again.";

                MessageBox.Show(warnMessage, "Close UnitySC Applications", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verify all files to identify locked ones
        /// </summary>
        /// <returns>true if all files are free. False otherwise</returns>
        private async Task<bool> CheckFiles()
        {
            string[] files = Directory.GetFiles(directoryPath + @"\" + RELEASE_FOLDER_NAME, "*", SearchOption.AllDirectories);
            string[] directories = Directory.GetDirectories(directoryPath + @"\" + RELEASE_FOLDER_NAME, "*", SearchOption.AllDirectories);

            PBText = "Scanning files ...";
            PBMaximum = files.Length;
            PBValue = 0;
            var lockedFiles = await Task.Run(() =>
                files.Where((file) =>
                {
                    PBValue++;
                    return IsLocked(file);
                }).ToList());

            // If any process is considered as UnitySC process
            if (lockedFiles.Any())
            {
                string warnMessage = lockedFiles.Aggregate(
                    "Some UnitySC files are locked by other programs or are in read-only mode : \n\n",
                    (current, file) => current + (file + "\n"));

                warnMessage += "\nPlease fix them all and try again.";

                MessageBox.Show(warnMessage, "Unlock UnitySC Files", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if the provided process is a UnitySC process, and not the current process
        /// </summary>
        /// <param name="process">The process to verify</param>
        /// <returns>True if it's a UnitySC Process and not the Version Switcher. False otherwise</returns>
        private bool IsUnityProcess(Process process)
        {
            try
            {
                if (process.HasExited || process.Id == Process.GetCurrentProcess().Id)
                    return false;

                return (process.MainModule?.FileName.StartsWith(directoryPath) ?? false) && process.Id != Process.GetCurrentProcess().Id;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check if a file is usedd by another program
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool IsLocked(string file)
        {
            try
            {
                using (var stream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite))
                {
                    stream.Close();
                    return false;
                }
            }
            catch
            {
                return true;
            }
        }

        private string GetTimestampedName(string name)
        {
            var timestamp = DateTime.Now;
            var strBuiler = new StringBuilder(name);
            strBuiler.Append("_");
            strBuiler.Append(timestamp.Year.ToString());
            strBuiler.Append(timestamp.Month.ToString());
            strBuiler.Append(timestamp.Day.ToString());
            strBuiler.Append("_");
            strBuiler.Append(timestamp.Hour.ToString());
            strBuiler.Append(timestamp.Minute.ToString());
            strBuiler.Append(timestamp.Second.ToString());

            return strBuiler.ToString();
        }

        #endregion
    }
}
