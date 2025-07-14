using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Service.Shared.TestUtils.LiseTestUtils;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Input
{
    [TestClass]
    public class CalibrationDualLiseInputTest
    {
        private CalibrationDualLiseInput _validCalibrationDualLiseInput;

        private MeasureLiseInput _validMeasureLiseUpInput;
        private MeasureLiseInput _validMeasureLiseDownInput;
        private MeasureLiseInput _invalidMeasureLiseInput;

        [TestInitialize]
        public void Init()
        {
            _validMeasureLiseUpInput = new MeasureLiseInput(
                new ThicknessLiseInput(
                probeId: "LiseUp",
                gain: 1.8,
                sample: CreateProbeSample(new List<Length>() { 750.Micrometers() }, MaterialRefractionIndex)));

            _validMeasureLiseDownInput = new MeasureLiseInput(
                new ThicknessLiseInput(
                probeId: "LiseBottom",
                gain: 1.8,
                sample: CreateProbeSample(new List<Length>() { 750.Micrometers() }, MaterialRefractionIndex)));

            _invalidMeasureLiseInput = new MeasureLiseInput(
                new ThicknessLiseInput(
                probeId: null,
                gain: double.NaN,
                sample: null));

            _validCalibrationDualLiseInput = new CalibrationDualLiseInput(
                probeID: "DualLise",
                measureLiseUpInput: _validMeasureLiseUpInput,
                measureLiseDownInput: _validMeasureLiseDownInput,
                calibrationSample: CreateProbeSample(new List<Length>() { 750.Micrometers() }, MaterialRefractionIndex),
                calibrationPosition: new XYPosition(new WaferReferential(), 0, 0));
        }

        [TestMethod]
        public void Check_input_validity_with_good_inputs()
        {
            // Given : right inputs are provided
            var validInput = _validCalibrationDualLiseInput;

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
            var validInput = _validCalibrationDualLiseInput;

            // When serializing in xml Then it works
            SerializationUtils.AssertXmlSerializable(validInput);
        }

        [TestMethod]
        public void Probe_ID_is_mandatory()
        {
            // Given : the measure data isn't provided
            var invalidInput = _validCalibrationDualLiseInput;
            invalidInput.ProbeID = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Measure_data_up_is_mandatory()
        {
            // Given : the measure data isn't provided
            var invalidInput = _validCalibrationDualLiseInput;
            invalidInput.MeasureLiseUp = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Measure_data_up_must_be_valid()
        {
            // Given : the measure data isn't provided
            var invalidInput = _validCalibrationDualLiseInput;
            invalidInput.MeasureLiseUp = _invalidMeasureLiseInput;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(2, validity.Message.Count);
        }

        [TestMethod]
        public void Measure_data_down_is_mandatory()
        {
            // Given : the measure data isn't provided
            var invalidInput = _validCalibrationDualLiseInput;
            invalidInput.MeasureLiseDown = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Measure_data_down_must_be_valid()
        {
            // Given : the measure data isn't provided
            var invalidInput = _validCalibrationDualLiseInput;
            invalidInput.MeasureLiseDown = _invalidMeasureLiseInput;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(2, validity.Message.Count);
        }

        [TestMethod]
        public void Calibration_sample_is_mandatory()
        {
            // Given : the calibration sample isn't provided
            var invalidInput = _validCalibrationDualLiseInput;
            invalidInput.CalibrationSample = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Calibration_position_is_mandatory()
        {
            // Given : calibration position isn't provided
            var invalidInput = _validCalibrationDualLiseInput;
            invalidInput.CalibrationPosition = null;

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
            var invalidInput = _validCalibrationDualLiseInput;
            invalidInput.ProbeID = null;
            invalidInput.MeasureLiseUp = null;
            invalidInput.MeasureLiseDown = null;
            invalidInput.CalibrationSample = null;
            invalidInput.CalibrationPosition = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(5, validity.Message.Count);
        }
    }
}
