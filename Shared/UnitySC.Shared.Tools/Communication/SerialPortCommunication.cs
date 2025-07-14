using System;
using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using UnitySC.Shared.Logger;

namespace UnitySC.Shared.Tools.Communication
{
    public class SerialPortCommunication : IDisposable
    {
        private readonly ILogger _logger;

        private readonly ISerialPort _serialPort;

        private ConcurrentQueue<string> _dataBuffer;

        private static readonly object s_commLock = new object();

        public TimeSpan ResponseTimeout = TimeSpan.FromSeconds(100);

        public Regex ConsistentResponsePattern { get; set; }
        public Regex ErrorResponsePattern { get; set; }
        public Regex IgnoredResponsePattern { get; set; }

        /// <summary>
        /// Serial port communication, handling sequential requests or queries (one at a time).
        /// </summary>
        public SerialPortCommunication(string portName, int baudRate, ISerialPort serialPort = null)
        {
            _logger = ClassLocator.Default.GetInstance<ILogger>();

            _serialPort = serialPort ?? new SerialPortWrapper(portName, baudRate);

            _dataBuffer = new ConcurrentQueue<string>();
            _serialPort.DataReceived += OnDataReceived;
        }

        public void Connect()
        {
            try
            {
                lock (s_commLock)
                {
                    _serialPort.Open();
                }
            }
            catch (UnauthorizedAccessException e)
            {
                string errorMessage = $"SerialPort {_serialPort.PortName} is currently used by another process. Please close any program using it and try again.";
                throw new UnauthorizedAccessException(errorMessage, e);
            }
        }

        public void Disconnect()
        {
            lock (s_commLock)
            {
                _serialPort.Close();
            }
        }

        private string _previousData = "";

        private void OnDataReceived()
        {
            string data = _previousData + _serialPort.ReadExisting();

            if (IsAConsistentResponse(data))
            {
                string[] responses = data.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string response in responses)
                {
                    _dataBuffer.Enqueue(response + "\r\n");
                }
                _previousData = "";
            }
            else
            {
                _previousData = data;
            }
        }

        public void Dispose()
        {
            lock (s_commLock)
            {
                if (_serialPort is null)
                {
                    return;
                }
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
                _serialPort.Dispose();
            }
        }

        /// <summary>
        /// This method returns true if ConsistentResponsePattern is not defined (null) or the
        /// data given in parameter satisfies the consistent response pattern, false otherwise.
        /// </summary>
        private bool IsAConsistentResponse(string response)
        {
            return ConsistentResponsePattern?.IsMatch(response) ?? true;
        }

        /// <summary>
        /// Fires a query through the serial port and returns the answer:<br/>
        ///     - the query is defined with the message parameter (as raw text to send to the controller),<br/>
        ///     - the returned answer matches the first capturing group specified in the query (ResponsePattern property).
        /// </summary>
        public string Query(SerialPortQuery<byte[]> query)
        {
            return Query(new SerialPortQuery<string>
            {
                Message = Encoding.UTF8.GetString(query.Message),
                ResponsePattern = query.ResponsePattern
            });
        }

        // TODO: refactor with Query(SerialPortQuery<byte[]> query)
        /// <summary>
        /// Fires a query through the serial port and returns the answer:<br/>
        ///     - the query is defined with the message parameter (as raw text to send to the controller),<br/>
        ///     - the returned answer matches the first capturing group specified in the query (ResponsePattern property).
        /// </summary>
        public string Query(SerialPortQuery<string> query)
        {
            lock (s_commLock)
            {
                _dataBuffer = new ConcurrentQueue<string>();
                _serialPort.Write(query.Message);

                string receivedData = "";
                do
                {
                    receivedData = GetConsistentReceivedData();
                }
                while (IsIgnored(receivedData));

                if (IsError(receivedData))
                {
                    throw new Exception("Controller sends an error response.");
                }

                return ExtractResponse(receivedData, query.ResponsePattern);
            }
        }

        private string GetConsistentReceivedData()
        {
            string response = "";
            bool hasStoppedWithinTimeout = SpinWait.SpinUntil(() =>
            {
                if (_dataBuffer.TryDequeue(out string dataReceived))
                {
                    response = dataReceived;
                    return true;
                }

                return false;
            }, ResponseTimeout);

            if (!hasStoppedWithinTimeout) throw new TimeoutException($"No consistent message received within {ResponseTimeout.TotalMilliseconds} ms. Please check requested command is well formed.");

            return response;
        }

        /// <summary>
        /// Extract the specified group pattern from the string data given in parameter.
        /// The pattern should define exactly one capturing group.
        /// </summary>
        private string ExtractResponse(string data, Regex pattern)
        {
            // Explanation here : https://docs.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regex.getgroupnames?view=netframework-4.7.1#remarks
            if (pattern.GetGroupNames().Length != 2)
            {
                throw new Exception("Pattern should define only one capturing group.");
            }

            string response = pattern.Match(data).Groups[1].ToString();
            if (!response.HasContent())
            {
                throw new Exception($"Controller sends an unknown response : {data}");
            }

            return response;
        }

        /// <summary>
        /// This method returns true if the data given in parameter satisfies the ErrorResponsePattern,
        /// false otherwise or if ErrorResponsePattern is not defined (null).
        /// </summary>
        private bool IsError(string input)
        {
            return ErrorResponsePattern?.IsMatch(input) ?? false;
        }

        private bool IsIgnored(string input)
        {
            bool ignored = IgnoredResponsePattern?.IsMatch(input) ?? false;
            if (ignored)
            {
                _logger.Debug($"Input ignored: '{input}'");
            }
            return ignored;
        }

        /// <summary>
        /// Sends the specified command to the controller as raw text. You can optionnaly verifies with  the AcknowlegedResponsePattern property
        /// (of the command parameter) that the controller validated the command, i.e. an acknowleged response is returned back.
        /// </summary>
        public void Command(SerialPortCommand<string> command)
        {
            try
            {
                lock (s_commLock)
                {
                    _serialPort.Write(command.Message);

                    if (!(command.AcknowlegedResponsePattern is null))
                    {
                        string response = GetConsistentReceivedData();
                        if (IsNotAckResponse(response, command.AcknowlegedResponsePattern)) throw new Exception($"Returned response '{response}' does not match with ack pattern '{command.AcknowlegedResponsePattern}'.");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Command '{command.Message}' failed", e);
            }
        }

        private bool IsNotAckResponse(string response, Regex acknowlegedResponsePattern)
        {
            return !acknowlegedResponsePattern?.IsMatch(response) ?? false;
        }
    }
}
