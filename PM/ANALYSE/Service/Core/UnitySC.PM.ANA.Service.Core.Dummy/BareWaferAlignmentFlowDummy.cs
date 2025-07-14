using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.BareWaferAlignment;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Dummy
{
    public class BareWaferAlignmentFlowDummy : BareWaferAlignmentFlow
    {
        private AnaHardwareManager _hardwareManager;

        public BareWaferAlignmentFlowDummy(BareWaferAlignmentInput input) : base(input)
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
        }

        protected override void Process()
        {
            var iswaferclamped = Task.Run(async () => await IsWaferClampedAsync().ConfigureAwait(false)).Result;
            if (!iswaferclamped)
            {
                string errorMsg = "The BWA failed because it was impossible to clamp the wafer";
                throw new Exception($"Error during {Name} : {errorMsg}");
            }
            var edgePointsList = new List<ServicePoint>();
            edgePointsList.Add(new ServicePoint(10, 10));
            edgePointsList.Add(new ServicePoint(1154, 862));
            var imageResult = new BareWaferAlignmentImage();
            var imageStarted = new BareWaferAlignmentImage();
            var image = new ServiceImage();

            // We do not use the same folder for Tests and application execution
            try
            {
                image.LoadFromFile(@".\Debug\DummyImages\EdgeTop.png");
            }
            catch
            {
                image.LoadFromFile(@".\DummyImages\EdgeTop.png");
            }

            image.Type = ServiceImage.ImageType.RGB;
            CheckCancellation();
            imageResult.Image = image;
            imageResult.EdgePoints = edgePointsList;
            imageResult.ImageState = FlowState.Success;

            if (Input.ImageTop is null)
            {
                imageStarted.ImageState = FlowState.InProgress;
                imageStarted.EdgePosition = WaferEdgePositions.Top;
                SetProgressMessage("Image Up Acquiring", imageStarted);

                imageResult.Position = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0);
                imageResult.EdgePosition = WaferEdgePositions.Top;

                Thread.Sleep(500);
                CheckCancellation();
                SetProgressMessage("Image Up Acquired", imageResult);
            }

            if (Input.ImageRight is null)
            {
                imageStarted.ImageState = FlowState.InProgress;
                imageStarted.EdgePosition = WaferEdgePositions.Right;
                SetProgressMessage("Image Right Acquiring", imageStarted);

                edgePointsList.Add(new ServicePoint(80, 100));
                imageResult.EdgePosition = WaferEdgePositions.Right;
                Thread.Sleep(500);
                CheckCancellation();
                SetProgressMessage("Image Right Acquired", imageResult);
            }

            if (Input.ImageLeft is null)
            {
                imageStarted.ImageState = FlowState.InProgress;
                imageStarted.EdgePosition = WaferEdgePositions.Left;
                SetProgressMessage("Image Left Acquiring", imageStarted);

                edgePointsList.Add(new ServicePoint(200, 130));
                imageResult.EdgePosition = WaferEdgePositions.Left;
                Thread.Sleep(500);
                CheckCancellation();
                SetProgressMessage("Image Left Acquired", imageResult);
            }

            if (Input.ImageBottom is null)
            {
                var notchLines = new List<(ServicePoint pt1, ServicePoint pt2)>();
                notchLines.Add((new ServicePoint(400, 0), new ServicePoint(400, 862)));
                imageResult.NotchLines = notchLines;

                imageStarted.ImageState = FlowState.InProgress;
                imageStarted.EdgePosition = WaferEdgePositions.Bottom;
                SetProgressMessage("Image Bottom Acquiring", imageStarted);

                edgePointsList.Add(new ServicePoint(120, 20));
                imageResult.EdgePosition = WaferEdgePositions.Bottom;
                Thread.Sleep(500);
                CheckCancellation();
                SetProgressMessage("Image Bottom Acquired", imageResult);
            }

            //var chuckCenterOffset = Input.ChuckCenter;
            var chuckCenterOffset = new XYPosition(new StageReferential(), 0.585, -2.023); // debug test affichage

            var bwaResult = new BareWaferAlignmentResult();
            bwaResult.ShiftX = 10.Micrometers();
            bwaResult.ShiftY = 10.Micrometers();
            bwaResult.ShiftStageX = bwaResult.ShiftX + chuckCenterOffset.X.Millimeters().ToUnit(LengthUnit.Micrometer);
            bwaResult.ShiftStageY = bwaResult.ShiftY + chuckCenterOffset.Y.Millimeters().ToUnit(LengthUnit.Micrometer); 
            bwaResult.Confidence = 0.9;
            bwaResult.Angle = 2.Degrees();
            Result = bwaResult;
            (FdcProvider as BareWaferAlignmentFlowFDCProvider).CreateFDC((BareWaferAlignmentResult)Result);
        }
    }
}
