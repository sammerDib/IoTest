namespace UnitySC.Equipment.Abstractions.Vendor.JobDefinition
{
    public enum JobStatus
    {
        Created,
        Queued,
        Executing,
        Completed,
        Failed,
        Pausing,
        Paused,
        Stopping,
        Stopped
    }
}
