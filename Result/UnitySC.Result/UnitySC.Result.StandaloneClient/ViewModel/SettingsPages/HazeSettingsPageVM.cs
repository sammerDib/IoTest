using UnitySC.Result.StandaloneClient.ViewModel.Common;
using UnitySC.Shared.Data.ColorMap;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.Result.StandaloneClient.ViewModel.SettingsPages
{
    public class HazeSettingsPageVM : ColorMapSettingsPageBase
    {
        #region Overrides of BaseSettingsPageViewModel

        public override string PageName => "Haze";

        #endregion

        #region Overrides of ColorMapSettingsPageBase

        protected override string PathToFile => Settings.HazeConfigPath;

        protected override ColorMapSettings ColorMapSettings {
            get => App.Instance.Settings.HazeSettings;
            set => App.Instance.Settings.HazeSettings = value;
        }

        #endregion

        #region Public Methods

        protected override void ReadColorMapSettings(string filePath)
        {
            base.ReadColorMapSettings(filePath);

            ResFactory.GetDisplayFormat(ResultType.ADC_Haze).UpdateInternalDisplaySettingsPrm(ColorMap.Name);
        }

        #endregion

        #region Commands

        protected override void SaveColorMapCommandExecute()
        {
            base.SaveColorMapCommandExecute();

            ResFactory.GetDisplayFormat(ResultType.ADC_Haze).UpdateInternalDisplaySettingsPrm(ColorMap.Name);

            SaveColorMapCommand.NotifyCanExecuteChanged();
        }

        #endregion
    }
}
