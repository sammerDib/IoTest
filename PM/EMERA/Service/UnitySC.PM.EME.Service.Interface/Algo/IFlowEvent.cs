using System;

using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    public class FlowEventArgs : EventArgs
    {
        public FlowEventArgs(IFlowResult result)
        {
            FlowResult = result;
        }

        public IFlowResult FlowResult { get; set; }
    }

    public interface IFlowEvent
    {
        void AddFlowEventListener(EventHandler<FlowEventArgs> l);

        void RemoveFlowEventListener(EventHandler<FlowEventArgs> l);

        void OnRaiseFlowEvent(FlowEventArgs e);
    }
}
