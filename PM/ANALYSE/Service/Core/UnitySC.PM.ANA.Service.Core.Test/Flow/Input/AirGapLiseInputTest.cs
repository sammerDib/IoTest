using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Shared.TestUtils;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Input
{
    [TestClass]
    public class AirGapLiseInputTest
    {
        private AirGapLiseInput _validAirGapLiseInput;

        [TestInitialize]
        public void Init()
        {
            _validAirGapLiseInput = new AirGapLiseInput(
                probeId: "probeId",
                gain: 1.8);
        }

        [TestMethod]
        public void Check_input_validity_with_good_inputs()
        {
            // Given : right inputs are provided
            var validInput = _validAirGapLiseInput;

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
            var validInput = _validAirGapLiseInput;

            // When serializing in xml Then it works
            SerializationUtils.AssertXmlSerializable(validInput);
        }

        [TestMethod]
        public void Probe_ID_is_mandatory()
        {
            // Given : the probe ID isn't provided
            var invalidInput = _validAirGapLiseInput;
            invalidInput.LiseData.ProbeID = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void NbAveraging_is_mandatory()
        {
            // Given : the probe ID isn't provided
            var invalidInput = _validAirGapLiseInput;
            invalidInput.LiseData.NbAveraging = -1;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Gain_is_optional()
        {
            // Given : the gain isn't provided
            var validInput = _validAirGapLiseInput;
            validInput.LiseData.Gain = double.NaN;

            // When
            var validity = validInput.CheckInputValidity();

            // Then
            Assert.IsTrue(validity.IsValid);
            Assert.AreEqual(0, validity.Message.Count);
        }
    }
}
