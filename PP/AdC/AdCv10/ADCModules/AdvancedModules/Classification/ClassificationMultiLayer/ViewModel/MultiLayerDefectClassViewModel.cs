using System.Collections.Generic;
using System.Linq;

using AdcBasicObjects;

using CommunityToolkit.Mvvm.ComponentModel;

namespace AdvancedModules.ClassificationMultiLayer
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class representant une ligne de la DataTable, c'est à dire une 
    /// classe de défaut
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class MultiLayerDefectClassViewModel : ObservableRecipient
    {
        //=================================================================
        // Propriétés
        //=================================================================

        //-----------------------------------------------------------------
        // Propriétés provenant de MultiLayerDefectClass
        //-----------------------------------------------------------------
        public string DefectLabel
        {
            get { return MultiLayerDefectClass.DefectLabel; }
            set { MultiLayerDefectClass.DefectLabel = value; }
        }

        public int MeasuredBranch
        {
            get { return MultiLayerDefectClass.MeasuredBranch; }
            set
            {
                if (MultiLayerDefectClass.MeasuredBranch != value)
                {
                    MultiLayerDefectClass.MeasuredBranch = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(MeasuredBranchString));
                    vm.Module.ReportChange();
                }
            }
        }

        public string MeasuredBranchString
        {
            // +/-1 parce que la liste commence par "automatic"
            get { return BranchList[MeasuredBranch + 1].Value; }
            set { MeasuredBranch = BranchList.First(x => x.Value == value).Key; }
        }

        public Characteristic CharacteristicForAutomaticMeasure
        {
            get { return MultiLayerDefectClass.CharacteristicForAutomaticLayer; }
            set
            {
                if (value == MultiLayerDefectClass.CharacteristicForAutomaticLayer)
                    return;
                MultiLayerDefectClass.CharacteristicForAutomaticLayer = value;
                OnPropertyChanged();
                vm.Module.ReportChange();
            }
        }

        //-----------------------------------------------------------------
        // Propriétés pour l'affichage dans la DataGrid
        //-----------------------------------------------------------------
        public BranchBooleanViewModel[] LayerTable { get; set; }
        public List<KeyValuePair<int, string>> BranchList { get; private set; }
        public List<Characteristic> AvailableCharacteristicList { get; set; }

        //-----------------------------------------------------------------
        // Autres Propriétés 
        //-----------------------------------------------------------------
        public ClassificationMultiLayerViewModel vm;
        public MultiLayerDefectClass MultiLayerDefectClass { get; private set; }
        public ClassificationMultiLayerParameter Parameter { get { return vm.Parameter; } }

        //=================================================================
        // Constructeur
        //=================================================================
        public MultiLayerDefectClassViewModel(ClassificationMultiLayerViewModel vm, MultiLayerDefectClass multiLayerDefectClass)
        {
            this.vm = vm;
            MultiLayerDefectClass = multiLayerDefectClass;

            DefectLabel = MultiLayerDefectClass.DefectLabel;
            AvailableCharacteristicList = vm.AvailableCharacteristicList;

            int count = vm.BranchList.Count;
            LayerTable = new BranchBooleanViewModel[count];
            for (int i = 0; i < count; i++)
                LayerTable[i] = new BranchBooleanViewModel(vm, this, i);

            BranchList = new List<KeyValuePair<int, string>>();
            BranchList.Add(vm.AvailableBranchListwithAutomatic[0]);   // automatic
            for (int i = 0; i < multiLayerDefectClass.DefectBranchList.Count; i++)
            {
                bool possible = multiLayerDefectClass.DefectBranchList[i] != DefectTestType.DefectClassNotUsed;
                if (possible)
                    BranchList.Add(vm.AvailableBranchListwithAutomatic[i + 1]);
            }
        }

    }
}
