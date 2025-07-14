using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Input
{
    [TestClass]
    public class CheckPatternRecInputTest
    {
        private CheckPatternRecInput _validCheckPatternRecInput;

        [TestInitialize]
        public void Init()
        {
            _validCheckPatternRecInput = SimulatedData.ValidCheckPatternRecInput();
        }

        [TestMethod]
        public void Check_input_validity_with_good_inputs()
        {
            // Given : right inputs are provided
            var validInput = _validCheckPatternRecInput;

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
            var validInput = _validCheckPatternRecInput;

            // When serializing in xml Then it works
            SerializationUtils.AssertXmlSerializable(validInput);
        }

        [TestMethod]
        public void Pattern_recognition_data_is_mandatory()
        {
            // Given : the pattern recognition data isn't provided
            var invalidInput = _validCheckPatternRecInput;
            invalidInput.PositionWithPatternRec = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Pattern_recognition_data_must_be_valid()
        {
            // Given : the pattern recognition data provided is invalid
            var invalidInput = _validCheckPatternRecInput;
            invalidInput.PositionWithPatternRec = SimulatedData.InvalidPositionWithPatternRecData();

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(4, validity.Message.Count);
        }

        [TestMethod]
        public void Validation_positions_is_mandatory()
        {
            // Given : the validation positions are not provided
            var invalidInput = _validCheckPatternRecInput;
            invalidInput.ValidationPositions = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Validation_positions_must_not_be_empty()
        {
            // Given : the validation positions list provided is empty
            var invalidInput = _validCheckPatternRecInput;
            invalidInput.ValidationPositions = new List<XYZTopZBottomPosition>();

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Tolerance_is_mandatory()
        {
            // Given : the pattern recognition data provided is invalid
            var invalidInput = _validCheckPatternRecInput;
            invalidInput.Tolerance = null;

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
            var invalidInput = _validCheckPatternRecInput;
            invalidInput.PositionWithPatternRec = null;
            invalidInput.ValidationPositions = null;
            invalidInput.Tolerance = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(3, validity.Message.Count);
        }
    }
}
