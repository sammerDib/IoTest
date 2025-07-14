using System.Collections.Generic;

namespace UnitySC.UTO.Controller.Remote.Constants
{
    internal static class DVs
    {
        public const string AlarmCode = nameof(AlarmCode);
        public const string AlarmID = nameof(AlarmID);
        public const string AlarmText = nameof(AlarmText);
        public const string AlarmExtendedText = nameof(AlarmExtendedText);
        public const string AlarmDescription = nameof(AlarmDescription);

        public const string ChamberId = nameof(ChamberId);
        public const string ChamberRecipeName = nameof(ChamberRecipeName);

        public const string OcrProfileName = nameof(OcrProfileName);
        public const string OcrFrontRecipeName = nameof(OcrFrontRecipeName);
        public const string OcrBackRecipeName = nameof(OcrBackRecipeName);
        public const string OcrUsed = nameof(OcrUsed);
        public const string OcrScribeAngle = nameof(OcrScribeAngle);

        #region Wafer measurement

        #region Global

        public const string PW_SlotID = nameof(PW_SlotID);
        public const string PW_LoadPortID = nameof(PW_LoadPortID);
        public const string PW_LotID = nameof(PW_LotID);
        public const string PW_ControlJobID = nameof(PW_ControlJobID);
        public const string PW_ProcessJobID = nameof(PW_ProcessJobID);
        public const string PW_StartTime = nameof(PW_StartTime);
        public const string PW_EndTime = nameof(PW_EndTime);
        public const string PW_CarrierID = nameof(PW_CarrierID);
        public const string PW_SubstrateID = nameof(PW_SubstrateID);
        public const string PW_AcquiredID = nameof(PW_AcquiredID);
        public const string PW_RecipeID = nameof(PW_RecipeID);

        #endregion Global

        #region Specific to Analyse PM

        public const string PW_GlobalWaferStatistics = nameof(PW_GlobalWaferStatistics);
        public const string PW_DiesStatistics = nameof(PW_DiesStatistics);
        public const string PW_PointMeasure = nameof(PW_PointMeasure);

        #endregion Specific to Analyse PM

        #endregion Wafer measurement

        #region E87

        public static List<string> Usages { get; } = new() { "TEST", "DUMMY", "PRODUCT", "FILLER" };

        #endregion
    }
}
