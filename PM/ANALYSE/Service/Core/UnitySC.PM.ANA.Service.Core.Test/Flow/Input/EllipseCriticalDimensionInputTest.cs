using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.Shared.Image;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Input
{
    [TestClass]
    public class EllipseCriticalDimensionInputTest
    {
        private EllipseCriticalDimensionInput _validEllipseCriticalDimensionInput;

        [TestInitialize]
        public void Init()
        {
            _validEllipseCriticalDimensionInput = new EllipseCriticalDimensionInput(
                image: new ServiceImage(),
                objectiveId: "objectiveId",
                roi: new CenteredRegionOfInterest(),
                approximateLength: 1.Millimeters(),
                approximateWidth: 1.Millimeters(),
                lengthTolerance: 1.Millimeters(),
                widthTolerance: 1.Millimeters());
        }

        [TestMethod]
        public void Check_input_validity_with_good_inputs()
        {
            // Given : right inputs are provided
            var validInput = _validEllipseCriticalDimensionInput;

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
            var validInput = _validEllipseCriticalDimensionInput;

            // When serializing in xml Then it works
            SerializationUtils.AssertXmlSerializable(validInput);
        }

        [TestMethod]
        public void Objective_ID_is_mandatory()
        {
            // Given : the objective ID isn't provided
            var invalidInput = _validEllipseCriticalDimensionInput;
            invalidInput.ObjectiveId = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Image_is_mandatory()
        {
            // Given : the image isn't provided
            var invalidInput = _validEllipseCriticalDimensionInput;
            invalidInput.Image = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Approximate_length_is_mandatory()
        {
            // Given : the approximate length isn't provided
            var invalidInput = _validEllipseCriticalDimensionInput;
            invalidInput.ApproximateLength = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Approximate_width_is_mandatory()
        {
            // Given : the approximate width isn't provided
            var invalidInput = _validEllipseCriticalDimensionInput;
            invalidInput.ApproximateWidth = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Detection_tolerance_is_mandatory()
        {
            // Given : the detection tolerance isn't provided
            var invalidInput = _validEllipseCriticalDimensionInput;
            invalidInput.LengthTolerance = null;
            invalidInput.WidthTolerance = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(2, validity.Message.Count);
        }

        [TestMethod]
        public void Region_of_intereset_is_optional()
        {
            // Given : the region of interest isn't provided
            var validInput = _validEllipseCriticalDimensionInput;
            validInput.RegionOfInterest = null;

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
            var invalidInput = _validEllipseCriticalDimensionInput;
            invalidInput.ObjectiveId = null;
            invalidInput.Image = null;
            invalidInput.ApproximateLength = null;
            invalidInput.ApproximateWidth = null;
            invalidInput.LengthTolerance = null;
            invalidInput.WidthTolerance = null;
            invalidInput.RegionOfInterest = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(6, validity.Message.Count);
        }
    }
}
