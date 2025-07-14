using System;
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Text;

namespace UnitySC.PM.Shared.Hardware.AxesSpace
{
    public class HardwareCommander : IDisposable
    {
        private SerialPort _serialPort;

        private static ConcurrentQueue<string> s_queue = new ConcurrentQueue<string>();

        private readonly UnitySC.Shared.Logger.ILogger _logger;
        public SerialPort SerialPort { get => _serialPort; set => _serialPort = value; }

        public event EventHandler<string> OnMessageRecieved;

        public HardwareCommander(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits, Handshake handshake, UnitySC.Shared.Logger.ILogger logger)
        {
            try
            {
                _serialPort = new SerialPort()
                {
                    PortName = portName,
                    BaudRate = baudRate,
                    Parity = parity,
                    DataBits = dataBits,
                    StopBits = stopBits,
                    Handshake = handshake,
                    DtrEnable = true
                };

                _serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
                _serialPort.ErrorReceived += S_serialPort_ErrorReceived;

                _logger = logger;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error during create HardwareCommander instance: {0}", ex.Message), ex);
            }
        }

        ~HardwareCommander()
        {
            Dispose(false);
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            switch (e.EventType)
            {
                case SerialData.Chars:
                    {
                        if (!_serialPort.IsOpen) return;                    // if serial port is closed do nothing
                        int bytes = _serialPort.BytesToRead;                // get the number of bytes available in the port
                        byte[] buffer = new byte[bytes];                    // reserve place in memory to stock the available bytes
                        _serialPort.Read(buffer, 0, bytes);                 // Read data in the port and stock them in the buffer
                        string message = Encoding.ASCII.GetString(buffer);  // Convert Data to string

                        _logger.Information($"Port {_serialPort.PortName}: Message recieved: {message}");

                        if (OnMessageRecieved != null)
                        {
                            OnMessageRecieved.Invoke(this, message);
                        }
                        break;
                    }
                case SerialData.Eof:
                    break;
            }
        }

        private void S_serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            _logger.Error($"Error received {e.EventType}");
        }

        public void ShowPorts()
        {
            var portNames = SerialPort.GetPortNames();

            foreach (var port in portNames)
            {
                _logger.Information("Port {0} has been detected! ", port);
            }
        }

        /// <summary>
        /// Open serial Connection
        /// </summary>
        /// <returns>returns 0 if connection fails and 1 if it succeed</returns>
        public int OpenConnection()
        {
            try
            {
                int results = 0;
                _logger.Information("Openning Serial Port Connection");

                _serialPort.Open();

                if (_serialPort.IsOpen)
                {
                    _logger.Information("Port {0} is Open!", _serialPort.PortName);
                    results = 1;
                }
                else
                {
                    _logger.Information("Port {0} is Closed!", _serialPort.PortName);
                    results = 0;
                }
                return results;
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Error during Open Serial Port Connection: {0}", ex.Message), ex);
                return -1;
            }
        }

        public void SendCommand(string commandToApply)
        {
            try
            {
                //StartReading();
                _logger.Information(String.Format("{0} {1}", "Sending Command: ", commandToApply));

                _serialPort.WriteLine($"{commandToApply}\r\n");

                _logger.Information(String.Format("{0} {1}", "Command Sent: ", commandToApply));

                //Thread.Sleep(5000);
                //StopReading();
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Error during Sending Command: {0}", ex.Message), ex);
            }
        }

        /// <summary>
        /// Close serial Connection
        /// </summary>
        /// <returns>returns 0 if connection fails and 1 if it succeed</returns>
        public int CloseConnection()
        {
            try
            {
                int results = 0;
                _logger.Information("Closing Serial Port Connection");

                _serialPort.Close();

                if (_serialPort.IsOpen)
                {
                    _logger.Information("Port {0} is Open!", _serialPort.PortName);
                    results = 1;
                }
                else
                {
                    _logger.Information("Port {0} is Closed!", _serialPort.PortName);
                    results = 0;
                }
                return results;
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Error during Close Serial Port Connection: {0}", ex.Message), ex);
                return -1;
            }
        }

        public bool IsSerialPortOpen()
        {
            return _serialPort.IsOpen ? true : false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _logger.Information("Dispose HardwareCommander");
            if (disposing)
            {
                if (_serialPort != null)
                {
                    _serialPort.DataReceived -= SerialPort_DataReceived;
                    _serialPort.ErrorReceived -= S_serialPort_ErrorReceived;
                    _serialPort.Dispose();
                }
            }
        }
    }
}
