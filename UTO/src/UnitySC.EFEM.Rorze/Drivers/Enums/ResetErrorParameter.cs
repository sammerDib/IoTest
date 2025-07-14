using System.ComponentModel;

namespace UnitySC.EFEM.Rorze.Drivers.Enums
{
    public enum ResetErrorParameter
    {
        [Description("Resumes the operation from the paused condition. Retry if possible when a light error occurred.")]
        ResumeOrRetry = 0,

        [Description("Resets the error, and then stops the motion.")]
        ResetAndStop = 1
    }
}
