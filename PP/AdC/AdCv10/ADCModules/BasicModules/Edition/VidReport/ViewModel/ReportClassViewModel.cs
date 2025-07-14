using System.Collections.Generic;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.DataAccess.Dto;

namespace BasicModules.VidReport
{
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class ReportClassViewModel : ObservableRecipient
    {
        private ReportClass ReportClass;
        private VidReportViewModel ParentVM;

        //=================================================================
        // Constructeur
        //=================================================================
        public ReportClassViewModel(ReportClass reportClass, VidReportViewModel parentVM)
        {
            ReportClass = reportClass;
            ParentVM = parentVM;
        }

        //=================================================================
        // Propriétés Bindables
        //=================================================================        
        public string DefectLabel
        {
            get { return ReportClass.DefectLabel; }
            set
            {
                if (value == ReportClass.DefectLabel)
                    return;
                ReportClass.DefectLabel = value;
                OnPropertyChanged();
            }
        }

        public int VidNumber
        {
            get { return ReportClass.VID; }
            set
            {
                if (value == ReportClass.VID)
                    return;
                ReportClass.VID = value;
                OnPropertyChanged();
            }
        }

        public string VidLabel
        {
            get { return ReportClass.VidLabel; }
            set
            {
                if (value == ReportClass.VidLabel)
                    return;
                ReportClass.VidLabel = value;
                OnPropertyChanged();
            }
        }

        public double[] Bin
        {
            get { return ReportClass.Bin2; }
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
