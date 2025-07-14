using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.Shared.Data;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Input
{
    [TestClass]
    public class BareWaferAlignmentInputTest
    {
        private BareWaferAlignmentInput _validBareWaferAlignmentInput;

        [TestInitialize]
        public void Init()
        {
            _validBareWaferAlignmentInput = new BareWaferAlignmentInput(
                cameraId: "cameraId",
                waferDimensionalCharacteristic: new WaferDimensionalCharacteristic());
        }

        [TestMethod]
        public void Check_input_validity_with_good_inputs()
        {
            // Given : right inputs are provided
            var validInput = _validBareWaferAlignmentInput;

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
            var validInput = _validBareWaferAlignmentInput;

            // When serializing in xml Then it works
            SerializationUtils.AssertXmlSerializable(validInput);
        }

        [TestMethod]
        public void Camera_ID_is_mandatory()
        {
            // Given : the camera ID isn't provided
            var invalidInput = _validBareWaferAlignmentInput;
            invalidInput.CameraId = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Wafer_dimensional_characteristic_is_mandatory()
        {
            // Given : the wafer characteristic isn't provided
            var invalidInput = _validBareWaferAlignmentInput;
            invalidInput.Wafer = null;

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
            var invalidInput = _validBareWaferAlignmentInput;
            invalidInput.Wafer = null;
            invalidInput.CameraId = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(2, validity.Message.Count);
        }
    }
}
