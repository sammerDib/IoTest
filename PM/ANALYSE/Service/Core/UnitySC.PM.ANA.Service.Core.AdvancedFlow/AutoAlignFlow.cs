using System;
using System.Linq;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Autofocus;
using UnitySC.PM.ANA.Service.Core.Autolight;
using UnitySC.PM.ANA.Service.Core.BareWaferAlignment;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.AdvancedFlow
{
    public class AutoAlignFlow : FlowComponent<AutoAlignInput, AutoAlignResult>
    {
        private AnaHardwareManager _hardwareManager;

        public AutoAlignFlow(AutoAlignInput input) : base(input, "AutoAlignFlow")

        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
        }

        protected override void Process()
        {
            var camera = _hardwareManager.Cameras.FirstOrDefault(x => x.Value.Name == "Camera Up").Value;

            // TODO we should look for the first single lise up
            var probeID = "ProbeLiseUp"; //TODO ProbeLISEBottom
            var afInput = new AFLiseInput(probeID);
            var afFlow = new AFLiseFlow(afInput);
            var afTask = new FlowTask<AFLiseInput, AFLiseResult, AFLiseConfiguration>(afFlow);

            var alInput = new AutolightInput(camera.DeviceID, "VIS_WHITE_LED", 30, new ScanRangeWithStep(1, 100, 0.1));
            var alFlow = new AutolightFlow(alInput);
            var alTask = new FlowTask<AutolightInput, AutolightResult, AutolightConfiguration>(alFlow);

            var bwaInput = new BareWaferAlignmentInput(Input.Wafer, camera.DeviceID);
            var bwaFlow = new BareWaferAlignmentFlow(bwaInput);
            var bwaTask = new FlowTask<BareWaferAlignmentInput, BareWaferAlignmentChangeInfo, BareWaferAlignmentConfiguration>(bwaFlow);

            CheckCancellation();
            SetProgressMessage("Go to home position", Result);
            _hardwareManager.Axes.GotoHomePos(AxisSpeed.Fast);
            _hardwareManager.Axes.WaitMotionEnd(10000);

            CheckCancellation();
            SetProgressMessage("Start AF LISE", Result);
            afTask.Start();
            afTask.Wait();
            if (IsResultStateError(afTask.Result, afTask.Name))
                throw new Exception("AF LISE Failure");

            CheckCancellation();
            SetProgressMessage("Start Auto Light", Result);
            alTask.Start();
            alTask.Wait();
            if (IsResultStateError(alTask.Result, alTask.Name))
                throw new Exception("Auto Light Failure");

            CheckCancellation();
            SetProgressMessage("Start BWA", Result);
            bwaTask.Start();
            bwaTask.Wait();
            if (IsResultStateError(bwaTask.Result, bwaTask.Name))
                throw new Exception("BWA Failure");
        }
    }
}
