using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Recipe.Context;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.Shared.Data.ExternalFile;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Input
{
    [TestClass]
    public class PatternRecognitionDataWithContextTest
    {
        private PatternRecognitionDataWithContext _validPatternRecData;

        [TestInitialize]
        public void Init()
        {
            _validPatternRecData = new PatternRecognitionDataWithContext(
                context: new TopImageAcquisitionContext(),
                cameraId: "camera",
                patternReference: new ExternalImage(),
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
        public void Context_is_mandatory()
        {
            // Given : context isn't provided
            var patternRecDataWithoutRefImage = _validPatternRecData;
            patternRecDataWithoutRefImage.Context = null;

            // When
            var validity = patternRecDataWithoutRefImage.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void All_problems_are_listed_in_the_error_message()
        {
            // Given : nothing is provided
            var patternRecData = new PatternRecognitionDataWithContext(null, null, null, double.NaN, null);

            // When
            var validity = patternRecData.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(3, validity.Message.Count);
        }
    }
}
