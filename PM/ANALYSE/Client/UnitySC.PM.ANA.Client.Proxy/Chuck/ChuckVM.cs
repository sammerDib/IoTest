using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.Shared.UI.AutoRelayCommandExt;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.ANA.Client.Proxy.Axes.Models;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ViewModel;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Client.Proxy.Chuck
{
    public class ChuckVM : ViewModelBaseExt
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IChuckService _chuckSupervisor;
        private ChuckBaseConfig _chuckConfiguration;
        private ChuckStatus _status;

        private readonly IDialogOwnerService _dialogService;

        private AutoRelayCommand _getChuckConfiguration;
        private AutoRelayCommand _changeClampStatus;
        private GlobalStatusSupervisor _globalStatusSupervisor;

        #endregion Fields

        #region Constructors

        public ChuckVM(IChuckService chuckSupervisor, IDialogOwnerService dialogService, ILogger logger)
        {
            _logger = logger;
            _chuckSupervisor = chuckSupervisor;
            _dialogService = dialogService;

            _globalStatusSupervisor = ClassLocator.Default.GetInstance<SharedSupervisors>().GetGlobalStatusSupervisor(ActorType.ANALYSE);
            _status = new ChuckStatus();
            var toolService = new ServiceInvoker<IToolService>("ToolService", ClassLocator.Default.GetInstance<SerilogLogger<IToolService>>(), ClassLocator.Default.GetInstance<IMessenger>(), ClientConfiguration.GetDataAccessAddress());
            WaferCategories = toolService.Invoke(x => x.GetWaferCategories());
            SelectedWaferCategory = WaferCategories.OrderByDescending(x => x.DimentionalCharacteristic.Diameter).FirstOrDefault();
        }

        #endregion Constructors

        #region Private methods

        public void AddMessagesOrExceptionToErrorsList<T>(Response<T> response, Exception e)
        {
            // If there is at least one message
            if ((response != null) && response.Messages.Any())
            {
                foreach (var message in response.Messages)
                {
                    if (!string.IsNullOrEmpty(message.UserContent))
                        _globalStatusSupervisor.SendUIMessage(message);
                }
            }
            else
            {
                if (e != null)
                    _globalStatusSupervisor.SendUIMessage(new Message(MessageLevel.Error, e.Message));
            }
        }

        public void Init()
        {
            try
            {
                var resultState = _chuckSupervisor.GetCurrentState()?.Result;
                if (resultState != null)
                {
                    UpdateStatus(resultState);
                }

                if (_chuckConfiguration == null)
                {
                    ChuckConfiguration = _chuckSupervisor.GetChuckConfiguration()?.Result;
                    OnPropertyChanged(nameof(AnaChuckConfiguration));
                }
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, $"ChuckVM Init Failed");
            }
        }

        #endregion Private methods

        #region Properties

        public List<WaferCategory> WaferCategories { get; private set; }

        private WaferCategory _selectedWaferCategory;

        public WaferCategory SelectedWaferCategory
        {
            get => _selectedWaferCategory;
            set
            {
                if (_selectedWaferCategory != value)
                {
                    _selectedWaferCategory = value;
                    OnPropertyChanged();
                }
            }
        }

        public void SelectWaferCategory(int waferCategoryId)
        {
            SelectedWaferCategory = WaferCategories.FirstOrDefault(x => x.Id == waferCategoryId);
        }

        public ChuckStatus Status => _status;

        public void UpdateStatus(ChuckState state)
        {
            var size = AnaChuckConfiguration?.SubstrateSlotConfigs[0]?.Diameter;
            if (size != null)
            {
                Console.WriteLine("Is wafer clamped :" + state.WaferClampStates[size]);
                Console.WriteLine("Chuck UpdateStatus");
                _status.IsWaferClamped = state.WaferClampStates[size];
                _status.IsWaferPresent = state.WaferPresences[size] == MaterialPresence.Present;
                Console.WriteLine($"Status IsWaferClamped changed : {_status.IsWaferClamped} ");
            }
            else
                Console.WriteLine("Material diameter from configuration is unknown. Check SubstrateSlotConfig in configuration !");
        }

        public ChuckBaseConfig ChuckConfiguration
        {
            get
            {
                if (_chuckConfiguration == null)
                {
                    _chuckConfiguration = _chuckSupervisor.GetChuckConfiguration()?.Result;
                }
                return _chuckConfiguration;
            }

            set
            {
                if (_chuckConfiguration == value)
                {
                    return;
                }
                _chuckConfiguration = value;
                OnPropertyChanged();
            }
        }

        public ANAChuckConfig AnaChuckConfiguration => (ANAChuckConfig)ChuckConfiguration;

        #endregion Properties

        #region RelayCommands

        public AutoRelayCommand ChangeClampStatus
        {
            get
            {
                return _changeClampStatus ?? (_changeClampStatus = new AutoRelayCommand(
                () =>
                {
                    Response<bool> response = null;
                    try
                    {
                        if (!Status.IsWaferClamped)
                        {
                            response = _chuckSupervisor.ReleaseWafer(SelectedWaferCategory.DimentionalCharacteristic);
                        }
                        else
                        {
                            response = _chuckSupervisor.ClampWafer(SelectedWaferCategory.DimentionalCharacteristic);
                        }
                    }
                    catch (Exception e)
                    {
                        AddMessagesOrExceptionToErrorsList(response, e);
                    }
                },
                () => true));
            }
        }

        public AutoRelayCommand GetChuckConfiguration
        {
            get
            {
                return _getChuckConfiguration ?? (_getChuckConfiguration = new AutoRelayCommand(
              () =>
              {
                  ChuckConfiguration = _chuckSupervisor.GetChuckConfiguration()?.Result;
              },
              () => true));
            }
        }

        #endregion RelayCommands
    }
}
