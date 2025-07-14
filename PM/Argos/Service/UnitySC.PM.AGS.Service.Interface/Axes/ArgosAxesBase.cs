using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.AGS.Service.Interface.Axes
{
    public abstract class ArgosAxesBase : DeviceBase
    {
        public AxesConfig AxesConfiguration { get; }

        protected ArgosAxesBase(AxesConfig config, IGlobalStatusServer statusService, ILogger logger) : base(statusService, logger)
        {
            AxesConfiguration = config;
        }

        public AxisConfig GetAxisConfigById(string axisId)
        {
            return AxesConfiguration.AxisConfigs.Find(x => x.AxisID == axisId);
        }

        public static Speed ConvertAxisSpeed(AxisSpeed aSpeed, AxisConfig config)
        {
            switch (aSpeed)
            {
                case AxisSpeed.Slow: return new Speed(config.SpeedSlow);
                case AxisSpeed.Normal: return new Speed(config.SpeedNormal);
                case AxisSpeed.Fast: return new Speed(config.SpeedFast);
                case AxisSpeed.Measure: return new Speed(config.SpeedMeasure);
                default:
                    throw new ArgumentOutOfRangeException(nameof(aSpeed), aSpeed, null);
            }
        }

        public abstract void Move(params PMAxisMove[] moves);

        public abstract void RelativeMove(params PMAxisMove[] moves);

        public abstract void WaitMotionEnd(int timeout_ms);

        public abstract double GetNearestPSOPixelSize(double pixelSizeMicrometers, double waferRadiusMillimeters);

        public abstract void SetPSOInFixedWindowMode(double beginPosition, double endPosition, double psoAngularInterval);

        public abstract void DisablePSO();

        public abstract void Init(List<Message> initErrors);

        public abstract void StopAllMotion();

        public abstract void GoToLoadUnload(AxisSpeed speed);

        public abstract void GoToHomePosition(AxisSpeed speed);

        public abstract PositionBase GetPositon();

        public abstract void Home(AxisSpeed speed);
    }
}
