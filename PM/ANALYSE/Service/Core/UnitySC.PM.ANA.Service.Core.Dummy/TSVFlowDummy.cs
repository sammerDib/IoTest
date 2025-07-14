using System;

using UnitySC.PM.ANA.Service.Core.TSV;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Format.Metro.TSV;

namespace UnitySC.PM.ANA.Service.Core.Dummy
{
    public class TSVFlowDummy : TSVFlow
    {
        public TSVFlowDummy(TSVInput input) : base(input)
        {
        }

        protected override void Process()
        {         
            // Note de rti : TSV never fail right now (no exception) so we do not need to handle partial exception, only DummyCD could send 
            SetProgressMessage("Starting TSV Depth");
            var tsvDepthResult = DummyExecuteTsvDepth();
            Result.Depth = tsvDepthResult.Depth;
            Result.QualityScore = tsvDepthResult.Quality / 100.0;
            Result.DepthRawSignal = tsvDepthResult.DepthRawSignal;
            CheckCancellation();

            switch (Input.Shape)
            {

                case TSVShape.Circle:
                case TSVShape.Elipse:
                    SetProgressMessage("Starting TSV Ellipse or Circle Critical Dimension");
                    EllipseCriticalDimensionInput ellipseCDDummyInput = new EllipseCriticalDimensionInput(new ServiceImage(), "1", Input.RegionOfInterest, Input.ApproximateLength, Input.ApproximateWidth, Input.LengthTolerance, Input.WidthTolerance);
                    var ellipseCriticalDimensionResult = new EllipseCriticalDimensionFlowDummy(ellipseCDDummyInput).Execute();
                    CheckCancellation();
                    if ((ellipseCriticalDimensionResult.Status.State != FlowState.Success) && (tsvDepthResult.Status.State != FlowState.Success))
                    {
                        throw new Exception($"TSV Ellipse or Circle Critical Dimension and Depth failed.");
                    }
                    if (ellipseCriticalDimensionResult.Status.State != FlowState.Success)
                    {
                        throw new PartialException($"TSV Ellipse or circle Critical Dimension failed.");
                    }
                    Result.Length = ellipseCriticalDimensionResult.Length;
                    Result.Width = ellipseCriticalDimensionResult.Width;
                    Result.ResultImage = ellipseCriticalDimensionResult.ResultImage;
                    break;

                case TSVShape.Rectangle:
                    throw new NotImplementedException("Rectangle Critical Dimension not yet implemented !");
            }

            var targetDepth_um = (Input as TSVInput)?.ApproximateDepth.Micrometers ?? 0.0;
            var tolerance_um = (Input as TSVInput)?.DepthTolerance.Micrometers ?? 5.0;
            bool isDummyFailMeasureDepth = Math.Abs(targetDepth_um - tsvDepthResult.Depth.Micrometers) > 0.98 * tolerance_um;
            if (isDummyFailMeasureDepth)
            {
                Result.Depth = null;
                 throw new PartialException($"TSV Depth failed : Dummy NotMeasured - delta to target too big ");
            }

        }

        private EllipseCriticalDimensionResult DummyExecuteEllipseCriticalDimension()
        {
            var input = new EllipseCriticalDimensionInput(null, "1", null, Input.ApproximateLength, Input.ApproximateWidth, Input.LengthTolerance, Input.WidthTolerance);

            var CDflow = new EllipseCriticalDimensionFlowDummy(input);
            EllipseCriticalDimensionResult res;
            try
            {
                CDflow.StatusChanged += CDflow_StatusChanged;
                res = CDflow.Execute();
            }
            finally
            {
                CDflow.StatusChanged -= CDflow_StatusChanged;
            }
            return res;
        }

        private void CDflow_StatusChanged(FlowStatus status, EllipseCriticalDimensionResult statusData)
        {
            if (status.State != FlowState.Success)
                SetProgressMessage($"[CDFlowDummy] {status.State} - {status.Message}");
            else
                SetProgressMessage($"[CDFlowDummy] {status.State} -  Length: {statusData.Length} - Width: {statusData.Width}");
        }

        private TSVDepthResult DummyExecuteTsvDepth()
        {
            var input = new TSVDepthInput(Input.ApproximateDepth, Input.ApproximateWidth, Input.DepthTolerance, Input.Probe, Input.AcquisitionStrategy, Input.MeasurePrecision);

            var tsvDepthflow = new TSVDepthFlowDummy(input);

            TSVDepthResult res;
            try
            {
                tsvDepthflow.StatusChanged += TsvDepthflow_StatusChanged;
                res = tsvDepthflow.Execute();
            }
            finally
            {
                tsvDepthflow.StatusChanged -= TsvDepthflow_StatusChanged;
            }
            return res;
        }

        private void TsvDepthflow_StatusChanged(FlowStatus status, TSVDepthResult statusData)
        {
            if (status.State != FlowState.Success)
                SetProgressMessage($"[TSVDepth] {status.State} - {status.Message}");
            else
                SetProgressMessage($"[TSVDepth] {status.State} -  Depth: {statusData.Depth}");
        }
    }
}
