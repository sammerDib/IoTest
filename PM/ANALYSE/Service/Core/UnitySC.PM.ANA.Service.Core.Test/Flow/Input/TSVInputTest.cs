using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Input
{
    [TestClass]
    public class TSVInputTest
    {
        private TSVInput _validTSVInput;

        [TestInitialize]
        public void Init()
        {
            _validTSVInput = new TSVInput(
                cameraId: "cameraId",
                probeSettings: new SingleLiseSettings() { ProbeId = "probeLiseId", LiseGain = 1.8 },
                shape: UnitySC.Shared.Format.Metro.TSV.TSVShape.Elipse,
                shapeDetectionMode: ShapeDetectionModes.AverageInArea,
                approximateDepth: 1.Millimeters(),
                approximateLength: 1.Millimeters(),
                approximateWidth: 1.Millimeters(),
                acquisitionStrategy: TSVAcquisitionStrategy.Standard,
                measurePrecision: TSVMeasurePrecision.Fast,
                depthTolerance: 1.Millimeters(),
                lengthTolerance: 1.Millimeters(),
                widthTolerance: 1.Millimeters(),
                roi: null,
                physicalLayers: new List<LayerSettings>());
        }

        [TestMethod]
        public void Check_input_validity_with_good_inputs()
        {
            // Given : right inputs are provided
            var validInput = _validTSVInput;

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
            var validInput = _validTSVInput;

            // When serializing in xml Then it works
            SerializationUtils.AssertXmlSerializable(validInput);
        }

        [TestMethod]
        public void Camera_ID_is_mandatory()
        {
            // Given : the camera Id isn't provided
            var invalidInput = _validTSVInput;
            invalidInput.CameraId = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Probe_is_mandatory()
        {
            // Given : the probe Lise ID isn't provided
            var invalidInput = _validTSVInput;
            invalidInput.Probe = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Approximate_depth_is_mandatory()
        {
            // Given : the approximate depth isn't provided
            var invalidInput = _validTSVInput;
            invalidInput.ApproximateDepth = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(1, validity.Message.Count);
        }

        [TestMethod]
        public void Depth_Tolerance_is_mandatory()
        {
            // Given : the approximate depth isn't provided
            var invalidInput = _validTSVInput;
            invalidInput.DepthTolerance = null;

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
            var invalidInput = _validTSVInput;
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
            var invalidInput = _validTSVInput;
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
            var invalidInput = _validTSVInput;
            invalidInput.LengthTolerance = null;
            invalidInput.WidthTolerance = null;

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
            var invalidInput = _validTSVInput;
            invalidInput.CameraId = null;
            invalidInput.Probe = null;
            invalidInput.ApproximateDepth = null;
            invalidInput.ApproximateLength = null;
            invalidInput.ApproximateWidth = null;

            // When
            var validity = invalidInput.CheckInputValidity();

            // Then
            Assert.IsFalse(validity.IsValid);
            Assert.AreEqual(5, validity.Message.Count);
        }
    }
}
