using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Shared.TestUtils;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Input
{
    [TestClass]
    public class ScanRangeTest
    {
        private ScanRange _validScanRange;

        [TestInitialize]
        public void Init()
        {
            _validScanRange = new ScanRange(
                min: -10,
                max: 15);
        }

        [TestMethod]
        public void Check_input_validity_with_good_inputs()
        {
            // Given : right inputs are provided
            var validInput = _validScanRange;

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
            var validInput = _validScanRange;

            // When serializing in xml Then it works
            SerializationUtils.AssertXmlSerializable(validInput);
        }

        [TestMethod]
        public void Min_value_is_mandatory()
        {
            // Given : the min value isn't provided
            var invalidInput = _validScanRange;
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
            var invalidInput = _validScanRange;
            invalidInput.Max = double.NaN;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Min_value_must_be_less_than_max_value()
        {
            // Given : the min value isn't provided
            var invalidInput = _validScanRange;
            invalidInput.Min = 5;
            invalidInput.Max = 4;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }
    }
}
