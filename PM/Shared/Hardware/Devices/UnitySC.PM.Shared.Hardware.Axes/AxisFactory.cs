using System;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.AxesSpace
{
    public static class AxisFactory
    {
        public static IAxis CreateAxis(AxisConfig config, ILogger logger)
        {
            switch (config)
            {
                case ACSAxisConfig acsAxisConfig: return new ACSAxis(acsAxisConfig, logger);
                case DummyAxisConfig dummyAxisConfig: return new DummyAxis(dummyAxisConfig, logger);
                case PiezoAxisConfig piezoAxisConfig: return new PiezoAxis(piezoAxisConfig, logger);
                case AerotechAxisConfig aerotechAxisConfig: return new AerotechAxis(aerotechAxisConfig, logger);
                case RelianceAxisConfig realianceAxisConfig: return new RelianceAxis(realianceAxisConfig);
                case PhytronAxisConfig phytronAxisConfig: return new PhytronAxis(phytronAxisConfig);
                case ThorlabsSliderAxisConfig thorlabsSliderAxisConfig: return new ThorlabsSliderAxis(thorlabsSliderAxisConfig, logger);
                case ParallaxAxisConfig parallaxAxisConfig: return new ParallaxAxis(parallaxAxisConfig);
                case IoAxisConfig ioAxisConfig: return new IoAxis(ioAxisConfig);
                case CNCAxisConfig cncAxisConfig: return new CNCAxis(cncAxisConfig);
                default: throw new Exception($"Unknown axis type '{config.AxisID}'.");
            }
        }
    }
}
