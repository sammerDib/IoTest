using System.Collections.Generic;
using System.Linq;

using AcquisitionAdcExchange;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ADC.ViewModel.Operator
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class WaferInfoViewModel : ObservableRecipient
    {
        private List<eWaferInfo> _displayedWaferInfo = new List<eWaferInfo>()
        {
            eWaferInfo.SlotID,
            eWaferInfo.ToolRecipe,
            eWaferInfo.LotID,
            eWaferInfo.ADCRecipeFileName,
            eWaferInfo.WaferID,
        };

        private List<KeyValuePair<eWaferInfo, string>> _infos;
        public List<KeyValuePair<eWaferInfo, string>> Infos
        {
            get => _infos; set { if (_infos != value) { _infos = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasInfos)); } }
        }

        public bool HasInfos
        {
            get => _infos != null ? _infos.Any() : false;
        }

        public void UpdateWafer(WaferInfo waferInfo)
        {
            if (waferInfo != null && waferInfo.dico != null)
                Infos = waferInfo.dico.Where(x => _displayedWaferInfo.Contains(x.Key)).OrderBy(x => x.Key.ToString()).ToList();
            var value = Infos[0].Value;
            var info = Infos[0].Key;
        }

        public void ClearInfos()
        {
            Infos = null;
        }
    }
}
