using Agileo.Common.Tracing;
using Agileo.StateMachine;

namespace UnitySC.Equipment.Devices.Controller.OperatingModeSM
{
    internal partial class ControllerOperatingModeSm
    {
        private class InitRequested : Event
        {
        }

        private class InitCompleted : Event
        {
        }

        private class InitFailed : Event
        {
        }

        private class MaintenanceRequested : Event
        {
            public string Message { get; }
            public TraceLevelType Level { get; }

            public MaintenanceRequested(TraceLevelType level, string message)
            {
                Level = level;
                Message = message;
            }
        }

        private class JobExecutionStarted : Event
        {
        }

        private class JobExecutionEnded : Event
        {
        }

        private class EngineeringRequested : Event
        {
        }
    }
}
