using System;
using System.Globalization;
using System.IO.Ports;
using System.Threading;

using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Controllers
{
    public class SMC100Api
    {
        public SMC100Api(SerialPort serialPort, int updateInterval, ILogger logger)
        {
            _newPortSerialPort = serialPort;
            _updateInterval = updateInterval;
            _logger = logger;
        }

        private readonly SerialPort _newPortSerialPort;
        private const int INTERCMD_DELAY = 30; //ms
        private readonly ILogger _logger;
        private readonly int _updateInterval;

        public bool SendSimpleCommand(string command, bool waitBeforeRelease)
        {
            try
            {
                _newPortSerialPort.DiscardInBuffer();
                _newPortSerialPort.WriteLine(command);
                if (waitBeforeRelease)
                {
                    Thread.Sleep(INTERCMD_DELAY);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("In SendSimpleCommand(" + command + "," + waitBeforeRelease + ") : " + ex.Message);
                return false;
            }
        }

        public bool LockedSendSimpleCommand(string command, bool waitBeforeRelease)
        {
            bool result;
            lock (_newPortSerialPort)
            {
                result = SendSimpleCommand(command, waitBeforeRelease);
            }
            return result;
        }

        public bool SendComplexCommand(string command, string commandForResult, bool waitBeforeRelease, ref string response)
        {
            lock (_newPortSerialPort)
            {
                if (SendSimpleCommand(command, true))
                {
                    if (SendSimpleCommand(commandForResult, waitBeforeRelease))
                    {
                        try
                        {
                            response = _newPortSerialPort.ReadLine().TrimStart(new char[] { '\0' });
                            return true;
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("In SendComplexCommand(" + command + "," + commandForResult + "," + waitBeforeRelease + ",Ref String), during reading ResponseCommand Result : " + ex.Message);
                            return false;
                        }
                    }
                    else
                    {
                        _logger.Error("In SendComplexCommand(" + command + "," + commandForResult + "," + waitBeforeRelease + ",Ref String), during send of ResponseCommand ");
                        return false;
                    }
                }
                else
                {
                    _logger.Error("In SendComplexCommand(" + command + "," + commandForResult + "," + waitBeforeRelease + ",Ref String), during send of Command ");
                    return false;
                }
            }
        }

        public bool SendQueryCommand(string query, ref string response, bool waitBeforeRelease)
        {
            bool state;
            lock (_newPortSerialPort)
            {
                if (SendSimpleCommand(query + "?", false))
                {
                    try
                    {
                        response = _newPortSerialPort.ReadLine().TrimStart(new char[] { '\0' });
                        state = true;
                    }
                    catch (Exception Ex)
                    {
                        _logger.Error("In SendQueryCommand(" + query + ",ref string," + waitBeforeRelease + ") : " + Ex.Message);
                        state = false;
                    }
                }
                else
                {
                    _logger.Error("In SendQueryCommand(" + query + ",ref string," + waitBeforeRelease + ") : during send of command");
                    state = false;
                }
            }
            return state;
        }

        private static SimpleControllerState AnalyzeControllerState(byte value)
        {
            switch (value)
            {
                case 0x0A:
                    return SimpleControllerState.NOT_REFERENCED;

                case 0x0B:
                    return SimpleControllerState.NOT_REFERENCED;

                case 0x0C:
                    return SimpleControllerState.NOT_REFERENCED;

                case 0x0D:
                    return SimpleControllerState.NOT_REFERENCED;

                case 0x0E:
                    return SimpleControllerState.NOT_REFERENCED;

                case 0x0F:
                    return SimpleControllerState.NOT_REFERENCED;

                case 0x10:
                    return SimpleControllerState.NOT_REFERENCED;

                case 0x11:
                    return SimpleControllerState.NOT_REFERENCED;

                case 0x14:
                    return SimpleControllerState.CONFIGURATION;

                case 0x1E:
                    return SimpleControllerState.HOMING;

                case 0x1F:
                    return SimpleControllerState.HOMING;

                case 0x28:
                    return SimpleControllerState.MOVING;

                case 0x32:
                    return SimpleControllerState.READY;

                case 0x33:
                    return SimpleControllerState.READY;

                case 0x34:
                    return SimpleControllerState.READY;

                case 0x35:
                    return SimpleControllerState.READY;

                case 0x3C:
                    return SimpleControllerState.DISABLE;

                case 0x3D:
                    return SimpleControllerState.DISABLE;

                case 0x3E:
                    return SimpleControllerState.DISABLE;

                case 0x46:
                    return SimpleControllerState.JOGGING;

                case 0x47:
                    return SimpleControllerState.JOGGING;

                default:
                    return SimpleControllerState.UNKNOWN;
            }
        }

        private static FullPositionnerState AnalyzePositionerError(short value)
        {
            return (FullPositionnerState)value;
        }

        public static NewportControllerState GetPublicState(SimpleControllerState state)
        {
            switch (state)
            {
                case SimpleControllerState.CONFIGURATION:
                    return NewportControllerState.UNKNOWN;

                case SimpleControllerState.DISABLE:
                    return NewportControllerState.UNKNOWN;

                case SimpleControllerState.HOMING:
                    return NewportControllerState.HOMING;

                case SimpleControllerState.JOGGING:
                    return NewportControllerState.MOVING;

                case SimpleControllerState.MOVING:
                    return NewportControllerState.MOVING;

                case SimpleControllerState.NOT_REFERENCED:
                    return NewportControllerState.ENABLED;

                case SimpleControllerState.READY:
                    return NewportControllerState.READY;

                case SimpleControllerState.UNKNOWN:
                    return NewportControllerState.UNKNOWN;

                default:
                    return NewportControllerState.UNKNOWN;
            }
        }

        public bool AnalyzeStateResponse(string tsCommandResult, out SimpleControllerState controllerState, out FullPositionnerState positionerState)
        {
            //check if the input string is actually a TS command result, ex : 1TS00000A
            if (tsCommandResult.Substring(1, 2) == SMC100Command.GetState.Code)
            {
                tsCommandResult = tsCommandResult.Substring(3);
                byte controllerStateByte = byte.Parse(tsCommandResult.Substring(4), NumberStyles.HexNumber);
                short positionerErrorStateShort = short.Parse(tsCommandResult.Substring(0, 4));
                if (tsCommandResult.Length == 6)
                {
                    try
                    {
                        controllerState = AnalyzeControllerState(controllerStateByte);
                        positionerState = AnalyzePositionerError(positionerErrorStateShort);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("In AnalyzeStateResponse(" + tsCommandResult + ",out SimpleControllerState,out FullPositionnerState) : " + ex.Message);
                        controllerState = SimpleControllerState.UNKNOWN;
                        positionerState = FullPositionnerState.None;
                        return false;
                    }
                }
                else
                {
                    controllerState = SimpleControllerState.UNKNOWN;
                    positionerState = FullPositionnerState.None;
                    return false;
                }
            }
            else
            {
                _logger.Error("In AnalyzeStateResponse(" + tsCommandResult + ",out SimpleControllerState,out FullPositionnerState) : Not a TS command result");
                controllerState = SimpleControllerState.UNKNOWN;
                positionerState = FullPositionnerState.None;
                return false;
            }
        }

        public bool GenericRelativeMove(NewPortSubDrive drive, double position, bool waitEndOfMove)
        {
            position = Math.Round(position, 4);
            if (drive.Position + position < drive.LowerTravelLimit || drive.Position + position > drive.UpperTravelLimit)
            {
                _logger.Error(drive.Config.DriveAxisId + " : Requested Relative move outside system limits");
                return false;
            }
            _logger.Information(drive.Config.DriveAxisId + " : Performing Relative move of " + position);
            if (ComplexCommandResultCMP(drive.Config.DriveAddress + SMC100Command.MoveRelative.Code, position.ToString(CultureInfo.InvariantCulture), true))
            {
                if (waitEndOfMove)
                {
                    _logger.Information(drive.Config.DriveAxisId + " : Waiting Relative move End Of Move...");
                    Thread.Sleep(2 * _updateInterval);
                    drive.NewDiagArrived.WaitOne();
                    drive.BeforeDiag.WaitOne();
                    drive.NewDiagArrived.WaitOne();

                    while (drive.DriveState == NewportControllerState.MOVING)
                    {
                        Thread.Sleep(_updateInterval);
                    }
                    if (drive.DriveState == NewportControllerState.READY)
                    {
                        _logger.Information(drive.Config.DriveAxisId + " : Relative move done");
                        return true;
                    }
                    else
                    {
                        _logger.Error(drive.Config.DriveAxisId + " :  Inconsistent Relative move status : " + drive.DriveState);
                        return false;
                    }
                }
                _logger.Information(drive.Config.DriveAxisId + " : Relative move done");
                return true;
            }
            return false;
        }

        public bool GenericSensorAbsoluteMove(NewPortSubDrive drive, double position, bool waitEndOfMove)
        {
            position = Math.Round(position, 4);
            if (position < drive.LowerTravelLimit || position > drive.UpperTravelLimit)
            {
                _logger.Error(drive.Config.DriveAxisId + " : Requested Absolute move outside system limits");
                return false;
            }

            _logger.Information(drive.Config.DriveAxisId + " : Performing Absolute move to " + position);
            if (ComplexCommandResultCMP(drive.Config.DriveAddress + SMC100Command.MoveAbsolute.Code,
                position.ToString(CultureInfo.InvariantCulture), true))
            {
                if (waitEndOfMove)
                {
                    _logger.Information(drive.Config.DriveAxisId + " : Waiting Absolute move End Of Move...");
                    Thread.Sleep(2 * _updateInterval);
                    drive.NewDiagArrived.WaitOne();
                    drive.BeforeDiag.WaitOne();
                    drive.NewDiagArrived.WaitOne();
                    while (drive.DriveState == NewportControllerState.MOVING)
                    {
                        Thread.Sleep(_updateInterval);
                    }

                    if (drive.DriveState == NewportControllerState.READY ||
                        drive.DriveState == NewportControllerState.ENABLED)
                    {
                        _logger.Information(drive.Config.DriveAxisId + " : Absolute move done");
                        return true;
                    }
                    else
                    {
                        _logger.Error(drive.Config.DriveAxisId + " :  Inconsistent Absolute move status : " + drive.DriveState);
                        return false;
                    }
                }

                _logger.Information(drive.Config.DriveAxisId + " : Absolute move done");
                return true;
            }

            _logger.Error(drive.Config.DriveAxisId + " :  Error while sending command");
            return false;
        }

        public bool GenericAbortMove(NewPortSubDrive drive)
        {
            return LockedSendSimpleCommand(drive.Config.DriveAddress + SMC100Command.StopMotion.Code, true);
        }

        public bool ComplexCommandResultCMP(string commandBase, string commandValue, bool waitBeforeRelease)
        {
            string complexCommandResult = "";
            if (SendComplexCommand(commandBase + commandValue, commandBase + "?", waitBeforeRelease, ref complexCommandResult))
            {
                // We split the result to get only the value
                string resultValueString = complexCommandResult.Substring(commandBase.Length);
                double resultValueDouble = double.Parse(resultValueString, CultureInfo.InvariantCulture);
                if (Math.Abs(resultValueDouble - Double.Parse(commandValue, CultureInfo.InvariantCulture)) < 0.0001)
                {
                    return true;
                }
                else
                {
                    if (resultValueDouble == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        public bool HomeGenericDrive(NewPortSubDrive drive, bool waitEndOfMove)
        {
            bool result = false;
            if (LockedSendSimpleCommand(drive.Config.DriveAddress + SMC100Command.Home.Code, true))
            {
                _logger.Information(drive.Config.DriveAxisId + " : Homing...");
                string queryResult = "";
                if (SendQueryCommand(drive.Config.DriveAddress + SMC100Command.GetState.Code, ref queryResult, true))
                {
                    SimpleControllerState controllerState;
                    if (AnalyzeStateResponse(queryResult, out controllerState, out _))
                    {
                        while (controllerState != SimpleControllerState.READY)
                        {
                            if (!SendQueryCommand(drive.Config.DriveAddress + SMC100Command.GetState.Code, ref queryResult, true))
                            {
                                break;
                            }
                            if (!AnalyzeStateResponse(queryResult, out controllerState, out _))
                            {
                                break;
                            }
                        }
                        result = true;
                    }
                }
            }
            if (result)
            {
                _logger.Information(drive.Config.DriveAxisId + " : Homing done, applying security offset...");
                result = GenericSensorAbsoluteMove(drive, drive.LowerTravelLimit, waitEndOfMove);
                _logger.Information(drive.Config.DriveAxisId + " : Homing done");
            }
            else
            {
                _logger.Error("Unable to home stage");
            }
            return result;
        }
    }
}
