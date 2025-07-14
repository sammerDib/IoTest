namespace UnitySC.EquipmentController.Simulator.Driver.EventArgs
{
    internal class FfuSpeedReceivedEventArgs : System.EventArgs
    {
        public FfuSpeedReceivedEventArgs(uint speedRpm)
        {
            SpeedRpm = speedRpm;
        }

        public uint SpeedRpm { get; }
    }
}
