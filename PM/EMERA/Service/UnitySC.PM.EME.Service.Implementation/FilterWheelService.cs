using System;
using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.FilterWheel;
using UnitySC.PM.EME.Service.Interface.FilterWheel;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class FilterWheelService : BaseService, IFilterWheelService
    {
        private readonly EmeHardwareManager _hardwareManager;

        public FilterWheelService(EmeHardwareManager hardwareManager, ILogger logger)
            : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = hardwareManager;
        }

        public FilterWheel FilterWheel
        {
            get
            {
                if (!(_hardwareManager.Wheel is FilterWheel filterWheel))
                {
                    throw new Exception("Filter Wheel not found.");
                }
                return filterWheel;
            }
        }

        public Response<List<FilterSlot>> GetFilterSlots()
        {
            return InvokeDataResponse(_ => FilterWheel.FilterSlots);
        }

        public Response<AxisConfig> GetAxisConfiguration()
        {
            return InvokeDataResponse(_ => FilterWheel.AxisConfig);
        }

        public Response<double> GetCurrentPosition()
        {
            return InvokeDataResponse(_ => FilterWheel.GetCurrentPosition());
        }

        public Response<VoidResult> GoToPosition(double targetPosition)
        {
            return InvokeVoidResponse(_ => FilterWheel.Move(targetPosition));
        }

        public Response<VoidResult> WaitMotionEnd(int timeout)
        {
            return InvokeVoidResponse(_ => FilterWheel.WaitMotionEnd(timeout));
        }
    }
}
