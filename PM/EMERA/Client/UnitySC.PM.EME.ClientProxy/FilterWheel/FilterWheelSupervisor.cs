using System.Collections.Generic;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Service.Interface.FilterWheel;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Client.Proxy.FilterWheel
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class FilterWheelSupervisor : IFilterWheelService
    {
        private readonly ServiceInvoker<IFilterWheelService> _filterWheelService;

        public FilterWheelSupervisor(SerilogLogger<IFilterWheelService> logger, IMessenger messenger)
        {
            var customAddress = ClientConfiguration.GetServiceAddress(ActorType.EMERA);
            _filterWheelService = new ServiceInvoker<IFilterWheelService>("EMERAFilterWheelService", logger, messenger, customAddress);
        }

        public Response<List<FilterSlot>> GetFilterSlots()
        {
            return _filterWheelService.InvokeAndGetMessages(s => s.GetFilterSlots());
        }

        public Response<AxisConfig> GetAxisConfiguration()
        {
            return _filterWheelService.InvokeAndGetMessages(s => s.GetAxisConfiguration());
        }

        public Response<double> GetCurrentPosition()
        {
            return _filterWheelService.InvokeAndGetMessages(s => s.GetCurrentPosition());
        }

        public Response<VoidResult> GoToPosition(double targetPosition)
        {
            return _filterWheelService.InvokeAndGetMessages(s => s.GoToPosition(targetPosition));
        }

        public Response<VoidResult> WaitMotionEnd(int timeout)
        {
            return _filterWheelService.InvokeAndGetMessages(s => s.WaitMotionEnd(timeout));
        }
    }
}
