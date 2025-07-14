namespace UnitySC.Rorze.Emulator.Common
{
	/// <summary>
	/// Summary description for Constants.
	/// </summary>
    internal class Constants
	{
        public const int MaxCommunicationLogSize = 1000;
        public const string DataLogFilePath = @"Log\";
        public const int TcpConnectionTimeout = 999999;

        public const int AlignerLocationId300mm = 7;
        public const int AlignerLocationId200mm = 107;
        public const int ProcessModule1LocationId = 5;
        public const int ProcessModule2LocationId = 93;
        public const int ProcessModule3LocationId = 96;

        public const int LoadPort1LocationId = 127;
        public const int LoadPort2LocationId = 147;
        public const int LoadPort3LocationId = 168;
        public const int LoadPort4LocationId = 188;

        #region Mapping

        #region 100mm

        public const int LoadPort1Mapping100LocationId1 = 31;
        public const int LoadPort1Mapping100LocationId2 = 36;

        public const int LoadPort2Mapping100LocationId1 = 32;
        public const int LoadPort2Mapping100LocationId2 = 37;

        public const int LoadPort3Mapping100LocationId1 = 33;
        public const int LoadPort3Mapping100LocationId2 = 38;

        public const int LoadPort4Mapping100LocationId1 = 34;
        public const int LoadPort4Mapping100LocationId2 = 39;

        #endregion

        #region 150mm

        public const int LoadPort1Mapping150LocationId1 = 21;
        public const int LoadPort1Mapping150LocationId2 = 26;

        public const int LoadPort2Mapping150LocationId1 = 22;
        public const int LoadPort2Mapping150LocationId2 = 27;

        public const int LoadPort3Mapping150LocationId1 = 23;
        public const int LoadPort3Mapping150LocationId2 = 28;

        public const int LoadPort4Mapping150LocationId1 = 24;
        public const int LoadPort4Mapping150LocationId2 = 29;

        #endregion

        #region 200mm

        public const int LoadPort1Mapping200LocationId1 = 11;
        public const int LoadPort1Mapping200LocationId2 = 16;

        public const int LoadPort2Mapping200LocationId1 = 12;
        public const int LoadPort2Mapping200LocationId2 = 17;

        public const int LoadPort3Mapping200LocationId1 = 13;
        public const int LoadPort3Mapping200LocationId2 = 18;

        public const int LoadPort4Mapping200LocationId1 = 14;
        public const int LoadPort4Mapping200LocationId2 = 19;

        #endregion

        #endregion

    }
}
