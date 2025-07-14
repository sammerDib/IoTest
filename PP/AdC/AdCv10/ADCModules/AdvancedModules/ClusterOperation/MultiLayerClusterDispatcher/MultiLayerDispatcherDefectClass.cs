using System;

using ADCEngine;

using AdcTools;

namespace AdvancedModules.MultiLayerClusterDispatcher
{
    ///////////////////////////////////////////////////////////////////////
    // Classe pour les aiguiller les classes de défauts dans les branches
    ///////////////////////////////////////////////////////////////////////
    [Serializable]
    public class DispatcherDefectClass : Serializable, IValueComparer
    {
        public string DefectLabel { get; set; }
        public string DefectLayer { get; set; }
        public int BranchIndex { get; set; }

        public DispatcherDefectClass()
        {
            BranchIndex = -1;
        }

        public override string ToString()
        {
            return DefectLabel + "/" + DefectLayer + "=>branch-" + BranchIndex;
        }

        public bool HasSameValue(object obj)
        {
            var @class = obj as DispatcherDefectClass;
            return @class != null &&
                   DefectLabel == @class.DefectLabel &&
                   DefectLayer == @class.DefectLayer &&
                   BranchIndex == @class.BranchIndex;
        }

    }
}
