using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Algo;

using UnitySC.PM.ANA.Service.Shared.TestUtils;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Input
{
    [TestClass]
    public class AlignmentMarksInputTest
    {
        [TestMethod]
        public void Check_input_validity_with_good_inputs()
        {
            // Given : right inputs are provided
            var validInput = SimulatedData.ValidAlignmentMarksInput();

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
            var validInput = SimulatedData.ValidAlignmentMarksInput();

            // When serializing in xml Then it works
            SerializationUtils.AssertXmlSerializable(validInput);
        }

        [TestMethod]
        public void Site_1_image_data_is_mandatory()
        {
            // Given : the Site 1 image data is not provided
            var invalidInput = SimulatedData.ValidAlignmentMarksInput();
            invalidInput.Site1Images = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Site_2_image_data_is_mandatory()
        {
            // Given : the site 2 image data is not provided
            var invalidInput = SimulatedData.ValidAlignmentMarksInput();
            invalidInput.Site2Images = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Pattern_recognition_data_must_not_be_empty()
        {
            // Given : the two pattern recognition data provided  are invalid
            var invalidInput = SimulatedData.ValidAlignmentMarksInput();
            invalidInput.Site1Images = new List<PositionWithPatternRec>() { };
            invalidInput.Site2Images = new List<PositionWithPatternRec>() { };

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(2, validity.Message.Count);
        }

        [TestMethod]
        public void Pattern_recognition_data_must_be_valid()
        {
            // Given : the two pattern recognition data provided  are invalid
            var invalidInput = SimulatedData.ValidAlignmentMarksInput();
            invalidInput.Site1Images = new List<PositionWithPatternRec>() { SimulatedData.InvalidPositionWithPatternRecData() };
            invalidInput.Site2Images = new List<PositionWithPatternRec>() { SimulatedData.InvalidPositionWithPatternRecData() };

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.IsTrue(validity.Message.Count >= 2);
        }

        [TestMethod]
        public void Autofocus_settings_can_be_null()
        {
            // Given : no autofocus input is provided
            var invalidInput = SimulatedData.ValidAlignmentMarksInput();
            invalidInput.AutoFocusSettings = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsTrue(validity.IsValid);
            Assert.AreEqual(0, validity.Message.Count);
        }

        [TestMethod]
        public void Autofocus_settings_must_be_valid()
        {
            // Given : camera autofocus input provided is invalid
            var invalidInput = SimulatedData.ValidAlignmentMarksInput();
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
            var invalidInput = SimulatedData.ValidAlignmentMarksInput();
            invalidInput.Site1Images = null;
            invalidInput.Site2Images = null;
            // AutoFocusSettings can be null
            invalidInput.AutoFocusSettings = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(2, validity.Message.Count);
        }
    }
}
