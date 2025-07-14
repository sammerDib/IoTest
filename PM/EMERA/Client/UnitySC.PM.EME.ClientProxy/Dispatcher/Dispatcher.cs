using System;
using System.Windows;

namespace UnitySC.PM.EME.Client.Proxy.Dispatcher
{
    public class Dispatcher : IDispatcher
    {
        public void Invoke(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }
    }
}
