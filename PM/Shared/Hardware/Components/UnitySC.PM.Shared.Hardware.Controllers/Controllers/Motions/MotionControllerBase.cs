using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers
{
    public abstract class MotionControllerBase : ControllerBase
    {
        public event AxesPositionChangedDelegate AxesPositionChangedEvent;

        public event AxesStateChangedDelegate AxesStateChangedEvent;

        public event AxesErrorDelegate AxesErrorEvent;

        public event AxesEndMoveDelegate AxesEndMoveEvent;

        public MotionControllerBase(ControllerConfig controllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger)
            : base(controllerConfig, globalStatusServer, logger) { }

        public bool IsAxesPositionChangedEventSet => !(AxesPositionChangedEvent is null);
        public bool IsAxesStateChangedEventSet => !(AxesStateChangedEvent is null);
        public bool IsAxesErrorEventSet => !(AxesErrorEvent is null);
        public bool IsAxesEndMoveEventSet => !(AxesEndMoveEvent is null);

        public List<IAxis> AxisList = new List<IAxis>();

        public IAxis GetAxis(string axisId)
        {
            var axis = AxisList.FirstOrDefault(a => a.AxisID == axisId);
            if (axis is null)
                throw new Exception($"{axisId} axis not found in {DeviceID} configuration");
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
        public List<IAxis> GetAxesControlledFromList(List<IAxis> axesList)
        {
            return axesList.FindAll(a => AxisList.Any<IAxis>(ca => a.AxisID == ca.AxisID));
        }


        public abstract void CheckControllerIsConnected();

        public abstract void StopAllMotion();

        public abstract void WaitMotionEnd(int timeout, bool waitStabilization = true);

        public abstract void InitializeAllAxes(List<Message> initErrors);

        public abstract bool IsAxisManaged(IAxis axis);

        public abstract void HomeAllAxes();

        public abstract PositionBase GetPosition();

        public abstract void RefreshAxisState(IAxis axis);

        public virtual void TriggerUpdateEvent() { }

        // Required by interface
        public abstract void RefreshCurrentPos(List<IAxis> axis);
    }
}
