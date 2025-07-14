using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Measure.Thickness
{
    public class UnifiedLayers
    {
        public Dictionary<ProbeSettings, List<Layer>> LayersByProbeSettings;
        public List<Layer> LayersToMeasure;
        public List<LayerSettings> PhysicalLayers;
        public Dictionary<string, Length> OffsetByLayerToMeasureName;
        private ThicknessSettings _thicknessSettings;

        public UnifiedLayers(ThicknessSettings thicknessSettings)
        {
            _thicknessSettings = thicknessSettings;
            UnifyLayersAndGroupByProbeSettings();
        }

        public void UnifyLayersAndGroupByProbeSettings()
        {
            LayersToMeasure = new List<Layer>();
            OffsetByLayerToMeasureName = new Dictionary<string, Length>();
            PhysicalLayers = new List<LayerSettings>(_thicknessSettings.PhysicalLayers);

            foreach (var layerToMeasure in _thicknessSettings.LayersToMeasure)
            {
                if (layerToMeasure.IsWaferTotalThickness)
                {
                    LayersToMeasure.Add(layerToMeasure);
                    continue;
                }

                var newLayer = layerToMeasure;
                if (layerToMeasure.PhysicalLayers.Count > 1)
                {
                    newLayer = GatherLayers(layerToMeasure);
                    if (newLayer.MultipleLayersOffset.Value > 0)
                    {
                        OffsetByLayerToMeasureName.Add(newLayer.Name, newLayer.MultipleLayersOffset);
                    }
                    UpdatePhysicalLayersList(layerToMeasure, newLayer);
                }

                LayersToMeasure.Add(newLayer);
            }

            LayersByProbeSettings = LayersToMeasure.GroupBy(layer => layer.ProbeSettings).ToDictionary(gc => gc.Key, gc => gc.ToList());
        }

        private static Layer GatherLayers(Layer layer)
        {
            var totalThickness = 0.Micrometers();
            var newLayer = new LayerSettings();
            var newListOfLayer = new List<LayerSettings>();

            foreach (var l in layer.PhysicalLayers)
            {
                totalThickness += l.Thickness;
            }

            newLayer.Name = layer.Name;
            newLayer.Thickness = totalThickness;
            newLayer.RefractiveIndex = layer.RefractiveIndex;
            newLayer.MaterialName = "Composites";
            newLayer.LayerColor = layer.LayerColor;
            newListOfLayer.Add(newLayer);
            var newLayerAddThickness = new Layer()
            {
                Name = layer.Name,
                PhysicalLayers = newListOfLayer,
                ProbeSettings = layer.ProbeSettings,
                ThicknessTolerance = layer.ThicknessTolerance,
                IsWaferTotalThickness = layer.IsWaferTotalThickness,
                RefractiveIndex = layer.RefractiveIndex,
                MultipleLayersOffset = layer.MultipleLayersOffset,
            };
            return newLayerAddThickness;
        }

        private void UpdatePhysicalLayersList(Layer layerToMeasureBeforeUnification, Layer layerUnified)
        {
            int index = int.MaxValue;
            foreach (var physicalLayer in layerToMeasureBeforeUnification.PhysicalLayers.ToList())
            {
                int indexOfLayer = PhysicalLayers.FindIndex(x => x.Name == physicalLayer.Name);
                if (indexOfLayer < 0)
                {
                    return;
                }

                index = Math.Min(index, indexOfLayer);
                PhysicalLayers.RemoveAt(indexOfLayer);
            }

            PhysicalLayers.Insert(index, layerUnified.PhysicalLayers[0]);
        }
    }
}
