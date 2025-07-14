using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySCSharedAlgosOpenCVWrapper;

namespace SharedOpenCV.WrapperTests
{
    [TestClass]
    public class UnitTestRegistration
    {
        [TestMethod]
        public void Registration_wrapper_nominal_case()
        {
            // Given : two shifted images
            var ref_img = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("refImage.png"));
            var sensed_img = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("sensedImage.png"));
            RegionOfInterest roi = new RegionOfInterest();
            roi.X = 0;
            roi.Y = 0;
            roi.Width = ref_img.Width;
            roi.Height = ref_img.Height;

            float gamma = 0.5f;

            // When : Run image registration
            var refImg = EdgeDetector.edgeDetection(ref_img, gamma, roi, BlurFilterMethod.Shen, true);
            var sensedImg = EdgeDetector.edgeDetection(sensed_img, gamma, roi, BlurFilterMethod.Shen, false);
            var result = Registration.ImgRegistration(refImg, sensedImg, roi, 0, "");
            var qualityScore = Registration.ComputeQualityOfRegistration(ref_img, sensed_img, result, roi, BlurFilterMethod.Shen, gamma, true);

            // Then : We obtain correct values
            var expectedPixelShiftX = 16;
            var expectedPixelShiftY = 208;
            Assert.AreEqual(expectedPixelShiftX, result.PixelShiftX, 1);
            Assert.AreEqual(expectedPixelShiftY, result.PixelShiftY, 1);

            Assert.IsTrue(qualityScore.InitialSimilarityScore < qualityScore.FinalSimilarityScore);
            Assert.IsTrue(qualityScore.FinalSimilarityScore > 0.90);

