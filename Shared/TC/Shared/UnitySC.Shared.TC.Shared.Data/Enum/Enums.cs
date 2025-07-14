namespace UnitySC.Shared.TC.Shared.Data
{
    public enum CheckReadyState
    { eReady, eFailedError, eRequestNeeded }

    #region Status variable name
    public enum SVName_DF
    {
        DataflowState
    }

    public enum SVName_ANA
    {
        State = 0,
        ReadyForProcess,
        RecipeActive,
        RecipeName,
        TransferState,
        MaterialPresenceState,
        PMProcessProgress_Percentage,
        TransferValidationState
    }
    public enum SVName_DMT
    {
        State = 0,
        ReadyForProcess,
        RecipeActive,
        RecipeName,
        TransferState,
        MaterialPresenceState,
        PMAcquisitionProgress_Percentage,
        PMComputationProgress_Percentage,
        PMAcquisitionProgress_SubstID,
        PMComputationProgress_SubstID,
        TransferValidationState
    }

    public enum SVName_EME
    {
        State = 0,
        ReadyForProcess,
        RecipeActive,
        RecipeName,
        TransferState,
        MaterialPresenceState,
        PMProcessProgress_Percentage,
        TransferValidationState
    }
    #endregion
}
