using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Controllers
{
    public abstract class PiezoController : AxesControllerBase, IDisposable
    {
        protected PiezoController(PiezoControllerConfig piezoControllerConfig) : this(piezoControllerConfig, ClassLocator.Default.GetInstance<IGlobalStatusServer>(), ClassLocator.Default.GetInstance<ILogger<PiezoController>>())
        {
        }

        protected PiezoController(PiezoControllerConfig piezoControllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger) : base(piezoControllerConfig, globalStatusServer, logger)
        {
        }

        /// <summary>
        /// Controller initialization.
        /// </summary>
        /// <param name="initErrors"></param>
        public override void Init(List<Message> initErrors)
        { Connect(); }

        /// <summary>
        /// Command the controller to setup (servo mode, internal parameters, trigger I/O, etc.)
        /// </summary>
        /// <exception cref="Exception">Thrown if an error occurs.</exception>
        public abstract override void InitializationAllAxes(List<Message> initErrors);

        public override bool CheckAxisTypesInListAreValid(List<IAxis> axesList)
        {
            bool axisTypeNotSupported = axesList.Any(axis => !(axis is PiezoAxis));
            if (axisTypeNotSupported)
            {
                string errorMsg = "Piezo controller handles only axes of type PiezoAxis.";
                Logger?.Error(errorMsg);
                throw new Exception(errorMsg);
            }
            return true;
        }

        public abstract void Dispose();

        /******************
         *   CONNECTION   *
         ******************/

        /// <summary>
        /// Current connection Id established with the controller.
        /// </summary>
        public abstract int ConnectionId { get; }

        /// <summary>
        /// Returns true if the controller is connected, false otherwise.
        /// </summary>
        public abstract bool IsConnected { get; }

        /// <summary>
        /// Initialize the connection with the controller.
        /// </summary>
        /// <exception cref="TimeoutException">Thrown if interface could not be opened or no controller is responding.</exception>
        public abstract override void Connect();

        public override void Connect(string deviceId)
        {
            Connect();
        }

        /// <summary>
        /// Close connection to the controller associated with ID.
        /// ID will not be valid after this call.
        /// </summary>
        /// <exception cref="Exception">Thrown if an error occurs.</exception>
        public abstract override void Disconnect();

        public override void Disconnect(string deviceID)
        {
            Disconnect();
        }

        /******************
         *   PROPERTIES   *
         ******************/

        /// <summary>
        /// Returns the axis name.
        /// </summary>
        /// <exception cref="Exception">Trown if an error occurs or there is more than one axis.</exception>
        public abstract string GetAxisName();

        /// <summary>
        /// Returns the piezo travel speed.
        /// </summary>
        public abstract double GetSpeed();

        /// <summary>
        /// Set the piezo speed.
        /// </summary>
        public abstract void SetSpeed(double speed);

        /// <summary>
        /// Sets the given travel speeds.
        /// </summary>
        /// <param name="axes">Concerned axes</param>
        /// <param name="speeds">Enum speeds</param>
        public abstract override void SetSpeedAxis(List<IAxis> axes, List<AxisSpeed> speeds);

        /// <summary>
        /// Sets the given travel speeds and accelerations.
        /// </summary>
        /// <param name="axes">Concerned axes</param>
        /// <param name="speeds">Specific speed values</param>
        /// <param name="accels">Specific acceleration values</param>
        public abstract override void SetSpeedAccelAxis(List<IAxis> axes, List<double> speeds, List<double> accels);

        /******************
         *  POSITIONNING  *
         ******************/

        /// <summary>
        /// Returns the current position of the piezo.
        /// </summary>
        /// <returns>The current position if the controller is responding, -1 otherwise.</returns>
        public abstract Length GetCurrentPosition();

        /// <summary>
        /// Get the lower bound value of the axis travel range.
        /// </summary>
        public abstract Length GetPositionMin();

        /// <summary>
        /// Get the upper bound value of the axis travel range.
        /// </summary>
        public abstract Length GetPositionMax();

        /// <summary>
        /// Move to the given positions.
        /// </summary>
        /// <param name="targetPositions">Positions to reach</param>
        /// <param name="axes">Concerned axes</param>
        /// <param name="speeds">Enum speeds</param>
        public abstract override void SetPosAxis(List<double> targetPositions, List<IAxis> axes, List<AxisSpeed> speeds);

        /// <summary>
        /// Move to the given positions at specific values of speed and acceleration.
        /// </summary>
        /// <param name="targetPositions">Positions to reach</param>
        /// <param name="axes">Concerned axes</param>
        /// <param name="speeds">Specific speed values</param>
        /// <param name="accels">Specific acceleration values</param>
        public abstract override void SetPosAxisWithSpeedAndAccel(List<double> targetPositions, List<IAxis> axes, List<double> speeds, List<double> accels);

        /// <summary>
        /// Waits until the piezo stop moving.
        /// </summary>
        /// <param name="timeout_ms"></param>
        public abstract override void WaitMotionEnd(int timeout_ms, bool waitStabilization = true);

        /****************
         *  TRIGGERING  *
         ****************/

        /// <summary>
        /// Trigger IN configuration.
        /// Specifies the initial piezo position and the motion step to perform when receiving an electrical impulse.
        /// </summary>
        /// <param name="initialPosition"></param>
        /// <param name="motionStep"></param>
        public abstract void ConfigureTriggerIn(Length initialPosition, Length motionStep);

        /// <summary>
        /// Trigger OUT configuration.
        /// "Position Distance" trigger mode
        /// </summary>
        /// <param name="startThreshold"></param>
        /// <param name="stopThreshold"></param>
        /// <param name="triggerStep"></param>
        public abstract void ConfigureTriggerPositionDistance(double startThreshold, double stopThreshold, double triggerStep);

        public abstract void DisableTriggerIn();

        /********************************
         * IRRELEVANT METHODS FOR PIEZI *
         ********************************/

        //TODO GVA Method is utils for other axis, no for piezo => TODO Refacto motionMotionBase
        public override void Land()
        {
            throw new NotImplementedException();
        }

        public override bool IsLanded()
        {
            throw new NotImplementedException();
        }

        public override void StopLanding()
        {
            throw new NotImplementedException();
        }

        public override void CheckControllerCommunication()
        {
            throw new NotImplementedException();
        }

        public override void EnableAxis(List<IAxis> axisList)
        {
            throw new NotImplementedException();
        }

        public override void DisableAxis(List<IAxis> axisList)
        {
            throw new NotImplementedException();
        }

        public override void CheckServiceSpeed(IAxis axis, ref double speed)
        {
            throw new NotImplementedException();
        }

        public override bool ResetController()
        {
            return true;
        }

        public override void LinearMotionMultipleAxis(List<IAxis> axisList, AxisSpeed axisSpeed, List<double> coordsList)
        {
            throw new NotImplementedException();
        }

        public override void StopAxesMotion()
        {
        }

        public override void MoveIncremental(IAxis axis, AxisSpeed speed, double step)
        {
            throw new NotImplementedException();
        }

        public override void LinearMotionSingleAxis(IAxis axis, AxisSpeed speed, double coordsList)
        {
            throw new NotImplementedException();
        }
    }
}
