using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Service.DMTCalTransform;
using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Service.Interface.AlgorithmManager;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.DMT.Service.Implementation
{
    public class AlgorithmManager : IDMTAlgorithmManager
    {
        private const string AlgorithmsConfigurationFileName = "AlgorithmsConfiguration.xml";

        private readonly ILogger _logger = ClassLocator.Default.GetInstance<ILogger<AlgorithmManager>>();

        //public Dictionary<string, Curvature.GlobalTopo> GlobalTopos = new Dictionary<string, Curvature.GlobalTopo>();
        public AlgorithmsConfiguration Config { get; private set; }

        public void Init()
        {
            var hardwareManager = ClassLocator.Default.GetInstance<DMTHardwareManager>();
            var serviceConfigurationManager = ClassLocator.Default.GetInstance<IDMTServiceConfigurationManager>();

            // Lecture du fichier de config
            //.............................

            PathString algorithmConfigurationFilePath =
                Path.Combine(serviceConfigurationManager.ConfigurationFolderPath, AlgorithmsConfigurationFileName);
            _logger.Information("Loading " + algorithmConfigurationFilePath);
            Config = XML.Deserialize<AlgorithmsConfiguration>(algorithmConfigurationFilePath);

            foreach (var exposureCalibrationSettings in Config.ExposureCalibration)
            {
                if (!exposureCalibrationSettings.IsEnabled
                    && hardwareManager.CamerasBySide.ContainsKey(exposureCalibrationSettings.Side))
                {
                    var message = $"Camera for side {exposureCalibrationSettings.Side} is activated but exposure calibration is disable." +
                        $" Please check {AlgorithmsConfigurationFileName} file";
                    _logger.Warning(message);
                    ClassLocator.Default.GetInstance<IGlobalStatusServer>().AddMessage(new Message(MessageLevel.Warning, message));
                }
            }
        }

        public void Shutdown()
        {
            
        }
    }
}
