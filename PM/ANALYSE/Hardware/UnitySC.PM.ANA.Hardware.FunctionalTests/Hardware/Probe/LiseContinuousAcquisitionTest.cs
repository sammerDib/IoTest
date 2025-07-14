using System.Collections.Generic;
using System.Threading;

using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Hardware.FunctionalTests.Hardware.Probe
{
    public class LiseContinuousAcquisitionTest : FunctionalTest
    {
        public override void Run()
        {
            GotoRef();
            foreach (var probe in HardwareManager.Probes)
            {
                var objectivesSelector = HardwareManager.ObjectivesSelectors["ObjectiveSelector01"];

                objectivesSelector.SetObjective(objectivesSelector.Config.Objectives[0]);
                if (probe.Value.Configuration is ProbeLiseConfig)
                {
                    LiseContinuousAcquisition((ProbeLise)probe.Value);
                }
            }
        }

        private void GotoRef()
        {
            Thread.Sleep(4000); // Wait for stage to be initialized and LOH, UOH parked
            var position = new XYZTopZBottomPosition(new StageReferential(), -209.2, 102.7758, 13.0992, 0.2366);
            HardwareManager.Axes.GotoPosition(position, AxisSpeed.Normal);
        }

        private void LiseContinuousAcquisition(ProbeLise probeLise)
        {
            var liseInputParams = CreateLiseInputParams();
            liseInputParams.NbMeasuresAverage = 4;

            probeLise.StartContinuousAcquisition(liseInputParams);
            var rawsignal = probeLise.LastRawSignal;
            probeLise.StopContinuousAcquisition();
        }

        private SingleLiseInputParams CreateLiseInputParams()
        {
            var tolerance = new LengthTolerance(10, LengthToleranceUnit.Nanometer);
            var probeSampleLayerMeasured = new ProbeSampleLayer(750.Nanometers(), tolerance, 1.4621);
            var probeLayers = new List<ProbeSampleLayer>() { probeSampleLayerMeasured };
            var sample = new ProbeSample(probeLayers, "REF 750UM", "SampleInfo");

            double qualityThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter
            double detectionThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter

            double gain = 1.8;
            int nbMeasures = 16;

            var probeInputParams = new SingleLiseInputParams(sample, gain, qualityThreshold, detectionThreshold, nbMeasures);

            return probeInputParams;
        }
    }
}
