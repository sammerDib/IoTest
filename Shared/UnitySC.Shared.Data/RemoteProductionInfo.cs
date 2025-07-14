using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace UnitySC.Shared.Data
{
    public class RemoteProductionInfo
    {
        public Material ProcessedMaterial { get; set; }

        public string DFRecipeName { get; set; } = string.Empty;

        // Module == for both PM or PP
        public string ModuleRecipeName { get; set; } = string.Empty;

        public DateTime ModuleStartRecipeTime { get; set; } = DateTime.MinValue;
    }
}
