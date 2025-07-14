using System;
using System.IO;

namespace UnitySC.Shared.FDC.Helpers
{
    static public class HelperFDCDriveInfo
    {
        private static readonly ulong s_inMo = (1024UL *1024UL);
        public static bool IsDriveValid(string driveLetter)
        {
            if (String.IsNullOrEmpty(driveLetter))
                return false;

            bool isValid = false;
            try
            {
                var drive = new DriveInfo(driveLetter);
                isValid = drive.IsReady;
            }
            catch (ArgumentNullException ) { } //The drive letter cannot be null.
            catch (ArgumentException ) { } //driveName does not refer to a valid drive.
            catch (Exception ex) { string msg = ex.Message; }
            return isValid;
        }

        // return in Mo if exist
        public static ulong? DiskUsage(string driveLetter)
        {
            ulong? diskUsage = null;
            if (!String.IsNullOrEmpty(driveLetter))
            {
                try
                {
                    var drive = new DriveInfo(driveLetter);
                    if (drive.IsReady)
                    {
                        diskUsage = (ulong)(drive.TotalSize - drive.TotalFreeSpace) / s_inMo;
                    }
                }
                catch (ArgumentNullException) { } //The drive letter cannot be null.
                catch (ArgumentException) { } //driveName does not refer to a valid drive.
                catch (Exception ex) { string msg = ex.Message; }
            }
            return diskUsage;
        }

        public static ulong? DiskFree(string driveLetter)
        {
            ulong? diskFree = null;
            if (!String.IsNullOrEmpty(driveLetter))
            {
                try
                {
                    var drive = new DriveInfo(driveLetter);
                    if (drive.IsReady)
                    {
                        diskFree = (ulong)(drive.TotalFreeSpace) / s_inMo;
                    }
                }
                catch (ArgumentNullException) { } //The drive letter cannot be null.
                catch (ArgumentException) { } //driveName does not refer to a valid drive.
                catch (Exception ex) { string msg = ex.Message; }
            }
            return diskFree;
        }

        public static double? DiskPercentFree(string driveLetter)
        {
            double? diskPercentFree = null;
            if (!String.IsNullOrEmpty(driveLetter))
            {
                try
                {
                    var drive = new DriveInfo(driveLetter);
                    if (drive.IsReady)
                    {
                        diskPercentFree = 100.0 * ((double)drive.TotalFreeSpace / (double)drive.TotalSize);
                    }
                }
                catch (ArgumentNullException) { } //The drive letter cannot be null.
                catch (ArgumentException) { } //driveName does not refer to a valid drive.
                catch (Exception ex) { string msg = ex.Message; }
            }
            return diskPercentFree;
        }
    }
}
