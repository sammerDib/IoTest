using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    public delegate void AxesPositionChangedDelegate(PositionBase axesPosition);

    public delegate void AxesStateChangedDelegate(AxesState axesState);

    public delegate void AxesErrorDelegate(Message error);

    public delegate void AxesEndMoveDelegate(bool targetReached);

    public interface IAxesController : IDevice
    {
        ControllerConfig ControllerConfiguration { get; set; }
        AxesConfig AxesConfiguration { get; set; }

        #region Properties

        event AxesPositionChangedDelegate AxesPositionChangedEvent;

        event AxesStateChangedDelegate AxesStateChangedEvent;

        event AxesErrorDelegate AxesErrorEvent;

        event AxesEndMoveDelegate AxesEndMoveEvent;

        List<IAxis> AxesList { get; set; }

        #endregion Properties

        #region Abstract functions

        /// <summary>
        /// - Start notifying <see cref="AxesStateChangedEvent"/> with new <see cref="AxesState"/>
        /// - Call <see cref="RefreshAxisState"/> for all axes
        /// - Notify <see cref="AxesState"/>
        /// (frequency: TO BE DEFINED).
        /// </summary>
        /// <param name="initErrors"></param>
        /// <exception cref="Exception">if controller is not connected</exception>
        void InitializationAllAxes(List<Message> initErrors);

        void InitControllerAxes(List<Message> initErrors);

        bool IsAxesPositionChangedEventSet { get; }
        bool IsAxesStateChangedEventSet { get; }
        bool IsAxesErrorEventSet { get; }
        bool IsAxesEndMoveEventSet { get; }

        bool IsAxesControlled(List<IAxis> axesList);

        List<IAxis> GetAxesControlledFromList(List<IAxis> axesList);

        bool ResetController();

        /// <summary>
        /// Stop motion of all axes.
        /// </summary>
        void StopAxesMotion();

        /// <summary>
        /// Enable motor servo control. Enabled by default. Required for motion.
        /// </summary>
        /// <exception cref="Exception">if controller is not initialized or not connected</exception>
        void EnableAxis(List<IAxis> axisList);

        /// <summary>
        /// Disable motor servo control (to allow manual intervention).
        /// Warning: motion is no more possible until motor servo control is enabled again.
        /// </summary>
        /// <exception cref="Exception">if controller is not initialized or not connected</exception>
        void DisableAxis(List<IAxis> axisList);            

        void GotoHomePos(List<IAxis> axisList, List<AxisSpeed> speedList);

        void ResetTimestamp();

        /// <summary>
        /// Refresh CurrentPos attribute of given axis with current position.
        /// </summary>
        /// <exception cref="Exception">if controller is not initialized or not connected</exception>
        void RefreshCurrentPos(List<IAxis> axis);

        /// <summary>
        /// Return current given axis position with a timestamp.
        /// </summary>
        /// <exception cref="Exception">if controller is not initialized or not connected</exception>
        TimestampedPosition GetCurrentAxisPosWithTimestamp(IAxis axis);

        /// <summary>
        /// Refresh Moving and Enabled properties of given axis by querying controller.
        /// </summary>
        /// <exception cref="Exception">if controller is not initialized or not connected</exception>
        void RefreshAxisState(IAxis axis);

        /// <summary>
        /// Returns true if stage is landed. False otherwise.
        /// Warning: Only relevant for ACSController
        /// </summary>
        /// <exception cref="Exception">if controller is not initialized or not connected</exception>
        bool IsLanded();

        /// <summary>
        /// Move to relative position, according to current position. Accelerations associated to
        /// given speed are used (ex: NORMAL acceleration for NORMAL speed...). <see
        /// cref="AxesEndMoveEvent"/> is notified when motion end. <see
        /// cref="AxesPositionChangedEvent"/> is notified when motion end.
        /// Note: this method is asynchronous.
        /// </summary>
        /// <param name="step">
        /// Motion distance (unit: millimeters) from current position, in motor referential.
        /// Negative value means moving in opposite direction compared to motor referential.
        /// </param>
        /// <exception cref="Exception">if controller is not initialized or not connected</exception>
        /// <seealso cref="WaitMotionEnd"/>
        void MoveIncremental(IAxis axis, AxisSpeed speed, double step);

        // FIXME: use dedicated struct to prevent error prone parameters when using multiple lists,
        // which are linked.
        // FIXME: prefer using more precise type instead of double (which unit?)
        /// <summary>
        /// Move to absolute positions, according motor referential, along multiple axes, with
        /// custom speeds and accelerations. For each axis, a new value should be defined in all parameters.
        /// Ex: first value of coordsList is used with first value of axisList, speedsList and
        /// accelsList. <see cref="AxesEndMoveEvent"/> is notified when motion end. <see
        /// cref="AxesPositionChangedEvent"/> is notified when motion end.
        /// Note: this method is asynchronous.
        /// </summary>
        /// <param name="coordsList">
        /// Coordinates (unit: TO BE DEFINED) in motor referential, for each axis
        /// </param>
        /// <param name="axisList">All concerned axis</param>
        /// <param name="speedsList">Speeds (unit: TO BE DEFINED)</param>
        /// <param name="accelsList">Accelerations (unit: TO BE DEFINED)</param>
        /// <exception cref="Exception">if controller is not initialized or not connected</exception>
        /// <seealso cref="WaitMotionEnd"/>
        void SetPosAxisWithSpeedAndAccel(List<double> coordsList, List<IAxis> axisList, List<double> speedsList, List<double> accelsList);

        // FIXME: use dedicated struct to prevent error prone parameters when using multiple linked lists.
        // FIXME: prefer using more precise type instead of double (which unit?)
        /// <summary>
        /// Move to absolute positions, according motor referential, along multiple axes, with
        /// custom speeds. Accelerations associated to given speed are used (ex: NORMAL acceleration
        /// for NORMAL speed...). For each axis, a new value should be defined in all parameters.
        /// Ex: first value of coordsList is used with first value of axisList and speedsList. <see
        ///     cref="AxesEndMoveEvent"/> is notified when motion end. <see
        ///     cref="AxesPositionChangedEvent"/> is notified when motion end.
        /// Note: this method is asynchronous.
        /// </summary>
        /// <param name="coordsList">
        /// Coordinates (unit: TO BE DEFINED) in motor referential, for each axis
        /// </param>
        /// <param name="axisList">All concerned axis</param>
        /// <param name="speedsList">Speeds (unit: TO BE DEFINED)</param>
        /// <exception cref="Exception">if controller is not initialized or not connected</exception>
        /// <seealso cref="WaitMotionEnd"/>
        void SetPosAxis(List<double> coordsList, List<IAxis> axisList, List<AxisSpeed> speedsList);

        /// <summary>
        /// Set speed and acceleration for next motion (if not overriden). For each axis, a new
        /// value should be defined in all parameters.
        /// Ex: first value of axisList is used with first value of speedsList and accelsList.
        /// </summary>
        /// <param name="axisList"></param>
        /// <param name="speedsList">Speeds (unit: TO BE DEFINED)</param>
        /// <param name="accelsList">Accelerations (unit: TO BE DEFINED)</param>
        /// <exception cref="Exception">if controller is not initialized or not connected</exception>
        void SetSpeedAccelAxis(List<IAxis> axisList, List<double> speedsList, List<double> accelsList);

        /// <summary>
        /// Set speed for next motion (if not overriden). For each axis, a new value should be
        /// defined in all parameters.
        /// Ex: first value of axisList is used with first value of speedsList.
        /// </summary>
        /// <param name="axisList"></param>
        /// <param name="speedsList">Speeds (unit: TO BE DEFINED)</param>
        /// <exception cref="Exception">if controller is not initialized or not connected</exception>
        void SetSpeedAxis(List<IAxis> axisList, List<AxisSpeed> speedsList);

        /// <summary>
        /// Verify that given speed is valid for given axis.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="speed">Speed (unit: TO BE DEFINED)</param>
        void CheckServiceSpeed(IAxis axis, ref double speed);

        void LinearMotionSingleAxis(IAxis axis, AxisSpeed speed, double coordsList);

        void LinearMotionMultipleAxis(List<IAxis> axisList, AxisSpeed axisSpeed, List<double> coordsList);

        /// <summary>
        /// Wait until all axes are stationary.
        /// </summary>
        /// <param name="timeout">Duration (in milliseconds) until throwing an exception</param>
        /// <exception cref="Exception">
        /// if controller is not initialized or not connected, or timeout reached
        /// </exception>
        void WaitMotionEnd(int timeout, bool waitStabilization = true);

        /// <summary>
        /// Take off the stage.
        /// Warning: Only relevant for ACSController
        /// </summary>
        /// <exception cref="Exception">if controller is not initialized or not connected</exception>
        void StopLanding();

        /// <summary>
        /// Land the stage.
        /// Warning: Only relevant for ACSController
        /// </summary>
        /// <exception cref="Exception">if controller is not initialized or not connected</exception>
        void Land();

        void Init(List<Message> initErrors);

        /// <summary>
        /// Initialize z top focus.
        /// Warning: Only relevant for ACSController
        /// </summary>
        void InitZTopFocus();

        /// <summary>
        /// Initialize z bottom focus.
        /// Warning: Only relevant for ACSController
        /// </summary>
        void InitZBottomFocus();

        #endregion Abstract functions
    }
}
