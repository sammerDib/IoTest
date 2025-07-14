using System.Collections.Generic;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.DataAccess.Dto;


namespace AdvancedModules.Edition.VID.ViewModel
{
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class BF3DParameterViewModel_DC : ObservableRecipient
    {
        private DataCollect_3D DataCollect;
        private BF3DParameterViewModel ParentVM;

        //=================================================================
        // Constructeur
        //=================================================================
        public BF3DParameterViewModel_DC(DataCollect_3D dataCollect, BF3DParameterViewModel parentVM)
        {
            DataCollect = dataCollect;
            ParentVM = parentVM;
        }

        //=================================================================
        // Propriétés Bindables
        //=================================================================        
        public string DataName
        {
            get { return DataCollect.DataName; }
            set
            {
                if (value == DataCollect.DataName)
                    return;
                DataCollect.DataName = value;
                OnPropertyChanged();
            }
        }

        public int VidNumber
        {
            get { return DataCollect.VID; }
            set
            {
                if (value == DataCollect.VID)
                    return;
                DataCollect.VID = value;
                OnPropertyChanged();
            }
        }

        public string VidLabel
        {
            get { return DataCollect.VidLabel; }
            set
            {
                if (value == DataCollect.VidLabel)
                    return;
                DataCollect.VidLabel = value;
                OnPropertyChanged();
            }
        }

        public List<Vid> VidList { get { return ParentVM.VidList; } }

        public Vid VidObject
        {
            get
            {
                return VidList.FirstOrDefault(v => v.Id == VidNumber);
            }
            set
            {
                if (value == null)
                    return;
                VidNumber = value.Id;
                VidLabel = value.Label;
                OnPropertyChanged();
                ParentVM.Parameter.ReportChange();
            }
        }
    }
}
