using UnitySC.Shared.ResultUI.Common.ViewModel;
using UnitySC.Shared.UI.ViewModel.Navigation;

namespace UnitySC.Result.StandaloneClient.ViewModel
{
    public class ResultVM : PageNavigationVM
    {
        private ResultWaferVM _curResultWaferDetailVM;

        public ResultWaferVM CurrentResultWaferVM
        {
            get => _curResultWaferDetailVM;
            set => SetProperty(ref _curResultWaferDetailVM, value);
        }

        public override string PageName => "Wafer detail";
    }
}
