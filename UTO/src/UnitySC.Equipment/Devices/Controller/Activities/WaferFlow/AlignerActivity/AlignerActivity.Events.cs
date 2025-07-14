using Agileo.StateMachine;

namespace UnitySC.Equipment.Devices.Controller.Activities.WaferFlow.AlignerActivity
{
    public partial class AlignerActivity
    {
        private class AlignerDone : Event
        {
        }

        private class ReaderDone : Event
        {
        }

        private class ProceedWithSubstrate : Event
        {
        }

        private class CancelSubstrate : Event
        {
        }
    }
}
