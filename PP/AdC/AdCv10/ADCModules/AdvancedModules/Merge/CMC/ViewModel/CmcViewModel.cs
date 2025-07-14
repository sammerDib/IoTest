using System.Collections.ObjectModel;

using ADCEngine;

using AdcTools;

using CommunityToolkit.Mvvm.ComponentModel;

namespace AdvancedModules.CmcNamespace
{
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class CmcViewModel : ObservableRecipient
    {
        //=================================================================
        // Propriétés
        //=================================================================
        public CmcParameter Parameter;
        public ObservableCollection<CmcBranchViewModel> BranchInfoList { get; } = new ObservableCollection<CmcBranchViewModel>();

        //=================================================================
        // Constructeur
        //=================================================================
        public CmcViewModel(CmcParameter parameter)
        {
            Parameter = parameter;
        }

        //=================================================================
        // Init
        //=================================================================
        public void Init()
        {
            if (Parameter.Module.Parents.Count == 0 || Parameter.Module.Parents[0] is RootModule)
                return; // CMC non connecté

            // Synchronise le paramètre avec le graphe de la recette
            //......................................................
            Parameter.IntraClusterization.Resize(Parameter.Module.Parents.Count);

            // Crée la liste des Branches
            //...........................
            BranchInfoList.Clear();
            for (int i = 0; i < Parameter.Module.Parents.Count; i++)
            {
                ModuleBase parent = Parameter.Module.Parents[i];
                CmcBranchViewModel info = new CmcBranchViewModel(this, parent);
                BranchInfoList.Add(info);
            }
        }

    }
}
