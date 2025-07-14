using Agileo.StateMachine;

namespace UnitySC.Equipment.Devices.Controller.Activities
{
    internal partial class LoadToPm
    {
        private class AlignerDone : Event
        {
        }

        private class RobotDone : Event
        {
        }

        private class PMDone : Event
        {
        }

        private class ReadyToTransfer : Event
        {
        }

        private class NotReadyToTransfer : Event
        {
        }
    }
}
