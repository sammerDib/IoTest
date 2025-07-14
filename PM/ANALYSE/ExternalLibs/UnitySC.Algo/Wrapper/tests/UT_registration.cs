using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlgosLibrary;
using System.Collections.Generic;

namespace UnitySC.Algorithms.Wrapper.Tests
{
    [TestClass]
    public class RegistrationTest
    {
        [TestInitialize]
        public void Init() { }

        [TestMethod]
        public void Registration_wrapper_return_correct_result()
        {
            // Given : Grayscale 8-bit images (Gray8)
            var ref_img = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat_gray8.tif"));
            var sensed_img = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat_gray8.tif"));
            RegionOfInterest roi = new RegionOfInterest();
            roi.X = 0;
            roi.Y = 0;
            roi.Width = ref_img.Width;
            roi.Height = ref_img.Height;

            // When : Run managed registration
            var result = Registration.ImgRegistration(ref_img, sensed_img, roi);

            // Then : We obtain correct values
            Assert.AreEqual(0, result.PixelShiftX, 0.01);
            Assert.AreEqual(0, result.PixelShiftY, 0.01);
            Assert.AreEqual(0.99, result.InitialSimilarityScore, 0.01);
            Assert.AreEqual(0.99, result.FinalSimilarityScore, 0.01);
            Assert.AreEqual(sensed_img.ByteArray.Length, result.ImgRegistered.ByteArray.Length);
            Assert.AreEqual(sensed_img.Width, result.ImgRegistered.Width);
            Assert.AreEqual(sensed_img.Height, result.ImgRegistered.Height);
            Assert.AreEqual(sensed_img.Depth, result.ImgRegistered.Depth);
        }

        [TestMethod]
        public void Registration_wrapper_compute_registration_on_roi_when_an_roi_is_provided()
        {
            // Given
            var ref_img = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat_gray8.tif"));
            var sensed_img = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat_gray8.tif"));

            RegionOfInterest allImageROI = new RegionOfInterest();
            allImageROI.X = 0;
            allImageROI.Y = 0;
            allImageROI.Width = ref_img.Width;
            allImageROI.Height = ref_img.Height;

            RegionOfInterest goodROI = new RegionOfInterest();
            goodROI.X = 10;
            goodROI.Y = 10;
            goodROI.Width = ref_img.Width - 10;
            goodROI.Height = ref_img.Height - 10;

            RegionOfInterest smallROI = new RegionOfInterest();
            smallROI.X = 0;
            smallROI.Y = 0;
            smallROI.Width = ref_img.Width / 4;
            smallROI.Height = ref_img.Height / 4;

            // When
            var resultWithGoodROI = Registration.ImgRegistration(ref_img, sensed_img, goodROI);
            var resultWithTooSmallROI = Registration.ImgRegistration(ref_img, sensed_img, smallROI);

            // Then : Good results are obtained when the region of interest is correct, but poor results when the region of interest is too small
            Assert.AreEqual(0, resultWithGoodROI.PixelShiftX, 0.01);
            Assert.AreEqual(0, resultWithGoodROI.PixelShiftY, 0.01);
            Assert.AreEqual(0.99, resultWithGoodROI.InitialSimilarityScore, 0.01);
            Assert.AreEqual(0.99, resultWithGoodROI.FinalSimilarityScore, 0.01);

            Assert.IsTrue(resultWithTooSmallROI.FinalSimilarityScore < resultWithGoodROI.FinalSimilarityScore);
        }
    }
}
