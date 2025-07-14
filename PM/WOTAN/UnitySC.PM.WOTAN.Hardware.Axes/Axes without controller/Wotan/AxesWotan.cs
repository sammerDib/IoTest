using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes.Enum;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.AxesSpace
{
    public class AxesWotan : IAxes
    {
        private const int ERROR_POSITION = -99999999;

        #region Fields

        private readonly ILogger _logger;
        private HardwareCommander _hardwareCommander;
        private ConcurrentQueue<string> _queue;
        private int _axesPosition;
        private int _chuckPosition;

#pragma warning disable CS0067 // Event not used. Not detected by the compiler

        public event StateChangedEventHandler OnStatusChanged;

        #endregion Fields

        #region Constructors

        public AxesWotan(string stageName, string stageID)
        {
            AxesLogger.CreateLogger(DeviceLogLevel.Debug.ToString(), Family.ToString(), Name);
            AxesLogger.Log.Information("** STAGEWOTAN STARTUP **");
            _logger = AxesLogger.Log;

            // We don't know the position of the stage and chuck at the beginning
            _axesPosition = ERROR_POSITION;
            _chuckPosition = ERROR_POSITION;

            Name = stageName;
            DeviceID = stageID;
        }

        #endregion Constructors

        #region Properties

        public AxesConfig AxesConfiguration { get; set; }
        public string Name { get; set; }
        public string DeviceID { get; set; }
        public DeviceState State { get; set; } = new DeviceState(DeviceStatus.Unknown);
        public DeviceFamily Family { get => DeviceFamily.Axes; }
        public ConcurrentQueue<string> Queue { get => _queue; set => _queue = value; }
        public int AxesPosition { get => _axesPosition; set => _axesPosition = value; }
        public int ChuckPosition { get => _chuckPosition; set => _chuckPosition = value; }

        public List<IAxis> Axes { get => throw new NotImplementedException(); }
        public List<IAxesController> AxesControllers { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion Properties

        #region Public methods

        public void Init(AxesConfig config, List<Message> initErrors)
        {
            _logger.Information(FormatMessage("Wotan Stage Init Called"));

            // Create a new SerialPort Connection.
            //_hardwareCommander = new HardwareCommander(config.PortName, config.BaudRate, config.Parity, config.DataBits, config.StopBits, config.Handshake, _logger);
            _hardwareCommander.OnMessageRecieved += _hardwareCommander_OnMessageRecieved;
            _queue = new ConcurrentQueue<string>();
        }

        private void _hardwareCommander_OnMessageRecieved(object sender, string e)
        {
            // add recieved messages
            try
            {
                // Add message to queue
                _queue.Enqueue(e);

                //Analyze if the message can give the stage position
                string pattern = @".*pa event getpos 0x1 a0\s";
                _axesPosition = GetValueFromPattern(pattern, e);

                //Analyze if the message can give the chuck position
                pattern = @".*pa event getpos 0x1 a1\s";
                _chuckPosition = GetValueFromPattern(pattern, e);
            }
            catch (Exception ex)
            {
                _logger.Error("Error on enqueuing a message recieved, exception description: {0}", ex.Message);
            }
        }

        public void SendCommandToAxes(string commandToApply)
        {
            _logger.Information("Sending Command to WOTAN Stage");
            if (!_hardwareCommander.IsSerialPortOpen())
            {
                _hardwareCommander.OpenConnection();
            }
            _hardwareCommander.SendCommand(commandToApply);
        }

        public void DisconnectAxes()
        {
            _logger.Information("Disconnect WOTAN Stage");
            _hardwareCommander.CloseConnection();
        }

        public List<string> GetReceivedMessages()
        {
            try
            {
                string message;
                List<string> returnedMessages = new List<string>();
                while (_queue.TryDequeue(out message))
                {
                    returnedMessages.Add(message);
                }
                return returnedMessages;
            }
            catch (Exception ex)
            {
                _logger.Error("Error on dequeuing a message recieved, exception description: {0}", ex.Message);
                return null;
            }
        }

        public int GetAxesPosition()
        {
            return _axesPosition;
        }

        public int GetChuckPosition()
        {
            return _chuckPosition;
        }

        #endregion Public methods

        #region Utilities

        private string FormatMessage(string message)
        {
            return $"[Stage wotan] {message}";
        }

        private int GetValueFromPattern(string pattern, string message)
        {
            var myRegex = new Regex(pattern, RegexOptions.IgnoreCase);
            if (myRegex.IsMatch(message))
            {
                string[] splitResults = Regex.Split(message, pattern);
                return int.Parse(splitResults[1]);
            }

            return ERROR_POSITION;
        }

        #endregion Utilities

        #region Not Implemented methods

        public void CheckAxisLimits(IAxis axis, double wantedPosition)
        {
            throw new NotImplementedException();
        }

        private void LinkAxisMembers()
        {
            throw new NotImplementedException();
        }

        public void GotoPointCustomSpeedAccel(AxisMove moveX, AxisMove moveY, AxisMove moveZTop, AxisMove moveZBottom)
        {
            throw new NotImplementedException();
        }

        public void GotoManualLoadPos(AxisSpeed speed)
        {
            throw new NotImplementedException();
        }

        public void GotoParkPos(AxisSpeed speed)
        {
            throw new NotImplementedException();
        }

        public void GotoHomePos(AxisSpeed speed)
        {
            throw new NotImplementedException();
        }

        public void GotoPosition(PositionBase position, AxisSpeed speed)
        {
            throw new NotImplementedException();
        }

        public void MoveIncremental(IncrementalMoveBase move, AxisSpeed speed)
        {
            throw new NotImplementedException();
        }

        public void StopAllMoves()
        {
            throw new NotImplementedException();
        }

        public void WaitMotionEnd(int timeout)
        {
            throw new NotImplementedException();
        }

        private void ReportStateTask()
        {
            throw new NotImplementedException();
        }

        private void MoveTask()
        {
            throw new NotImplementedException();
        }

        public PositionBase GetPos()
        {
            throw new NotImplementedException();
        }

        public AxesState GetState()
        {
            throw new NotImplementedException();
        }

        public void StopLanding()
        {
            throw new NotImplementedException();
        }

        public void Land()
        {
            throw new NotImplementedException();
        }

        public void ReleaseWafer(List<string> valvesToUse)
        {
            throw new NotImplementedException();
        }

        public void Init(List<Message> initErrors)
        {
            throw new NotImplementedException();
        }

        public void LinearMotion(PositionBase position, AxisSpeed speed)
        {
            throw new NotImplementedException();
        }

        public bool IsAtPosition(PredefinedPosition position)
        {
            throw new NotImplementedException();
        }

        public PositionBase ConvertTo(PositionBase positionToConvert, ReferentialTag referentialTo)
        {
            throw new NotImplementedException();
        }

        #endregion Not Implemented methods
    }
}
