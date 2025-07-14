using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data.ExternalFile;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Input
{
    [TestClass]
    public class PatternRecognitionDataTest
    {
        private PatternRecognitionData _validPatternRecData;

        [TestInitialize]
        public void Init()
        {
            _validPatternRecData = new PatternRecognitionData(
                patternReference: new ExternalImage(),
                cameraId: "camera",
                roi: new RegionOfInterest(),
                gamma: 0.25);
        }

        [TestMethod]
        public void Check_input_validity_with_good_inputs()
        {
            // Given : right inputs are provided
            var patternRecData = _validPatternRecData;

            // When
            var validity = patternRecData.CheckInputValidity();

            // Then
            Assert.IsTrue(validity.IsValid);
            Assert.AreEqual(0, validity.Message.Count);
        }

        [TestMethod]
        public void Check_input_xml_serialization()
        {
            // Given : valid input
            var validInput = _validPatternRecData;

            // When serializing in xml Then it works
            SerializationUtils.AssertXmlSerializable(validInput);
        }

        [TestMethod]
        public void Camera_Id_is_mandatory()
        {
            // Given : the camera Id isn't provided
            var invalidInput = _validPatternRecData;
            invalidInput.CameraId = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Image_of_reference_is_mandatory()
        {
            // Given : image of reference isn't provided
            var patternRecDataWithoutRefImage = _validPatternRecData;
            patternRecDataWithoutRefImage.PatternReference = null;

            // When
            var validity = patternRecDataWithoutRefImage.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Region_of_interest_is_optional()
        {
            // Given : region of interest isn't provided
            var patternRecDataWithoutROI = _validPatternRecData;            
            patternRecDataWithoutROI.RegionOfInterest = null;

            // When
            var validity = patternRecDataWithoutROI.CheckInputValidity();

            // Then
            Assert.IsTrue(validity.IsValid);
            Assert.AreEqual(0, validity.Message.Count);
        }

        [TestMethod]
        public void Gamma_is_optional()
        {
            // Given : gamma isn't provided
            var patternRecDataWithoutGamma = _validPatternRecData;
            patternRecDataWithoutGamma.Gamma = double.NaN;

            // When
            var validity = patternRecDataWithoutGamma.CheckInputValidity();

            // Then
            Assert.IsTrue(validity.IsValid);
            Assert.AreEqual(0, validity.Message.Count);
        }

        [TestMethod]
        public void All_problems_are_listed_in_the_error_message()
        {
            // Given : nothing is provided
            var patternRecData = new PatternRecognitionData(null, null, null, double.NaN);

            // When
            var validity = patternRecData.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(2, validity.Message.Count);
        }
    }
}
