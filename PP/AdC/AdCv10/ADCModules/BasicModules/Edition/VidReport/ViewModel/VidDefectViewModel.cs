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


namespace BasicModules.VidReport
{
    [System.Reflection.Obfuscation(Exclude = true)]
    class VidReportViewModel : ViewModelBase
    {
        IVidService dbVidService = SimpleIoc.Default.GetInstance<IVidService>();

        //=================================================================
        // Propriétés
        //=================================================================
        public VidReportParameter Parameter { get; private set; }

        // Propriétés bindables
        //.....................
        public ObservableCollection<ReportClassViewModel> ReportClassViewModelList { get; private set; }
        public List<Database.Service.Dto.Vid> VidList { get; private set; }

        //=================================================================
        // Constructor
        //=================================================================
        public VidReportViewModel(VidReportParameter parameter)
		{
			Parameter = parameter;
            ReportClassViewModelList = new ObservableCollection<ReportClassViewModel>();
        }

        //=================================================================
        // 
        //=================================================================
        public void Init()
        {
            //-------------------------------------------------------------
            // Récupération des infos
            //-------------------------------------------------------------
            Parameter.Synchronize();

            VidList = dbVidService.GetAll().OrderBy(v => v.Id).ToList();
            RaisePropertyChanged(() => VidList);

            //-------------------------------------------------------------
            // Vérifie que toutes les classes de défauts sont bien gérées par
            // l'object Parameter et la Collection
            //-------------------------------------------------------------
            foreach (ReportClass reportClass in Parameter.ReportClasses.Values)
            {
                bool found = ReportClassViewModelList.FirstOrDefault(x => x.DefectLabel == reportClass.InnerLabel) != null;
                if (!found)
                {
                    ReportClassViewModel vm = new ReportClassViewModel(reportClass, this);
                    ReportClassViewModelList.Add(vm);
                }
            }

            //-------------------------------------------------------------
            // Supprime les classes en trop dans le paramètre et la collection
            //-------------------------------------------------------------
            foreach (ReportClassViewModel vm in ReportClassViewModelList.ToList())
            {
                bool found = Parameter.ReportClasses.ContainsKey(vm.DefectLabel);
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
