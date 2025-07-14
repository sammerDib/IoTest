using CommunityToolkit.Mvvm.ComponentModel;

namespace BasicModules.DeepControl
{
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class ClassificationDefectClassViewModel : ObservableRecipient
    {
        public DefectClass DefectClass;    // le modèle sous-jacent
        private ClassificationParameter _parameter;

        //=================================================================
        // Constructor
        //=================================================================
        public ClassificationDefectClassViewModel(ClassificationParameter parameter, DefectClass defectClass)
        {
            _parameter = parameter;
            DefectClass = defectClass;
        }

        //=================================================================
        // 
        //=================================================================
        public string Label
        {
            get { return DefectClass.label; }
            set
            {
                if (value == DefectClass.label)
                    return;
                DefectClass.label = value;
                OnPropertyChanged();
                _parameter.ReportChange();
            }
        }

        //=================================================================
        // 
        //=================================================================
        public override string ToString()
        {
            return Label;
        }


    }
}
