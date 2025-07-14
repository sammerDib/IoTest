using System.Collections.Generic;
using System.Windows.Media;

using ADCEngine;

using CommunityToolkit.Mvvm.ComponentModel;

namespace AdvancedModules.MultiLayerClusterDispatcher
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
        // Propriétés Bindables
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
                    OnPropertyChanged(nameof(Branch));
                    Parameter.ReportChange();
                }
            }
        }

        public string Branch
        {
            get
            {
                if (BranchIndex >= 0)
                    return vm.Branches[BranchIndex];
                else
                    return "Not in branch";
            }
        }

        public Brush Brush { get; set; }

        public string DefectLabel
        {
            get { return DispatcherDefectClass.DefectLabel; }
            set { DispatcherDefectClass.DefectLabel = value; }
        }

        public string DefectLayer
        {
            get { return DispatcherDefectClass.DefectLayer; }
            set { DispatcherDefectClass.DefectLayer = value; }
        }

        public List<BranchBooleanViewModel> BranchTable { get; private set; }

        //-----------------------------------------------------------------
        // Autres Propriétés 
        //-----------------------------------------------------------------
        public MultiLayerClusterDispatcherViewModel vm;
        public DispatcherDefectClass DispatcherDefectClass { get; private set; }
        public MultiLayerClusterDispatcherParameter Parameter { get { return vm.Parameter; } }

        //=================================================================
        // Constructeur
        //=================================================================
        public DispatcherDefectClassViewModel(MultiLayerClusterDispatcherViewModel vm, DispatcherDefectClass dispatcherDefectClass)
        {
            this.vm = vm;
            DispatcherDefectClass = dispatcherDefectClass;
            DefectLabel = dispatcherDefectClass.DefectLabel;
            DefectLayer = dispatcherDefectClass.DefectLayer;

            int count = vm.Parameter.Module.Children.Count;
            BranchTable = new List<BranchBooleanViewModel>();
            for (int i = 0; i < count; i++)
            {
                if (vm.Parameter.Module.Children[i] is TerminationModule)
                    continue;

                BranchBooleanViewModel branchvm = new BranchBooleanViewModel(this, i);
                branchvm.Bool = (i == DispatcherDefectClass.BranchIndex);
                BranchTable.Add(branchvm);
            }
        }

        //=================================================================
        //
        //=================================================================
        public override string ToString()
        {
            return DefectLabel;
        }

    }
}
