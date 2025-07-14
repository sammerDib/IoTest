using System.Collections.Generic;
using System.Linq;

using AdvancedModules.Edition.VID.HazeVID;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.DataAccess.Dto;
using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;

namespace AdvancedModules.Edition.VID.ViewModel
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class HazeParameterViewModel : ObservableRecipient
    {
        //=================================================================
        // Propriétés
        //=================================================================
        public HazeParameter Parameter { get; private set; }

        #region Propriétés bindables
        //..........................
        public List<Vid> VidList { get; private set; }

        public string Label => Parameter.Label;

        public int VidNumber
        {
            get { return Parameter.VidNumber; }
            set
            {
                if (value == Parameter.VidNumber)
                    return;
                Parameter.VidNumber = value;
                OnPropertyChanged();
            }
        }

        public string VidLabel
        {
            get { return Parameter.VidLabel; }
            set
            {
                if (value == Parameter.VidLabel)
                    return;
                Parameter.VidLabel = value;
                OnPropertyChanged();
            }
        }

        private Vid _vidObject;
        public Vid VidObject
        {
            get { return _vidObject; }
            set
            {
                if (value == null || value == _vidObject)
                    return;
                VidNumber = value.Id;
                VidLabel = value.Label;
                OnPropertyChanged();
                Parameter.ReportChange();
            }
        }
        #endregion

        //=================================================================
        // Constructor
        //=================================================================
        public HazeParameterViewModel() { }

        public HazeParameterViewModel(HazeParameter parameter)
        {
            Parameter = parameter;
        }

        //=================================================================
        // 
        //=================================================================
        public void Init()
        {
            var dbToolServiceForVid = ClassLocator.Default.GetInstance<DbToolServiceProxy>();
            VidList = dbToolServiceForVid.GetAllVid().OrderBy(v => v.Id).ToList();
            OnPropertyChanged(nameof(VidList));
            _vidObject = VidList.FirstOrDefault(v => v.Id == VidNumber);
            OnPropertyChanged(nameof(VidObject));
        }

    }
}
