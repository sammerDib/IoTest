using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers
{
    public abstract class AxesControllerBase : ControllerBase, IAxesController
    {
        public event AxesPositionChangedDelegate AxesPositionChangedEvent;

        public event AxesStateChangedDelegate AxesStateChangedEvent;

        public event AxesErrorDelegate AxesErrorEvent;

        public event AxesEndMoveDelegate AxesEndMoveEvent;

        protected DateTime StartTime;
        protected Stopwatch StopWatch;

        public AxesControllerBase(ControllerConfig controllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger) : base(controllerConfig, globalStatusServer, logger)
        {
            StartTime = DateTime.UtcNow;
            StopWatch = Stopwatch.StartNew();
        }

        public bool IsAxesPositionChangedEventSet { get => (AxesPositionChangedEvent != null); }
        public bool IsAxesStateChangedEventSet { get => (AxesStateChangedEvent != null); }
        public bool IsAxesErrorEventSet { get => (AxesErrorEvent != null); }
        public bool IsAxesEndMoveEventSet { get => (AxesEndMoveEvent != null); }

        protected List<IAxis> ControlledAxesList = new List<IAxis>();

        public virtual List<IAxis> AxesList
        {
            get { return ControlledAxesList; }
            set
            {
                if (CheckAxisTypesInListAreValid(value))
                    ControlledAxesList = value;
            }
        }

        // TODO: what is the expected behavior of this method in case of violation? return false or throw?
        public abstract bool CheckAxisTypesInListAreValid(List<IAxis> axesList);

        public void CheckAxesListIsValid(List<IAxis> axesList)
        {
            CheckAxisTypesInListAreValid(axesList);

            if (!IsAxesControlled(axesList))
            {
                // Error
                string errorMsg = $"Axis list is invalid for {Name} controller";
                Logger?.Error(errorMsg);
                throw new Exception(errorMsg);
            }
        }

        public bool IsAxesControlled(List<IAxis> axesList)
        {
            return axesList.All<IAxis>(a => ControlledAxesList.Any<IAxis>(ca => a.AxisID == ca.AxisID));
        }

        public List<IAxis> GetAxesControlledFromList(List<IAxis> axesList)
        {
            return axesList.FindAll(a => ControlledAxesList.Any<IAxis>(ca => a.AxisID == ca.AxisID));
        }

        public IAxis GetAxeControlledById(string axisId)
        {
            var axis = ControlledAxesList.FirstOrDefault(a => a.AxisID == axisId);
            if (axis is null)
                throw new Exception(axisId + " axis not found in configuration");
            return axis;
        }

        protected void RaiseStateChangedEvent(AxesState axesState)
        {
            AxesStateChangedEvent?.Invoke(axesState);
        }

        protected void RaiseErrorEvent(Message message)
        {
            AxesErrorEvent?.Invoke(message);
        }

        protected void RaisePositionChangedEvent(PositionBase position)
        {
            AxesPositionChangedEvent?.Invoke(position);
        }

        protected void RaiseMotionEndEvent(bool motionEnd)
        {
            AxesEndMoveEvent?.Invoke(motionEnd);
        }

        public void GotoHomePos(List<IAxis> axesList, List<AxisSpeed> speedList)
        {
            var positionsList = new List<double>() { };
            foreach (var axis in axesList)
                positionsList.Add(axis.AxisConfiguration.PositionHome.Millimeters);

            SetPosAxis(positionsList, axesList, speedList);
        }       
       

        public void ResetTimestamp()
        {
            StartTime = DateTime.UtcNow;
            StopWatch = Stopwatch.StartNew();
        }

        #region IAxesController methods


        public AxesConfig AxesConfiguration { get; set; }

        /// <summary>
        /// Verify that communication is still alive.
        /// </summary>
        /// <exception cref="Exception">in case of communication issue</exception>
        public abstract void CheckControllerCommunication();

        // Required by interface
        public abstract void StopAxesMotion();

        // Required by interface
        public abstract void EnableAxis(List<IAxis> axisList);

        // Required by interface
        public abstract void DisableAxis(List<IAxis> axisList);

        // Required by interface
        public abstract void RefreshCurrentPos(List<IAxis> axis);

        // Required by interface
        public abstract TimestampedPosition GetCurrentAxisPosWithTimestamp(IAxis axis);

        // Required by interface
        public abstract void RefreshAxisState(IAxis axis);

        // Required by interface
        public abstract bool IsLanded();

        // Required by interface
        public abstract void
            MoveIncremental(
                IAxis axis,
                AxisSpeed speed,
                double step
            ); // FIXME: rename step + prefer using more precise type instead of double (which unit?)

        // Required by interface
        public abstract void SetPosAxisWithSpeedAndAccel(
            List<double> coordsList,
            List<IAxis> axisList,
            List<double> speedsList,
            List<double> accelsList
        );

        // Required by interface
        public abstract void SetPosAxis(List<double> coordsList, List<IAxis> axisList, List<AxisSpeed> speedsList);

        // Required by interface
        public abstract void SetSpeedAccelAxis(List<IAxis> axisList, List<double> speedsList, List<double> accelsList);

        // Required by interface
        public abstract void SetSpeedAxis(List<IAxis> axisList, List<AxisSpeed> speedsList);

        // Required by interface
        public abstract void CheckServiceSpeed(IAxis axis, ref double speed);

        public abstract void LinearMotionSingleAxis(IAxis axis, AxisSpeed speed, double coordsList);

        public abstract void LinearMotionMultipleAxis(
            List<IAxis> axisList,
            AxisSpeed axisSpeed,
            List<double> coordsList
        );

        // Required by interface
        public abstract void WaitMotionEnd(int timeout, bool waitStabilization = true);

        // Required by interface
        public abstract void StopLanding();

        // Required by interface
        public abstract void Land();

        // Required by interface
        public abstract void InitializationAllAxes(List<Message> initErrors);

        public abstract void InitControllerAxes(List<Message> initErrors);


        public abstract void InitZTopFocus();

        public abstract void InitZBottomFocus();

        #endregion IAxesController methods
    }
}
