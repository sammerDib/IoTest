using System.Collections.Generic;
using System.Windows;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Hardware.FilterWheel;
using UnitySC.PM.EME.Service.Core.Flows.Vignetting;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;

namespace UnitySC.PM.EME.Service.Core.Test.Flow
{
    [TestClass]
    public class VignettingTest : TestWithMockedHardware<VignettingTest>, ITestWithCamera, ITestWithFilterWheel
    {
        public Mock<FilterWheel> SimulatedFilterWheel { get; set; }

        public DummyIDSCamera SimulatedCamera { get; set; }
        public IEmeraCamera EmeraCamera { get; set; }

        protected override void PostGenericSetup()
        {
            Bootstrapper.SimulatedEmeraCamera.Setup(_ => _.IsAcquiring()).Returns(true);
        }

        [TestMethod]
        public void Vignetting_test()
        {
            var images = new List<DummyUSPImage>();

            for (int i = 0; i < 22; i++)
            {
                images.Add(new DummyUSPImage(400, 400, 10));
            }

            TestWithCameraHelper.SetupCameraWithImagesForSingleScaledAcquisition(images);

            SimulatedCamera.SetImageResolution(new Size(400, 400));

            var vignettingInput = new VignettingInput()
            {
                RangeType = ScanRangeType.Small,
            };

            var vignettingFlow = new VignettingFlow(vignettingInput, Bootstrapper.SimulatedEmeraCamera.Object);

            var result = vignettingFlow.Execute();
            Assert.AreEqual(FlowState.Success, result.Status.State);
        }

        [TestMethod]
        public void Filter_wheel_moves_to_only_image_without_vignetting()
        {
            var images = new List<DummyUSPImage>();

            for (int i = 0; i < 22; i++)
            {
                if (i == 3)
                {
                    images.Add(new DummyUSPImage(400, 400, 10));
                    continue;
                }

                images.Add(new DummyUSPImage(400, 400, 10, true));
            }

            TestWithCameraHelper.SetupCameraWithImagesForSingleScaledAcquisition(images);

            SimulatedCamera.SetImageResolution(new Size(400, 400));

            var vignettingInput = new VignettingInput()
            {
                RangeType = ScanRangeType.Small,
            };

            var vignettingFlow = new VignettingFlow(vignettingInput, Bootstrapper.SimulatedEmeraCamera.Object);

            vignettingFlow.Execute();

            //image without vignetting is fourth, min scan is -5 normalized to 355 and step is 0.5 so result should be 356.5
            Assert.IsTrue(vignettingFlow.Result.FilterWheelPosition == 356.5);
        }

        [TestMethod]
        public void Filter_wheel_moves_to_last_position_when_every_image_has_the_same_vignetting_value()
        {
            var images = new List<DummyUSPImage>();

            for (int i = 0; i < 22; i++)
            {
                images.Add(new DummyUSPImage(400, 400, 10, true));
            }

            TestWithCameraHelper.SetupCameraWithImagesForSingleScaledAcquisition(images);

            SimulatedCamera.SetImageResolution(new Size(400, 400));

            var vignettingInput = new VignettingInput()
            {
                RangeType = ScanRangeType.Small,
            };

            var vignettingFlow = new VignettingFlow(vignettingInput, Bootstrapper.SimulatedEmeraCamera.Object);

            vignettingFlow.Execute();

            //Every Image are the same, last position is 5, so the result should be 5
            Assert.IsTrue(vignettingFlow.Result.FilterWheelPosition == 5);
        }
    }
}
