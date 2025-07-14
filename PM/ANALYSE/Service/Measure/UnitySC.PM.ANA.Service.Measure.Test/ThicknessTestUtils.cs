using System;
using System.Collections.Generic;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Thickness;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Service.Shared.TestUtils.LiseTestUtils;

namespace UnitySC.PM.ANA.Service.Measure.Test
{
    public static class ThicknessTestUtils
    {
        public static MeasureContext CreateContext()
        {
            int id = 1;
            double x = 100, y = 300;
            var measurePoint = new MeasurePoint(id, x, y, false);
            var dieIndex = new DieIndex();
            var context = new MeasureContext(measurePoint, dieIndex, null);
            return context;
        }

        public static SingleLiseSettings CreateProbeSettings(string probeId, double gain = 1.8, string objective = "ObjectiveSelector01")
        {
            return new SingleLiseSettings()
            {
                ProbeId = probeId,
                LiseGain = gain,
                ProbeObjectiveContext = new ObjectiveContext
                {
                    ObjectiveId = objective
                }
            };
        }

        public static DualLiseSettings CreateDualProbeSettings(string probeId, SingleLiseSettings probeUp, SingleLiseSettings probeDown)
        {
            return new DualLiseSettings()
            {
                ProbeId = probeId,
                LiseUp = probeUp,
                LiseDown = probeDown
            };
        }

        public static LayerSettings CreateLayer(Length thickness, int id, double refractiveIndex = double.NaN)
        {
            return new LayerSettings()
            {
                Name = $"MeasurableLayers number {id}",
                Thickness = thickness,
                RefractiveIndex = double.IsNaN(refractiveIndex) ? MaterialRefractionIndex : refractiveIndex
            };
        }

        public static LayerSettings CreateUnknownLayer(Length thickness, int id)
        {
            return new LayerSettings()
            {
                Name = $"MeasurableLayers number {id}",
                Thickness = thickness,
                RefractiveIndex = double.NaN
            };
        }

        public static Layer CreateLayerGroup(ProbeSettings probeSettings, List<LayerSettings> layers, int id, double refractiveIndex = 1.43, bool totalThickness = false)
        {
            return new Layer()
            {
                Name = $"MeasurableLayers to measure {id}",
                PhysicalLayers = layers,
                ProbeSettings = probeSettings,
                ThicknessTolerance = new LengthTolerance(50, LengthToleranceUnit.Micrometer),
                IsWaferTotalThickness = totalThickness,
                RefractiveIndex = refractiveIndex
            };
        }

        public static Layer CreateLayerGroupWithOffset(ProbeSettings probeSettings, List<LayerSettings> layers, int id, Length offset, double refractiveIndex = double.NaN, bool totalThickness = false)
        {
            return new Layer()
            {
                Name = $"MeasurableLayers to measure {id}",
                PhysicalLayers = layers,
                ProbeSettings = probeSettings,
                ThicknessTolerance = new LengthTolerance(50, LengthToleranceUnit.Micrometer),
                IsWaferTotalThickness = totalThickness,
                RefractiveIndex = double.IsNaN(refractiveIndex) ? MaterialRefractionIndex : refractiveIndex,
                MultipleLayersOffset = offset
            };
        }

        public static Layer CreateFakeLayerGroupToMeasureTotalThickness(List<LayerSettings> layers, ProbeSettings probeSettings)
        {
            return new Layer()
            {
                Name = $"MeasurableLayers to measure total thickness",
                PhysicalLayers = layers,
                ProbeSettings = probeSettings,
                ThicknessTolerance = new LengthTolerance(50, LengthToleranceUnit.Micrometer),
                IsWaferTotalThickness = true,
                RefractiveIndex = MaterialRefractionIndex
            };
        }

        public static ThicknessSettings CreateThicknessSettings(List<LayerSettings> layers, List<Layer> layerGroupsToMeasure)
        {
            return new ThicknessSettings()
            {
                Name = "Thickness Test",
                IsActive = true,
                MeasurePoints = new List<int> { 1, 20, 30 },
                NbOfRepeat = 1,
                PhysicalLayers = layers,
                LayersToMeasure = layerGroupsToMeasure,
                WarpTargetMax = 5.Millimeters(),
                Strategy = AcquisitionStrategy.Standard
            };
        }

        public static ThicknessPointResult CreateThicknessPointResult(double x, double y, Length airGapUp, Length airGapDown, double totalThicknessMm)
        {
            return new ThicknessPointResult()
            {
                XPosition = x,
                YPosition = y,
                Datas = new List<MeasurePointDataResultBase>()
                {
                    CreateThicknessPointData(airGapUp, airGapDown, totalThicknessMm)
                }
            };
        }

        public static ThicknessPointData CreateThicknessPointData(Length airGapUp, Length airGapDown, double totalThicknessMm)
        {
            // Needed because TotalThickness is private set
            var thicknessResultSettings = new ThicknessResultSettings()
            {
                TotalTarget = 0.Millimeters(), // Needed for the unit in ComputeTotalThickness
                TotalNotMeasuredLayers = totalThicknessMm,
            };
            var pointData = new ThicknessPointData()
            {
                AirGapUp = airGapUp,
                AirGapDown = airGapDown,
                ThicknessLayerResults = new List<ThicknessLengthResult>(), // Needed non null for ComputeTotalThickness
                Timestamp = DateTime.UtcNow
            };
            pointData.ComputeTotalThickness(thicknessResultSettings);
            return pointData;
        }
    }
}
