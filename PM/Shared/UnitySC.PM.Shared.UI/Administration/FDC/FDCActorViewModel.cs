using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using AutoMapper;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.ClientProxy.FDC;
using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.Shared.UI.Administration.FDC
{
    public class FDCActorViewModel : ObservableObject
    {
        private ILogger _logger;
        private IDialogOwnerService _dialogOwnerService;
        private FDCSupervisor _fdcSupervisor;
        private bool _subscribedToChanges = false;
        private bool _isAnyFDCRemoved = false;

        private IMapper _mapper;

        public IMapper AutoMap
        {
            get
            {
                if (_mapper == null)
                {
                    var configuration = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<FDCItemConfig, FDCItemViewModel>().ConstructUsing(x => new FDCItemViewModel(_fdcSupervisor));
                        cfg.CreateMap<FDCItemViewModel, FDCItemConfig>();
                    });
                    _mapper = configuration.CreateMapper();
                }
                return _mapper;
            }
        }

        public ObservableCollection<FDCItemViewModel> FDCItems { get; set; }

        public FDCActorViewModel(FDCSupervisor fdcSupervisor, string actorName, ILogger<FDCViewModel> logger, IDialogOwnerService dialogOwnerService)
        {
            _logger = logger;
            _dialogOwnerService = dialogOwnerService;
            var messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Register<SendFDCMessage>(this, (r, m) => { UpdateFDCValue(m.Data); });

            FDCItems = new ObservableCollection<FDCItemViewModel>();

            _fdcSupervisor = fdcSupervisor;
            // For design
            //var newFDCItem=new FDCItemViewModel() { Name = "fdcName", Value="42", Unit = "µm", SendFrequency = FDCSendFrequency.Hour, ValueType = FDCValueType.TypeInt };

            //FDCItems.Add(newFDCItem);
            ActorName = actorName.ToUpperInvariant();
        }

        public void Init()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                FDCItems.Clear();
            }));

            var resp = _fdcSupervisor.SubscribeToChanges();
            if (resp is null)
            {
                _subscribedToChanges = false;
                return;
            }

            _subscribedToChanges = true;
            var fdcConfigs = _fdcSupervisor.GetFDCsConfig();
            if (fdcConfigs is null)
                return;
            foreach (var fdcConfig in fdcConfigs.Result)
            {
                var newFDCItemVM = AutoMap.Map<FDCItemViewModel>(fdcConfig);
                newFDCItemVM.FDCsActorViewModel = this;

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    FDCItems.Add(newFDCItemVM);
                }));
            }

            _=UpdateFDCValuesAsync();
        }

        public bool CanClose()
        {
            if (IsAnyFDCItemModified() || IsAnyFDCItemEditing())
            {
                var res = ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox($"The modifications have not been saved. Are you sure you want to leave ?", "Modifications not saved", MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.No);
                if (res == MessageBoxResult.No)
                {
                    return false;
                }
            }
            if (_subscribedToChanges)
                Task.Run(() => _fdcSupervisor.UnSubscribeToChanges());
            return true;
        }

        private string _actorName;

        public string ActorName
        {
            get => _actorName; set { if (_actorName != value) { _actorName = value; OnPropertyChanged(); } }
        }

        #region Commands

        private AutoRelayCommand<string> _deleteFDCCommand;

        public AutoRelayCommand<string> DeleteFDCCommand
        {
            get
            {
                return _deleteFDCCommand ?? (_deleteFDCCommand = new AutoRelayCommand<string>(
                    (fdcName) =>
                    {
                        var fdcItemToDelete = FDCItems.Find(f => f.Name == fdcName);
                        if (fdcItemToDelete != null)
                        {
                            FDCItems.Remove(fdcItemToDelete);
                            _isAnyFDCRemoved = true;
                        }
                    },
                    (fdcName) => { return true; }
                ));
            }
        }

        private AutoRelayCommand _addFDCCommand;

        public AutoRelayCommand AddFDCCommand
        {
            get
            {
                return _addFDCCommand ?? (_addFDCCommand = new AutoRelayCommand(
                    () =>
                    {
                        var newFDCItemVM = new FDCItemViewModel(_fdcSupervisor) { Name = "NewFDCName", IsEditing = true, FDCsActorViewModel = this };
                        FDCItems.Add(newFDCItemVM);
                    },
                    () => { return true; }
                ));
            }
        }

        private AutoRelayCommand _saveFDCsConfigCommand;

        public AutoRelayCommand SaveFDCsConfigCommand
        {
            get
            {
                return _saveFDCsConfigCommand ?? (_saveFDCsConfigCommand = new AutoRelayCommand(
                    () =>
                    {
                        // Check that all the FDC Names are unique
                        var duplicatedNames = FDCItems.GroupBy(fdc => fdc.Name)
                          .Where(fdcName => fdcName.Count() > 1)
                          .Select(fdc => fdc.Key)
                          .ToList();

                        if (duplicatedNames.Count > 0)
                        {
                            var res = ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox($"The FDC Names must be unique. The name {duplicatedNames.First()} is duplicated", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        var newFDCsConfig = new List<FDCItemConfig>();

                        foreach (var fdcItem in FDCItems)
                        {
                            if (fdcItem.IsModified)
                            {
                                _fdcSupervisor.SetInitialCountdownValue(fdcItem.Name, fdcItem.InitValue);
                            }
                            var newFDCItemConfig = AutoMap.Map<FDCItemConfig>(fdcItem);
                            newFDCsConfig.Add(newFDCItemConfig);
                            fdcItem.IsModified = false;
                        }

                        _isAnyFDCRemoved = false;
                        _fdcSupervisor.SetFDCsConfig(newFDCsConfig);

                        _ = UpdateFDCValuesAsync();

                    },
                    () => { return (IsAnyFDCItemModified() || _isAnyFDCRemoved) && !IsAnyFDCItemEditing(); }
                ));
            }
        }

        private AutoRelayCommand _refreshValuesCommand;

        public AutoRelayCommand RefreshValuesCommand
        {
            get
            {
                return _refreshValuesCommand ?? (_refreshValuesCommand = new AutoRelayCommand(
                    () =>
                    {
                        _ = UpdateFDCValuesAsync();
                    },
                    () => { return true; }
                ));
            }
        }

        private bool IsAnyFDCItemModified()
        {
            return FDCItems.Any(fdc => fdc.IsModified);
        }

        private bool IsAnyFDCItemEditing()
        {
            return FDCItems.Any(fdc => fdc.IsEditing);
        }

        #endregion Commands

        private async Task UpdateFDCValuesAsync()
        {
            await Task.Run(() =>
            {
                foreach (var fdcItem in FDCItems)
                {
                    var newFDCData = _fdcSupervisor.GetFDC(fdcItem.Name);
                    if (newFDCData?.Result != null)
                    {
                        UpdateFDCItemViewModel(newFDCData.Result, fdcItem);
                    }
                }
            });
            
        }
        private string FDCValueToString(object value)
        {
            switch (value)
            {
                case double valueDouble:
                    if (double.IsNaN(valueDouble))
                        return String.Empty;
                    else
                        return valueDouble.ToString("F3");

                case float valueFloat:
                    return valueFloat.ToString("F3");

                default:
                    return value.ToString();
            }
        }

        private void UpdateFDCValue(FDCData fdcData)
        {
            var fdcItemToUpdate = FDCItems.FirstOrDefault(fdc => fdc.Name == fdcData.Name);
            if (fdcItemToUpdate != null)
            {
                UpdateFDCItemViewModel(fdcData, fdcItemToUpdate);
            }
        }

        private void UpdateFDCItemViewModel(FDCData fdcData, FDCItemViewModel fdcItemToUpdate)
        {
            fdcItemToUpdate.Value = FDCValueToString(fdcData.ValueFDC.Value);
            fdcItemToUpdate.SendDate = fdcData.Date;
        }

        public void UpdateFDCValue(string fdcName)
        {
            var newFDCData = _fdcSupervisor.GetFDC(fdcName).Result;
            UpdateFDCValue(newFDCData);
        }
    }
}
