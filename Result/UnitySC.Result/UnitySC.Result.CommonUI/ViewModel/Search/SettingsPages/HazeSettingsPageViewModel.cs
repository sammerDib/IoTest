using System;
using System.Linq;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Data.ColorMap;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.Result.CommonUI.ViewModel.Search.SettingsPages
{
    public class HazeSettingsPageViewModel : BaseSettingsPageViewModel
    {
        #region Overrides of BaseSettingsPageViewModel

        public override string PageName => "Haze";
        
        #endregion

        #region Fields

        private ColorMap _configuredColorMap;

        #endregion

        public HazeSettingsPageViewModel(DuplexServiceInvoker<IResultService> resultService) : base(resultService)
        {
            ColorMap = ColorMapHelper.ColorMaps.FirstOrDefault();
        }

        #region Properties

        private ColorMap _colorMap;

        public ColorMap ColorMap
        {
            get => _colorMap;
            set => SetProperty(ref _colorMap, value);
        }

        private AutoRelayCommand _saveColorMapCommand;

        public AutoRelayCommand SaveColorMapCommand => _saveColorMapCommand ?? (_saveColorMapCommand = new AutoRelayCommand(SaveColorMapCommandExecute, SaveColorMapCommandCanExecute));

        private bool SaveColorMapCommandCanExecute()
        {
            return _configuredColorMap == null || _configuredColorMap.Name != ColorMap.Name;
        }

        private void SaveColorMapCommandExecute()
        {
            bool connectOk = false;

            try
            {
                // update database
                ResultService.Invoke(x => x.RemoteUpdateHazeColorMap(_colorMap.Name));

                _configuredColorMap = ColorMap;

                connectOk = true;
            }
            catch (Exception)
            {
                var notifierVm = ClassLocator.Default.GetInstance<NotifierVM>();
                notifierVm.AddMessage(new Message(MessageLevel.Error, "Connection lost while saving Haze color map"));
            }

            // update haze display
            if (connectOk)
            {
                ResFactory.GetDisplayFormat(ResultType.ADC_Haze).UpdateInternalDisplaySettingsPrm(_colorMap.Name);
            }

            SaveColorMapCommand.NotifyCanExecuteChanged();
        }

        #endregion

        #region Public Methods

        public void RefreshHazeSettings()
        {
            var notifierVm = ClassLocator.Default.GetInstance<NotifierVM>();

            try
            {
                string ColorMapHazeSettingsName = ResultService.Invoke(x => x.GetHazeSettingsFromTables());
                var colormap = ColorMapHelper.ColorMaps.SingleOrDefault(map => map.Name.Equals(ColorMapHazeSettingsName));

                _configuredColorMap = colormap;
                
                if (colormap == null)
                {
                    notifierVm.AddMessage(new Message(MessageLevel.Warning, "No corresponding color-map in the application, use of the first available color-map"));
                    colormap = ColorMapHelper.ColorMaps.First();
                }

                // Updates the property without passing the set
                SetProperty(ref _colorMap, colormap, nameof(ColorMap));

                ResFactory.GetDisplayFormat(ResultType.ADC_Haze).UpdateInternalDisplaySettingsPrm(_colorMap.Name);
            }
            catch (Exception)
            {
                notifierVm.AddMessage(new Message(MessageLevel.Error, "Connection lost while Refresh database Haze Settings"));
            }

            SaveColorMapCommand.NotifyCanExecuteChanged();
        }

        #endregion
    }
}
