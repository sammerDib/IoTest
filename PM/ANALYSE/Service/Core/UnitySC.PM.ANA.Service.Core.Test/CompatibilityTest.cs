using System.Collections.Generic;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface.Compatibility;
using UnitySC.PM.ANA.Service.Interface.Compatibility.Capability;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.Test
{
    [TestClass]
    public class CompatibilityTest
    {
        [TestMethod]
        public void CreateProbeCompatibility()
        {
            var compatibility = new ProbeCompatibility
            {
                Probes = new List<Probe>()
            };

            // Lise
            var lise = new Probe
            {
                Name = "Lise",
                Capabilities = new List<CapabilityBase>()
            };
            var crossLayer = new CrossLayer
            {
                MaxRefractiveIndexOfLayerToCross = 1.0,
                MaxThicknessOfLayersToCross = 50000,
                NumbersOfLayersToCross = 4
            };

            var distanceMeasure = new DistanceMeasure
            {
                MaxMeasureRange = 5000,
                MinMeasureRange = 30,
                MultiRefractiveIndexCompatibility = true
            };

            var thicknessMeasure = new ThicknessMeasure
            {
                MaxThickness = 5000,
                MinThickness = 90,
                MinRefractiveIndex = 0.8,
                MaxRefractiveIndex = 1.8,
                MaxNumberOfLayersMeasured = 5
            };

            lise.Capabilities.Add(crossLayer);
            lise.Capabilities.Add(distanceMeasure);
            lise.Capabilities.Add(thicknessMeasure);

            // Dual Lise
            var dualLise = new Probe
            {
                Name = "DualLise",
                Capabilities = new List<CapabilityBase>()
            };

            var dualLisethicknessMeasure = new ThicknessMeasure
            {
                MaxThickness = 30,
                MinThickness = 5,
                MinRefractiveIndex = 0.2,
                MaxRefractiveIndex = 3,
                MaxNumberOfLayersMeasured = 1
            };
            dualLise.Capabilities.Add(dualLisethicknessMeasure);

            // Spiro
            var spiro = new Probe
            {
                Name = "Spiro",
                Capabilities = new List<CapabilityBase>()
            };

            var spiroThicknessMeasure = new ThicknessMeasure();
            spiroThicknessMeasure.MaxThickness = 300;
            spiroThicknessMeasure.MinThickness = 5;
            spiroThicknessMeasure.MinRefractiveIndex = 0.8;
            spiroThicknessMeasure.MaxRefractiveIndex = 1.8;
            spiroThicknessMeasure.MaxNumberOfLayersMeasured = 1;

            spiro.Capabilities.Add(spiroThicknessMeasure);

            compatibility.Probes.Add(lise);
            compatibility.Probes.Add(dualLise);
            compatibility.Probes.Add(spiro);

            string fullPath = Path.GetFullPath(@".\ProbeCompatibility.xml");
            XML.Serialize(compatibility, fullPath);
            var res = XML.Deserialize<ProbeCompatibility>(fullPath);
        }
    }
}