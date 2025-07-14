using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Service.Shared.TestUtils.LiseTestUtils;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Input
{
    [TestClass]
    public class MeasureLiseInputTest
    {
        private MeasureLiseInput _validMeasureLiseInput;

        [TestInitialize]
        public void Init()
        {
            _validMeasureLiseInput = new MeasureLiseInput(
                new ThicknessLiseInput(
                    probeId: "probeID",
                    gain: 1.8,
                    sample: CreateProbeSample(new List<Length>() { 750.Micrometers() }, MaterialRefractionIndex)
                )
            );
        }

        [TestMethod]
        public void Check_input_validity_with_good_inputs()
        {
            // Given : right inputs are provided
            var validInput = _validMeasureLiseInput;

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
            var validInput = _validMeasureLiseInput;

            // When serializing in xml Then it works
            SerializationUtils.AssertXmlSerializable(validInput);
        }

        [TestMethod]
        public void Probe_ID_is_mandatory()
        {
            // Given : the camera Id isn't provided
            var invalidInput = _validMeasureLiseInput;
            invalidInput.LiseData.ProbeID = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Measure_sample_is_mandatory()
        {
            // Given : the measure sample isn't provided
            var invalidInput = _validMeasureLiseInput;
            invalidInput.LiseData.Sample = null;

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
            var validInput = _validMeasureLiseInput;
            validInput.LiseData.Gain = double.NaN;

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
            var invalidInput = _validMeasureLiseInput;
            invalidInput.LiseData.Sample = null;
            invalidInput.LiseData.ProbeID = null;
            invalidInput.LiseData.Gain = double.NaN;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(2, validity.Message.Count);
        }
    }
}
