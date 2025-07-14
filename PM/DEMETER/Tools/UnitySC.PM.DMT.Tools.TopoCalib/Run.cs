using System;
using System.IO;

using UnitySC.PM.DMT.Service.Interface;

namespace UnitySC.PM.DMT.Tools.TopoCalib
{
    /// <summary>
    /// Source images and results for a calibation run (cam or sys).
    /// </summary>
    public class Run :
        IComparable<Run>// Sorted by date, ascending.
    {
        /// <summary>
        /// Run date (floor to second).
        /// </summary>
        public DateTime Date;

        private const string SourceImages = "SourceImages";
        public const string CameraCalibration = "CameraCalibration";
        public const string SystemCalibration = "SystemCalibration";

        /// <summary>
        /// Subfolder for the run (typically in the side folder).
        /// </summary>
        private string _runSubFolder;

        /// <summary>
        /// Where to find the source images used for camera calibration.
        /// </summary>
        public string SourceImagesFolderCam => Path.Combine(_sideFolder, _runSubFolder, SourceImages, CameraCalibration);

        /// <summary>
        /// Where to find the source images used for system calibration.
        /// </summary>
        public string SourceImagesFolderSys => Path.Combine(_sideFolder, _runSubFolder, SourceImages, SystemCalibration);

        /// <summary>
        /// Run folder (contains both sources images and results).
        /// </summary>
        public string RunFolder => Path.Combine(_sideFolder, _runSubFolder);

        /// <summary>
        /// Where to find the calibration results.
        /// The results folder must have the same name as in the C++ side, hence calibPaths.
        /// </summary>
        public string ResultFolder(CalibPaths calibPaths)
        {
            return Path.Combine(_sideFolder, _runSubFolder, Path.GetFileName(calibPaths.LastCalibPath));
        }

        /// <summary>
        /// Creates a run in a temp folder, for a new calibration.
        /// </summary>
        public static Run NewCalibTempRun(CalibPaths calibPaths)
        {
            return new Run(Path.GetDirectoryName(calibPaths.LastCalibPath), newCalib: true);
        }
        private Run(string runFolder, bool newCalib)
        {
            _runSubFolder = Path.GetFileName(runFolder);
            _sideFolder = Path.GetDirectoryName(runFolder);

            Date = DateTime.Now;
            Date = new DateTime(Date.Year, Date.Month, Date.Day, Date.Hour, Date.Minute, Date.Second);
        }

        public Run(DateTime date, string sideFolder)
        {
            Date = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
            _runSubFolder = Date.ToString("yyyy.MM.dd_HH.mm.ss");

            _sideFolder = sideFolder;
        }

        /// <summary>
        /// Parses date from a specific run folder.
        /// throws IOException if the name is incorrect.
        /// </summary>
        public Run(string runFolder)
        {
            _runSubFolder = Path.GetFileName(runFolder);

            string[] dateAndTime = _runSubFolder.Split('_');
            if (dateAndTime.Length != 2) { throw new IOException(); }

            string[] yearMonthDay = dateAndTime[0].Split('.');
            if (yearMonthDay.Length != 3) { throw new IOException(); }

            string[] hoursMinutesSeconds = dateAndTime[1].Split('.');
            if (hoursMinutesSeconds.Length != 3) { throw new IOException(); }

            try
            {
                Date = new DateTime(Int32.Parse(yearMonthDay[0]), Int32.Parse(yearMonthDay[1]), Int32.Parse(yearMonthDay[2]),
                    Int32.Parse(hoursMinutesSeconds[0]), Int32.Parse(hoursMinutesSeconds[1]), Int32.Parse(hoursMinutesSeconds[2]));

                _sideFolder = Path.GetDirectoryName(runFolder);
            }
            catch (IOException) { throw; }
            catch (Exception ex) { throw new IOException("", ex); }
        }

        /// <summary>
        /// FW or BW folder.
        /// </summary>
        private string _sideFolder;

        public int CompareTo(Run other)
        {
            return Date.CompareTo(other.Date);
        }
    }
}
