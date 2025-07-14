using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlgosLibrary;
using System.Collections.Generic;

namespace UnitySC.Algorithms.Wrapper.Tests
{
    [TestClass]
    public class ImgOperatorsTest
    {
        [TestInitialize]
        public void Init() { }

        [TestMethod]
        public void Image_operator_allows_to_mesure_focus_value()
        {
            // Given : Grayscale 8-bit images (Gray8)
            var img_at_focus = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat_gray8.tif"));
            var img_blured_with_median_kernel_3 = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat_gray8_blured_with_median_kernel_3.tif"));
            var img_blured_with_median_kernel_5 = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat_gray8_blured_with_median_kernel_5.tif"));

            // When : Compute focus measures thanks to wrapper
            double value_at_focus = ImageOperators.FocusMeasurement(img_at_focus);
            double value_img_blured_with_median_kernel_3 = ImageOperators.FocusMeasurement(img_blured_with_median_kernel_3);
            double value_img_blured_with_median_kernel_5 = ImageOperators.FocusMeasurement(img_blured_with_median_kernel_5);

            // Then : The focus values ​​are provided and consistent (decrease in pair with image blur)
            Assert.IsNotNull(value_at_focus);
            Assert.IsNotNull(value_img_blured_with_median_kernel_3);
            Assert.IsNotNull(value_img_blured_with_median_kernel_5);
            Assert.IsTrue(value_at_focus > value_img_blured_with_median_kernel_3);
            Assert.IsTrue(value_img_blured_with_median_kernel_3 > value_img_blured_with_median_kernel_5);
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
    }
}
