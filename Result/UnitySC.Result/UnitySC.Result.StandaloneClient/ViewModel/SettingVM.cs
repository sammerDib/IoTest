using System.Collections.Generic;

using UnitySC.Result.StandaloneClient.ViewModel.SettingsPages;
using UnitySC.Shared.UI.ViewModel.Navigation;

namespace UnitySC.Result.StandaloneClient.ViewModel
{
    public class SettingVM : PageNavigationVM
    {
        public override string PageName => "Settings";

        public SettingVM()
        {
            KlarfSettingsPage = new KlarfSettingsPageVM();
            HazeSettingsPage = new HazeSettingsPageVM();
            ThumbnailSettingsPage = new ThumbnailSettingsPageVM();

            SettingsPages.Add(KlarfSettingsPage);
            SettingsPages.Add(HazeSettingsPage);
            SettingsPages.Add(ThumbnailSettingsPage);

            SelectedPage = KlarfSettingsPage;

            KlarfSettingsPage.LoadKlarfSettings();
            HazeSettingsPage.LoadColorMapSettings();
            ThumbnailSettingsPage.LoadColorMapSettings();
        }

        #region Properties

        public KlarfSettingsPageVM KlarfSettingsPage { get; }

        public HazeSettingsPageVM HazeSettingsPage { get; }

        public ThumbnailSettingsPageVM ThumbnailSettingsPage { get; }

        public List<BaseSettingsPageVM> SettingsPages { get; } = new List<BaseSettingsPageVM>();

        private BaseSettingsPageVM _selectedPage;

        public BaseSettingsPageVM SelectedPage
        {
            get { return _selectedPage; }
            set { SetProperty(ref _selectedPage, value); }
        }

        #endregion Properties
    }
}
