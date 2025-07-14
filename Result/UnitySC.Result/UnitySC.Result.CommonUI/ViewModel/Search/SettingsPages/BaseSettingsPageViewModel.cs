using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Result.CommonUI.ViewModel.Search.SettingsPages
{
    public abstract class BaseSettingsPageViewModel : ObservableRecipient
    {
        protected readonly IResultDataFactory ResFactory;
        protected readonly DuplexServiceInvoker<IResultService> ResultService;

        public abstract string PageName { get; }

        public BaseSettingsPageViewModel(DuplexServiceInvoker<IResultService> resultService)
        {
            ResFactory = ClassLocator.Default.GetInstance<IResultDataFactory>();
            ResultService = resultService;
        }
    }
}
