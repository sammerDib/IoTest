using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Service.Shared.TestUtils.LiseTestUtils;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Input
{
    [TestClass]
    public class MultipleMeasuresLiseInputTest
    {
        private MultipleMeasuresLiseInput _validMultipleMeasuresLiseInput;
        private ThicknessLiseInput _validMeasureLiseData;
        private ThicknessLiseInput _invalidMeasureLiseData;

        [TestInitialize]
        public void Init()
        {
            _validMeasureLiseData = new ThicknessLiseInput(
                probeId: "LiseUp",
                gain: 1.8,
                sample: CreateProbeSample(new List<Length>() { 750.Micrometers() }, MaterialRefractionIndex));

            _invalidMeasureLiseData = new ThicknessLiseInput(
                probeId: null,
                gain: double.NaN,
                sample: null);

            _validMultipleMeasuresLiseInput = new MultipleMeasuresLiseInput(
                measureLiseData: _validMeasureLiseData,
                nbMeasures: 2);
        }

        [TestMethod]
        public void Check_input_validity_with_good_inputs()
        {
            // Given : right inputs are provided
            var validInput = _validMultipleMeasuresLiseInput;

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
            var validInput = _validMultipleMeasuresLiseInput;

            // When serializing in xml Then it works
            SerializationUtils.AssertXmlSerializable(validInput);
        }

        [TestMethod]
        public void Measure_sample_is_mandatory()
        {
            // Given : the measure sample isn't provided
            var invalidInput = _validMultipleMeasuresLiseInput;
            invalidInput.MeasureLise = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Measure_sample_must_be_valid()
        {
            // Given : the measure sample provided is invalid
            var invalidInput = _validMultipleMeasuresLiseInput;
            invalidInput.MeasureLise = _invalidMeasureLiseData;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(2, validity.Message.Count);
        }

        [TestMethod]
        public void Measure_number_must_be_positive()
        {
            // Given : the measure number provided is invalid
            var invalidInput = _validMultipleMeasuresLiseInput;
            invalidInput.NbMeasures = 0;

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
            var invalidInput = _validMultipleMeasuresLiseInput;
            invalidInput.MeasureLise = null;
            invalidInput.NbMeasures = -1;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(2, validity.Message.Count);
        }
    }
}
