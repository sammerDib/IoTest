using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.Shared.Data;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Input
{
    [TestClass]
    public class AutoAlignInputTest
    {
        private AutoAlignInput _validAutoAlignInput;

        [TestInitialize]
        public void Init()
        {
            _validAutoAlignInput = new AutoAlignInput(
                waferDimensionalCharacteristic: new WaferDimensionalCharacteristic()); ;
        }

        [TestMethod]
        public void Check_input_validity_with_good_inputs()
        {
            // Given : right inputs are provided
            var validInput = _validAutoAlignInput;

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
            var validInput = _validAutoAlignInput;

            // When serializing in xml Then it works
            SerializationUtils.AssertXmlSerializable(validInput);
        }

        [TestMethod]
        public void Wafer_dimensional_characteristic_is_mandatory()
        {
            // Given : the wafer dimensional characteristic isn't provided
            var invalidInput = _validAutoAlignInput;
            invalidInput.Wafer = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }
    }
}
