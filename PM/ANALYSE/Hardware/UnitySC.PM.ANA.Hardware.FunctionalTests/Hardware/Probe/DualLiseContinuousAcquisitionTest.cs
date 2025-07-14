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
    public class DualLiseContinuousAcquisitionTest : FunctionalTest
    {
        public override void Run()
        {
            GotoRef();
            foreach (var probe in HardwareManager.Probes)
            {
                var objectivesSelector = HardwareManager.ObjectivesSelectors["ObjectiveSelector01"];

                objectivesSelector.SetObjective(objectivesSelector.Config.Objectives[0]);
                if (probe.Value.Configuration is ProbeDualLiseConfig)
                {
                    DualLiseContinuousAcquisition((ProbeDualLise)probe.Value);
                }
            }
        }

        private void GotoRef()
        {
            Thread.Sleep(4000); // Wait for stage to be initialized and LOH, UOH parked
            var position = new XYZTopZBottomPosition(new StageReferential(), -209.2, 102.7758, 13.0992, 0.2366);
            HardwareManager.Axes.GotoPosition(position, AxisSpeed.Normal);
        }

        private void DualLiseContinuousAcquisition(ProbeDualLise probeLiseDouble)
        {
            var dualLiseInputParams = CreateDualLiseInputParams();
            dualLiseInputParams.ProbeUpParams.NbMeasuresAverage = 4;

            probeLiseDouble.StartContinuousAcquisition(dualLiseInputParams);
            var rawsignal = probeLiseDouble.LastRawSignal;
            probeLiseDouble.StopContinuousAcquisition();
        }

        private DualLiseInputParams CreateDualLiseInputParams()
        {
            var tolerance = new LengthTolerance(10, LengthToleranceUnit.Nanometer);
            var probeSampleLayerMeasured = new ProbeSampleLayer(750.Nanometers(), tolerance, 0.0);
            var probeLayers = new List<ProbeSampleLayer>() { probeSampleLayerMeasured };
            var sample = new ProbeSample(probeLayers, "REF 750UM", "SampleInfo");

            double qualityThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter
            double detectionThreshold = 0.1; // We have to provide something to the dll but the values ​​don't matter
            int nbMeasures = 16;
            double gain = 1.8;

            var probeUpInputParameters = new SingleLiseInputParams(sample, gain, qualityThreshold, detectionThreshold, nbMeasures);
            var probeDownInputParameters = new SingleLiseInputParams(sample, gain, qualityThreshold, detectionThreshold, nbMeasures);
            var probeInputParamsDouble = new DualLiseInputParams(sample, probeUpInputParameters, probeDownInputParameters);

            return probeInputParamsDouble;
        }
    }
}
