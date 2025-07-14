using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Input
{
    [TestClass]
    public class BareWaferAlignmentImageInputTest
    {
        private BareWaferAlignmentImageInput _validBareWaferAlignmentImageInput;

        [TestInitialize]
        public void Init()
        {
            _validBareWaferAlignmentImageInput = new BareWaferAlignmentImageInput(
                position: new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0),
                cameraId: "cameraId",
                edgePosition: WaferEdgePositions.Bottom,
                waferDimensionalCharacteristic: new WaferDimensionalCharacteristic());
        }

        [TestMethod]
        public void Check_input_validity_with_good_inputs()
        {
            // Given : right inputs are provided
            var validInput = _validBareWaferAlignmentImageInput;

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
            var validInput = _validBareWaferAlignmentImageInput;

            // When serializing in xml Then it works
            SerializationUtils.AssertXmlSerializable(validInput);
        }

        [TestMethod]
        public void Camera_ID_is_mandatory()
        {
            // Given : the camera ID isn't provided
            var invalidInput = _validBareWaferAlignmentImageInput;
            invalidInput.CameraId = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Position_is_mandatory()
        {
            // Given : the position isn't provided
            var invalidInput = _validBareWaferAlignmentImageInput;
            invalidInput.Position = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Position_provided_must_be_XYZTopZBottomPosition()
        {
            // Given : the position provided is invalid
            var invalidInput = _validBareWaferAlignmentImageInput;
            invalidInput.Position = new XYPosition(new WaferReferential(), 0, 0);

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
            var invalidInput = _validBareWaferAlignmentImageInput;
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
            var invalidInput = _validBareWaferAlignmentImageInput;
            invalidInput.CameraId = null;
            invalidInput.Position = null;
            invalidInput.Wafer = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(3, validity.Message.Count);
        }
    }
}
