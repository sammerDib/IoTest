using System.Collections.Generic;

using ACS.SPiiPlusNET;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Axes.PI.Dummy
{
    public class PIE709DummyController : PiezoController
    {
        public PIE709DummyController(PiezoControllerConfig configuration) : base(configuration)
        {
            InitAxesList();
        }

        /// <summary>
        /// Single axis piezo controller from PI, model E-709.
        /// </summary>
        public PIE709DummyController(PIE709ControllerConfig configuration, IGlobalStatusServer globalStatusServer, ILogger logger) : base(configuration, globalStatusServer, logger)
        {
            InitAxesList();
        }

        public override void Init(List<Message> initErrors)
        {
            base.Init(initErrors);
            Logger.Information("Init PIE709Controller as dummy");
        }
        public override int ConnectionId => -1;

        public override bool IsConnected => false;

        public override void ConfigureTriggerIn(Length initialPosition, Length motionStep)
        {

        }

        public override void ConfigureTriggerPositionDistance(double startThreshold, double stopThreshold, double triggerStep)
        {

        }

        public override void Connect()
        {

        }

        public override void DisableTriggerIn()
        {

        }

        public override void Disconnect()
        {

        }

        public override void Dispose()
        {

        }

        public override string GetAxisName()
        {
            return string.Empty;
        }

        public override TimestampedPosition GetCurrentAxisPosWithTimestamp(IAxis axis)
        {
            return null;
        }

        public override Length GetCurrentPosition()
        {
            return null;
        }

        public override Length GetPositionMax()
        {
            return null;
        }

        public override Length GetPositionMin()
        {
            return null;
        }

        public override double GetSpeed()
        {
            return double.NaN;
        }

        public override void InitControllerAxes(List<Message> initErrors)
        {

        }

        public override void InitializationAllAxes(List<Message> initErrors)
        {

        }

        public override void InitZBottomFocus()
        {

        }

        public override void InitZTopFocus()
        {

        }

        public override void RefreshAxisState(IAxis axis)
        {

        }

        public override void RefreshCurrentPos(List<IAxis> axis)
        {

        }

        public override void SetPosAxis(List<double> targetPositions, List<IAxis> axes, List<AxisSpeed> speeds)
        {

        }

        public override void SetPosAxisWithSpeedAndAccel(List<double> targetPositions, List<IAxis> axes, List<double> speeds, List<double> accels)
        {

        }

        public override void SetSpeed(double speed)
        {

        }

        public override void SetSpeedAccelAxis(List<IAxis> axes, List<double> speeds, List<double> accels)
        {

        }

        public override void SetSpeedAxis(List<IAxis> axes, List<AxisSpeed> speeds)
        {

        }

        public override void WaitMotionEnd(int timeout_ms, bool waitStabilization = true)
        {

        }

        private void InitAxesList()
        {
            AxesList = new List<IAxis>();
            var piezoAxisConfig1 = new AxisConfig()
            {
                AxisID = "ZPiezo-1",
                PositionMax = 10.Micrometers(),
                PositionMin = 0.Millimeters(),
                PositionHome = 5.Micrometers(),
            };
            var piezoAxisConfig2 = new AxisConfig()
            {
                AxisID = "ZPiezo-2",
                PositionMax = 100.Micrometers(),
                PositionMin = 0.Millimeters(),
                PositionHome = 50.Micrometers(),
            };
            AxesList.Add(new ACSAxis(piezoAxisConfig1, null));
            AxesList.Add(new ACSAxis(piezoAxisConfig2, null));
        }
    }
}
