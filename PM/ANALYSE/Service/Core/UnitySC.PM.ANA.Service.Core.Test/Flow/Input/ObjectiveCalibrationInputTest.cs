using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Input
{
    [TestClass]
    public class ObjectiveCalibrationInputTest
    {
        private ObjectiveCalibrationInput _validObjectiveCalibrationInput;

        [TestInitialize]
        public void Init()
        {
            _validObjectiveCalibrationInput = new ObjectiveCalibrationInput(
                probeId: "probeId",
                objectiveId: "objectiveId",
                gain: 1.8,
                previousCalibration: new ObjectiveCalibration());
        }

        [TestMethod]
        public void Check_input_validity_with_good_inputs()
        {
            // Given : right inputs are provided
            var validInput = _validObjectiveCalibrationInput;

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
            var validInput = _validObjectiveCalibrationInput;

            // When serializing in xml Then it works
            SerializationUtils.AssertXmlSerializable(validInput);
        }

        [TestMethod]
        public void Probe_ID_is_mandatory()
        {
            // Given : the probe ID isn't provided
            var invalidInput = _validObjectiveCalibrationInput;
            invalidInput.ProbeId = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Objective_ID_is_mandatory()
        {
            // Given : the objective ID isn't provided
            var invalidInput = _validObjectiveCalibrationInput;
            invalidInput.ObjectiveId = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Previous_calibration_is_optional()
        {
            // Given : the previous calibration isn't provided
            var validInput = _validObjectiveCalibrationInput;
            validInput.PreviousCalibration = null;

            // When
            var validity = validInput.CheckInputValidity();

            // Then
            Assert.IsTrue(validity.IsValid);
            Assert.AreEqual(0, validity.Message.Count);
        }

        [TestMethod]
        public void Gain_is_optional()
        {
            // Given : the gain isn't provided
            var validInput = _validObjectiveCalibrationInput;
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
            var invalidInput = _validObjectiveCalibrationInput;
            invalidInput.ProbeId = null;
            invalidInput.ObjectiveId = null;
            invalidInput.PreviousCalibration = null;
            invalidInput.Gain = double.NaN;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(2, validity.Message.Count);
        }
    }
}
