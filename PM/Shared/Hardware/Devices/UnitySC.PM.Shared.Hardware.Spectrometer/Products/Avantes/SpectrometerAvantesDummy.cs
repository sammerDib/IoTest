using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Spectrometer;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Spectrometer;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Spectrometer
{
    public class SpectrometerAvantesDummy : SpectrometerBase
    {
        public override DeviceFamily Family => DeviceFamily.Spectrometer;

        private SpectrometerDummyController _controller;

        public SpectrometerAvantesDummy(IGlobalStatusServer globalStatusServer, ILogger logger, SpectrometerConfig config, ControllerBase spectroController) : base(globalStatusServer, logger)
        {
            _controller = (SpectrometerDummyController)spectroController;
        }

        public override void Init(SpectrometerConfig config)
        {
            base.Init(config);
            Logger.Information("Init SpectrometerAvantes as a Dummy");
        }

        public override SpectroSignal DoMeasure(SpectrometerParamBase param, bool isSilent)
        {
            return _controller.DoMeasure(param,isSilent);
        }

        public override void StartContinuousAcquisition(SpectrometerParamBase param)
        {
           
        }

        public override void StopContinuousAcquisition()
        {

        }
        public override SpectroSignal GetLastMeasure()
        {
            return null;
        }

    }
}
