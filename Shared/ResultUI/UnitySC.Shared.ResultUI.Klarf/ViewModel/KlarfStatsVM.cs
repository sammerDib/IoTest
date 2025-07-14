using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.Shared.ResultUI.Common.ViewModel.Defect;

namespace UnitySC.Shared.ResultUI.Klarf.ViewModel
{
    public class KlarfStatsVM : DefectVM
    {
        public override void UpdateKlarfSettings(object settingsdata)
        {
            var data = (KlarfSettingsData)settingsdata;
            DefectBinDictionary.Clear();
            if (data != null && data.RoughBins != null && data.RoughBins.RoughBinList.Count > 0) // A verifier si c'est utile
                foreach (int roughBin in data.RoughBins.RoughBinList)
                {
                    var defectBin = data.RoughBins.GetDefectBin(roughBin);
                    DefectBinDictionary.Add(defectBin.RoughBin.ToString(), defectBin);
                }
        }

        private AutoRelayCommand _commandExportCSV;

        public override AutoRelayCommand CommandExportCSV => _commandExportCSV ?? (_commandExportCSV = new AutoRelayCommand(ExportCSV));
    }
}
