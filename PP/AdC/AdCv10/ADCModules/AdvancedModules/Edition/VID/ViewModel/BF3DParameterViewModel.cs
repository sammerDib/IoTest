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
    internal class BF3DParameterViewModel : ObservableRecipient
    {
        public IEnumerable<enDataType_3D> _dataTypes { get; private set; }
        //=================================================================
        // Propriétés
        //=================================================================
        public BF3DParameter Parameter { get; private set; }

        #region Propriétés bindables
        public ObservableCollection<BF3DParameterViewModel_DC> BF3DParameterViewModelList { get; private set; }
        //..........................
        public List<Vid> VidList { get; private set; }

        #endregion
        //=================================================================
        // Constructor
        //=================================================================
        public BF3DParameterViewModel()
        {
        }

        public BF3DParameterViewModel(BF3DParameter parameter)
        {
            Parameter = parameter;
            BF3DParameterViewModelList = new ObservableCollection<BF3DParameterViewModel_DC>();
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
            foreach (DataCollect_3D dtaCategory in Parameter.DataCollect_3DList.Values)
            {
                bool found = BF3DParameterViewModelList.FirstOrDefault(x => x.DataName == dtaCategory.DataName) != null;
                if (!found)
                {
                    BF3DParameterViewModel_DC vm = new BF3DParameterViewModel_DC(dtaCategory, this);
                    BF3DParameterViewModelList.Add(vm);
                }
            }

            //-------------------------------------------------------------
            // Supprime les classes en trop dans le paramètre et la collection
            //-------------------------------------------------------------
            foreach (BF3DParameterViewModel_DC vm in BF3DParameterViewModelList.ToList())
            {
                bool found = Parameter.DataCollect_3DList.ContainsKey(vm.DataName);
                if (!found)
                    BF3DParameterViewModelList.Remove(vm);
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
