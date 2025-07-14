using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BasicModules.YieldmapEditor
{
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class YieldEditorKillerDefectViewModel : ObservableRecipient
    {
        //=================================================================
        // Propriétés
        //=================================================================
        public YieldEditorKillerDefectParameter Parameter { get; private set; }
        public List<PrmDefectKiller> DefectKillers { get; private set; }

        //=================================================================
        // Constructor
        //=================================================================
        public YieldEditorKillerDefectViewModel(YieldEditorKillerDefectParameter parameter)
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
            DefectKillers = new List<PrmDefectKiller>(Parameter.DefectKillerStatus.Values);
            OnPropertyChanged(nameof(DefectKillers));
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
