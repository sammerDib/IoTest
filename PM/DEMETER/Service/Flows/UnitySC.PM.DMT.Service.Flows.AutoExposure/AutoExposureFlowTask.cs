using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.DMT.Service.Flows.AcquireOneImage;
using UnitySC.PM.DMT.Service.Flows.Deflectometry;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;

using AcquireOneImageFlowTask = UnitySC.PM.Shared.Flow.Implementation.FlowTask<
    UnitySC.PM.DMT.Service.Interface.Flow.AcquireOneImageInput,
    UnitySC.PM.DMT.Service.Interface.Flow.AcquireOneImageResult>;
using AcquirePhaseImagesForPeriodAndDirectionFlowTask = UnitySC.PM.Shared.Flow.Implementation.FlowTask<
    UnitySC.PM.DMT.Service.Interface.Flow.AcquirePhaseImagesForPeriodAndDirectionInput,
    UnitySC.PM.DMT.Service.Interface.Flow.AcquirePhaseImagesForPeriodAndDirectionResult,
    UnitySC.PM.DMT.Service.Interface.Flow.AcquirePhaseImagesForPeriodAndDirectionConfiguration>;

namespace UnitySC.PM.DMT.Service.Flows.AutoExposure
{
    public class AutoExposureFlowTask : FlowTask<AutoExposureInput, AutoExposureResult, AutoExposureConfiguration>
    {
        public AutoExposureFlowTask(AutoExposureFlow flowComponent, TimeSpan? timeout = null) : base(flowComponent, timeout)
        {
        }

        public AutoExposureFlowTask(AutoExposureFlow flowComponent, CancellationTokenSource cancellationTokenSource) : base(flowComponent, cancellationTokenSource)
        {
        }

        public AcquireOneImageFlowTask ContinueWithAcquireOneImageFlow(AcquireOneImageFlow continuationFlow)
        {
            return ContinueWith(continuationFlow, autoExposureFlowTask =>
            {
                if (Status != TaskStatus.RanToCompletion)
                {
                    throw new TaskCanceledException();
                }
                if (Result.Status.State == FlowState.Success)
                {
                    continuationFlow.Input.ExposureTimeMs = Result.ExposureTimeMs;
                }
                else if (Configuration.IgnoreAutoExposureFailure)
                {
                    continuationFlow.Input.ExposureTimeMs = GetDefaultExposureTimeMsIfFailure(continuationFlow);
                }
                else
                {
                    throw new TaskCanceledException();
                }
            });
        }

        public AcquirePhaseImagesForPeriodAndDirectionFlowTask ContinueWithAcquirePhaseImagesForPeriodAndDirectionFlow(
            AcquirePhaseImagesForPeriodAndDirectionFlow continuationFlow)
        {
            return ContinueWith(continuationFlow, autoExposureFlowTask =>
            {
                if (Status != TaskStatus.RanToCompletion)
                {
                    throw new TaskCanceledException();
                }
                if (Result.Status.State == FlowState.Success)
                {
                    continuationFlow.Input.ExposureTimeMs = Result.ExposureTimeMs;
                }
                else if (Configuration.IgnoreAutoExposureFailure)
                {
                    continuationFlow.Input.ExposureTimeMs = GetDefaultExposureTimeMsIfFailure(continuationFlow);
                }
                else
                {
                    throw new TaskCanceledException();
                }
            });
        }
        
        private double GetDefaultExposureTimeMsIfFailure(AcquireOneImageFlow flow)
        {
            return Configuration.DefaultAutoExposureSetting
                .First(defaultSetting => defaultSetting.Measure == flow.Input.MeasureType &&
                                         defaultSetting.WaferSide == flow.Input.CameraSide)
                .DefaultExposureTimeMsIfFailure;
        }
        
        private double GetDefaultExposureTimeMsIfFailure(AcquirePhaseImagesForPeriodAndDirectionFlow flow)
        {
            return Configuration.DefaultAutoExposureSetting
                .First(defaultSetting => defaultSetting.WaferSide == flow.Input.AcquisitionSide &&
                                         defaultSetting.Measure == Interface.Measure.MeasureType.DeflectometryMeasure)
                .DefaultExposureTimeMsIfFailure;
        }
    }
}
