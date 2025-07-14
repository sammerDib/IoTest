using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Threading;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Controller;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Controllers
{
    public class SMC100Controller : AxesControllerBase
    {
        private readonly SMC100ControllerConfig _controllerConfig;
        private SMC100Api _api;
        private SerialPort _newPortSerialPort;
        private Thread _backgroundThread;
        private readonly object _mutex = new object();
        private readonly Dictionary<EnumDriveAxisId, NewPortSubDrive> _drives;

        public SMC100Controller(ControllerConfig controllerConfig, IGlobalStatusServer globalStatusServer,
            ILogger logger) : base(controllerConfig, globalStatusServer, logger)
        {
            if (controllerConfig is SMC100ControllerConfig)
            {
                _controllerConfig = controllerConfig as SMC100ControllerConfig;
            }
            else
                throw (new Exception("Bad controller configuration type. Controller creation failed !"));

            _drives = new Dictionary<EnumDriveAxisId, NewPortSubDrive>();
        }

        public override void Init(List<Message> initErrors)
        {
            try
            {
                if (_newPortSerialPort != null)
                {
                    if (_newPortSerialPort.IsOpen)
                    {
                        _newPortSerialPort.Close();
                    }
                }

                State = new DeviceState(DeviceStatus.Starting, "Connecting to Newport Controller array...");
                Logger.Information("Opening connection to NewPort controller...");
                Connect();
                State = new DeviceState(DeviceStatus.Starting, "COM port connected, waiting for axes initialization");
            }
            catch (Exception e)
            {
                initErrors.Add(new Message(MessageLevel.Fatal, "Unable to open serial connection : " + e.Message));
                State = new DeviceState(DeviceStatus.Error, "Unable to open serial connection, check settings");
            }
        }

        public override void Connect()
        {
            _newPortSerialPort = new SerialPort(
                _controllerConfig.COMPort,
                _controllerConfig.BaudRate,
                (Parity)Enum.Parse(typeof(Parity), _controllerConfig.Parity),
                _controllerConfig.DataBits,
                (StopBits)Enum.Parse(typeof(StopBits), _controllerConfig.StopBits));
            _newPortSerialPort.NewLine = "\r\n";
            _newPortSerialPort.ReadTimeout = _controllerConfig.TimeOut;
            _newPortSerialPort.Open();
            _newPortSerialPort.WriteLine("");
            Thread.Sleep(100);
            _newPortSerialPort.DiscardInBuffer();
            _api = new SMC100Api(_newPortSerialPort, _controllerConfig.UpdateInterval, Logger);
            //Connection opened, we check if we are connected to a SMC100 by sending a get state command
            _api.SendSimpleCommand("1TS", true);
            Thread.Sleep(_controllerConfig.TimeOut);
            if (_newPortSerialPort.BytesToRead == 0)
            {
                throw new Exception("No controller connected or bad COM settings");
            }
        }

        public override void InitializationAllAxes(List<Message> initErrors)
        {
            try
            {
                if (_backgroundThread != null)
                {
                    if (_backgroundThread.IsAlive)
                    {
                        _backgroundThread.Abort();
                    }
                }

                _drives.Clear();
                foreach (NewPortSubDriveConfig config in _controllerConfig.SubDrivesList)
                {
                    _drives.Add(config.DriveAxisId, new NewPortSubDrive(config));
                }

                SimpleControllerState simpleControllerState = SimpleControllerState.READY;
                foreach (NewPortSubDrive drive in _drives.Values)
                {
                    State = new DeviceState(DeviceStatus.Starting,
                        "Initializing SubDrive : " + drive.Config.DriveAxisId.ToString());
                    SimpleControllerState midState = InitDrive(drive);
                    Logger.Information((drive.Config.DriveAxisId + " : " + midState));
                    if (midState != SimpleControllerState.READY)
                    {
                        simpleControllerState = midState;
                    }
                    CheckHardwareLimits(drive);
                }

                switch (simpleControllerState)
                {
                    case SimpleControllerState.NOT_REFERENCED:
                        State = new DeviceState(DeviceStatus.Warning, "Some devices are not homed");
                        break;

                    case SimpleControllerState.READY:
                        State = new DeviceState(DeviceStatus.Ready, "Ready");
                        break;

                    default:
                        State = new DeviceState(DeviceStatus.Warning, "Some devices are in unknown state");
                        break;
                }

                _backgroundThread = new Thread(GetControllerInfo_Threaded) { Name = "NewPort Controller Polling Thread" };
                _backgroundThread.Start();
            }
            catch (Exception ex)
            {
                State = new DeviceState(DeviceStatus.Unknown, "Unable to set up connection : " + ex.Message);
                Logger.Error("In Connect(), Unable to set up connection : " + ex.Message + " " + ex.StackTrace);
            }
        }

        private SimpleControllerState InitDrive(NewPortSubDrive drive)
        {
            SimpleControllerState tempState = 0;
            try
            {
                string result = "";
                if (!_api.SendQueryCommand(drive.Config.DriveAddress + SMC100Command.GetState.Code, ref result, true))
                {
                    throw new Exception("Unable to send command TS");
                }
                if (!_api.AnalyzeStateResponse(result, out tempState, out _))
                {
                    throw new Exception("Unable to parse result of command TS");
                }

                drive.DriveState = SMC100Api.GetPublicState(tempState);
            }
            catch (Exception ex)
            {
                Logger.Error("In Connect(), Unable to communicate with controller : " + ex.Message + " - Settings : " +
                              _controllerConfig.COMPort + " @ " + _controllerConfig.BaudRate + " Parity : " +
                              _controllerConfig.Parity + " Data bits : " + _controllerConfig.DataBits + " StopBits : " +
                              _controllerConfig.StopBits);
                return SimpleControllerState.UNKNOWN;
            }
            String CmdResult = "";
            if (_api.SendQueryCommand(drive.Config.DriveAddress + SMC100Command.LowerLimit.Code, ref CmdResult, true))
            {
                drive.LowerTravelLimit = double.Parse(CmdResult.Substring(3), CultureInfo.InvariantCulture);
            }
            else
            {
                throw new Exception("Unable to query SL");
            }
            if (_api.SendQueryCommand(drive.Config.DriveAddress + SMC100Command.UpperLimit.Code, ref CmdResult, true))
            {
                drive.UpperTravelLimit = double.Parse(CmdResult.Substring(3), CultureInfo.InvariantCulture);
            }
            else
            {
                throw new Exception("Unable to query SR");
            }
            return tempState;
        }

        private void CheckHardwareLimits(NewPortSubDrive drive)
        {
            IAxis driveAxis;
            try
            {
                driveAxis = GetAxeControlledById(drive.Config.DriveAxisId.ToString());
            }
            catch (Exception)
            {
                throw new Exception("Error in configuration, no axis corresponding to the drive with axisId : " +
                                    drive.Config.DriveAxisId);
            }

            if (Math.Abs(drive.LowerTravelLimit - driveAxis.AxisConfiguration.PositionMin.Millimeters) > 0.0001)
            {
                Logger.Warning("The lower travel limit set in the newport drive doesn't match the axis configuration. In Drive "
                    + drive.Config.DriveAxisId + " : " + drive.LowerTravelLimit + " != AxisConf : " + driveAxis.AxisConfiguration.PositionMin.Millimeters);
            }

            if (Math.Abs(drive.UpperTravelLimit - driveAxis.AxisConfiguration.PositionMax.Millimeters) > 0.0001)
            {
                Logger.Warning("The upper travel limit set in the newport drive doesn't match the axis configuration. In Drive "
                    + drive.Config.DriveAxisId + " : " + drive.UpperTravelLimit + " != AxisConf : " + driveAxis.AxisConfiguration.PositionMax.Millimeters);
            }
            //This function was supposed to use SL and SR commands to set the limits inside the newport drive software
            //We can only enter config mode when the drive is in Not Referenced State (after booting, before homing)
            //So we have to settle for checking before we send the command absolute or relative moves in SMC100.
            //AxesConfiguration has to be consistent with the drive limits if there are any configured
        }

        private void GetControllerInfo_Threaded()
        {
            //Run on a separate thread to poll drives
            while (true)
            {
                foreach (NewPortSubDriveConfig drive in _controllerConfig.SubDrivesList)
                {
                    _drives[drive.DriveAxisId].NewDiagArrived.Reset();
                    _drives[drive.DriveAxisId].BeforeDiag.Set();
                    NewPortStatusUpdate update = GetUpdateFromDrive(_drives[drive.DriveAxisId]);
                    UpdateInternalStatusFromDriveUpdate(_drives[drive.DriveAxisId], update);
                    _drives[drive.DriveAxisId].NewDiagArrived.Set();
                    _drives[drive.DriveAxisId].BeforeDiag.Reset();
                }

                Thread.Sleep(_controllerConfig.UpdateInterval);
            }
        }

        private void UpdateInternalStatusFromDriveUpdate(NewPortSubDrive drive, NewPortStatusUpdate update)
        {
            lock (_mutex)
            {
                drive.Position = update.Position;
                drive.Speed = update.Speed;
                drive.DriveState = SMC100Api.GetPublicState(update.State);
            }
        }

        private NewPortStatusUpdate GetUpdateFromDrive(NewPortSubDrive drive)
        {
            NewPortStatusUpdate update = new NewPortStatusUpdate();
            try
            {
                update.DriveFunction = drive.Config.DriveAxisId;
                string tsResponse = "";
                string paResponse = "";
                string vaResponse = "";

                // Get controller state (TS)
                if (_api.SendQueryCommand(drive.Config.DriveAddress + SMC100Command.GetState.Code, ref tsResponse, true))
                {
                    SimpleControllerState CState;
                    if (_api.AnalyzeStateResponse(tsResponse, out CState, out _))
                    {
                        update.State = CState;
                    }
                    else
                    {
                        update.State = SimpleControllerState.UNKNOWN;
                    }
                }
                else
                {
                    update.State = SimpleControllerState.UNKNOWN;
                }

                //Getting drive position (TP)
                if (_api.SendQueryCommand(drive.Config.DriveAddress + SMC100Command.GetPosition.Code, ref paResponse, true))
                {
                    update.Position = double.Parse(paResponse.Substring(3), CultureInfo.InvariantCulture);
                }
                else
                {
                    update.Position = 0;
                }

                //Getting drive speed (VA)
                if (_api.SendQueryCommand(drive.Config.DriveAddress + SMC100Command.SetGetSpeed.Code, ref vaResponse, true))
                {
                    update.Speed = double.Parse(vaResponse.Substring(3), CultureInfo.InvariantCulture);
                }
                else
                {
                    update.Speed = 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to get the state of drive :" + drive.Config.DriveAxisId, ex);
            }

            return update;
        }

        public override void StopAxesMotion()
        {
            foreach (var drive in _drives)
            {
                _api.GenericAbortMove(drive.Value);
            }
        }

        public override void RefreshCurrentPos(List<IAxis> axisList)
        {
            CheckAxesListIsValid(axisList);
            CheckControllerCommunication();

            try
            {
                foreach (var axis in axisList)
                {
                    if (axis.AxisConfiguration is PiezoAxisConfig)
                    {
                        if (Enum.TryParse(axis.AxisID, out EnumDriveAxisId matchingAxisId))
                        {
                            axis.CurrentPos = _drives[matchingAxisId].Position.Millimeters();
                        }
                    }
                    else
                        throw new Exception("AxisConfig is not a PiezoAxisConfig - check configuration");
                }
            }
            catch (Exception ex)
            {
                base.Logger?.Error("RefreshCurrentPos - Exception: " + ex.Message);
                throw;
            }
        }

        public override TimestampedPosition GetCurrentAxisPosWithTimestamp(IAxis axis)
        {
            CheckAxesListIsValid(new List<IAxis>() { axis });
            CheckControllerCommunication();

            try
            {
                if (axis.AxisConfiguration is PiezoAxisConfig)
                {
                    if (Enum.TryParse(axis.AxisID, out EnumDriveAxisId matchingAxisId))
                    {
                        var position = _drives[matchingAxisId].Position;
                        var highResolutionDateTime = StartTime.AddTicks(StopWatch.Elapsed.Ticks);
                        return new TimestampedPosition(position.Millimeters(), highResolutionDateTime);
                    }
                    throw new Exception("AxisID not matching");
                }
                else
                    throw new Exception("AxisConfig is not a PiezoAxisConfig - check configuration");
            }
            catch (Exception ex)
            {
                base.Logger?.Error("RefreshCurrentPos - Exception: " + ex.Message);
                throw;
            }
        }

        public override void SetPosAxis(List<double> coordsList, List<IAxis> axisList, List<AxisSpeed> speedsList)
        {
            for (int i = 0; i < axisList.Count; i++)
            {
                _api.GenericSensorAbsoluteMove(FindDriveByAxisId(axisList[i].AxisID), coordsList[i], false);
            }
        }

        private NewPortSubDrive FindDriveByAxisId(string axisId)
        {
            return _drives.SingleOrDefault(drive => drive.Key.ToString() == axisId).Value;
        }

        public override void RefreshAxisState(IAxis axis)
        {
            CheckAxesListIsValid(new List<IAxis>() { axis });
            CheckControllerCommunication();
            var ax = FindDriveByAxisId(axis.AxisID);
            axis.Moving = ax.DriveState == NewportControllerState.MOVING;
            axis.CurrentPos = ax.Position.Millimeters();
        }

        public override void MoveIncremental(IAxis axis, AxisSpeed speed, double step)
        {
            _api.GenericRelativeMove(FindDriveByAxisId(axis.AxisID), step, false);
        }

        public override void LinearMotionSingleAxis(IAxis axis, AxisSpeed speed, double pos)
        {
            SetPosAxis(
                new[] { pos }.ToList(),
                new[] { axis }.ToList(),
                new[] { speed }.ToList()
            );
        }

        public override void LinearMotionMultipleAxis(List<IAxis> axisList, AxisSpeed axisSpeed,
            List<double> posList)
        {
            SetPosAxis(posList, axisList, Enumerable.Repeat(axisSpeed, axisList.Count).ToList());
        }

        public override void WaitMotionEnd(int timeou, bool waitStabilization = true)
        {
            foreach (var drive in _drives.Values)
            {
                while (drive.DriveState == NewportControllerState.MOVING)
                {
                    Thread.Sleep(_controllerConfig.UpdateInterval);
                }
                if (drive.DriveState != NewportControllerState.READY)
                {
                    throw new Exception("Error with Newport drive " + drive.Config.DriveAxisId + " : " + drive.DriveState);
                }
            }
        }

        public override bool CheckAxisTypesInListAreValid(List<IAxis> axesList)
        {
            if (axesList.All(axis => (axis is PiezoAxis))) return true;
            const string errorMsg = "An axis type is not PiezoAxis";
            base.Logger?.Error(errorMsg);
            throw (new Exception(errorMsg));
        }

        public override void CheckControllerCommunication()
        {
            if (_newPortSerialPort != null)
            {
                if (_newPortSerialPort.IsOpen)
                {
                    return;
                }
            }

            const string errorMsg = "Serial port is null or closed";
            base.Logger?.Error(errorMsg);
            throw (new Exception(errorMsg));
        }

        public override void SetSpeedAxis(List<IAxis> axisList, List<AxisSpeed> speedsList)
        {
            for (int i = 0; i < axisList.Count; i++)
            {
                string axisId = axisList[i].AxisID;
                MotorController.ConvertSpeedEnum(axisList[i], speedsList[i], out double speed, out _);
                SetGenericSpeed(FindDriveByAxisId(axisId), speed);
            }
        }

        private bool SetGenericSpeed(NewPortSubDrive drive, double speed)
        {
            base.Logger?.Information(drive.Config.DriveAxisId + " : Setting speed @ " + speed + "mm/s");
            return _api.ComplexCommandResultCMP(drive.Config.DriveAddress + SMC100Command.SetGetSpeed.Code,
                speed.ToString(CultureInfo.InvariantCulture), true);
        }

        // NO NEED TO IMPLEMENT FOLLOWING METHODS

        public override void EnableAxis(List<IAxis> axisList)
        {
            throw new NotImplementedException();
        }

        public override void DisableAxis(List<IAxis> axisList)
        {
            throw new NotImplementedException();
        }

        public override void Connect(string deviceId)
        {
            throw new NotImplementedException();
        }

        public override void Disconnect()
        {
            throw new NotImplementedException();
        }

        public override void Disconnect(string deviceID)
        {
            throw new NotImplementedException();
        }

        public override void SetSpeedAccelAxis(List<IAxis> axisList, List<double> speedsList, List<double> accelsList)
        {
            throw new NotImplementedException();
        }

        public override void SetPosAxisWithSpeedAndAccel(List<double> coordsList, List<IAxis> axisList,
            List<double> speedsList, List<double> accelsList)
        {
            throw new NotImplementedException();
            //Speed list en double plutot que AxisSpeed ?
        }

        public override void CheckServiceSpeed(IAxis axis, ref double speed)
        {
            throw new NotImplementedException();
        }

        public override bool ResetController()
        {
            throw new NotImplementedException();
        }

        public override void Land()
        {
            throw new NotImplementedException();
        }

        public override void StopLanding()
        {
            throw new NotImplementedException();
        }

        public override bool IsLanded()
        {
            throw new NotImplementedException();
        }

        public override void InitControllerAxes(List<Message> initErrors)
        {
        }

        public override void InitZTopFocus()
        {
            //Nothing
        }

        public override void InitZBottomFocus()
        {
            //Nothing
        }
    }
}
