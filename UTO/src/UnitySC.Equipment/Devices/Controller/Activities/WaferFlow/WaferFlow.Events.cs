using Agileo.StateMachine;

namespace UnitySC.Equipment.Devices.Controller.Activities.WaferFlow
{
    public partial class WaferFlow
    {
        private class AlignerSelected : Event
        {
        }

        private class PickLoadPortSelected : Event
        {
        }

        private class PlaceLoadPortSelected : Event
        {
        }

        private class ProcessModuleSelected : Event
        {
        }

        private class RobotDone : Event
        {
        }

        private class AlignerActivityDone : Event
        {
        }

        private class AlignerActivityStarted : Event
        {
        }

        private class PmDone : Event
        {
        }

        private class PmReadyToTransfer : Event
        {
        }

        private class PmNotReadyToTransfer : Event
        {
        }

        private class Reevaluate : Event
        {
        }

        private class PmInError : Event
        {
        }

        private class StopRequested : Event
        {
        }
    }
}
