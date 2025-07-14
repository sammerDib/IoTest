namespace UnitySC.Shared.Tools.Communication
{
    public delegate void DataReceivedEventHandler();

    public interface ISerialPort
    {
        string PortName { get; }

        event DataReceivedEventHandler DataReceived;

        void Open();
        bool IsOpen { get; }
        void Close();
        string ReadExisting();
        void Dispose();
        void Write(string message);
        void Write(byte[] message);
    }
}
