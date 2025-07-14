using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Shared.TestUtils;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Input
{
    [TestClass]
    public class AFCameraInputTest
    {
        [TestMethod]
        public void Check_input_validity_with_good_inputs()
        {
            // Given : right inputs are provided
            var validInput = SimulatedData.ValidAFCameraInput();

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
            var validInput = SimulatedData.ValidAFCameraInput();

            // When serializing in xml Then it works
            SerializationUtils.AssertXmlSerializable(validInput);
        }

        [TestMethod]
        public void Camera_ID_is_mandatory()
        {
            // Given : the camera Id isn't provided
            var invalidInput = SimulatedData.ValidAFCameraInput();
            invalidInput.CameraId = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Scan_range_configured_is_mandatory_when_range_type_is_set_to_Configured()
        {
            // Given : the range type is set to configured but the scan range isn't provided
            var invalidInput = SimulatedData.ValidAFCameraInput();
            invalidInput.RangeType = ScanRangeType.Configured;
            invalidInput.ScanRangeConfigured = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Scan_range_configured_provided_must_be_valid_when_range_type_is_set_to_Configured()
        {
            // Given : the range type is set to configured but the scan range isn't valid
            var invalidInput = SimulatedData.ValidAFCameraInput();
            invalidInput.RangeType = ScanRangeType.Configured;
            invalidInput.ScanRangeConfigured = SimulatedData.InvalidScanRangeWithStep();

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(2, validity.Message.Count);
        }

        [TestMethod]
        public void Scan_range_configured_is_not_mandatory_when_range_type_is_not_set_to_Configured()
        {
            // Given : the scan range isn't provided but the range type is not set to configured
            var validInput1 = SimulatedData.ValidAFCameraInput();
            validInput1.RangeType = ScanRangeType.Medium;
            validInput1.ScanRangeConfigured = null;

            var validInput2 = SimulatedData.ValidAFCameraInput();
            validInput2.RangeType = ScanRangeType.Small;
            validInput2.ScanRangeConfigured = null;

            var validInput3 = SimulatedData.ValidAFCameraInput();
            validInput3.RangeType = ScanRangeType.Large;
            validInput3.ScanRangeConfigured = null;

            // When
            var validity1 = validInput1.CheckInputValidity();
            var validity2 = validInput2.CheckInputValidity();
            var validity3 = validInput3.CheckInputValidity();

            // Then
            Assert.IsTrue(validity1.IsValid);
            Assert.AreEqual(0, validity1.Message.Count);

            Assert.IsTrue(validity2.IsValid);
            Assert.AreEqual(0, validity2.Message.Count);

            Assert.IsTrue(validity3.IsValid);
            Assert.AreEqual(0, validity3.Message.Count);
        }

        [TestMethod]
        public void All_problems_are_listed_in_the_error_message()
        {
            // Given : nothing is provided
            var invalidInput = SimulatedData.ValidAFCameraInput();
            invalidInput.CameraId = null;
            invalidInput.RangeType = ScanRangeType.Configured;
            invalidInput.ScanRangeConfigured = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(2, validity.Message.Count);
        }
    }
}
