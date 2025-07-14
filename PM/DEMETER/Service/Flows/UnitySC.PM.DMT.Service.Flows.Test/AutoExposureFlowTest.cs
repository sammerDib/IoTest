using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using FluentAssertions;

using Matrox.MatroxImagingLibrary;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Service.Flows.AutoExposure;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Shared.TestUtils;
using UnitySC.PM.DMT.Shared;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.DMT.Service.Flows.Test
{
    [TestClass]
    public class AutoExposureFlowTest : TestWithMockedCameraAndScreenRequiringMIL<AutoExposureFlowTest>
    {
        [TestMethod]
        public void GivenGeneratedLighterImagesAndWhiteScreenResultShouldHaveAValidExposureTime()
        {
            // Given
            int cameraWidth = 100;
            int cameraHeight = 100;
            var imageSequence = CreateLighterImageSequence(cameraWidth, cameraHeight, 250);
            SimulatedCameraFront.Setup(camera => camera.Height).Returns(cameraHeight);
            SimulatedCameraFront.Setup(camera => camera.Width).Returns(cameraWidth);
            SimulatedCameraFront.Setup(camera => camera.MaxExposureTimeMs).Returns(60000000);
            SimulatedCameraFront.Setup(camera => camera.MinExposureTimeMs).Returns(10);
            SetupCameraWithImages(Side.Front, imageSequence);
            var roi = new ROI();
            roi.RoiType = RoiType.Rectangular;
            roi.Rect = new Rect(0, 0, cameraWidth, cameraHeight);
            var autoExpoInput = new AutoExposureInput(Side.Front, MeasureType.BrightFieldMeasure, roi,
                AcquisitionScreenDisplayImage.Color,
                Colors.White, null, null, 220, 20);
            var autoExpoFlow = new AutoExposureFlow(autoExpoInput, HardwareManager,
                ClassLocator.Default.GetInstance<IDMTInternalCameraMethods>());

            // When
            var result = autoExpoFlow.Execute();

            // Then
            SimulatedCameraManager.Verify(
                cameraManager => cameraManager.GetLastCameraImage(SimulatedCameraFront.Object), Times.Exactly(2));
            SimulatedScreenFront.Verify(screen => screen.ClearAsync(Colors.White), Times.Once);
            SimulatedScreenFront.Verify(screen => screen.Clear(), Times.Once);
            result.Status.State.Should().Be(FlowState.Success);
            result.ExposureTimeMs.Should().BeInRange(57.89, 57.91);
            result.ResultImage.Data.Should().Equal(imageSequence[1].ToServiceImage().Data);

            // Cleanup
            imageSequence.ForEach(img => img.Dispose());
        }

        [TestMethod]
        public void GivenGeneratedLighterFringeImagesWithLowPeriodResultShouldHaveAValidExposureTime()
        {
            // Given
            int cameraWidth = 100;
            int cameraHeight = 100;
            var imageSequence = CreateLighterFringeImageSequence(cameraWidth, cameraHeight, 250);
            SimulatedCameraFront.Setup(camera => camera.Height).Returns(cameraHeight);
            SimulatedCameraFront.Setup(camera => camera.Width).Returns(cameraWidth);
            SimulatedCameraFront.Setup(camera => camera.MaxExposureTimeMs).Returns(60000000);
            SimulatedCameraFront.Setup(camera => camera.MinExposureTimeMs).Returns(10);
            SimulatedScreenFront.Setup(screen => screen.Width).Returns(3840);
            SimulatedScreenFront.Setup(screen => screen.Height).Returns(2160);
            SetupCameraWithImages(Side.Front, imageSequence);
            var roi = new ROI();
            roi.RoiType = RoiType.Rectangular;
            roi.Rect = new Rect(0, 0, cameraWidth, cameraHeight);
            var fringeImage = new USPImageMil();
            var fringe = new Fringe { Period = 36, Periods = new List<int> { 36 } };
            var autoExposureInput = new AutoExposureInput(Side.Front, MeasureType.DeflectometryMeasure, roi,
                AcquisitionScreenDisplayImage.FringeImage, Colors.White, fringe, fringeImage, 220, 20);
            var autoExposureFlow = new AutoExposureFlow(autoExposureInput, HardwareManager,
                ClassLocator.Default.GetInstance<IDMTInternalCameraMethods>());

            // When
            var result = autoExposureFlow.Execute();

            // Then
            SimulatedScreenFront.Verify(screen => screen.DisplayImage(fringeImage), Times.Once());
            SimulatedScreenFront.Verify(screen => screen.Clear(), Times.Once());
            SimulatedScreenFront.Verify(screen => screen.ClearAsync(Colors.White), Times.Never());
            SimulatedCameraManager.Verify(
                cameraManager => cameraManager.GetLastCameraImage(SimulatedCameraFront.Object), Times.Exactly(2));
            result.Status.State.Should().Be(FlowState.Success);
            result.ExposureTimeMs.Should().BeInRange(58.02, 58.04);
            result.ResultImage.Data.Should().Equal(imageSequence[1].ToServiceImage().Data);

            // Cleanup
            imageSequence.ForEach(img => img.Dispose());
        }

        private static List<USPImageMil> CreateLighterImageSequence(int width, int height, int nbRandom)
        {
            return new List<USPImageMil>(2)
            {
                CreateGreyImageWithRandomWhiteBytes(width, height, 75, nbRandom),
                CreateGreyImageWithRandomWhiteBytes(width, height, 220, nbRandom)
            };
        }

        private static List<USPImageMil> CreateLighterFringeImageSequence(int width, int height, int nbRandom)
        {
            return new List<USPImageMil>(2)
            {
                CreateFringeImageWithRandomWhiteBytes(width, height, 75, 36, nbRandom),
                CreateFringeImageWithRandomWhiteBytes(width, height, 220, 36, nbRandom)
            };
        }

        private static USPImageMil CreateGreyImageWithRandomWhiteBytes(int width, int height, byte greyValue,
            int nbRandomBytes)
        {
            int nbPixels = width * height;
            byte[] imageBytes = Enumerable.Repeat(greyValue, nbPixels).ToArray();
            var rng = new Random();
            for (int i = 0; i < nbRandomBytes; i++)
            {
                int index = rng.Next(0, nbPixels);
                int value = rng.Next(greyValue + 1, 256);
                imageBytes[index] = (byte)value;
            }

            return CreateMilImageFromByteArray(width, height, imageBytes);
        }

        private static USPImageMil CreateFringeImageWithRandomWhiteBytes(int width, int height, byte greyValue,
            int period, int nbRandomBytes)
        {
            int nbPixels = width * height;
            byte[] linePixelArray = Enumerable.Range(0, width)
                .Select(pixel => (byte)Math.Round(0.5 * greyValue * (1 + Math.Cos(2 * Math.PI * pixel / period))))
                .ToArray();
            byte[] imageBytes = Enumerable.Repeat(linePixelArray, height).SelectMany(lineArray => lineArray).ToArray();
            var rng = new Random();
            for (int i = 0; i < nbRandomBytes; i++)
            {
                int index = rng.Next(0, nbPixels);
                int value = rng.Next(greyValue + 1, 256);
                imageBytes[index] = (byte)value;
            }

            return CreateMilImageFromByteArray(width, height, imageBytes);
        }

        private static USPImageMil CreateMilImageFromByteArray(int width, int height, byte[] byteArray)
        {
            var testImage = new USPImageMil(byteArray, width, height, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC);

            return testImage;
        }
    }
}
