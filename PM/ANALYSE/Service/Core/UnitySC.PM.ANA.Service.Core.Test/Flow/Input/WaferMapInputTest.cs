using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Input
{
    [TestClass]
    public class WaferMapInputTest
    {
        [TestMethod]
        public void Check_input_validity_with_good_inputs()
        {
            // Given : right inputs are provided
            var validInput = SimulatedData.ValidWaferMapInput();

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
            var validInput = SimulatedData.ValidWaferMapInput();

            // When serializing in xml Then it works
            SerializationUtils.AssertXmlSerializable(validInput);
        }

        [TestMethod]
        public void Wafer_characteristics_are_mandatory()
        {
            // Given : the wafer characteritics are not provided
            var invalidInput = SimulatedData.ValidWaferMapInput();
            invalidInput.WaferCharacteristics = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Edge_exclusion_is_mandatory()
        {
            // Given : the edge exclusion is not provided
            var invalidInput = SimulatedData.ValidWaferMapInput();
            invalidInput.EdgeExclusion = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Die_dimension_is_mandatory()
        {
            // Given : the die dimension is not provided
            var invalidInput = SimulatedData.ValidWaferMapInput();
            invalidInput.DieDimensions = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Die_width_must_not_be_zero()
        {
            // Given
            var invalidInput = SimulatedData.ValidWaferMapInput();
            invalidInput.DieDimensions.DieWidth = 0.Millimeters();

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Die_height_must_not_be_zero()
        {
            // Given
            var invalidInput = SimulatedData.ValidWaferMapInput();
            invalidInput.DieDimensions.DieHeight = 0.Millimeters();

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Die_width_must_not_be_negative()
        {
            // Given
            var invalidInput = SimulatedData.ValidWaferMapInput();
            invalidInput.DieDimensions.DieWidth = -1.Millimeters();

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Die_height_must_not_be_negative()
        {
            // Given
            var invalidInput = SimulatedData.ValidWaferMapInput();
            invalidInput.DieDimensions.DieHeight = -1.Millimeters();

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Street_width_must_not_be_negative()
        {
            // Given
            var invalidInput = SimulatedData.ValidWaferMapInput();
            invalidInput.DieDimensions.StreetWidth = -1.Millimeters();

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Street_height_must_not_be_negative()
        {
            // Given
            var invalidInput = SimulatedData.ValidWaferMapInput();
            invalidInput.DieDimensions.StreetHeight = -1.Millimeters();

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Street_width_can_be_zero()
        {
            // Given
            var invalidInput = SimulatedData.ValidWaferMapInput();
            invalidInput.DieDimensions.StreetWidth = 0.Millimeters();

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsTrue(validity.IsValid);
        }

        [TestMethod]
        public void Street_height_can_be_zero()
        {
            // Given
            var invalidInput = SimulatedData.ValidWaferMapInput();
            invalidInput.DieDimensions.StreetHeight = 0.Millimeters();

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsTrue(validity.IsValid);
        }

        [TestMethod]
        public void Top_left_corner_data_is_mandatory()
        {
            // Given : the top left corner data is not provided
            var invalidInput = SimulatedData.ValidWaferMapInput();
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
            // Given : the bottom right corner data is not provided
            var invalidInput = SimulatedData.ValidWaferMapInput();
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
            var invalidInput = SimulatedData.ValidWaferMapInput();
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
            var invalidInput = SimulatedData.ValidWaferMapInput();
            invalidInput.BottomRightCorner = SimulatedData.InvalidPositionWithPatternRecData();

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
            var invalidInput = SimulatedData.ValidWaferMapInput();
            invalidInput.WaferCharacteristics = null;
            invalidInput.EdgeExclusion = null;
            invalidInput.DieDimensions = null;
            invalidInput.TopLeftCorner = null;
            invalidInput.BottomRightCorner = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(5, validity.Message.Count);
        }
    }
}
