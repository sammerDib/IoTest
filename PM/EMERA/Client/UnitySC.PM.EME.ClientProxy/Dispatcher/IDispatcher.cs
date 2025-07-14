using System;

namespace UnitySC.PM.EME.Client.Proxy.Dispatcher
{
    public interface IDispatcher
    {
        void Invoke(Action action);
    }
}
