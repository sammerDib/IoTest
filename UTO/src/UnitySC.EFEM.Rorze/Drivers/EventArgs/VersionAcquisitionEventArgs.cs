namespace UnitySC.EFEM.Rorze.Drivers.EventArgs
{
    public class VersionAcquisitionEventArgs : System.EventArgs
    {
        public string Version { get; }

        public VersionAcquisitionEventArgs(string version)
        {
            Version = version;
        }
    }
}
