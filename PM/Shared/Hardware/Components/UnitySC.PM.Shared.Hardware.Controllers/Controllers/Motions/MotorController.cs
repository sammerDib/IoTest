using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers
{
    public abstract class MotorController : AxesControllerBase
    {
        public MotorController(ControllerConfig controllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger)
            : base(controllerConfig, globalStatusServer, logger)
        {
        }

        public List<double> ConvertCoordinatesForAxes(List<IAxis> axesList, List<double> coordList)
        {
            var ConvertCoordinates = new List<double>();

            for (int i = 0; i < axesList.Count; i++)
            {
                MotorizedAxisConfig config;
                if (axesList[i].AxisConfiguration is MotorizedAxisConfig)
                {
                    config = (MotorizedAxisConfig)axesList[i].AxisConfiguration;
                    double convertedCoord = coordList[i] + config.PositionZero.Millimeters;
                    CheckAxisLimits(axesList[i], convertedCoord);
                    ConvertCoordinates.Add(convertedCoord * config.MotorDirection);
                }
            }
            return ConvertCoordinates;
        }

        public static void CheckAxisLimits(IAxis axis, double wantedPosition)
        {
            // Limitation des valeurs min et max
            if (wantedPosition > axis.AxisConfiguration.PositionMax.Millimeters)
                throw new Exception($"CheckAxisLimits for {axis.AxisID} : {wantedPosition:0.000} mm out of axis maximum limit {axis.AxisConfiguration.PositionMax:0.000} mm");
            else if (wantedPosition < axis.AxisConfiguration.PositionMin.Millimeters)
                throw new Exception($"CheckAxisLimits for {axis.AxisID} : {wantedPosition:0.000} mm out of axis minimum limit {axis.AxisConfiguration.PositionMin:0.000} mm");
        }

        protected bool CheckNoMotorMoving()
        {
            bool noMotorsMoving = true;
            foreach (var axis in ControlledAxesList)
                noMotorsMoving &= !axis.Moving;
            return noMotorsMoving;
        }

        public static void ConvertSpeedEnum(IAxis axis, AxisSpeed axisSpeedEnum, out double speed, out double accel)
        {
            if (axis.AxisConfiguration is MotorizedAxisConfig)
            {
                var axisConfig = (MotorizedAxisConfig)axis.AxisConfiguration;
                switch (axisSpeedEnum)
                {
                    case AxisSpeed.Fast:
                        speed = axisConfig.SpeedFast;
                        accel = axisConfig.AccelFast;
                        break;

                    case AxisSpeed.Measure:
                        speed = axisConfig.SpeedMeasure;
                        accel = axisConfig.AccelMeasure;
                        break;

                    case AxisSpeed.Normal:
                        speed = axisConfig.SpeedNormal;
                        accel = axisConfig.AccelNormal;
                        break;

                    case AxisSpeed.Slow:
                        speed = axisConfig.SpeedSlow;
                        accel = axisConfig.AccelSlow;
                        break;

                    default:
                        //_logger?.Error("ACS SetSpeed : Unknown speed: " + speedsList[index]);
                        speed = axisConfig.SpeedMeasure;
                        accel = axisConfig.AccelMeasure;
                        break;
                }
            }
            else
                throw new Exception("Axis configuration is not a Motorized Axis configuration. Check configuration");
        }

        public override void SetPosAxis(List<double> coordsList, List<IAxis> axesList, List<AxisSpeed> speedsList)
        {
            int index = 0;
            var speedsConvertedToDouble = new List<double>();
            var accelsConvertedToDouble = new List<double>();
            foreach (IAxis axis in axesList)
            {
                ConvertSpeedEnum(axis, speedsList[index], out double speed, out double accel);
                speedsConvertedToDouble.Add(speed);
                accelsConvertedToDouble.Add(accel);
                index++;
            }
            SetPosAxisWithSpeedAndAccel(coordsList, axesList, speedsConvertedToDouble, accelsConvertedToDouble);
        }
    }
}
