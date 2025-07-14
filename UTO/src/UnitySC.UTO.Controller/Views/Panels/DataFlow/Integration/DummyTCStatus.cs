using UnitySC.Dataflow.UI.Shared;

namespace UnitySC.UTO.Controller.Views.Panels.DataFlow.Integration
{
    public class DummyTCStatus : ITcStatus
    {
        public bool SomeControlJobsAreExecuting => false;
    }
}
