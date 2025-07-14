using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Tools;

namespace UnitySC.Result.StandaloneClient.ViewModel.SettingsPages
{
    public abstract class BaseSettingsPageVM : ObservableRecipient
    {
        protected readonly IResultDataFactory ResFactory;

        public abstract string PageName { get; }

        protected BaseSettingsPageVM()
        {
            ResFactory = ClassLocator.Default.GetInstance<IResultDataFactory>();
        }
    }
}
