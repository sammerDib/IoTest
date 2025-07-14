namespace UnitySC.Equipment.Devices.Controller.Throughput
{
    public class ThroughputEventArgs : System.EventArgs
    {
        #region Properties

        public double Throughput { get; }

        #endregion

        #region Constructor

        public ThroughputEventArgs(double throughput)
        {
            Throughput = throughput;
        }

        #endregion
    }
}
