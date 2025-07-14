using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Shared.TestUtils;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Input
{
    [TestClass]
    public class PatternRecInputTest
    {
        [TestMethod]
        public void Check_input_validity_with_good_inputs()
        {
            // Given : right inputs are provided
            var validInput = SimulatedData.ValidPatternRecInput();

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
            var validInput = SimulatedData.ValidPatternRecInput();

            // When serializing in xml Then it works
            SerializationUtils.AssertXmlSerializable(validInput);
        }

        [TestMethod]
        public void Pattern_recognition_data_is_mandatory()
        {
            // Given : the recognition data isn't provided
            var invalidInput = SimulatedData.ValidPatternRecInput();
            invalidInput.Data = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Pattern_recognition_data_must_be_valid()
        {
            // Given : an invalid pattern recognition data is provided
            var invalidInput = SimulatedData.ValidPatternRecInput();
            invalidInput.Data = SimulatedData.InvalidPatternRecognitionData();

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.IsTrue(validity.Message.Count >= 1);
        }

        [TestMethod]
        public void Autofocus_settings_are_optionnal_if_autofocus_is_not_used()
        {
            // Given : autofocus inputs isn't provided but autofocus is not set to run
            var validInput = SimulatedData.ValidPatternRecInput();
            validInput.RunAutofocus = false;
            validInput.AutoFocusSettings = null;

            // When
            var validity = validInput.CheckInputValidity();

            // Then
            Assert.IsTrue(validity.IsValid);
            Assert.AreEqual(0, validity.Message.Count);
        }

        [TestMethod]
        public void Autofocus_settings_are_mandatory_if_autofocus_is_used()
        {
            // Given : autofocus inputs isn't provided and autofocus is set to run
            var invalidInput = SimulatedData.ValidPatternRecInput();
            invalidInput.RunAutofocus = true;
            invalidInput.AutoFocusSettings = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Camera_autofocus_input_must_be_valid_if_provided()
        {
            // Given : camera autofocus input provided is invalid
            var invalidInput = SimulatedData.ValidPatternRecInput();
            invalidInput.AutoFocusSettings = SimulatedData.InvalidAFSettings();

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.IsTrue(validity.Message.Count >= 1);
        }

        [TestMethod]
        public void Lise_autofocus_input_must_be_valid_if_provided()
        {
            // Given : Lise autofocus input provided is invalid
            var invalidInput = SimulatedData.ValidPatternRecInput();
            invalidInput.AutoFocusSettings = SimulatedData.InvalidAFSettings();

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.IsTrue(validity.Message.Count >= 1);
        }

        [TestMethod]
        public void All_problems_are_listed_in_the_error_message()
        {
            // Given : nothing is provided
            var invalidInput = SimulatedData.ValidPatternRecInput();
            invalidInput.Data = null;
            invalidInput.RunAutofocus = true;
            invalidInput.AutoFocusSettings = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(2, validity.Message.Count);
        }
    }
}
