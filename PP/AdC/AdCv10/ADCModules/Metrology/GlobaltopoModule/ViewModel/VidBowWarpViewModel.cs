using AdcBasicObjects;
using AdcTools;
using Database.Service;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace GlobaltopoModule.ViewModel
{
    [System.Reflection.Obfuscation(Exclude = true)]
    class VidBowWarpReportViewModel : ViewModelBase
    {
        IVidService dbVidService = SimpleIoc.Default.GetInstance<IVidService>();

        //=================================================================
        // Propriétés
        //=================================================================
        public GTVidBowWarpParameter Parameter { get; private set; }

        // Propriétés bindables
        //.....................
        public ObservableCollection<BowWarReportClassViewModel> ReportClassViewModelList { get; private set; }
        public List<Database.Service.Dto.Vid> VidList { get; private set; }

        //=================================================================
        // Constructor
        //=================================================================
        public VidBowWarpReportViewModel(GTVidBowWarpParameter parameter)
		{
			Parameter = parameter;
            ReportClassViewModelList = new ObservableCollection<BowWarReportClassViewModel>();
        }

        //=================================================================
        // 
        //=================================================================
        public void Init()
        {
            //-------------------------------------------------------------
            // Récupération des infos
            //-------------------------------------------------------------
            //Parameter.Synchronize();

            VidList = dbVidService.GetAll().OrderBy(v => v.Id).ToList();
            RaisePropertyChanged(() => VidList);

            //-------------------------------------------------------------
            // Vérifie que toutes les classes de défauts sont bien gérées par
            // l'object Parameter et la Collection
            //-------------------------------------------------------------
            foreach (GTVidBowWarpPrmClass reportClass in Parameter.BowWarpReportClasses.Values)
            {
                bool found = ReportClassViewModelList.FirstOrDefault(x => x.MeasureLabel == reportClass.Label) != null;
                if (!found)
                {
                    BowWarReportClassViewModel vm = new BowWarReportClassViewModel(reportClass, this);
                    ReportClassViewModelList.Add(vm);
                }
            }

            //-------------------------------------------------------------
            // Supprime les classes en trop dans le paramètre et la collection
            //-------------------------------------------------------------
            foreach (BowWarReportClassViewModel vm in ReportClassViewModelList.ToList())
            {
                bool found = Parameter.BowWarpReportClasses.ContainsKey(vm.MeasureLabel);
                if (!found)
                    ReportClassViewModelList.Remove(vm);
            }
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
