using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Shared.TestUtils;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Input
{
    [TestClass]
    public class AutolightInputTest
    {
        private AutolightInput _validAutolightInput;
        private ScanRangeWithStep _invalidScanRangeWithStep;

        [TestInitialize]
        public void Init()
        {
            _validAutolightInput = new AutolightInput(
                cameraId: "cameraId",
                lightId: "lightId",
                exposure: 1.8,
                lightPower: new ScanRangeWithStep(0, 1, 0.5));

            _invalidScanRangeWithStep = new ScanRangeWithStep(
                min: 10,
                max: 9,
                step: -0.5);
        }

        [TestMethod]
        public void Check_input_validity_with_good_inputs()
        {
            // Given : right inputs are provided
            var validInput = _validAutolightInput;

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
            var validInput = _validAutolightInput;

            // When serializing in xml Then it works
            SerializationUtils.AssertXmlSerializable(validInput);
        }

        [TestMethod]
        public void Camera_ID_is_mandatory()
        {
            // Given : the camera Id isn't provided
            var invalidInput = _validAutolightInput;
            invalidInput.CameraId = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Light_ID_is_mandatory()
        {
            // Given : the light Id isn't provided
            var invalidInput = _validAutolightInput;
            invalidInput.LightId = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Exposure_time_is_mandatory()
        {
            // Given : the exposure isn't provided
            var invalidInput = _validAutolightInput;
            invalidInput.ExposureTimeMs = double.NaN;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Light_power_range_is_mandatory()
        {
            // Given : the light power range isn't provided
            var invalidInput = _validAutolightInput;
            invalidInput.LightPower = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Light_power_range_must_be_valid()
        {
            // Given : the light power range provided is invalid
            var invalidInput = _validAutolightInput;
            invalidInput.LightPower = _invalidScanRangeWithStep;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(2, validity.Message.Count);
        }

        [TestMethod]
        public void All_problems_are_listed_in_the_error_message()
        {
            // Given : nothing is provided
            var invalidInput = _validAutolightInput;
            invalidInput.CameraId = null;
            invalidInput.LightId = null;
            invalidInput.ExposureTimeMs = double.NaN;
            invalidInput.LightPower = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(4, validity.Message.Count);
        }
    }
}
