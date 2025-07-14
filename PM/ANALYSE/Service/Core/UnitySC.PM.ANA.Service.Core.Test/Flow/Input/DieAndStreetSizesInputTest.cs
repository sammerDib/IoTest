using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Shared.TestUtils;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Input
{
    [TestClass]
    public class DieAndStreetSizesInputTest
    {
        [TestMethod]
        public void Check_input_validity_with_good_inputs()
        {
            // Given : right inputs are provided
            var validInput = SimulatedData.ValidDieAndStreetSizesInput();

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
            var validInput = SimulatedData.ValidDieAndStreetSizesInput();

            // When serializing in xml Then it works
            SerializationUtils.AssertXmlSerializable(validInput);
        }

        [TestMethod]
        public void Wafer_characteristic_is_mandatory()
        {
            // Given : the wafer characteristic isn't provided
            var invalidInput = SimulatedData.ValidDieAndStreetSizesInput();
            invalidInput.Wafer = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Edge_exclusion_is_mandatory()
        {
            // Given : the edge exclusion isn't provided
            var invalidInput = SimulatedData.ValidDieAndStreetSizesInput();
            invalidInput.EdgeExclusion = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Top_left_corner_data_is_mandatory()
        {
            // Given : the top left corner data isn't provided
            var invalidInput = SimulatedData.ValidDieAndStreetSizesInput();
            invalidInput.TopLeftCorner = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Bottom_right_corner_data_is_mandatory()
        {
            // Given : the bottom right corner data isn't provided
            var invalidInput = SimulatedData.ValidDieAndStreetSizesInput();
            invalidInput.BottomRightCorner = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Top_left_corner_data_must_be_valid()
        {
            // Given : the top left corner data provided is invalid
            var invalidInput = SimulatedData.ValidDieAndStreetSizesInput();
            invalidInput.TopLeftCorner = SimulatedData.InvalidPositionWithPatternRecData();

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.IsTrue(validity.Message.Count >= 1);
        }

        [TestMethod]
        public void Bottom_right_corner_data_must_be_valid()
        {
            // Given : the bottom right corner data provided is invalid
            var invalidInput = SimulatedData.ValidDieAndStreetSizesInput();
            invalidInput.BottomRightCorner = SimulatedData.InvalidPositionWithPatternRecData();

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.IsTrue(validity.Message.Count >= 1);
        }

   
        [TestMethod]
        public void Autofocus_settings_must_be_valid()
        {
            // Given : camera autofocus input provided is invalid
            var invalidInput = SimulatedData.ValidDieAndStreetSizesInput();
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
            var invalidInput = SimulatedData.ValidDieAndStreetSizesInput();
            invalidInput.Wafer = null;
            invalidInput.EdgeExclusion = null;
            invalidInput.TopLeftCorner = null;
            invalidInput.BottomRightCorner = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(4, validity.Message.Count);
        }
    }
}
