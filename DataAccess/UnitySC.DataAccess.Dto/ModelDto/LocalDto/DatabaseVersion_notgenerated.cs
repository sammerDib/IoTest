using System;

namespace UnitySC.DataAccess.Dto
{
    public partial class DatabaseVersion
    {
        public static readonly string CurrentVersion = "8.0.0";
        public static readonly DateTime CurrentDateVersion = new DateTime(2023, 06, 19, 12, 30, 00);

        public bool IsUptoDate()
        {
            return CurrentVersion == Version;
        }

        public bool IsNewer()
        {
            return CurrentDateVersion < Date;
        }

        public bool IsOlder()
        {
            return CurrentDateVersion > Date;
        }

    }
}
