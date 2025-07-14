using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Spectrometer;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Spectrometer;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Spectrometer
{
    public class SpectrometerAvantes : SpectrometerBase
    {
        public override DeviceFamily Family => DeviceFamily.Spectrometer;

        private SpectrometerAVSController _controller;

        public SpectrometerAvantes(IGlobalStatusServer globalStatusServer, ILogger logger, SpectrometerConfig config, ControllerBase spectroController) : base(globalStatusServer, logger)
        {
            _controller = (SpectrometerAVSController)spectroController;
        }

        public override void Init(SpectrometerConfig config)
        {
            base.Init(config);

            // init du controller ?

        }

        public override SpectroSignal DoMeasure(SpectrometerParamBase param, bool isSilent)
        {
            return _controller.DoMeasure(param); 
        }

        public override SpectroSignal GetLastMeasure()
        {
            return _controller.GetLastMeasure();
        }

        public override void StartContinuousAcquisition(SpectrometerParamBase param)
        {
            _controller.StartMeasure(param);
        }

        public override void StopContinuousAcquisition()
        {
            _controller.StopMeasure();
        }


    }
}
