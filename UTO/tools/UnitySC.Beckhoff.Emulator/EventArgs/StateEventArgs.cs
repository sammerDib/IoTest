namespace UnitySC.Beckhoff.Emulator.EventArgs
{
    public class StateEventArgs : System.EventArgs
    {
        public States State { get; private set; }

        public StateEventArgs(States state)
        {
            State = state;
        }
    }
}
