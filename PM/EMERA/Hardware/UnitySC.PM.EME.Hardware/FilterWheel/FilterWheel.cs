using System.Collections.Generic;

using UnitySC.PM.EME.Service.Interface.FilterWheel;
using UnitySC.PM.Shared.Hardware.AxesSpace;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Controllers.FeatureInterfaces;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Wheel;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Hardware.FilterWheel
{
    public class FilterWheel : WheelBase
    {
        public MotionControllerBase Controller { get; set; }
        public List<FilterSlot> FilterSlots { get; set; }
        public AxisConfig AxisConfig { get; set; }

        private readonly IAxis _axis;


        public FilterWheel(FilterWheelConfig config, MotionControllerBase motionController, IGlobalStatusServer globalStatusServer, ILogger logger) : base(globalStatusServer, logger)
        {
            Name = config.Name;
            DeviceID = config.DeviceID;
            FilterSlots = config.FilterSlots;

            AxisConfig = config.AxisConfig;
            _axis = AxisFactory.CreateAxis(AxisConfig, logger);

            Controller = motionController;
            Controller.AxisList.Add(_axis);
        }

        public override void Init()
        {
        }

        public override double GetCurrentPosition()
        {
            Controller.RefreshCurrentPos(new List<IAxis> { _axis });
            return _axis.CurrentPos.Value;
        }

        public override void Move(double targetPosition)
        {
            var move = new PMAxisMove(_axis.AxisID, targetPosition.Millimeters());
            (Controller as IMotion).Move(move);
        }

        public override void WaitMotionEnd(int timeout_ms, bool waitStabilization = true)
        {
            Controller.WaitMotionEnd(timeout_ms, waitStabilization);
        }
    }
}
