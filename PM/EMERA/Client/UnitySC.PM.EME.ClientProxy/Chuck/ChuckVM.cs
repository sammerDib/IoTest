using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.EME.Service.Interface.Axes;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.EME.Service.Interface.Chuck;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.Referential;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.EME.Client.Proxy.Chuck
{
    public class ChuckVM : ViewModelBaseExt
    {
        private readonly ILogger _logger;
        private readonly IMessenger _messenger;
        private readonly IEmeraMotionAxesService _motionAxesSupervisor;
        private readonly IEMEChuckService _chuckSupervisor;
        private readonly ICalibrationService _calibrationSupervisor;
        private readonly IReferentialService _referentialSupervisor;
        private readonly GlobalStatusSupervisor _globalStatusSupervisor;

        public List<WaferCategory> WaferCategories { get; set; }
        public ChuckStatus Status { get; }
        public ChuckVM(IEMEChuckService chuckSupervisor, ICalibrationService calibrationSupervisor, IReferentialService referentialSupervisor,IMessenger messenger)
        {
            _messenger = messenger;
            _chuckSupervisor = chuckSupervisor;
            _calibrationSupervisor = calibrationSupervisor;
            _referentialSupervisor = referentialSupervisor;
            Status = new ChuckStatus();            
            Messenger.Register<ChuckLoadingPositionChangedMessage>(this, (r, m) => { UpdateChuckIsInLoadingPosition(m.ChuckIsInLoadingPosition); });
        }
        public ChuckVM(IEMEChuckService chuckSupervisor, ICalibrationService calibrationSupervisor, IEmeraMotionAxesService motionAxesSupervisor, IReferentialService referentialSupervisor, GlobalStatusSupervisor globalStatusSupervisor, ILogger logger, IMessenger messenger)
        {
            _logger = logger;
            _messenger = messenger;
            _globalStatusSupervisor = globalStatusSupervisor;
            _chuckSupervisor = chuckSupervisor;
            _calibrationSupervisor = calibrationSupervisor;
            _motionAxesSupervisor = motionAxesSupervisor;
            _referentialSupervisor = referentialSupervisor;
            Status = new ChuckStatus();
            var toolService = new ServiceInvoker<IToolService>("ToolService", ClassLocator.Default.GetInstance<SerilogLogger<IToolService>>(), ClassLocator.Default.GetInstance<IMessenger>(), ClientConfiguration.GetDataAccessAddress());
            WaferCategories = toolService.Invoke(x => x.GetWaferCategories());

            Messenger.Register<ChuckLoadingPositionChangedMessage>(this, (r, m) => { UpdateChuckIsInLoadingPosition(m.ChuckIsInLoadingPosition); });

            Init();
        }

        public void Init()
        {
            try
            {
                PropertyChanged += ChuckVM_PropertyChanged;
                SelectedWaferCategory = WaferCategories?.FirstOrDefault(x => x.DimentionalCharacteristic.Diameter == 200.Millimeters()) ?? null;
                var state = _chuckSupervisor.GetCurrentState()?.Result;
                if (state != null)
                {
                    UpdateStatus(state);
                }
                _messenger.Register<ChuckState>(this, (_, p) => UpdateStatus(p));
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, $"ChuckVM Init Failed");
            }
        }

        private void ChuckVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedWaferCategory))
            {
                var state = _chuckSupervisor.GetCurrentState()?.Result;
                if (state != null)
                {
                    UpdateStatus(state);
                }
            }
        }

        private WaferCategory _selectedWaferCategory;
        public WaferCategory SelectedWaferCategory
        {
            get => _selectedWaferCategory;
            set
            {
                SetProperty(ref _selectedWaferCategory, value);
                SetReferentialForSelectedWafer();
            }
        }

        private void UpdateStatus(ChuckState state)
        {
            var currentWaferSize = SelectedWaferCategory.DimentionalCharacteristic.Diameter;
            Status.IsWaferClamped = state.WaferClampStates[currentWaferSize];
            Status.IsWaferPresent = state.WaferPresences[currentWaferSize] == MaterialPresence.Present;
        }

        public void SelectWaferCategory(int waferCategoryId)
        {
            SelectedWaferCategory = WaferCategories.FirstOrDefault(x => x.Id == waferCategoryId);
        }

        public void SetReferentialForSelectedWafer()
        {
            var waferReferentialSettings = _calibrationSupervisor
                .GetWaferReferentialSettings(SelectedWaferCategory?.DimentionalCharacteristic?.Diameter)?.Result;
            if (waferReferentialSettings != null)
            {
                _referentialSupervisor.SetSettings(waferReferentialSettings);
            }
            else
            {
                string msg =
                    $"No WaferReferentialSettings associated with wafer diameter : {_selectedWaferCategory?.DimentionalCharacteristic?.Diameter}";
                _logger?.Error(msg);
                _messenger.Send(new Message(MessageLevel.Error, msg));
            }
        }

        private void UpdateChuckIsInLoadingPosition(bool loadingPosition)
        {
            InterlockEfemChuckIsInLoadingPosition = loadingPosition.ToString();
        }

        private string _interlockEfemChuckIsInLoadingPosition;
        public string InterlockEfemChuckIsInLoadingPosition
        {
            get => _interlockEfemChuckIsInLoadingPosition;
            set
            {
                if (_interlockEfemChuckIsInLoadingPosition != value)
                {
                    _interlockEfemChuckIsInLoadingPosition = value;
                    OnPropertyChanged();
                }
            }
        }

        private AutoRelayCommand _changeClampStatus;

        public AutoRelayCommand ChangeClampStatus => _changeClampStatus ?? (_changeClampStatus = new AutoRelayCommand(PerformChangeClampStatus));

        private void PerformChangeClampStatus()
        {
            Response<bool> response = null;
            try
            {
                response = Status.IsWaferClamped
                    ? _chuckSupervisor.ReleaseWafer(SelectedWaferCategory.DimentionalCharacteristic)
                    : _chuckSupervisor.ClampWafer(SelectedWaferCategory.DimentionalCharacteristic);
            }
            catch (Exception e)
            {
                AddMessagesOrExceptionToErrorsList(response, e);
            }
        }

        private void AddMessagesOrExceptionToErrorsList<T>(Response<T> response, Exception e)
        {
            // If there is at least one message
            if ((response != null) && response.Messages.Any())
            {
                foreach (var message in response.Messages.Where(message => !string.IsNullOrEmpty(message.UserContent)))
                {
                    _globalStatusSupervisor.SendUIMessage(message);
                }
            }
            else
            {
                if (e != null)
                    _globalStatusSupervisor.SendUIMessage(new Message(MessageLevel.Error, e.Message));
            }
        }
    }
}
