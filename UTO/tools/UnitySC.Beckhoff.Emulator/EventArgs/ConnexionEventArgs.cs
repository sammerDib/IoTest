namespace UnitySC.Beckhoff.Emulator.EventArgs
{
    public class ConnexionEventArgs : System.EventArgs
    {
        public bool Connected { get; private set; }

        public ConnexionEventArgs(bool connected)
        {
            Connected = connected;
        }
    }
}
