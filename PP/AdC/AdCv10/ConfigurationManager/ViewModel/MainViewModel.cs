using System;
using System.Collections.Generic;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using ConfigurationManager.Configuration;
using ConfigurationManager.ViewModel.Setting;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace ConfigurationManager.ViewModel
{
    public class MainViewModel : ObservableRecipient
    {
        private GlobalConfiguration _config;
        public List<SettingBaseViewModel> Settings { get; private set; }

        public void Init()
        {
            _config = new GlobalConfiguration();
            try
            {
                _config.LoadValues();
                Settings = new List<SettingBaseViewModel>();

                foreach (Configuration.Setting setting in _config.Settings)
                {
                    SettingBaseViewModel vm = null;
                    switch (setting.ConfigurationType)
                    {
                        case ConfigurationType.Bool:
                            vm = new BoolSettingViewModel(setting);
                            break;
                        case ConfigurationType.Folder:
                            vm = new FolderSettingViewModel(setting);
                            break;
                        case ConfigurationType.String:
                            vm = new StringSettingViewModel(setting);
                            break;
                        case ConfigurationType.File:
                            vm = new FileSettingViewModel(setting);
                            break;
                        case ConfigurationType.SQLConnectionString:
                            vm = new SqlConnectionSettingViewModel(setting);
                            break;
                        case ConfigurationType.ResultDb:
                            vm = new ResultDbSettingViewModel(setting);
                            break;
                        case ConfigurationType.WcfAddress:
                            vm = new WcfAddressSettingViewModel(setting);
                            break;
                        case ConfigurationType.BaseAddress:
                            vm = new BaseAddressSettingViewModel(setting);
                            break;
                        case ConfigurationType.ProductionMode:
                            vm = new EnumSettingViewModel<ProductionMode>(setting);
                            break;
                        case ConfigurationType.LogLevel:
                            vm = new EnumSettingViewModel<LogLevel>(setting);
                            break;
                        case ConfigurationType.StartupMode:
                            vm = new EnumSettingViewModel<StartupMode>(setting);
                            break;
                        case ConfigurationType.Int:
                            vm = new IntSettingViewModel(setting);
                            break;
                        default:
                            throw new InvalidOperationException("Unknow ConfigurationType");
                    }

                    Settings.Add(vm);
                }

                Settings.ForEach(x => x.Validate());
                OnPropertyChanged(nameof(Settings));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Init Error: " + ex.Message);
            }
        }

        private void Save()
        {
            try
            {
                _config.SaveValues();
                System.Windows.MessageBox.Show("Configuration files has been saved.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Save Error: " + ex.Message);
            }
        }

        private void Refresh()
        {
            Init();
        }

        private AutoRelayCommand _saveCommand;
        public AutoRelayCommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new AutoRelayCommand(
              () =>
              {
                  Save();
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _refreshCommand;
        public AutoRelayCommand RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new AutoRelayCommand(
              () =>
              {
                  Refresh();
              },
              () => { return true; }));
            }
        }
    }
}
