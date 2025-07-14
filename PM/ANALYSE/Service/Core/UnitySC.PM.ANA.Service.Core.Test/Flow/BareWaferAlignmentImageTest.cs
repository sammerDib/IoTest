using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow
{
    /// <summary>
    /// A test-only position type used here for error case
    /// </summary>
    internal class BuggyPosition : PositionBase
    {
        public BuggyPosition(ReferentialBase baseRef) : base(baseRef)
        {
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override bool Near(PositionBase otherPosition, Length tolerance = null)
        {
            throw new NotImplementedException();
        }
    }

    [TestClass]
    public class BareWaferAlignmentImageTest : WaferAlignmentTestBase
    {
        private void DiscardFirstImageWhichIsANotchOne()
        {
            var cameraManager = ClassLocator.Default.GetInstance<ICameraManager>();
            var hwMngr = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            var camera = hwMngr.Cameras[CameraUpId];
            cameraManager.GetNextCameraImage(camera);
        }

        [TestMethod]
        public void Expect_error_when_using_unknown_position_type()
        {
            // given

            var wdc = new WaferDimensionalCharacteristic
            {
                Diameter = 300.Millimeters(),
                WaferShape = WaferShape.Notch
            };

            // the first image returned by the fake camera is the notch. In this test, we discard that image to work on a edge one
            SetupCameraWithImagesForBWA300Notched(1);
            DiscardFirstImageWhichIsANotchOne();
            
            var imageCenterPosition = new BuggyPosition(new WaferReferential());
            var input = new BareWaferAlignmentImageInput(wdc, CameraUpId, imageCenterPosition, WaferEdgePositions.Left);
            var flowUnderTest = new BareWaferAlignment.BareWaferAlignmentImageFlow(input);

            // when
            var result = flowUnderTest.Execute();

            // then
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        [TestMethod]
        public void Expect_extracted_contour_in_nominal_case()
        {
            // given

            var wdc = new WaferDimensionalCharacteristic
            {
                Diameter = 300.Millimeters(),
                WaferShape = WaferShape.Notch
            };

            // the first image returned by the fake camera is the notch. In this test, we discard that image to work on a edge one
            SetupCameraWithImagesForBWA300Notched(1);
            DiscardFirstImageWhichIsANotchOne();

            var imageCenterPosition = new XYZTopZBottomPosition(new WaferReferential(), 23, 148, 0, 0);
            var input = new BareWaferAlignmentImageInput(wdc, CameraUpId, imageCenterPosition, WaferEdgePositions.Top);
            var flowUnderTest = new BareWaferAlignment.BareWaferAlignmentImageFlow(input);

            // when
            var result = flowUnderTest.Execute();

            // then
            int expectedContourPointsCount = 833;
            Assert.IsNotNull(result);
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(expectedContourPointsCount, result.EdgePoints.Count);
        }
    }
}
