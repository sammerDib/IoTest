using System;
using System.Configuration;
using System.Linq;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Data.ColorMap;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.Result.CommonUI.ViewModel.Search.SettingsPages
{
    public class ThumbnailSettingsPageVM : BaseSettingsPageViewModel
    {
        #region Fields

        private const string ThumbnailSettingsKeyName = "SelectedThumbnailColorMap";

        private static string ColorMapAppSettings => ConfigurationManager.AppSettings.Get(ThumbnailSettingsKeyName);

        #endregion

        #region Constructor

        public ThumbnailSettingsPageVM(DuplexServiceInvoker<IResultService> resultService) : base(resultService)
        {

        }

        #endregion

        #region Overrides of BaseSettingsPageViewModel

        public override string PageName => "Thumbnail";

        #endregion

        #region Properties

        private ColorMap _configuredColorMap;

        private ColorMap _colorMap;

        public ColorMap ColorMap
        {
            get => _colorMap;
            set => SetProperty(ref _colorMap, value);
        }

        #endregion

        #region Public Methods

        public void LoadColorMapSettings()
        {
            var colorMap = GetColorMap();

            _configuredColorMap = colorMap;
            ColorMap = colorMap;
            ColorMapHelper.ThumbnailColorMap = colorMap;

            SaveColorMapCommand.NotifyCanExecuteChanged();
        }

        #endregion

        #region Private Methods

        private void SetColorMapSettings(ColorMap colorMap)
        {
            try
            {
                SaveColorMapInAppSettings(colorMap.Name);

                _configuredColorMap = colorMap;
                ColorMapHelper.ThumbnailColorMap = colorMap;

                SaveColorMapCommand.NotifyCanExecuteChanged();
            }
            catch (Exception ex)
            {
                var notifierVM = ClassLocator.Default.GetInstance<NotifierVM>();
                notifierVM.AddMessage(new Message(MessageLevel.Error, "An error occurred while saving the thumbnail color-map into the 'app.config' file. Error : " + ex.Message));
            }
        }

        private static ColorMap GetColorMap()
        {
            var colorMap = ColorMapHelper.ColorMaps.First();

            try
            {
                colorMap = ColorMapHelper.ColorMaps.First(x => x.Name == ColorMapAppSettings);
            }
            catch (Exception ex)
            {
                var notifierVM = ClassLocator.Default.GetInstance<NotifierVM>();
                notifierVM.AddMessage(new Message(MessageLevel.Error, "An error occurred while selecting the thumbnail color-map. The first available color-map is selected. Error : " + ex.Message));
            }

            return colorMap;
        }

        private static void SaveColorMapInAppSettings(string colorMapName)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;

            settings[ThumbnailSettingsKeyName].Value = colorMapName;

            configFile.Save(ConfigurationSaveMode.Minimal);
        }

        #endregion

        #region Commands

        private AutoRelayCommand _saveColorMapCommand;

        public AutoRelayCommand SaveColorMapCommand => _saveColorMapCommand ?? (_saveColorMapCommand = new AutoRelayCommand(SaveColorMapCommandExecute, SaveColorMapCommandCanExecute));

        private bool SaveColorMapCommandCanExecute()
        {
            return _configuredColorMap == null || _configuredColorMap.Name != ColorMap.Name;
        }

        private void SaveColorMapCommandExecute()
        {
            SetColorMapSettings(ColorMap);
        }

        #endregion

    }
}
