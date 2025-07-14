using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Measure.Shared;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Measure.Thickness
{
    public class MeasureToolsForLayers
    {
        private AnaHardwareManager _hardwareManager;
        private ThicknessMeasureTools _measureTools;
        private ThicknessSettings _thicknessSettings;
        private int _firstUnmeasurableLayer;
        private int _lastUnmeasurableLayer;

        public MeasureToolsForLayers(AnaHardwareManager hardwareManager)
        {
            _hardwareManager = hardwareManager;
        }

        public ThicknessMeasureTools FindMeasureTools(ThicknessSettings thicknessSettings)
        {
            _thicknessSettings = thicknessSettings;
            _measureTools = new ThicknessMeasureTools();
            _measureTools.MeasureToolsForLayers = new List<ThicknessMeasureToolsForLayer>();

            FindUnmeasurableLayers();
            foreach (var layerToMeasure in thicknessSettings.LayersToMeasure)
            {
                _measureTools.MeasureToolsForLayers.Add(FindMeasureToolsForLayer(layerToMeasure));
            }

            return _measureTools;
        }

        private void FindUnmeasurableLayers()
        {
            _firstUnmeasurableLayer = -1;
            _lastUnmeasurableLayer = -1;
            for (var index = 0; index < _thicknessSettings.PhysicalLayers.Count; index++)
            {
                LayerSettings physicalLayer = _thicknessSettings.PhysicalLayers[index];
                // TODO: to be define : Phantom layer?, Opaque Layer. According specs, we should have the same behavior for all these layers cases
                if (IsUnknownLayer(physicalLayer))
                {
                    if (_firstUnmeasurableLayer == -1)
                    {
                        _firstUnmeasurableLayer = index;
                        _lastUnmeasurableLayer = index;
                    }
                    else
                    {
                        _lastUnmeasurableLayer = index;
                    }
                }
            }
        }

        private static bool IsUnknownLayer(LayerSettings physicalLayer)
        {
            return physicalLayer.RefractiveIndex.IsNullOrNaN();
        }

        private ThicknessMeasureToolsForLayer FindMeasureToolsForLayer(Layer layerToMeasure)
        {
            var thicknessMeasureToolsForLayer = new ThicknessMeasureToolsForLayer();
            thicknessMeasureToolsForLayer.UpProbes = new List<ProbeWithObjectivesMaterial>();
            thicknessMeasureToolsForLayer.DownProbes = new List<ProbeWithObjectivesMaterial>();
            thicknessMeasureToolsForLayer.DualProbes = new List<DualProbeWithObjectivesMaterial>();

            thicknessMeasureToolsForLayer.NameLayerToMeasure = layerToMeasure.Name;
            ProbeWithObjectivesMaterial probeUp = MeasureToolsHelper.GetProbeLiseWithObjectives(_hardwareManager, ModulePositions.Up);
            ProbeWithObjectivesMaterial probeDown = MeasureToolsHelper.GetProbeLiseWithObjectives(_hardwareManager, ModulePositions.Down);
            DualProbeWithObjectivesMaterial dualProbe = MeasureToolsHelper.GetDualLiseProbe(_hardwareManager, probeUp, probeDown);

            var ismeasurableLayer = IsMeasurableLayer(layerToMeasure);
            var canUseUpProbe = IsThereOnlyMeasurableLayersAbove(layerToMeasure);
            var canUseDownProbe = IsThereOnlyMeasurableLayersBelow(layerToMeasure);

            if (ismeasurableLayer && canUseUpProbe)
            {
                thicknessMeasureToolsForLayer.UpProbes.Add(probeUp);
            }

            if (ismeasurableLayer && canUseDownProbe)
            {
                thicknessMeasureToolsForLayer.DownProbes.Add(probeDown);
            }

            if (!ismeasurableLayer && canUseUpProbe && canUseDownProbe || ismeasurableLayer && IsLayerToMeasureTotalWafer(layerToMeasure))
            {
                thicknessMeasureToolsForLayer.DualProbes.Add(dualProbe);
            }

            return thicknessMeasureToolsForLayer;
        }

        private bool IsLayerToMeasureTotalWafer(Layer layerToMeasure)
        {
            return layerToMeasure.IsWaferTotalThickness || _thicknessSettings.LayersToMeasure.Count == 1
                && _thicknessSettings.PhysicalLayers.Count == layerToMeasure.PhysicalLayers.Count;
        }

        private bool IsMeasurableLayer(Layer layerToMeasure)
        {
            return !layerToMeasure.PhysicalLayers.Any(pLayer => pLayer.RefractiveIndex.IsNullOrNaN());
        }

        private bool IsThereOnlyMeasurableLayersAbove(Layer layerToMeasure)
        {
            if (_firstUnmeasurableLayer == -1)
            {
                return true;
            }

            var physicalLayerToMeasure = layerToMeasure.PhysicalLayers.First();
            var physicalLayerToMeasureIndex = _thicknessSettings.PhysicalLayers.FindIndex(pLayer => pLayer == physicalLayerToMeasure);
            if (physicalLayerToMeasureIndex > _firstUnmeasurableLayer)
            {
                return false;
            }

            return true;
        }

        private bool IsThereOnlyMeasurableLayersBelow(Layer layerToMeasure)
        {
            if (_firstUnmeasurableLayer == -1 && _lastUnmeasurableLayer == -1)
            {
                return true;
            }

            var physicalLayerToMeasure = layerToMeasure.PhysicalLayers.Last();
            var physicalLayerToMeasureIndex = _thicknessSettings.PhysicalLayers.FindIndex(pLayer => pLayer == physicalLayerToMeasure);
            if (physicalLayerToMeasureIndex < _lastUnmeasurableLayer)
            {
                return false;
            }

            return true;
        }
    }
}
