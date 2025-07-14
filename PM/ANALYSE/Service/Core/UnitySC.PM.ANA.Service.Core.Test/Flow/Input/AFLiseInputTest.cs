using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Shared.TestUtils;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Input
{
    [TestClass]
    public class AFLiseInputTest
    {
        [TestMethod]
        public void Check_input_validity_with_good_inputs()
        {
            // Given : right inputs are provided
            var validInput = SimulatedData.ValidAFLiseInput();

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
            var validInput = SimulatedData.ValidAFLiseInput();

            // When serializing in xml Then it works
            SerializationUtils.AssertXmlSerializable(validInput);
        }

        [TestMethod]
        public void Probe_ID_is_mandatory()
        {
            // Given : the probe ID isn't provided
            var invalidInput = SimulatedData.ValidAFLiseInput();
            invalidInput.ProbeID = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Scan_range_is_optional()
        {
            // Given : the scan range isn't provided
            var validInput = SimulatedData.ValidAFLiseInput();
            validInput.ZPosScanRange = null;

            // When
            var validity = validInput.CheckInputValidity();

            // Then
            Assert.IsTrue(validity.IsValid);
            Assert.AreEqual(0, validity.Message.Count);
        }

        [TestMethod]
        public void Valid_scan_range_is_mandatory_if_scan_range_is_provided()
        {
            // Given : the scan range provided is invalid
            var invalidInput = SimulatedData.ValidAFLiseInput();
            invalidInput.ZPosScanRange = SimulatedData.InvalidScanRange();

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.IsTrue(validity.Message.Count >= 1);
        }

        [TestMethod]
        public void Gain_is_optional()
        {
            // Given : the gain isn't provided
            var validInput = SimulatedData.ValidAFLiseInput();
            validInput.Gain = double.NaN;

            // When
            var validity = validInput.CheckInputValidity();

            // Then
            Assert.IsTrue(validity.IsValid);
            Assert.AreEqual(0, validity.Message.Count);
        }

        [TestMethod]
        public void All_problems_are_listed_in_the_error_message()
        {
            // Given : nothing is provided
            var invalidInput = SimulatedData.ValidAFLiseInput();
            invalidInput.ProbeID = null;
            invalidInput.ZPosScanRange = SimulatedData.InvalidScanRange();

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.IsTrue(validity.Message.Count >= 2);
        }
    }
}
