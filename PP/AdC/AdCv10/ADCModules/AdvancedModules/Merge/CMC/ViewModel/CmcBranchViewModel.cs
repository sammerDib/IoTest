using System.Collections.Generic;

using ADCEngine;

using CommunityToolkit.Mvvm.ComponentModel;

namespace AdvancedModules.CmcNamespace
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Classe représentant une branche du CMC
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class CmcBranchViewModel : ObservableRecipient
    {
        //=================================================================
        // Propriétés exposées dans la dataGrid
        //=================================================================
        public int Branch { get; set; }
        public string ParentName { get; set; }
        public string LayerName { get; set; }

        public bool IntraClusterization
        {
            get
            {
                return cmcViewModel.Parameter.IntraClusterization[Branch];
            }
            set
            {
                if (value == cmcViewModel.Parameter.IntraClusterization[Branch])
                    return;
                cmcViewModel.Parameter.IntraClusterization[Branch] = value;

                cmcViewModel.Parameter.ReportChange();
                OnPropertyChanged(nameof(IntraClusterization));
            }
        }

        //=================================================================
        // Variables membres
        //=================================================================
        private CmcViewModel cmcViewModel;

        //=================================================================
        // Constructeur
        //=================================================================
        public CmcBranchViewModel(CmcViewModel cmcViewModel, ModuleBase parent)
        {
            this.cmcViewModel = cmcViewModel;

            Branch = cmcViewModel.Parameter.Module.Parents.IndexOf(parent);
            ParentName = parent.ToString();

            LayerName = GetLayerName(parent);
        }

        //=================================================================
        //
        //=================================================================
        private string GetLayerName(ModuleBase module)
        {
            List<ModuleBase> loaders = module.FindAncestors(m => m is IDataLoader);
            if (loaders.Count == 0)
                return "<no layer>";

            IDataLoader loader = (IDataLoader)loaders[0];
            return loader.LayerName;
        }

    }
}
