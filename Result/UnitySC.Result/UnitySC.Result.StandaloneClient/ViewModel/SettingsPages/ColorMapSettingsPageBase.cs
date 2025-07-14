using System.Linq;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.Shared.Data.ColorMap;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.ViewModel;

using Message = UnitySC.Shared.Tools.Service.Message;

namespace UnitySC.Result.StandaloneClient.ViewModel.SettingsPages
{
    public abstract class ColorMapSettingsPageBase : BaseSettingsPageVM
    {
        #region Fields

        private ColorMap _configuredColorMap;

        protected abstract ColorMapSettings ColorMapSettings { get; set; }

        protected abstract string PathToFile { get; }

        #endregion

        protected ColorMapSettingsPageBase()
        {
            ColorMap = ColorMapHelper.ColorMaps.LastOrDefault();
        }

        #region Properties

        private ColorMap _colorMap;

        public ColorMap ColorMap
        {
            get => _colorMap;
            set => SetProperty(ref _colorMap, value);
        }

        #endregion

        #region Private Methods

        protected virtual void ReadColorMapSettings(string filePath)
        {
            ColorMapSettings = ColorMapSettings.ReadFromXml(filePath);

            var colorMap = ColorMapHelper.ColorMaps.FirstOrDefault(x => x.Name == ColorMapSettings?.ColorMapName);

            _configuredColorMap = colorMap;

            if (colorMap == null)
            {
                var notifierVm = ClassLocator.Default.GetInstance<NotifierVM>();
                notifierVm.AddMessage(new Message(MessageLevel.Warning,
                    "No corresponding color-map in the application, use of the first available color-map"));

                colorMap = ColorMapHelper.ColorMaps.First();
            }

            ColorMap = colorMap;
        }

        #endregion

        #region Public Methods

        public void LoadColorMapSettings()
        {
            ReadColorMapSettings(PathToFile);

            SaveColorMapCommand.NotifyCanExecuteChanged();
        }

        #endregion

        #region Commands

        private AutoRelayCommand _saveColorMapCommand;

        public AutoRelayCommand SaveColorMapCommand => _saveColorMapCommand ?? (_saveColorMapCommand = new AutoRelayCommand(SaveColorMapCommandExecute, SaveColorMapCommandCanExecute));

        private bool SaveColorMapCommandCanExecute()
        {
            return _configuredColorMap == null || _configuredColorMap.Name != ColorMap.Name;
        }

        protected virtual void SaveColorMapCommandExecute()
        {
            ColorMapSettings.ColorMapName = ColorMap.Name;
            _configuredColorMap = ColorMap;

            bool res = ColorMapSettings.SaveToXml(ColorMapSettings, PathToFile);

            var notifierVm = ClassLocator.Default.GetInstance<NotifierVM>();

            notifierVm.AddMessage(!res
                ? new Message(MessageLevel.Error, "An error occurred while saving the color-map at the path : " + PathToFile)
                : new Message(MessageLevel.Information, "Color-map saved successfully."));
        }

        #endregion
    }
}
