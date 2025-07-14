using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.EME.Service.Core.Recipe;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.Shared.Image;

namespace UnitySC.PM.EME.Service.Core.Test.Recipe
{
    [TestClass]
    public class ImageProcessingTest
    {
        [TestMethod]
        public void ShouldNormalizePixelValue()
        {
            // Arrange
            ushort[] pixelValues = { 33424, 100, 29700, 30840, 34840, 33924, 39835, 64680, 32840 };
            byte[] byteArray = ConvertToByteArray(pixelValues);
            
            var originalImage = new ServiceImage
            {
                Data = byteArray,
                DataHeight = 3,
                DataWidth = 3,
                Type = ServiceImage.ImageType.Greyscale16Bit
            };

            var systemUnderTest = new ImageProcessing(null);
            var settings = new HashSet<ImageProcessingType>{ImageProcessingType.NormalizePixelValue, ImageProcessingType.ConvertTo8Bits};
            
            // Act
            var processedImage = systemUnderTest.Process(settings, originalImage);

            // Assert.
            int[] resultData = processedImage.Data.Select(x => (int)x).ToArray();
            int[] expected = { 127, 0, 112, 116, 133, 129, 153, 255, 125 };
            CollectionAssert.AreEqual(expected, resultData);
        }

        [TestMethod]
        public void ShouldDistortImage()
        {
            // Arrange
            byte[] originalPixels = { 128, 128, 128, 128, 255, 128, 128, 128, 128 };
            var originalImage = new ServiceImage
            {
                Data = originalPixels, DataHeight = 3, DataWidth = 3, Type = ServiceImage.ImageType.Greyscale
            };

            var distortion = new DistortionData
            {
                CameraMat = new[] { 1.0, 5.0, 2.0, 1.0, 5.0, 1.0, 0.0, 0.0, 1.0 },
                NewOptimalCameraMat = new[] { 1.0, 5.0, 7.0, 1.0, 4.0, 1.0, 0.0, 0.0, 1.0 },
                DistortionMat = new[] { 1.0, 5.0, 2.0, 1.0, 1.0 },
                RotationVec = new[] { 1.0, 5.0, 2.0 },
                TranslationVec = new[] { 4.0, 5.0, 2.0 }
            };

            var systemUnderTest = new ImageProcessing(distortion);
            var settings = new HashSet<ImageProcessingType>{ImageProcessingType.ReduceResolution};
            
            // Act
            var processedImage = systemUnderTest.Process(settings, originalImage);

            // Assert.
            CollectionAssert.AreNotEqual(originalPixels, processedImage.Data);
        }


        private static byte[] ConvertToByteArray(ushort[] pixelValues)
        {
            byte[] byteArray = new byte[pixelValues.Length * 2];
            for (int i = 0; i < pixelValues.Length; i++)
            {
                // Low byte (least significant)
                byteArray[2 * i] = (byte)(pixelValues[i] & 0xFF);
                // High byte (most significant)
                byteArray[2 * i + 1] = (byte)(pixelValues[i] >> 8);
            }

            return byteArray;
        }
    }
}
