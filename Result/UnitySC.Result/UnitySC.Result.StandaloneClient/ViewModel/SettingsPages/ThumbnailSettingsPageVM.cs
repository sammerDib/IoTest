using System.Linq;

using UnitySC.Result.StandaloneClient.ViewModel.Common;
using UnitySC.Shared.Data.ColorMap;

namespace UnitySC.Result.StandaloneClient.ViewModel.SettingsPages
{
    public class ThumbnailSettingsPageVM : ColorMapSettingsPageBase
    {
        #region Overrides of BaseSettingsPageViewModel

        public override string PageName => "Thumbnail";

        #endregion

        #region Overrides of ColorMapSettingsPageBase

        protected override string PathToFile => Settings.ThumbnailConfigPath;

        protected override ColorMapSettings ColorMapSettings {
            get => App.Instance.Settings.ThumbnailSettings;
            set => App.Instance.Settings.ThumbnailSettings = value;
        }

        #endregion

        #region Public Methods

        protected override void ReadColorMapSettings(string filePath)
        {
            base.ReadColorMapSettings(filePath);

            ColorMapHelper.ThumbnailColorMap = GetColorMap(ColorMapSettings);
        }

        #endregion

        private ColorMap GetColorMap(ColorMapSettings colorMapSettings)
        {
            return ColorMapHelper.ColorMaps.FirstOrDefault(x => x.Name == colorMapSettings?.ColorMapName);
        }

        #region Commands

        protected override void SaveColorMapCommandExecute()
        {
            base.SaveColorMapCommandExecute();

            ColorMapHelper.ThumbnailColorMap = GetColorMap(ColorMapSettings);

            SaveColorMapCommand.NotifyCanExecuteChanged();
        }

        #endregion
    }
}
