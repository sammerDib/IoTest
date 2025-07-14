using System;

using CommunityToolkit.Mvvm.ComponentModel;

namespace AdvancedModules.ClassificationMultiLayer
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Classe encapsulant un boolean car la datatable ne sais pas gérer un nullable (bool?)
    /// qui est le type géreé par la checkbox. Merci Crosoft.
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class BranchBooleanViewModel : ObservableRecipient
    {
        private MultiLayerDefectClassViewModel multiLayerDefectClassViewModel;
        private int branch;
        private ClassificationMultiLayerViewModel vm;

        //=================================================================
        // Constructeur
        //=================================================================
        public BranchBooleanViewModel(ClassificationMultiLayerViewModel vm, MultiLayerDefectClassViewModel multiLayerDefectClassViewModel, int branch)
        {
            this.vm = vm;
            this.multiLayerDefectClassViewModel = multiLayerDefectClassViewModel;
            this.branch = branch;
            SetBool();
        }

        //=================================================================
        // Propriétés
        //=================================================================
        public bool IsEnabled { get; set; } = true;

        private bool? _bool;    // NB: null⇒pas testée, true=>défaut absent, false⇒défaut présent 
        public bool? Bool
        {
            get
            {
                return _bool;
            }
            set
            {
                if (multiLayerDefectClassViewModel.MultiLayerDefectClass.DefectBranchList[branch] == DefectTestType.DefectClassNotUsed)
                    throw new ApplicationException("Can't select defect test");

                if (_bool == value)
                    return;

                _bool = value;

                DefectTestType test;
                if (_bool == false)
                    test = DefectTestType.DefectMustBePresent;
                else if (_bool == true)
                    test = DefectTestType.DefectMustNotBePresent;
                else
                    test = DefectTestType.DoNotTest;
                multiLayerDefectClassViewModel.MultiLayerDefectClass.DefectBranchList[branch] = test;

                OnPropertyChanged();
                vm.Parameter.ReportChange();
            }
        }

        //=================================================================
        //
        //=================================================================
        public override string ToString()
        {
            return "bool?-" + _bool.ToString();
        }


        //=================================================================
        // Teste la layer est utilisée pour cette classe de défauts.
        //=================================================================
        private void SetBool()
        {
            DefectTestType test = multiLayerDefectClassViewModel.MultiLayerDefectClass.DefectBranchList[branch];
            switch (test)
            {
                case DefectTestType.DefectClassNotUsed:
                    _bool = null;
                    IsEnabled = false;
                    break;
                case DefectTestType.DoNotTest:
                    _bool = null;
                    break;
                case DefectTestType.DefectMustBePresent:
                    _bool = false;
                    break;
                case DefectTestType.DefectMustNotBePresent:
                    _bool = true;
                    break;
                default:
                    throw new ApplicationException("unknown DefectTestType: " + test);
            }
        }


    }
}
