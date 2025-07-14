using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows.Forms.Design;

using CommunityToolkit.Mvvm.ComponentModel;


using UnitySC.DataAccess.Dto;
using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;

namespace BasicModules.VidReport
{
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class VidReportViewModel : ObservableRecipient
    {
        //=================================================================
        // Propriétés
        //=================================================================
        public VidReportParameter Parameter { get; private set; }

        // Propriétés bindables
        //.....................
        public ObservableCollection<ReportClassViewModel> ReportClassViewModelList { get; private set; }
        public List<Vid> VidList { get; private set; }

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

            var dbToolproxy = ClassLocator.Default.GetInstance<DbToolServiceProxy>();
            VidList = dbToolproxy.GetAllVid().OrderBy(v => v.Id).ToList();
            OnPropertyChanged(nameof(VidList));

            //-------------------------------------------------------------
            // Vérifie que toutes les classes de défauts sont bien gérées par
            // l'object Parameter et la Collection
            //-------------------------------------------------------------
            foreach (ReportClass reportClass in Parameter.ReportClasses.Values)
            {
                bool found = ReportClassViewModelList.FirstOrDefault(x => x.DefectLabel == reportClass.DefectLabel) != null;
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
