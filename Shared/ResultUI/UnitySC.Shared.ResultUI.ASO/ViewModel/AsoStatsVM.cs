using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.Shared.ResultUI.Common.ViewModel.Defect;

namespace UnitySC.Shared.ResultUI.ASO.ViewModel
{
    public class AsoStatsVM : DefectVM
    {
        private AutoRelayCommand _commandExportCSV;

        public override AutoRelayCommand CommandExportCSV => _commandExportCSV ?? (_commandExportCSV = new AutoRelayCommand(ExportCSV));
    }
}
