using System.Collections.Generic;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Client.Proxy.Calibration
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class CalibrationSupervisor : ICalibrationService
    {
        private readonly ServiceInvoker<ICalibrationService> _service;

        public CalibrationSupervisor(ILogger<ICalibrationService> logger, IMessenger messenger)
        {
            var customAddress = ClientConfiguration.GetServiceAddress(ActorType.EMERA);
            _service = new ServiceInvoker<ICalibrationService>("EMERACalibrationService", logger,
                messenger, customAddress);
        }

        public Response<string> GetCalibrationPath()
        {
            return _service.InvokeAndGetMessages(s => s.GetCalibrationPath());
        }

        public Response<IEnumerable<ICalibrationData>> GetCalibrations()
        {
            return _service.InvokeAndGetMessages(s => s.GetCalibrations());
        }
        public Response<CameraCalibrationData> GetCameraCalibrationData()
        {
            return _service.InvokeAndGetMessages(s => s.GetCameraCalibrationData());
        }
        public Response<WaferReferentialSettings> GetWaferReferentialSettings(Length waferDiameter)
        {
            return _service.InvokeAndGetMessages(s => s.GetWaferReferentialSettings(waferDiameter));
        }

        public Response<List<Filter>> GetFilters()
        {
            return _service.InvokeAndGetMessages(s => s.GetFilters());
        }
        public Response<VoidResult> SaveCalibration(ICalibrationData calibrationData)
        {
            return _service.InvokeAndGetMessages(s => s.SaveCalibration(calibrationData));
        }

        public Response<int> GetNeededCalibrationCount()
        {
            return _service.InvokeAndGetMessages(s => s.GetNeededCalibrationCount());
        }
    }
}
