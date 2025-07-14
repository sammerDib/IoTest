using System;

namespace UnitySC.Shared.Data.Enum
{
    /// <summary>
    /// Process module state extracted from TC
    /// </summary>
    public enum TC_PMState
    {
        Unknown = 0,          // PM state is not define yet
        Offline,            // PM is not available.
        Idle,               // PM is ready for processing.
        Active,             // PM is processing or transporting.
        Suspended,          // PM is suspended.
        Error,              // PM is in error mode.
        Initializing,       // PM is initializing.
        Shutting_Down,      // PM is shutting down.
        Pending_To_Active,  // PM state change from idle to active.
        Locked,             // PM is temporarily not available.
        PreConditioning,    // PM is processing a pre-conditioning recipe.
        PostConditioning    // PM is processing a post-conditioning recipe.
    }

    public enum TC_Transferstatus
    {
        Unknown = 0,        // Service not initialized - Default value
        Requested,
        Pending_EFEMMoveInPogress,            // Transfer material is in progress.
        Pending_EFEMMoveComplete,
        Complete,            // Transfer is complete with slitDoor closed if used
        Canceled
    }

    public enum TC_Transfer
    {
        None = 0,        // Service not initialized - Default value
        Pick,
        Place,            // Transfer material is in progress.
        PickPlace,
    }

    public enum TC_DataflowStatus
    {
        None = 0,        // Status not initialized - Default value
        Maintenance,
        Initializing,
        Idle,
        Executing,
    }

    /// <summary>
    /// Process module state communicated to Toolcommander (TC)
    /// </summary>
    public enum EnumPMTransferState
    {
        ReadyToLoad_SlitDoorClosed = 0,
        ReadyToLoad_SlitDoorOpened = 1,
        NotReady = 2,
        ReadyToUnload_SlitDoorClosed = 3,
        ReadyToUnload_SlitDoorOpened = 4
    }

    public enum MaterialPresence
    {
        Unknown = 0,
        Present = 1,
        NotPresent = 2
    }
    public enum MaterialClamp
    {
        Unknown = 0,
        Clamped = 1,
        NotClamped = 2
    }

    public enum EnableState
    {
        Disabled = 0,
        Enabled
    }

    public enum PMCommunicationMode
    {
        Client = 0,
        Server = 1
    }

    [Flags]
    public enum PMCommunicationState_AIS
    {
        Disabled = 0,
        Enabled = 1,
        Communicating = 2,
        WaitDelay = 4,
        WaitConnectionRequestAttempt = 8,
    }

    [Flags]
    public enum ECommunicationState
    {
        Disabled = 0,
        Enabled = 1,
        Communicating = 2,
        NotCommunicating = 4,
        CommunicationCheckingRequested = 8,
        CommunicationCheckingPending = 16,
    }

    public enum ToolMode
    {
        /* Comments describe the expected behavior on the NSTs.
        Note that currently, the 2238 is wired "in reverse," with its functions different. It will be rewired soon.
        */

        Unknown,
        Maintenance, // Blue button on the fixed machine. ServiceMode = 1
        Run, // Blue button on the machine turned off. ServiceMode = 0
        AcknowledgeAlarm, // Blue button on the machine blinking. AcknowledgeAlarm = 0
    }    
    
    public enum PMAccessMode
    {       
        Unknown,        // Default state
        Maintenance,    // State with Key in Maintenance Mode 
        Run,            // State with Key in run mode
    }

    public enum PMGlobalStates
    {
        NotInitialized,
        Initializing,
        Free,
        Error,
        ErrorHandling,
        Busy
    }

    public enum PMProcessingStates
    {
        NotReady,
        Idle,                   // Processing is ready to use.
        Processing             // Processing.
    }

    public enum PMControlMode
    {
        Unknown,
        Engineering, // PM used in engineering with client application by user
        Production,  // PM used in a Job by UTO
    }

    public enum PMControlModeSwitch
    {
        NoSwitch,
        SwitchToEngineering, // PM used in engineering with client application by user
        SwitchToProduction,  // PM used in a Job by UTO
    }
}
