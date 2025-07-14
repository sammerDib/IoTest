using System;
using System.Collections.Generic;
using System.Threading;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Controllers
{
    public class DummyController : MotorController, IAxesController
    {
        #region Constructors

        private List<IAxis> _axesList = new List<IAxis>();

        public DummyController(ILogger logger)
            : base(new ControllerConfig(), null, logger)
        {
        }

        public DummyController(ControllerConfig controllerConfig, AxesConfig axesConfig, ILogger logger)
            : base(controllerConfig, null, logger)
        {
            DeviceID = controllerConfig.DeviceID;
            Name = controllerConfig.DeviceID;

            foreach (var axisConfig in axesConfig.AxisConfigs)
            {
                ACSAxis newAxis = new ACSAxis(axisConfig, logger);
                _axesList.Add(newAxis);
            }
        }

        #endregion Constructors

        #region Public methods

        public void Dispose()
        {
        }

        public override void InitializationAllAxes(List<Message> initErrors)
        {
            try
            {
                foreach (IAxis curraxis in ControlledAxesList)
                {
                    curraxis.CurrentPos = 0.Millimeters();
                    curraxis.DeviceError = new Message(MessageLevel.Information, "NoError");
                    curraxis.Enabled = true;
                    curraxis.EnabledPrev = true;
                    curraxis.Initialized = true;
                    curraxis.Moving = false;
                    curraxis.MovingPrev = false;

                    Logger.Information("Action : " + curraxis.AxisID + " axis initialized - successfully completed");
                }
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("InitializationAllAxes - ACSException: " + Ex.Message));
                throw;
            }

            State = new DeviceState(DeviceStatus.Ready, "Dummy controller initialization complete");
        }

        public override bool ResetController()
        {
            return true;
        }

        public override void StopAxesMotion()
        {
            try
            {
                foreach (IAxis curraxis in ControlledAxesList)
                {
                    curraxis.CurrentPos = 0.Millimeters();
                    curraxis.DeviceError = new Message(MessageLevel.Information, "NoError");
                    curraxis.Moving = false;
                    curraxis.MovingPrev = false;

                    Logger.Information("Action : " + curraxis.AxisID + " axis stopped - successfully completed");
                }
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("StopAxesMotion - ACSException: " + Ex.Message));
                throw;
            }
        }

        public override void EnableAxis(List<IAxis> axesList)
        {
            try
            {
                foreach (IAxis axis in axesList)
                {
                    IAxis curraxis = _axesList.Find(a => a.AxisID == axis.AxisID);
                    curraxis.Enabled = true;
                    Logger.Information("Action : " + curraxis.AxisID + " axis enabled - successfully completed");
                }
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("EnableAxis - ACSException: " + Ex.Message));
                throw;
            }
        }

        private string FormatMessage(string message)
        {
            return ($"[{DeviceID}]{message}").Replace('\r', ' ').Replace('\n', ' ');
        }

        public override void DisableAxis(List<IAxis> axesList)
        {
            try
            {
                foreach (IAxis axis in axesList)
                {
                    IAxis curraxis = _axesList.Find(a => a.AxisID == axis.AxisID);
                    curraxis.Enabled = false;
                    Logger.Information("Action : " + curraxis.AxisID + " axis disabled - successfully completed");
                }
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("DisableAxis - ACSException: " + Ex.Message));
                throw;
            }
        }

        public override void RefreshCurrentPos(List<IAxis> axis)
        {
        }

        public override TimestampedPosition GetCurrentAxisPosWithTimestamp(IAxis axis)
        {
            var position = axis.CurrentPos;
            var highResolutionDateTime = StartTime.AddTicks(StopWatch.Elapsed.Ticks);
            return new TimestampedPosition(position, highResolutionDateTime);
        }

        public override void RefreshAxisState(IAxis axis)
        {
        }

        public override void MoveIncremental(IAxis axis, AxisSpeed speed, double step)
        {
            try
            {
                axis.CurrentPos = step.Millimeters();
                Logger.Information("Action : " + axis.AxisID + " axis moved - successfully completed");
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("MoveIncremental - ACSException: " + Ex.Message));
                throw;
            }
        }

        public override void SetPosAxisWithSpeedAndAccel(List<double> coordsList, List<IAxis> axesList, List<double> speedsList, List<double> accelsList)
        {
            try
            {
                for (int i = 0; i < axesList.Count; i++)
                {
                    IAxis curraxis = _axesList[i];
                    CheckAxisLimits(curraxis, coordsList[i]);
                    curraxis.CurrentPos = coordsList[i].Millimeters();
                    Logger.Information("Action : " + curraxis.AxisID + " axis moved - successfully completed");
                }
            }
            catch (Exception Ex)
            {
                Logger?.Error(FormatMessage("SetPosAxisWithSpeedAndAccel - Exception: " + Ex.Message));
                throw;
            }
        }

        public override void SetSpeedAccelAxis(List<IAxis> axisList, List<double> speedsList, List<double> accelsList)
        {
        }

        public override void SetSpeedAxis(List<IAxis> axisList, List<AxisSpeed> speedsList)
        {
        }

        public override void CheckServiceSpeed(IAxis axis, ref double speed)
        {
        }

        public override void LinearMotionSingleAxis(IAxis axis, AxisSpeed speed, double coords)
        {
            List<double> coordsList = new List<double>() { coords };
            List<double> speedList = new List<double>() { 2000 };
            List<IAxis> axesList = new List<IAxis>() { axis };
            List<double> accelList = new List<double>() { 2000 };

            SetPosAxisWithSpeedAndAccel(coordsList, axesList, speedList, accelList);
        }

        public override void LinearMotionMultipleAxis(List<IAxis> axesList, AxisSpeed axisSpeed, List<double> coordsList)
        {
            List<double> speedList = new List<double>() { 2000 };
            List<double> accelList = new List<double>() { 2000 };

            SetPosAxisWithSpeedAndAccel(coordsList, axesList, speedList, accelList);
        }

        public override void WaitMotionEnd(int timeout, bool waitStabilization = true)
        {
            foreach (IAxis axis in ControlledAxesList)
            {
                axis.Moving = true;
                RefreshAxisState(axis);
            }
            Thread.Sleep(1000); // Wait for move to start
            foreach (IAxis axis in ControlledAxesList)
            {
                axis.Moving = false;
                RefreshAxisState(axis);
            }
        }

        public override void Connect()
        {
        }

        public override void Disconnect()
        {
        }

        public override bool IsLanded()
        {
            return false;
        }

        public override void StopLanding()
        {
        }

        public override void Land()
        {
        }

        public override void Connect(string deviceId)
        {
        }

        public override void Disconnect(string deviceID)
        {
        }

        public override void CheckControllerCommunication()
        {
        }

        public override bool CheckAxisTypesInListAreValid(List<IAxis> axesList)
        {
            return true;
        }

        public override void Init(List<Message> initErrors)
        {
        }

        public override void InitControllerAxes(List<Message> initErrors)
        {
            //Nothing
        }

        public override void InitZTopFocus()
        {
            //Nothing
        }

        public override void InitZBottomFocus()
        {
            //Nothing
        }

        #endregion Public methods
    }
}
