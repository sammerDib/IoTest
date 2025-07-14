using System;

using UnitySC.PM.EME.Client.Proxy.Dispatcher;

namespace UnitySC.PM.EME.Client.TestUtils.Dispatcher
{
    public class TestDispatcher : IDispatcher
    {
        public void Invoke(Action action)
        {
            action();
        }
    }
}
