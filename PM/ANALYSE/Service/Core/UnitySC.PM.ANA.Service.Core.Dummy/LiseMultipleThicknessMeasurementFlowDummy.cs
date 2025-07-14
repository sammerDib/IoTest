using System.Collections.Generic;
using System.Threading;

using UnitySC.PM.ANA.Service.Core.Thickness;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Probe;

namespace UnitySC.PM.ANA.Service.Core.Dummy
{
    public class LiseMultipleThicknessMeasurementFlowDummy : LiseMultipleThicknessMeasurementFlow
    {
        public LiseMultipleThicknessMeasurementFlowDummy(MultipleMeasuresLiseInput input) : base(input)
        {
        }

        protected override void Process()
        {
            Thread.Sleep(1000);

            var probeThicknessMeasures = new List<ProbeThicknessMeasure>();
            foreach (var layer in Input.MeasureLise.Sample.Layers)
            {
                ProbeThicknessMeasure thickness = new ProbeThicknessMeasure(layer.Thickness, 1, layer.IsMandatory, layer.Name);
                probeThicknessMeasures.Add(thickness);
                Logger.Information($"Thickness: {thickness}");
            }

            for (int i = 0; i < Input.NbMeasures; i++)
            {
                Result.ProbeThicknessMeasures.Add(probeThicknessMeasures);
            }
        }
    }
}
