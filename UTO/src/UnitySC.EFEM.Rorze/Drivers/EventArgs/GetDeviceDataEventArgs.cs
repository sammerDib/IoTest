namespace UnitySC.EFEM.Rorze.Drivers.EventArgs
{
    /// <summary>
    /// Contain data from equipment
    /// </summary>
    public class GetDeviceDataEventArgs : System.EventArgs
    {
        public GetDeviceDataEventArgs(string devicePart, string[] commandParameters, string[] data)
        {
            DevicePart        = devicePart;
            CommandParameters = commandParameters;
            Data              = data;
        }

        public string DevicePart { get; }

        public string[] CommandParameters { get; }

        public string[] Data { get; }
    }
}
