namespace UnitySC.Shared.Tools.Service
{
    // ErrorID => Alarm.Name
    public enum ErrorID
    {
        // common
        Undefined,
        Unknown,
        SmokeDetected,         // Triggered on smoke detection in PM, used to reset EMO action
        MaterialTransferError, // Triggered after UTO material transfer fail in PM 
        BadMaterialTypeTransferError, // Triggered after UTO material transfer request fail in PM for bad material type
        InitializationError, // Triggered after UTO call on a PM initialization fail                   
        SubstrateDimensionIdentificationError, // Triggered after substrate size were not correctly identified (Chuck RFID + Configuration definition)
        PanelInterlockedError, // Triggered after a PM panels 
        MaintenanceStateError, // Triggered after a maintenance state change

        StageError,
        SlitDoorError,        

        RecipeStartingError_PMRequestStart,
        RecipeStartingError_PMStart,
        RecipeExecutionError_PMError,
        RecipeExecutionCanceled,
        RecipeAbortingFailed,
        RecipeAborted,

        //Post-Process
        RecipePostProcess_UnidentifiedMaterialError,
        RecipePostProcess_ADCProcessError,

        //Dataflow
        RecipeStartingError_StartDFRecipe,
        RecipeStartingError_LoadDFRecipe,
        RecipeStartingError_StartJob,
        RecipeStartingError_PrepareDFRecipe,

        RecipeExecutingError_DFUpdateStatus,
        RecipeExecutionError_DFErrorOnRecipeStarted,

        InvalidPMRecipeStartingRequested,
        InvalidPMRecipeCompleteError,
        PMRecipeStartingRequestFailed,

        InvalidPPRecipeStartedError,
        InvalidPPRecipeCompleteError,

        GetAllRecipesfromDBError,
        UpdateDFStatusError,
        UpdateDFRecipeStatusError,
        UpdateActorStatusError,
        GetDataflowRecipeError,
        GetPMRecipeError,
        GetRecipesToAbortError,
        GetADCRecipeForSideError,

        IonizerError,   
        MaterialIdentificationError,
        FFUWarningError,
        FFUMajorError
    }
}
