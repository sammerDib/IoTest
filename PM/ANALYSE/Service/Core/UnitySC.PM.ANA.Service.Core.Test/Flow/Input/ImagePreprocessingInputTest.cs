using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Shared.TestUtils;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Input
{
    [TestClass]
    public class ImagePreprocessingInputTest
    {
        [TestMethod]
        public void Check_input_validity_with_good_inputs()
        {
            // Given : right inputs are provided
            var validInput = SimulatedData.ValidImagePreprocessingInput();

            // When
            var validity = validInput.CheckInputValidity();

            // Then
            Assert.IsTrue(validity.IsValid);
            Assert.AreEqual(0, validity.Message.Count);
        }

        [TestMethod]
        public void Check_input_xml_serialization()
        {
            // Given : valid input
            var validInput = SimulatedData.ValidImagePreprocessingInput();

            // When serializing in xml Then it works
            SerializationUtils.AssertXmlSerializable(validInput);
        }

        [TestMethod]
        public void Camera_ID_is_mandatory()
        {
            // Given : the camera id isn't provided
            var invalidInput = SimulatedData.ValidImagePreprocessingInput();
            invalidInput.CameraId = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Position_is_mandatory()
        {
            // Given : the camera id isn't provided
            var invalidInput = SimulatedData.ValidImagePreprocessingInput();
            invalidInput.Position = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Gamma_is_mandatory()
        {
            // Given : the camera id isn't provided
            var invalidInput = SimulatedData.ValidImagePreprocessingInput();
            invalidInput.Gamma = double.NaN;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void All_problems_are_listed_in_the_error_message()
        {
            // Given : nothing is provided
            var invalidInput = SimulatedData.ValidImagePreprocessingInput();
            invalidInput.CameraId = null;
            invalidInput.Position = null;
            invalidInput.Gamma = double.NaN;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(3, validity.Message.Count);
        }
    }
}
