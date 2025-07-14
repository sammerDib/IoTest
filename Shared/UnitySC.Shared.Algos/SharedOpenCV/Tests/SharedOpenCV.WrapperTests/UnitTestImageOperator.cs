using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySCSharedAlgosOpenCVWrapper;

namespace SharedOpenCV.WrapperTests
{
    [TestClass]
    public class UnitTestImageOperator
    {
        [TestMethod]
        public void Saturated_Median_Should_Be_Higher()
        {
            // Given
            var img = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat_gray8.tif"));
            var imgSaturated = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat_gray8_overexposed.tif"));

            // When
            int medianValue = ImageOperators.GrayscaleMedianComputation(img);
            int medianValueSaturated = ImageOperators.GrayscaleMedianComputation(imgSaturated);

            // Then
            Assert.IsTrue(medianValue < medianValueSaturated);
        }


        [TestMethod]
        public void Image_operator_allows_to_mesure_focus_value()
        {
            // Given : Grayscale 8-bit images (Gray8)
            var img_at_focus = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat_gray8.tif"));
            var img_blured_with_median_kernel_3 = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat_gray8_blured_with_median_kernel_3.tif"));
            var img_blured_with_median_kernel_5 = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat_gray8_blured_with_median_kernel_5.tif"));

            foreach (FocusMeasureMethod method in (FocusMeasureMethod[])Enum.GetValues(typeof(FocusMeasureMethod)))
            {
                // When : Compute focus measures thanks to wrapper
                double value_at_focus = ImageOperators.FocusMeasurement(img_at_focus, method);
                double value_img_blured_with_median_kernel_3 = ImageOperators.FocusMeasurement(img_blured_with_median_kernel_3, method);
                double value_img_blured_with_median_kernel_5 = ImageOperators.FocusMeasurement(img_blured_with_median_kernel_5, method);

                // Then : The focus values ​​are provided and consistent (decrease in pair with image blur)
                Assert.IsNotNull(value_at_focus);
                Assert.IsNotNull(value_img_blured_with_median_kernel_3);
                Assert.IsNotNull(value_img_blured_with_median_kernel_5);
                Assert.IsTrue(value_at_focus > value_img_blured_with_median_kernel_3);
                Assert.IsTrue(value_img_blured_with_median_kernel_3 > value_img_blured_with_median_kernel_5);
            }
        }

        [TestMethod]
        public void Image_operator_allows_to_mesure_contrast_value()
        {
            // Given : Grayscale 8-bit images (Gray8)
            var img_at_focus = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat_gray8.tif"));
            var img_blured_with_median_kernel_3 = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat_gray8_blured_with_median_kernel_3.tif"));
            var img_blured_with_median_kernel_5 = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat_gray8_blured_with_median_kernel_5.tif"));

            // When : Compute contrast measures thanks to wrapper
            double value_at_focus = ImageOperators.ContrastMeasurement(img_at_focus);
            double value_img_blured_with_median_kernel_3 = ImageOperators.ContrastMeasurement(img_blured_with_median_kernel_3);
            double value_img_blured_with_median_kernel_5 = ImageOperators.ContrastMeasurement(img_blured_with_median_kernel_5);

            // Then : The contrast values ​​are provided and consistent (decrease in pair with image blur)
            Assert.IsNotNull(value_at_focus);
            Assert.IsNotNull(value_img_blured_with_median_kernel_3);
            Assert.IsNotNull(value_img_blured_with_median_kernel_5);
            Assert.IsTrue(value_at_focus > value_img_blured_with_median_kernel_3);
            Assert.IsTrue(value_img_blured_with_median_kernel_3 > value_img_blured_with_median_kernel_5);
        }

