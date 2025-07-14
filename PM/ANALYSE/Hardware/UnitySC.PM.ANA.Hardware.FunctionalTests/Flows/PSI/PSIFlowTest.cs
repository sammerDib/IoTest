using UnitySC.PM.ANA.Service.Core.PSI;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Service.Interface.Algo.PSIInput;

namespace UnitySC.PM.ANA.Hardware.FunctionalTests.Flows.PSI
{
    public class PSIFlowTest : FunctionalTest
    {
        /// <summary>
        /// Save images took during a nominal PSI use-case.
        /// </summary>
        public override void Run()
        {
            // Given
            var psiInput = new PSIInput("1", "1", 5.Micrometers(), 6, 5, new CenteredRegionOfInterest(), PhaseCalculationAlgo.Hariharan, PhaseUnwrappingAlgo.Goldstein, 630.Nanometers());

            var psiFlow = new PSIFlow(psiInput);
            psiFlow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;

            // When
            psiFlow.Execute();

            // Then
            // Check images saved in C:\Temp\ANALYSE\logs\PSI\<datetime>\
        }
    }
}
