namespace UnitySC.PM.Shared.Referentials.Interface
{
    public interface IReferentialManager
    {
        /// <summary>
        /// Convert a position into a position in a new referential
        /// </summary>
        /// <param name="positionToConvert">Position to convert</param>
        /// <param name="referentialTo">Destination referential</param>
        /// <returns></returns>
        PositionBase ConvertTo(PositionBase positionToConvert, ReferentialTag referentialTo);

        /// <summary>
        /// Get referential settings used by converter
        /// </summary>
        ReferentialSettingsBase GetSettings(ReferentialTag referentialTag);

        /// <summary>
        /// Set referential settings used by converter
        /// </summary>
        void SetSettings(ReferentialSettingsBase settings);

        /// <summary>
        /// Delete referential settings used by converter
        /// </summary>
        void DeleteSettings(ReferentialTag referentialTag);

        /// <summary>
        /// Disable converter between two referentials
        /// </summary>
        /// <param name="referentialTag1"></param>
        /// <param name="referentialTag2"></param>
        void DisableReferentialConverter(ReferentialTag referentialTag1, ReferentialTag referentialTag2);

        /// <summary>
        /// Enable converter between two referentials
        /// </summary>
        /// <param name="referentialTag1"></param>
        /// <param name="referentialTag2"></param>
        void EnableReferentialConverter(ReferentialTag referentialTag1, ReferentialTag referentialTag2);
    }
}
