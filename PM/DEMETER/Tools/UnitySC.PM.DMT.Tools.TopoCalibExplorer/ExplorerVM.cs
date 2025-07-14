using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.DMT.Service.Implementation.Curvature;
using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Tools.TopoCalib;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.DMT.Tools.TopoCalibExlorer
{
    public class ExplorerVM : ObservableRecipient
    {
        public override string ToString()
        {
            return "Calib " + AppDomain.CurrentDomain.BaseDirectory;
        }

        public ExplorerVM()
        {
            Timer();
        }

        /// <summary>
        /// C++ folders reference.
        /// </summary>
        private CalibPaths _calibPaths = new CalibPaths(NativeMethods.GetCalibFolderStructure());

        /// <summary>
        /// Compare 2 runs and outputs a digest.
        /// </summary>
        private string Compare(Run newer, Run older)
        {
            // Currently simply seeks for the biggest delta of individual rotation values.
            double delta_rad = 0d;

            IniFile newerIni = new IniFile(_calibPaths.MatrixScreenToCamPath(newer.ResultFolder(_calibPaths)));
            IniFile olderIni = new IniFile(_calibPaths.MatrixScreenToCamPath(older.ResultFolder(_calibPaths)));

            try
            {
                delta_rad = Math.Max(delta_rad,
                    Math.Abs(newerIni.GetDouble("EP_MIRE", "omc0", CultureInfo.InvariantCulture) - olderIni.GetDouble("EP_MIRE", "omc0", CultureInfo.InvariantCulture)));
            }
            catch (Exception) { }
            try
            {
                delta_rad = Math.Max(delta_rad,
                    Math.Abs(newerIni.GetDouble("EP_MIRE", "omc1", CultureInfo.InvariantCulture) - olderIni.GetDouble("EP_MIRE", "omc1", CultureInfo.InvariantCulture)));
            }
            catch (Exception) { }
            try
            {
                delta_rad = Math.Max(delta_rad,
                    Math.Abs(newerIni.GetDouble("EP_MIRE", "omc2", CultureInfo.InvariantCulture) - olderIni.GetDouble("EP_MIRE", "omc2", CultureInfo.InvariantCulture)));
            }
            catch (Exception) { }

            newerIni = new IniFile(_calibPaths.MatrixWaferToCameraPath(newer.ResultFolder(_calibPaths)));
            olderIni = new IniFile(_calibPaths.MatrixWaferToCameraPath(older.ResultFolder(_calibPaths)));

            try
            {
                delta_rad = Math.Max(delta_rad,
                    Math.Abs(newerIni.GetDouble("Calc0", "omc0", CultureInfo.InvariantCulture) - olderIni.GetDouble("Calc0", "omc0", CultureInfo.InvariantCulture)));
            }
            catch (Exception) { }
            try
            {
                delta_rad = Math.Max(delta_rad,
                    Math.Abs(newerIni.GetDouble("Calc0", "omc1", CultureInfo.InvariantCulture) - olderIni.GetDouble("Calc0", "omc1", CultureInfo.InvariantCulture)));
            }
            catch (Exception) { }
            try
            {
                delta_rad = Math.Max(delta_rad,
                    Math.Abs(newerIni.GetDouble("Calc0", "omc2", CultureInfo.InvariantCulture) - olderIni.GetDouble("Calc0", "omc2", CultureInfo.InvariantCulture)));
            }
            catch (Exception) { }

            return "delta max " + Math.Round(delta_rad, 3).ToString() + " rad";
        }

        public class HistoryEntry
        {
            public string Date { get; set; }
            public string Digest { get; set; }
        }

        public List<HistoryEntry> Report { get; set; }

        private async void Timer()
        {
            while (true)
            {
                Report = new List<HistoryEntry>();

                // Load calibration history (app should be placed in the history folder).
                History calibHistory = new History(_calibPaths);
                try
                {
                    //.IOException
                    await calibHistory.LoadAsync(AppDomain.CurrentDomain.BaseDirectory);

                    // Create diff history as datacontext.
                    Int32 idx = calibHistory.CalibHistory.Count;
                    if (idx > 1)
                    {
                        Run recent = calibHistory.CalibHistory[--idx];
                        while ((--idx) >= 0)
                        {
                            // Compare calib runs.
                            Run older = calibHistory.CalibHistory[idx];
                            bool olderHasSysCalib = File.Exists(_calibPaths.MatrixScreenToCamPath(older.ResultFolder(_calibPaths)));

                            if (olderHasSysCalib)
                            {
                                Report.Add(new HistoryEntry()
                                {
                                    Date = recent.Date.ToString("yyyy.MM.dd HH:mm"),
                                    Digest = Compare(recent, older)
                                });

                                recent = older;
                            }
                        }
                    }
                }
                catch (IOException) { }

                this.CallPropertyChanged(nameof(Report));

                await Task.Delay(4600);
            }
        }
    }
}
