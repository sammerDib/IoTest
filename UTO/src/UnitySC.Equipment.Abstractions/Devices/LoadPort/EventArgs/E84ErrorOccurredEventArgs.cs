using System;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort
{
    public class E84ErrorOccurredEventArgs : EventArgs
    {
        public E84ErrorOccurredEventArgs(E84Errors error)
        {
            Error = error;
        }

        public E84Errors Error { get; }
    }
}