        [TestMethod]
        public void Image_operator_allows_to_mesure_saturation_value()
        {
            // Given : Grayscale 8-bit images (Gray8)
            var img = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat_gray8.tif"));
            var imgSaturated = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat_gray8_overexposed.tif"));

            // When
            double value = ImageOperators.SaturationMeasurement(img);
            double valueOverexposed = ImageOperators.SaturationMeasurement(imgSaturated);

            // Then : The contrast values ​​are provided and consistent (decrease in pair with image blur)
            Assert.IsNotNull(value);
            Assert.IsNotNull(valueOverexposed);
            Assert.IsTrue(value < valueOverexposed);
        }

        [TestMethod]
        public void Compute_intensity_profile()
        {
            // Given : Grayscale 8-bit images (Gray8) and two pixel positions 
            var img = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat_gray8.tif"));
            var startPixel = new Point2i(0, 0);
            var endPixel = new Point2i(100, 100);

            // When
            Point2d[] profile = ImageOperators.ExtractIntensityProfile(img, startPixel, endPixel);

            // Then
            Assert.IsNotNull(profile);
            Assert.AreNotEqual(0, profile.Length);
        }

        [TestMethod]
        public void Resize_image_should_return_same_image_when_full_roi_and_no_rescale()
        {
            // Given
            var img = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat_gray8.tif"));
            var roi = new RegionOfInterest() { X = 0, Y = 0, Width = img.Width, Height = img.Height };
            var scale = 1.0;

            // When
            ImageData resizedImage = ImageOperators.Resize(img, roi, scale);

            // Then
            Assert.IsNotNull(resizedImage);
            Assert.AreEqual(img.Width, resizedImage.Width);
            Assert.AreEqual(img.Height, resizedImage.Height);
        }

        [TestMethod]
        public void Resize_image_with_smaller_roi()
        {
            // Given
            var img = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat_gray8.tif"));
            var roi = new RegionOfInterest()
            {
                X = img.Width / 4,
                Y = img.Height / 4,
                Width = img.Width / 2,
                Height = img.Height / 2
            };
            var scale = 1.0;

            // When
            ImageData resizedImage = ImageOperators.Resize(img, roi, scale);

            // Then
            Assert.IsNotNull(resizedImage);
            Assert.AreEqual(roi.Width, resizedImage.Width);
            Assert.AreEqual(roi.Height, resizedImage.Height);
        }

        [TestMethod]
        public void Resize_image_with_lower_resolution()
        {
            // Given
            var img = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat_gray8.tif"));
            var roi = new RegionOfInterest() { X = 0, Y = 0, Width = img.Width, Height = img.Height };
            var scale = 0.5;

            // When
            ImageData resizedImage = ImageOperators.Resize(img, roi, scale);

            // Then
            Assert.IsNotNull(resizedImage);
            Assert.AreEqual(img.Width / 2, resizedImage.Width);
            Assert.AreEqual(img.Height / 2, resizedImage.Height);
        }

        [TestMethod]
        public void Compute_Saturation_Level()
        {
            //Given
            var img = new ImageData(10, 10);
            img.ByteArray = Enumerable.Range(0, 100).Select(i => (byte)128).ToArray();
            img.Type = ImageType.GRAYSCALE_Unsigned8bits;

            //When
            double sat = ImageOperators.ComputeGreyLevelSaturation(img, 0.03f);

            //Then
            Assert.AreEqual(128.0, sat, 1e-5);
        }

        [TestMethod]
        public void Compute_Saturation_Exception()
        {
            //Given
            var img = new ImageData(10, 10);
            img.ByteArray = Enumerable.Range(0, 100).Select(i => (ushort)32000).SelectMany(sValue => BitConverter.GetBytes(sValue)).ToArray();
            img.Type = ImageType.GRAYSCALE_Unsigned16bits;

            //When + Then
            var ex = Assert.ThrowsException<Exception>(() => ImageOperators.ComputeGreyLevelSaturation(img, 0.03f));
            Assert.AreEqual("[ImageOperators] Only unsigned 8-bit or float 32-bit images are supported for grey level saturation computation.", ex.Message);
        }
    }
}
