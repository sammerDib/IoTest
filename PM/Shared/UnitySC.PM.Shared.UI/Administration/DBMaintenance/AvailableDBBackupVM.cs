using System;

namespace UnitySC.PM.Shared.UI.Administration.DBMaintenance
{
    public class AvailableDBBackupVM
    {
        public string DBName { get; set; }
        public DateTime Date { get; set; }

        public string FullPath { get; set; }
    }
}
