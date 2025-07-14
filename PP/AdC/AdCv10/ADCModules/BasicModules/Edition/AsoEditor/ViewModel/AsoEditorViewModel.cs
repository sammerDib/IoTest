using System.Collections.Generic;

using AdcBasicObjects;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BasicModules.AsoEditor
{
    ///////////////////////////////////////////////////////////////////////
    // View Model pour le paramètre AsoEditorParameter
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class AsoEditorViewModel : ObservableRecipient
    {
        //=================================================================
        // Propriétés
        //=================================================================
        public AsoEditorParameter Parameter { get; private set; }
        public List<DefectViewModel> AsoClassVMList { get; private set; }
        public List<DefectViewModel> AsoCategoryVMList { get; private set; }

        private bool _hasVids;
        public bool HasVids
        {
            get { return _hasVids; }
            protected set
            {
                if (_hasVids == value)
                    return;
                _hasVids = value;
                OnPropertyChanged();
            }
        }

        //=================================================================
        // Constructor
        //=================================================================
        public AsoEditorViewModel(AsoEditorParameter parameter)
        {
            Parameter = parameter;
        }

        //=================================================================
        // Conversion Dictionnary -> List
        //=================================================================
        public void Init()
        {
            HasVids = Parameter.Synchronize();

            //-------------------------------------------------------------
            // Pas de VID, sélectionne directement la catégorie pour chaque classe
            //-------------------------------------------------------------
            if (!HasVids)
            {
                AsoClassVMList = new List<DefectViewModel>();
                AsoCategoryVMList = null;
                foreach (AsoDefectClass defectClass in Parameter.DefectClassToCategoryMap.Values)
                {
                    DefectViewModel defectClassVM = new DefectViewModel(Parameter, defectClass);
                    AsoClassVMList.Add(defectClassVM);
                }
                OnPropertyChanged(nameof(AsoClassVMList));
            }
            else
            //-------------------------------------------------------------
            // VID, on contruit une liste VID -> Couleur
            //-------------------------------------------------------------
            {
                AsoClassVMList = null;
                AsoCategoryVMList = new List<DefectViewModel>();
                foreach (AsoDefectVidCategory cat in Parameter.VidToCategoryMap.Values)
                {
                    DefectViewModel defectClassVM = new DefectViewModel(Parameter, cat);
                    AsoCategoryVMList.Add(defectClassVM);
                }
                OnPropertyChanged(nameof(AsoCategoryVMList));
            }
        }

        //=================================================================
        //
        //=================================================================
        public void SelectDefectCategory(DefectViewModel defectClassVM)
        {
            string cat = DefectLabelStore.Categories.Popup();
            if (cat != null)
                defectClassVM.DefectCategory = cat;
        }


    }
}
