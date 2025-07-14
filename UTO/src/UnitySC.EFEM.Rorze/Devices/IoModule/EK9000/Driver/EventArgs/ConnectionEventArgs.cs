namespace UnitySC.EFEM.Rorze.Devices.IoModule.EK9000.Driver.EventArgs
{
    public class ConnectionEventArgs : System.EventArgs
    {
        public bool Connected { get; }

        public ConnectionEventArgs(bool connected)
        {
            Connected = connected;
        }
    }
}
