using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Shared.TestUtils;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Input
{
    [TestClass]
    public class ScanRangeWithStepTest
    {
        private ScanRangeWithStep _validScanRangeWithStep;

        [TestInitialize]
        public void Init()
        {
            _validScanRangeWithStep = new ScanRangeWithStep(
                min: -10,
                max: 15,
                step: 0.5);
        }

        [TestMethod]
        public void Check_input_validity_with_good_inputs()
        {
            // Given : right inputs are provided
            var validInput = _validScanRangeWithStep;

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
            var validInput = _validScanRangeWithStep;

            // When serializing in xml Then it works
            SerializationUtils.AssertXmlSerializable(validInput);
        }

        [TestMethod]
        public void Min_value_is_mandatory()
        {
            // Given : the min value isn't provided
            var invalidInput = _validScanRangeWithStep;
            invalidInput.Min = double.NaN;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Max_value_is_mandatory()
        {
            // Given : the max value isn't provided
            var invalidInput = _validScanRangeWithStep;
            invalidInput.Max = double.NaN;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Step_value_is_mandatory()
        {
            // Given : the step value isn't provided
            var invalidInput = _validScanRangeWithStep;
            invalidInput.Step = double.NaN;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Min_value_must_be_less_than_max_value()
        {
            // Given : min < max
            var invalidInput = _validScanRangeWithStep;
            invalidInput.Min = 5;
            invalidInput.Max = 4;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Step_value_must_not_be_equal_zero()
        {
            // Given : step = 0
            var invalidInput = _validScanRangeWithStep;
            invalidInput.Step = 0;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Step_value_must_not_be_negative()
        {
            // Given : step < 0
            var invalidInput = _validScanRangeWithStep;
            invalidInput.Step = -0.5;

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
            var invalidInput = _validScanRangeWithStep;
            invalidInput.Min = 6;
            invalidInput.Max = 5;
            invalidInput.Step = -0.5;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(2, validity.Message.Count);
        }
    }
}
