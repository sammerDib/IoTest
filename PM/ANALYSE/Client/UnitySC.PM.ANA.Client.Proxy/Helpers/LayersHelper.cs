using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.Shared.Configuration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client.Proxy.Helpers
{
    public static class LayersHelper
    {
        public static  List<LayerSettings> GetPhysicalLayers(int stepId)
        {
            var toolService = new ServiceInvoker<IToolService>("ToolService", ClassLocator.Default.GetInstance<SerilogLogger<IToolService>>(), ClassLocator.Default.GetInstance<IMessenger>(), ClientConfiguration.GetDataAccessAddress());

            var step = toolService.Invoke(x => x.GetStep(stepId));
            var layers = new List<LayerSettings>();
            foreach (var layer in step.Layers)
            {
                layers.Add(new LayerSettings()
                {
                    Name = layer.Name,
                    Thickness = new Length(layer.Thickness, LengthUnit.Micrometer),
                    RefractiveIndex = layer.RefractiveIndex
                });
            }
            return layers;
        }
    }
}
