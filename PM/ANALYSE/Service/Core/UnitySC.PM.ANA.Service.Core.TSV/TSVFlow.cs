using System;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.CD;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Format.Metro.TSV;
using TSVResult = UnitySC.PM.ANA.Service.Interface.Algo.TSVResult;
using UnitySC.PM.Shared.Hardware.Service.Interface.IOComponent;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;

namespace UnitySC.PM.ANA.Service.Core.TSV
{
    public class TSVFlow : FlowComponent<TSVInput, TSVResult, TSVConfiguration>
    {
        private AnaHardwareManager _hardwareManager;
        private ICameraManager _cameraManager;

        public TSVFlow(TSVInput input) : base(input, "TSVFlow")
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _cameraManager = ClassLocator.Default.GetInstance<ICameraManager>();
        }

        protected override void Process()
        {
            SetProgressMessage("Starting TSV Depth");

            bool isFailMeasureCD = false;
            switch (Input.Shape)
            {
                case TSVShape.Elipse:
                    SetProgressMessage("Starting TSV Ellipse Critical Dimension");
                    var ellipseCriticalDimensionResult = ExecuteEllipseCriticalDimension();
                    CheckCancellation();

                    if (ellipseCriticalDimensionResult.Status.State != FlowState.Success)
                    {
                        isFailMeasureCD = true;
                        Result.ResultImage = ellipseCriticalDimensionResult.ResultImage;
                    }
                    else
                    {
                        Result.Length = ellipseCriticalDimensionResult.Length;
                        Result.Width = ellipseCriticalDimensionResult.Width;
                        Result.ResultImage = ellipseCriticalDimensionResult.ResultImage;
                    }
                    break;

                case TSVShape.Circle:
                    SetProgressMessage("Starting TSV Circle Critical Dimension");
                    var circleCriticalDimensionResult = CircleShapeExecute();
                    CheckCancellation();

                    if (circleCriticalDimensionResult.Status.State != FlowState.Success)
                    {
                        isFailMeasureCD = true;
                        Result.ResultImage = circleCriticalDimensionResult.ResultImage;
                    }
                    else
                    {
                        Result.Length = circleCriticalDimensionResult.Diameter;
                        Result.Width = circleCriticalDimensionResult.Diameter;
                        Result.ResultImage = circleCriticalDimensionResult.ResultImage;
                    }
                    break;

                case TSVShape.Rectangle:
                    throw new NotImplementedException("Rectangle Critical Dimension not yet implemented !");
            }

            var tsvDepthResult = ExecuteTsvDepth();
            Result.Depth = tsvDepthResult.Depth;
            Result.QualityScore = tsvDepthResult.Quality / 100.0;
            Result.DepthRawSignal = tsvDepthResult.DepthRawSignal;

            bool isFailMeasureDepth = tsvDepthResult.Status.State != FlowState.Success;
            if (!isFailMeasureCD)
            {
                if (isFailMeasureDepth)
                    throw new PartialException($"TSV Depth failed : {tsvDepthResult.Status.Message}");
            }
            else
            {
                if (isFailMeasureDepth)
                    throw new Exception($"TSV CD & Depth failed : {tsvDepthResult.Status.Message}");
                else
                    throw new PartialException($"TSV CD failed.");
            }
        }

        protected EllipseCriticalDimensionResult ExecuteEllipseCriticalDimension()
        {
            var img = HardwareUtils.AcquireCameraImage(_hardwareManager, _cameraManager, Input.CameraId);

            CenteredRegionOfInterest roi = Input.RegionOfInterest;
            var objective = _hardwareManager.GetObjectiveInUseByCamera(Input.CameraId);
            var input = new EllipseCriticalDimensionInput(img, objective.DeviceID, roi, Input.ApproximateLength, Input.ApproximateWidth, Input.LengthTolerance, Input.WidthTolerance);

            var CDflow = new EllipseCriticalDimensionFlow(input);
            CDflow.CancellationToken = CancellationToken;
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
                SetProgressMessage($"[CDFlow] {status.State} - {status.Message}");
            else
                SetProgressMessage($"[CDFlow] {status.State} -  Length: {statusData.Length} - Width: {statusData.Width}");
        }

        private CircleCriticalDimensionResult CircleShapeExecute()
        {
            CircleCriticalDimensionResult res = null;
            switch (Input.ShapeDetectionMode)
            {
                default:
                case ShapeDetectionModes.AverageInArea:
                    res = ExecuteCircleCriticalDimension();
                    break;
                case ShapeDetectionModes.Central:
                    res = ExecuteCircleMetroCDmension();
                    break;

            }
            return res;
        }

        private CircleCriticalDimensionResult ExecuteCircleCriticalDimension()
        {
            var img = HardwareUtils.AcquireCameraImage(_hardwareManager, _cameraManager, Input.CameraId);

            CenteredRegionOfInterest roi = Input.RegionOfInterest;
            var objective = _hardwareManager.GetObjectiveInUseByCamera(Input.CameraId);
            var input = new CircleCriticalDimensionInput(img, objective.DeviceID, roi, Input.ApproximateLength, Input.LengthTolerance);

            var CDflow = new CircleCriticalDimensionFlow(input);
            CDflow.CancellationToken = CancellationToken;
            CircleCriticalDimensionResult res;
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

        private CircleCriticalDimensionResult ExecuteCircleMetroCDmension()
        {
            var img = HardwareUtils.AcquireCameraImage(_hardwareManager, _cameraManager, Input.CameraId);

            CenteredRegionOfInterest roi = Input.RegionOfInterest;
            var objective = _hardwareManager.GetObjectiveInUseByCamera(Input.CameraId);
            var input = new CircleCriticalDimensionInput(img, objective.DeviceID, roi, Input.ApproximateLength, Input.LengthTolerance);

            var CDMetroflow = new CircleMetroCDFlow(input);
            CDMetroflow.CancellationToken = CancellationToken;
            CircleCriticalDimensionResult res;
            try
            {
                CDMetroflow.StatusChanged += CDflow_StatusChanged;
                res = CDMetroflow.Execute();
            }
            finally
            {
                CDMetroflow.StatusChanged -= CDflow_StatusChanged;
            }
            return res;
        }

        private void CDflow_StatusChanged(FlowStatus status, CircleCriticalDimensionResult statusData)
        {
            if (status.State != FlowState.Success)
                SetProgressMessage($"[CDFlow] {status.State} - {status.Message}");
            else
                SetProgressMessage($"[CDFlow] {status.State} -  Diameter: {statusData.Diameter}");
        }

        private TSVDepthResult ExecuteTsvDepth()
        {
            var input = new TSVDepthInput(Input.ApproximateDepth, Input.ApproximateWidth, Input.DepthTolerance, Input.Probe, Input.AcquisitionStrategy, Input.MeasurePrecision);

            var tsvDepthflow = new TSVDepthFlow(input);
            tsvDepthflow.CancellationToken = CancellationToken;
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
