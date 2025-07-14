using System;
using System.Collections.Generic;
using System.Threading;

using UnitySC.PM.ANA.Service.Core.Thickness;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Probe;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Dummy
{
    public class DualLiseThicknessMeasurementFlowDummy : DualLiseThicknessMeasurementFlow
    {
        private Random _random;

        public DualLiseThicknessMeasurementFlowDummy(MeasureDualLiseInput input) : base(input)
        {
            _random = new Random();
        }

        protected override void Process()
        {
            Result.LayersThickness = new List<ProbeThicknessMeasure>();

            Thread.Sleep(1000);

            foreach (var layer in Input.MeasureLiseUp.LiseData.Sample.Layers)
            {
                ProbeThicknessMeasure thickness = new ProbeThicknessMeasure(layer.Thickness, 1, layer.IsMandatory, layer.Name);
                Result.LayersThickness.Add(thickness);
                Logger.Information($"Thickness: {thickness}");
            }

            Length customThickness = new Length(Input.UnknownLayer.Thickness.Micrometers + 10 * _random.NextDouble(), LengthUnit.Micrometer);
            ProbeThicknessMeasure unknownLayerThickness = new ProbeThicknessMeasure(customThickness, 1, Input.UnknownLayer.IsMandatory, Input.UnknownLayer.Name);
            Result.LayersThickness.Add(unknownLayerThickness);
            Result.AirGapDown = new Length(250 + 10 * _random.NextDouble(), LengthUnit.Micrometer);
            Result.AirGapUp = new Length(250 + 10 * _random.NextDouble(), LengthUnit.Micrometer);
            Logger.Information($"Thickness: {unknownLayerThickness}");

            foreach (var layer in Input.MeasureLiseDown.LiseData.Sample.Layers)
            {
                ProbeThicknessMeasure thickness = new ProbeThicknessMeasure(layer.Thickness, 1, layer.IsMandatory, layer.Name);
                Result.LayersThickness.Add(thickness);
                Logger.Information($"Thickness: {thickness}");
            }
        }
    }
}
