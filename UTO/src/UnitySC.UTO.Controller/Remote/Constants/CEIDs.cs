namespace UnitySC.UTO.Controller.Remote.Constants
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "False positive")]
    internal static class CEIDs
    {
        /// <summary>
        /// Process state model transitions events
        /// </summary>
        public class PSMEvents
        {
            public const string MaintenanceToInitialization = nameof(MaintenanceToInitialization);
            public const string InitializationToIdle = nameof(InitializationToIdle);
            public const string IdleToExecuting = nameof(IdleToExecuting);
            public const string ExecutingToIdle = nameof(ExecutingToIdle);
            public const string IdleToMaintenance = nameof(IdleToMaintenance);
            public const string ExecutingToMaintenance = nameof(ExecutingToMaintenance);
            public const string InitializationToMaintenance = nameof(InitializationToMaintenance);
        }

        public class CustomEvents
        {
            public const string CarrierPresenceSensorOn = nameof(CarrierPresenceSensorOn);
            public const string CarrierPlacementSensorOn = nameof(CarrierPlacementSensorOn);
            public const string CarrierIdReadStart = nameof(CarrierIdReadStart);
            public const string CarrierIdReadEnd = nameof(CarrierIdReadEnd);
            public const string WaferAlignerStart = nameof(WaferAlignerStart);
            public const string WaferAlignerEnd = nameof(WaferAlignerEnd);
            public const string ProcessModuleRecipeStarted = nameof(ProcessModuleRecipeStarted);
            public const string ProcessModuleRecipeCompleted = nameof(ProcessModuleRecipeCompleted);
            public const string OcrProfileCreated = nameof(OcrProfileCreated);
            public const string OcrProfileUpdated = nameof(OcrProfileUpdated);
            public const string OcrProfileDeleted = nameof(OcrProfileDeleted);
            public const string WaferMeasurementResults = nameof(WaferMeasurementResults);
            public const string CarrierUndocked = nameof(CarrierUndocked);
            public const string CarrierDocked = nameof(CarrierDocked);
            public const string SlotMapReadStart = nameof(SlotMapReadStart);
            public const string SlotMapReadEnd = nameof(SlotMapReadEnd);
            public const string ProcessModuleProcessStarted = nameof(ProcessModuleProcessStarted);
            public const string ProcessModuleProcessFinished = nameof(ProcessModuleProcessFinished);
            public const string ProcessModulePostProcessFinished = nameof(ProcessModulePostProcessFinished);
            public const string ProcessModuleProcessAborted = nameof(ProcessModuleProcessAborted);
            public const string ProcessModulePMError = nameof(ProcessModulePMError);
        }
    }
}
