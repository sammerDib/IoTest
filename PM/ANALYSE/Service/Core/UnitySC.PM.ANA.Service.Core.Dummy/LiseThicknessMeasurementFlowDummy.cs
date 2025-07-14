using System;
using System.Collections.Generic;
using System.Threading;

using UnitySC.PM.ANA.Service.Core.Thickness;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Probe;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Dummy
{
    public class LiseThicknessMeasurementFlowDummy : LiseThicknessMeasurementFlow
    {
        public LiseThicknessMeasurementFlowDummy(MeasureLiseInput input) : base(input)
        {
        }

        protected override void Process()
        {
            Thread.Sleep(1000);

            var rnd = new Random();

            Result.Name = Input.LiseData.Sample.Name;
            Logger.Information($"Sample Name: {Result.Name}");

            Result.LayersThickness = new List<ProbeThicknessMeasure>();

            foreach (var layer in Input.LiseData.Sample.Layers)
            {
                if (! layer.IsMandatory)
                    continue;

                var randcoef = rnd.NextDouble() - 0.5;
                double simulatedThickness_um = layer.Thickness.Micrometers + ((3.0 * randcoef) * layer.Tolerance.GetAbsoluteTolerance(layer.Thickness).Micrometers);
                var layerThickness = simulatedThickness_um.Micrometers();

                ProbeThicknessMeasure thickness = new ProbeThicknessMeasure(layerThickness, 1, layer.IsMandatory, layer.Name);
                Result.LayersThickness.Add(thickness);                
                Logger.Information($"Thickness: {thickness}");
            }

            Result.AirGap = (10.0 + 1.5 * (rnd.NextDouble() - 0.5)).Micrometers();
        }
    }
}
