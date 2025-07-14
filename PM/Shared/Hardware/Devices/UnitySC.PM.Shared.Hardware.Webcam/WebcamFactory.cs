using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Webcam
{
    static public class WebcamFactory
    {
        static public Webcam Create(WebcamConfig config, IGlobalStatusServer globalStatusServer)
        {
            return new Webcam(config, globalStatusServer, new HardwareLogger(config.LogLevel.ToString(), DeviceFamily.Webcam.ToString(), config.Name));
        }
    }
}
