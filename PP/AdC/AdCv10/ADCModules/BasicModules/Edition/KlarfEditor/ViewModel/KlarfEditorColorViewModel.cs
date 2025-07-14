using System.Collections.Generic;

using AdcBasicObjects;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BasicModules.KlarfEditor
{
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class KlarfEditorColorViewModel : ObservableRecipient
    {
        //=================================================================
        // Propriétés
        //=================================================================
        public KlarfDefectColorParameters Parameter { get; private set; }
        public List<DefectColorViewModel> DefectColorCategoryList { get; private set; }

        private bool _isVisible;
        public bool IsVisible
        {
            get { return _isVisible; }
            protected set
            {
                if (_isVisible == value)
                    return;
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        //=================================================================
        // Constructor
        //=================================================================
        public KlarfEditorColorViewModel(KlarfDefectColorParameters parameter)
        {
            Parameter = parameter;
            IsVisible = Parameter.IsEnabled;
            parameter.PropertyChanged += Parameter_PropertyChanged;
        }

        private void Parameter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            IsVisible = Parameter.IsEnabled;
        }



        //=================================================================
        // 
        //=================================================================
        public void Init()
        {
            IsVisible = Parameter.Synchronize();
            //Parameter.Synchronize();

            // Conversion Dictionnary -> List
            //...............................
            DefectColorCategoryList = new List<DefectColorViewModel>();
            foreach (KlarfDefectColorCategory cat in Parameter.LabelToCategoryMap.Values)
            {
                DefectColorViewModel defectClassVM = new DefectColorViewModel(Parameter, cat);
                DefectColorCategoryList.Add(defectClassVM);
            }
            OnPropertyChanged(nameof(DefectColorCategoryList));
        }

        //=================================================================
        //
        //=================================================================
        public void SelectDefectColorCategory(DefectColorViewModel defectColorCategory)
        {
            string cat = DefectLabelStore.Categories.Popup();
            if (cat != null)
                defectColorCategory.Color = cat;
        }
    }
}
