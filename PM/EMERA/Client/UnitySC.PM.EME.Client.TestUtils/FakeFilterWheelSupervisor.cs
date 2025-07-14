using System.Collections.Generic;
using System.Threading;

using UnitySC.PM.EME.Service.Interface.FilterWheel;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Client.TestUtils
{
    public class FakeFilterWheelSupervisor : IFilterWheelService
    {
        private readonly ManualResetEvent _synchro = new ManualResetEvent(false);

        private readonly AxisConfig _config = new AxisConfig()
        {
            AxisID = "Rotation",
            MovingDirection = MovingDirection.Rotation,
            PositionMin = 0.Millimeters(),
            PositionMax = 360.Millimeters()
        };

        private double _position;

        private readonly List<FilterSlot> _filterSlots = new List<FilterSlot>
        {
            new FilterSlot { Name = "Slot 1", Position = 0 }, new FilterSlot { Name = "Slot 2", Position = 60 }
        };

        public Response<AxisConfig> GetAxisConfiguration()
        {
            return new Response<AxisConfig>() { Result = _config };
        }

        public Response<double> GetCurrentPosition()
        {
            return new Response<double>() { Result = _position };
        }

        public Response<List<FilterSlot>> GetFilterSlots()
        {
            return new Response<List<FilterSlot>> { Result = _filterSlots };
        }

        public Response<VoidResult> GoToPosition(double targetPosition)
        {
            _synchro.Reset();
            _position = targetPosition;
            _synchro.Set();
            return new Response<VoidResult>();
        }

        public Response<VoidResult> WaitMotionEnd(int timeout)
        {
            _synchro.WaitOne(timeout);
            return new Response<VoidResult>();
        }
    }
}
