using System.Collections.Generic;

using Moq;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Shared.TestUtils
{
    public interface ITestWithPhotoLumAxes
    {
        EmeHardwareManager HardwareManager { get; set; }
        Mock<PhotoLumAxes> SimulatedMotionAxes { get; set; }
    }

    public static class TestWithPhotoLumHelper
    {
        public static void Setup(ITestWithPhotoLumAxes test)
        {
            // Handle manually setting up the motion axes mock (to have a strict mock for example)
            if (test.SimulatedMotionAxes is null)
            {
                SerilogLogger<PhotoLumAxes> logger = new SerilogLogger<PhotoLumAxes>();

                AerotechAxisConfig xAxisConfig = new AerotechAxisConfig();
                AerotechAxisConfig yAxisConfig = new AerotechAxisConfig();
                AerotechAxisConfig zAxisConfig = new AerotechAxisConfig();
                AerotechAxisConfig rotationAxisConfig = new AerotechAxisConfig();


                xAxisConfig.MovingDirection = MovingDirection.X;
                xAxisConfig.ControllerID = "X";
                xAxisConfig.AxisID = "X";

                yAxisConfig.MovingDirection = MovingDirection.Y;
                yAxisConfig.ControllerID = "Y";
                yAxisConfig.AxisID = "Y";


                zAxisConfig.MovingDirection = MovingDirection.Z;
                zAxisConfig.ControllerID = "Z";
                zAxisConfig.AxisID = "Z";


                zAxisConfig.PositionMin = -19.1.Millimeters();
                zAxisConfig.PositionMax = 2.9.Millimeters();
                zAxisConfig.SpeedMaxScan = 25;
                zAxisConfig.AccelNormal = 500;
                zAxisConfig.AccelFast = 1000;

                rotationAxisConfig.MovingDirection = MovingDirection.Rotation;
                rotationAxisConfig.ControllerID = "Rotation";
                rotationAxisConfig.AxisID = "Rotation";


                rotationAxisConfig.PositionMin = 0.Millimeters();
                rotationAxisConfig.PositionMax = 359.9999.Millimeters();



                var motionDict = new Dictionary<string, MotionControllerBase>();

                var aerotechConfigX = new ControllerConfig();
                aerotechConfigX.Name = "X";
                aerotechConfigX.DeviceID = "X";
                aerotechConfigX.IsEnabled = false;
                var aerotechDummyX = new AerotechMotionDummyController(aerotechConfigX, null, logger);
                motionDict.Add("X", aerotechDummyX);

                var aerotechConfigY = new ControllerConfig();
                aerotechConfigY.Name = "Y";
                aerotechConfigY.DeviceID = "Y";
                aerotechConfigY.IsEnabled = false;
                var aerotechDummyY = new AerotechMotionDummyController(aerotechConfigY, null, logger);
                motionDict.Add("Y", aerotechDummyY);

                var aerotechConfigZ = new ControllerConfig();
                aerotechConfigZ.Name = "Z";
                aerotechConfigZ.DeviceID = "Z";
                aerotechConfigZ.IsEnabled = false;
                var aerotechDummyZ = new AerotechMotionDummyController(aerotechConfigZ, null, logger);
                motionDict.Add("Z", aerotechDummyZ);

                var aerotechConfigRotation = new ControllerConfig();
                aerotechConfigRotation.Name = "Rotation";
                aerotechConfigRotation.DeviceID = "Rotation";
                aerotechConfigRotation.IsEnabled = false;
                var aerotechDummyRotation = new AerotechMotionDummyController(aerotechConfigRotation, null, logger);
                motionDict.Add("Rotation", aerotechDummyRotation);

                PhotoLumAxesConfig axesConfig = new PhotoLumAxesConfig();
                axesConfig.AxisConfigs = new List<AxisConfig> { xAxisConfig, yAxisConfig, zAxisConfig, rotationAxisConfig };

                test.SimulatedMotionAxes = new Mock<PhotoLumAxes>(axesConfig, motionDict, null, logger, null);
                test.SimulatedMotionAxes.Setup(_ => _.GetPosition()).Returns(new XYZPosition(new MotorReferential(), 0.0, 0.0, 0.0));

            }
            test.HardwareManager.MotionAxes = test.SimulatedMotionAxes.Object;
        }
    }
}
