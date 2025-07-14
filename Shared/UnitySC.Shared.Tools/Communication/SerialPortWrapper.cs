using System.IO.Ports;

namespace UnitySC.Shared.Tools.Communication
{
    /// <summary>
    /// Wrapper around SerialPort which makes it mockable in tests.
    /// </summary>
    public class SerialPortWrapper : ISerialPort
    {
        private readonly SerialPort _serialPort;

        public string PortName { get { return _serialPort.PortName; } }
        public bool IsOpen { get { return _serialPort.IsOpen; } }

        public event DataReceivedEventHandler DataReceived;

        public SerialPortWrapper(string portName, int baudRate)
        {
            _serialPort = new SerialPort(portName, baudRate) { DtrEnable = true };
            _serialPort.DataReceived += OnDataReceived;
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            DataReceived?.Invoke();
        }

        public void Open()
        {
            _serialPort.Open();
        }

        public void Close()
        {
            _serialPort.Close();
        }

        public string ReadExisting()
        {
            return _serialPort.ReadExisting();
        }

        public void Dispose()
        {
            _serialPort.Dispose();
        }

        public void Write(string message)
        {
            _serialPort.Write(message);
        }

        public void Write(byte[] message)
        {
            _serialPort.Write(message, 0, message.Length);
        }
    }
}
