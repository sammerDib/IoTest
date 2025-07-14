using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.DMT.Service.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.DMT.Tools.TopoCalib
{
    /// <summary>
    /// History of calibration images and result for one side (FW or BW).
    /// </summary>
    public class History
    {
        /// <summary>
        /// Calibration history.
        /// </summary>
        public List<Run> CalibHistory { get; private set; }

        /// <summary>
        /// Latest calibration. Null if no calibration available.
        /// </summary>
        public Run LatestCalibration => CalibHistory.Last();

        /// <summary>
        /// C++ side folder info.
        /// </summary>
        public CalibPaths CalibPaths { get; private set; }

        public History(CalibPaths calibPaths)
        {
            CalibPaths = calibPaths;
        }

        /// <summary>
        /// Loads calibration history, and prepares for latest calibration loading.
        /// throws IOException
        /// </summary>
        public async Task LoadAsync(string sideFolder)
        {
            SideFolder = sideFolder;

            // Make sure the side folder exists and read history.
            //>IOException
            await AsyncFileIO.FolderCreateIfNotExisting(sideFolder).ConfigureAwait(false);
            DirectoryInfo[] runs = await AsyncFileIO.FolderListSubFoldersAsync(sideFolder).ConfigureAwait(false);

            CalibHistory = new List<Run>(runs.Length);
            foreach (DirectoryInfo run in runs)
            {
                try
                {
                    //.IOException
                    CalibHistory.Add(new Run(run.FullName));
                }
                catch (IOException) { }// Ignored
            }

            CalibHistory.Sort();
        }

        private const string SourceImagesTempFolderName = "SourceImagesTemp";

        /// <summary>
        /// Image acquisitions are stored here before being sent to the calibration.
        /// </summary>
        public string SourceImagesTempFolderCam => Path.Combine(SideFolder, SourceImagesTempFolderName, Run.CameraCalibration);
        public string SourceImagesTempFolderSys => Path.Combine(SideFolder, SourceImagesTempFolderName, Run.SystemCalibration);

        /// <summary>
        /// Folder (FW or BW) containing the history.
        /// </summary>
        public string SideFolder { get; private set; }

        public enum CalibrationType : byte { camera, system };

        /// <summary>
        /// Prepares for a new calibration (cam or sys).
        /// throws IOException
        /// Cam calib: puts default settings in the run folder.
        /// Sys calib: puts a copy of the latest cam calib results in the run folder.
        /// Except from that, the folder remains empty (no source images).
        /// </summary>
        public async Task<Run> NewCalibPrepareAsync(CalibrationType calibType)
        {
            Run newRun = Run.NewCalibTempRun(CalibPaths);

            // Purge run folder.
            //>IOException
            await AsyncFileIO.FolderPurgeAsync(newRun.RunFolder).ConfigureAwait(false);

            switch (calibType)
            {
                case CalibrationType.camera:
                    // Place default compute settings in lastCalibFolder.
                    //>IOException
                    await AsyncFileIO.FolderCopyToAsync(@".\TopoCalibrationDefault", newRun.ResultFolder(CalibPaths)).ConfigureAwait(false);
                    break;

                case CalibrationType.system:
                    if (LatestCalibration == null)
                    {
                        throw new IOException("Cam calibration is required!");
                    }

                    // Copy current calibration data.
                    //>IOException
                    await AsyncFileIO.FolderCopyToAsync(LatestCalibration.ResultFolder(CalibPaths), newRun.ResultFolder(CalibPaths)).ConfigureAwait(false);

                    // Remove sys calibration, keep cam calibration.
                    //>IOException
                    await AsyncFileIO.FolderPurgeAsync(CalibPaths.SysCalibFolder(CalibPaths.LastCalibPath)).ConfigureAwait(false);

                    break;
            }

            return newRun;
        }

        /// <summary>
        /// After a successfull calibration, moves temp data to the history folder.
        /// throws IOException.
        /// </summary>
        public Task NewCalibSaveAsync(Run run)
        {
            Run newRun = new Run(run.Date, SideFolder);
            CalibHistory.Add(newRun);

            var dirSource = Directory.GetDirectoryRoot(run.RunFolder);
            var dirTarget = Directory.GetDirectoryRoot(newRun.RunFolder);

            if (dirSource == dirTarget)
            {
                //>IOException
                return AsyncFileIO.FileOrFolderMoveAsync(run.RunFolder, newRun.RunFolder);
            }
            else
            {
                //>IOException
                _ = AsyncFileIO.FolderCopyToAsync(run.RunFolder, newRun.RunFolder);
                return AsyncFileIO.FileOrFolderDeleteAsync(run.RunFolder);
            }            
        }
    }
}