            Assert.AreEqual(sensed_img.ByteArray.Length, qualityScore.ImgRegistered.ByteArray.Length);
            Assert.AreEqual(sensed_img.Width, qualityScore.ImgRegistered.Width);
            Assert.AreEqual(sensed_img.Height, qualityScore.ImgRegistered.Height);
            Assert.AreEqual(sensed_img.Type, qualityScore.ImgRegistered.Type);
        }

        [TestMethod]
        public void Registration_wrapper_return_zero_shift_if_images_are_no_shifted()
        {
            // Given : two images non shifted
            var ref_img = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat_gray8.tif"));
            var sensed_img = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat_gray8.tif"));
            RegionOfInterest roi = new RegionOfInterest();
            roi.X = 0;
            roi.Y = 0;
            roi.Width = ref_img.Width;
            roi.Height = ref_img.Height;

            // When : Run managed registration
            var result = Registration.ImgRegistration(ref_img, sensed_img, roi, 0, "");

            // Then : We obtain correct values
            var expectedPixelShiftX = 0;
            var expectedPixelShiftY = 0;
            Assert.AreEqual(expectedPixelShiftX, result.PixelShiftX, 0.1);
            Assert.AreEqual(expectedPixelShiftY, result.PixelShiftY, 0.1);
        }

        [TestMethod]
        public void Registration_wrapper_return_correct_shift_when_images_are_shifted()
        {
            // Given : two shifted images
            var ref_img = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat.png"));
            var sensed_img = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat2.png"));
            RegionOfInterest roi = new RegionOfInterest();
            roi.X = 0;
            roi.Y = 0;
            roi.Width = ref_img.Width;
            roi.Height = ref_img.Height;

            // When : Run image registration
            var result = Registration.ImgRegistration(ref_img, sensed_img, roi, 0, "");

            // Then : We obtain correct values
            var expectedPixelShiftX = 10;
            var expectedPixelShiftY = 10;
            Assert.AreEqual(expectedPixelShiftX, result.PixelShiftX, 0.1);
            Assert.AreEqual(expectedPixelShiftY, result.PixelShiftY, 0.1);
        }

        [TestMethod]
        public void Registration_wrapper_return_correct_shift_when_a_good_ROI_is_provided()
        {
            // Given : correct ROI (big enough)
            var ref_img = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat.png"));
            var sensed_img = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat2.png"));

            RegionOfInterest goodROI = new RegionOfInterest();
            goodROI.X = 10;
            goodROI.Y = 10;
            goodROI.Width = ref_img.Width - 10;
            goodROI.Height = ref_img.Height - 10;

            // When : Run image registration
            var result = Registration.ImgRegistration(ref_img, sensed_img, goodROI, 0, "");

            // Then : We obtain correct values
            var expectedPixelShiftX = 10;
            var expectedPixelShiftY = 10;
            Assert.AreEqual(expectedPixelShiftX, result.PixelShiftX, 0.1);
            Assert.AreEqual(expectedPixelShiftY, result.PixelShiftY, 0.1);
        }

        [TestMethod]
        public void Registration_wrapper_return_bad_shift_when_a_bad_ROI_is_provided()
        {
            // Given : too litle ROI
            var ref_img = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat.png"));
            var sensed_img = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat2.png"));

            RegionOfInterest badROI = new RegionOfInterest();
            badROI.X = 0;
            badROI.Y = 0;
            badROI.Width = 5;
            badROI.Height = 5;

            // When : Run image registration
            var result = Registration.ImgRegistration(ref_img, sensed_img, badROI, 0, "");

            // Then : We obtain incorrect values
            var expectedPixelShiftX = 10;
            var expectedPixelShiftY = 10;
            Assert.AreNotEqual(expectedPixelShiftX, result.PixelShiftX, 10);
            Assert.AreNotEqual(expectedPixelShiftY, result.PixelShiftY, 10);
        }

        [TestMethod]
        public void Quality_of_registration_is_good_when_registration_is_correct()
        {
            // Given : Correct registration informations
            var ref_img = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat.png"));
            var sensed_img = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat2.png"));
            RegionOfInterest roi = new RegionOfInterest();
            roi.X = 0;
            roi.Y = 0;
            roi.Width = ref_img.Width;
            roi.Height = ref_img.Height;

            float gamma = 0.5f;

            var pixelShiftX = 10;
            var pixelShiftY = 10;
            var registrationInfos = new RegistrationInfos() { PixelShiftX = pixelShiftX, PixelShiftY = pixelShiftY };

            // When : Compute quality of registration
            var qualityScore = Registration.ComputeQualityOfRegistration(ref_img, sensed_img, registrationInfos, roi, BlurFilterMethod.Shen, gamma, true);

            // Then : We obtain good quality
            Assert.IsTrue(qualityScore.InitialSimilarityScore < qualityScore.FinalSimilarityScore);
            Assert.IsTrue(qualityScore.FinalSimilarityScore > 0.90);

            Assert.AreEqual(sensed_img.ByteArray.Length, qualityScore.ImgRegistered.ByteArray.Length);
            Assert.AreEqual(sensed_img.Width, qualityScore.ImgRegistered.Width);
            Assert.AreEqual(sensed_img.Height, qualityScore.ImgRegistered.Height);
            Assert.AreEqual(sensed_img.Type, qualityScore.ImgRegistered.Type);
        }

        [TestMethod]
        public void Quality_of_registration_is_bad_when_registration_is_incorrect()
        {
            // Given : Incorrect registration informations
            var ref_img = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat.png"));
            var sensed_img = Helpers.CreateImageDataFromFile(Helpers.CreateWrapperDataPath("cat2.png"));
            RegionOfInterest roi = new RegionOfInterest();
            roi.X = 0;
            roi.Y = 0;
            roi.Width = ref_img.Width;
            roi.Height = ref_img.Height;

            float gamma = 0.5f;

            var pixelShiftX = 0;
            var pixelShiftY = 0;
            var registrationInfos = new RegistrationInfos() { PixelShiftX = pixelShiftX, PixelShiftY = pixelShiftY };

            // When : Compute quality of registration
            var qualityScore = Registration.ComputeQualityOfRegistration(ref_img, sensed_img, registrationInfos, roi, BlurFilterMethod.Shen, gamma, true);

            // Then : We obtain bad quality
            Assert.AreEqual(qualityScore.InitialSimilarityScore, qualityScore.FinalSimilarityScore, 1.0);
            Assert.IsTrue(qualityScore.FinalSimilarityScore < 0.50);

            Assert.AreEqual(sensed_img.ByteArray.Length, qualityScore.ImgRegistered.ByteArray.Length);
            Assert.AreEqual(sensed_img.Width, qualityScore.ImgRegistered.Width);
            Assert.AreEqual(sensed_img.Height, qualityScore.ImgRegistered.Height);
            Assert.AreEqual(sensed_img.Type, qualityScore.ImgRegistered.Type);
        }
    }
}
