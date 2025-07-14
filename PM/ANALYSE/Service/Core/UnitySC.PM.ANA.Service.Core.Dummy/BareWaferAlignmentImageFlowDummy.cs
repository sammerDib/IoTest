using System.Collections.Generic;
using System.Threading;

using UnitySC.PM.ANA.Service.Core.BareWaferAlignment;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Core.Dummy
{
    public class BareWaferAlignmentImageFlowDummy : BareWaferAlignmentImageFlow
    {
        public BareWaferAlignmentImageFlowDummy(BareWaferAlignmentImageInput input) : base(input)
        {
        }

        protected override void Process()
        {
            var resultImageFlowStarted = new BareWaferAlignmentImage()
            {
                ImageState = FlowState.InProgress,
                EdgePosition = Input.EdgePosition,
            };
            SetProgressMessage($"Dummy Bare wafer alignment Image in progress", resultImageFlowStarted);

            Thread.Sleep(1000);
            var image = new ServiceImage();
            image.LoadFromFile(@".\Debug\DummyImages\EdgeTop.png");
            image.Type = ServiceImage.ImageType.RGB;

            var edgePointsList = new List<ServicePoint>();
            edgePointsList.Add(new ServicePoint(50, 70));

            var resultImageFlow = new BareWaferAlignmentImage()
            {
                ImageState = FlowState.Success,
                Image = image,
                Position = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0),
                EdgePosition = Input.EdgePosition,
                EdgePoints = edgePointsList
            };

            Logger.Debug($"{LogHeader}", resultImageFlow);
        }
    }
}
