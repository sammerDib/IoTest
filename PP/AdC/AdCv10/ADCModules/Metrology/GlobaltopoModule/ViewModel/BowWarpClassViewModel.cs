using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GlobaltopoModule.ViewModel
{
    [System.Reflection.Obfuscation(Exclude = true)]
    class BowWarReportClassViewModel : ViewModelBase
    {
        private GTVidBowWarpPrmClass ReportClass;
        private VidBowWarpReportViewModel ParentVM;

        //=================================================================
        // Constructeur
        //=================================================================
        public BowWarReportClassViewModel(GTVidBowWarpPrmClass reportClass, VidBowWarpReportViewModel parentVM)
        {
            this.ReportClass = reportClass;
            this.ParentVM = parentVM;
        }

        //=================================================================
        // Propriétés Bindables
        //=================================================================
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

        public string MeasureLabel
        {
            get { return ReportClass.Label; }
            set
            {
                if (value == ReportClass.Label)
                    return;
                ReportClass.Label = value;
                RaisePropertyChanged();
            }
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
               // this.VidLabel = value.Label;
                RaisePropertyChanged();
                ParentVM.Parameter.ReportChange();
            }
        }

    }
}
