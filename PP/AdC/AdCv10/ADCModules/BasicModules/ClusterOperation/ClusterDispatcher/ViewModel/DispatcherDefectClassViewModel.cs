using System.Collections.Generic;

using AdcBasicObjects;

using ADCEngine;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BasicModules.ClusterDispatcher
{
    ///////////////////////////////////////////////////////////////////////
    // Class representant une ligne de la DataTable, 
    // C'est à dire une classe de défaut
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class DispatcherDefectClassViewModel : ObservableRecipient
    {
        //=================================================================
        // Propriétés
        //=================================================================

        //-----------------------------------------------------------------
        // Propriétés provenant de DispatcherDefectClass
        //-----------------------------------------------------------------
        public int BranchIndex
        {
            get { return DispatcherDefectClass.BranchIndex; }
            set
            {
                if (DispatcherDefectClass.BranchIndex != value)
                {
                    DispatcherDefectClass.BranchIndex = value;
                    OnPropertyChanged();
                    Parameter.ReportChange();
                }
            }
        }

        public string DefectClass
        {
            get { return DispatcherDefectClass.DefectClass; }
            set { DispatcherDefectClass.DefectClass = value; }
        }

        //-----------------------------------------------------------------
        // Propriétés pour l'affichage dans la DataGrid
        //-----------------------------------------------------------------
        public List<BranchBooleanViewModel> branchTable;
        public List<string> AvailableLayerList { get; set; }
        public List<Characteristic> AvailableCharacteristicList { get; set; }

        //-----------------------------------------------------------------
        // Autres Propriétés 
        //-----------------------------------------------------------------
        public ClusterDispatcherViewModel vm;
        public DispatcherDefectClass DispatcherDefectClass { get; private set; }
        public ClusterDispatcherParameter Parameter { get { return vm.Parameter; } }

        //=================================================================
        // Constructeur
        //=================================================================
        public DispatcherDefectClassViewModel(ClusterDispatcherViewModel vm, DispatcherDefectClass dispatcherDefectClass)
        {
            this.vm = vm;
            DispatcherDefectClass = dispatcherDefectClass;

            DefectClass = DispatcherDefectClass.DefectClass;

            int count = vm.Parameter.Module.Children.Count;
            branchTable = new List<BranchBooleanViewModel>();
            for (int i = 0; i < count; i++)
            {
                if (vm.Parameter.Module.Children[i] is TerminationModule)
                    continue;

                BranchBooleanViewModel branchvm = new BranchBooleanViewModel(this, i);
                branchvm.Bool = (i == DispatcherDefectClass.BranchIndex);
                branchTable.Add(branchvm);
            }
        }

    }
}
