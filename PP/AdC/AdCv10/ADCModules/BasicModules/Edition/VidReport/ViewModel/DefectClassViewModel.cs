using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BasicModules.VidReport
{
    [System.Reflection.Obfuscation(Exclude = true)]
    class ReportClassViewModel : ViewModelBase
    {
        private ReportClass ReportClass;
        private VidReportViewModel ParentVM;

        //=================================================================
        // Constructeur
        //=================================================================
        public ReportClassViewModel(ReportClass reportClass, VidReportViewModel parentVM)
        {
            this.ReportClass = reportClass;
            this.ParentVM = parentVM;
        }

        //=================================================================
        // Propriétés Bindables
        //=================================================================
        public string DefectLabel
        {
            get { return ReportClass.InnerLabel; }
            set
            {
                if (value == ReportClass.InnerLabel)
                    return;
                ReportClass.InnerLabel = value;
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
            }
        }

        public double[] Bin
        {
            get { return ReportClass.Bin2; }
        }

        public List<Database.Service.Dto.Vid> VidList { get { return ParentVM.VidList; } }

        public Database.Service.Dto.Vid VidObject
        {
            get
            {
                return VidList.FirstOrDefault(v => v.Id == this.VidNumber);
            }
            set
            {
                if (value == null)
                    return;
                this.VidNumber = value.Id;
                this.VidLabel = value.Label;
                RaisePropertyChanged();
                ParentVM.Parameter.ReportChange();
            }
        }

    }
}
