using System.Collections.Generic;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.AlignmentFlow
{
    public class LiseHFXYAnalysisFlow : FlowComponent<AlignmentLiseHFXYAnalysisInput, AlignmentLiseHFXYAnalysisResult,
        AlignmentLiseHFXYAnalysisConfiguration>
    {
        private readonly AnaHardwareManager _hardwareManager;

        private AlignmentLiseHFXYAnalysisResult _globalResult = new AlignmentLiseHFXYAnalysisResult();

        public LiseHFXYAnalysisFlow(AlignmentLiseHFXYAnalysisInput input) : base(input, "LiseHFXYAnalysisFlow")
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
        }

        delegate void ReportPercentageProgressDelegate(double progressPercentage, object data);

        protected override void Process()
        {
            void ReportPercentageProgressForMeasurement(double progressPercentage, object TSV = null)
            {
                if (TSV is List<double>)
                {
                    _globalResult.TSV = (List<double>)TSV;
                }
                SetProgressMessage($"Measurement Progress : {progressPercentage:F2}%", _globalResult);
            }

            void ReportPercentageProgressForComputation(double progressPercentage, object _)
            {
                SetProgressMessage($"Computation Progress : {progressPercentage:F2}%");
            }

            ReportPercentageProgressDelegate reportPercentageProgressForMeasurement =
                ReportPercentageProgressForMeasurement;
            ReportPercentageProgressDelegate reportPercentageProgressForComputation =
                ReportPercentageProgressForComputation;
            
            var xyMeasurement = new LiseHFXYMeasurement(Logger, reportPercentageProgressForMeasurement);
            var xyComputation = new LiseHFXYComputation(Logger, reportPercentageProgressForComputation);
            
            var intermediateResult = xyMeasurement.DoMeasurement(Input, CancellationToken);
            _globalResult.TSV = intermediateResult.TSV;
            SetProgressMessage("Measurement completed, moving to computation.",_globalResult);
            var result = xyComputation.DoComputation(intermediateResult, CancellationToken);
            Result = result;
        }
    }
}
