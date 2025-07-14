using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Input
{
    [TestClass]
    public class TSVDepthInputTest
    {
        private TSVDepthInput _validTSVDepthInput;

        [TestInitialize]
        public void Init()
        {
            _validTSVDepthInput = new TSVDepthInput(
                approximateDepth: 10.Micrometers(),
                approximateWidth: 1.Micrometers(),
                depthTolerance: 2.Micrometers(),
                acquisitionStrategy: TSVAcquisitionStrategy.Standard,
                measurePrecision: TSVMeasurePrecision.Fast,
                probeSettings: new SingleLiseSettings() { ProbeId = "LiseUpId" });
        }

        [TestMethod]
        public void Check_input_validity_with_good_inputs()
        {
            // Given : right inputs are provided
            var validInput = _validTSVDepthInput;

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
            var validInput = _validTSVDepthInput;

            // When serializing in xml Then it works
            SerializationUtils.AssertXmlSerializable(validInput);
        }

        [TestMethod]
        public void Approximate_depth_is_mandatory()
        {
            // Given : approximate depth isn't provided
            var invalidInput = _validTSVDepthInput;
            invalidInput.ApproximateDepth = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Approximate_width_is_mandatory()
        {
            // Given : approximate width isn't provided
            var invalidInput = _validTSVDepthInput;
            invalidInput.ApproximateWidth = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Probe_is_mandatory()
        {
            // Given : probe isn't provided
            var invalidInput = _validTSVDepthInput;
            invalidInput.Probe = null;

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
            var invalidInput = _validTSVDepthInput;
            invalidInput.Probe = null;
            invalidInput.ApproximateWidth = null;
            invalidInput.ApproximateDepth = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(3, validity.Message.Count);
        }
    }
}
