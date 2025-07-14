using System;

using ADCEngine;

using AdcTools;

namespace BasicModules.ClusterDispatcher
{
    ///////////////////////////////////////////////////////////////////////
    // Classe pour les aiguiller les classes de défauts dans les branches
    ///////////////////////////////////////////////////////////////////////
    [Serializable]
    public class DispatcherDefectClass : Serializable, IValueComparer
    {
        public string DefectClass { get; set; }
        public int BranchIndex { get; set; }

        public DispatcherDefectClass()
        {
            BranchIndex = -1;
        }

        public override string ToString()
        {
            return DefectClass + "=>branch-" + BranchIndex;
        }

        public bool HasSameValue(object obj)
        {
            var @class = obj as DispatcherDefectClass;
            return @class != null &&
                   DefectClass == @class.DefectClass &&
                   BranchIndex == @class.BranchIndex;
        }

    }
}
