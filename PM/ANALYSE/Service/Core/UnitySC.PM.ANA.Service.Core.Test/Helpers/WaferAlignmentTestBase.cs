using System.Collections.Generic;

using Moq;

using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Service.Shared.TestUtils.CameraTestUtils;

namespace UnitySC.PM.ANA.Service.Core.Test
{
    public class WaferAlignmentTestBase : TestWithMockedHardware<WaferAlignmentTestBase>, ITestWithCamera, ITestWithAxes
    {
        #region Interfaces properties

        public string CameraUpId { get; set; }
        public string CameraBottomId { get; set; }
        public Mock<CameraBase> SimulatedCameraUp { get; set; }
        public Mock<CameraBase> SimulatedCameraBottom { get; set; }
        public string ObjectiveUpId { get; set; }
        public string ObjectiveBottomId { get; set; }
        public Length PixelSizeX { get; set; }
        public Length PixelSizeY { get; set; }
        public Mock<IAxes> SimulatedAxes { get; set; }

        #endregion Interfaces properties

        protected override void PreGenericSetup()
        {
            PixelSizeX = 5.3.Micrometers();
            PixelSizeY = 5.3.Micrometers();
        }

        protected static void SetupCameraWithImagesForBWA300Notched(int nbStitchedNotchImages)
        {
            var images = new List<DummyUSPImage>();

           
            images.Add(new DummyUSPImage(GetDataPathInAlgoLibDir("metrological_wafer\\EdgeDetection_1_2X_VIS_X_-26047_Y_-147721.png")));
            images.Add(new DummyUSPImage(GetDataPathInAlgoLibDir("metrological_wafer\\EdgeDetection_3_2X_VIS_X_139077_Y_56190.png")));
            images.Add(new DummyUSPImage(GetDataPathInAlgoLibDir("metrological_wafer\\EdgeDetection_2_2X_VIS_X_23465_Y_148153.png")));

            for (int i = 0; i < nbStitchedNotchImages; i++) images.Add(new DummyUSPImage(GetDataPathInAlgoLibDir("metrological_wafer\\EdgeDetection_0_2X_VIS_X_0_Y_-150000.png")));

            TestWithCameraHelper.SetupCameraWithImages(images);
        }
    }
}
