using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.DataAccess.Dto;
using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;

namespace AdvancedModules.Edition.VID.ViewModel
{
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class BF2DParameterViewModel : ObservableRecipient
    {
        public IEnumerable<enDataType> _dataTypes { get; private set; }
        //=================================================================
        // Propriétés
        //=================================================================
        public BF2DParameter Parameter { get; private set; }

        #region Propriétés bindables
        public ObservableCollection<BF2DParameterViewModel_DC> BF2DParameterViewModelList { get; private set; }
        //..........................
        public List<Vid> VidList { get; private set; }

        #endregion
        //=================================================================
        // Constructor
        //=================================================================
        public BF2DParameterViewModel()
        {
        }

        public BF2DParameterViewModel(BF2DParameter parameter)
        {
            Parameter = parameter;
            BF2DParameterViewModelList = new ObservableCollection<BF2DParameterViewModel_DC>();
        }

        //=================================================================
        // 
        //=================================================================
        public void Init()
        {
            //-------------------------------------------------------------
            // Récupération des infos
            //-------------------------------------------------------------
            var dbToolServiceForVid = ClassLocator.Default.GetInstance<DbToolServiceProxy>();
            VidList = dbToolServiceForVid.GetAllVid().OrderBy(v => v.Id).ToList();
            OnPropertyChanged(nameof(VidList));

            //-------------------------------------------------------------
            // Vérifie que toutes les classes de défauts sont bien gérées par
            // l'object Parameter et la Collection
            //-------------------------------------------------------------
            foreach (DataCollect dtaCategory in Parameter.dataCollectList.Values)
            {
                bool found = BF2DParameterViewModelList.FirstOrDefault(x => x.DataName == dtaCategory.DataName) != null;
                if (!found)
                {
                    BF2DParameterViewModel_DC vm = new BF2DParameterViewModel_DC(dtaCategory, this);
                    BF2DParameterViewModelList.Add(vm);
                }
            }

            //-------------------------------------------------------------
            // Supprime les classes en trop dans le paramètre et la collection
            //-------------------------------------------------------------
            foreach (BF2DParameterViewModel_DC vm in BF2DParameterViewModelList.ToList())
            {
                bool found = Parameter.dataCollectList.ContainsKey(vm.DataName);
                if (!found)
                    BF2DParameterViewModelList.Remove(vm);
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
