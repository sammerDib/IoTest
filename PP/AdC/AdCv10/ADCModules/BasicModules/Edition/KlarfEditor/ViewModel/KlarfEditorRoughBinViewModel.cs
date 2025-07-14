using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BasicModules.KlarfEditor
{
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class KlarfEditorRoughBinViewModel : ObservableRecipient
    {
        //=================================================================
        // Propriétés
        //=================================================================
        public KlarfEditorRoughBinParameter Parameter { get; private set; }
        public List<DefectRoughBin> RoughBins { get; private set; }



        //=================================================================
        // Constructor
        //=================================================================
        public KlarfEditorRoughBinViewModel(KlarfEditorRoughBinParameter parameter)
        {
            Parameter = parameter;
        }

        //=================================================================
        // 
        //=================================================================
        public void Init()
        {
            Parameter.Synchronize();

            // Conversion Dictionnary -> List
            //...............................
            RoughBins = new List<DefectRoughBin>(Parameter.RoughBins.Values);
            OnPropertyChanged(nameof(RoughBins));
        }

        //=================================================================
        //
        //=================================================================
        public void ReportChange()
        {
            Parameter.ReportChange();
        }
    }
}
