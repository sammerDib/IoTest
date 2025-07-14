using System;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Result.CommonUI.ViewModel.LotWafer;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.Result.CommonUI.ViewModel.Wafers
{
    public class WaferSlotVM : ObservableRecipient
    {
        private IMessenger _messenger;
        private long _resultID;

        public WaferSlotVM()
           : base()
        {
            SlotID = 0;
            SlotIndex = -1;
            IsSlotExist = false;

            _resultID = -1;
        }

        public WaferSlotVM(int slotID, bool isSlotVMNotNull)
           : base()
        {
            SlotID = slotID;
            SlotIndex = SlotID - 1;

            _resultID = -1;

            IsSlotExist = isSlotVMNotNull;

            InitMessenger();
        }

        public WaferSlotVM(int slotID, ResultState resstate, long resultDBid)
            : base()
        {
            SlotID = slotID;
            SlotIndex = SlotID - 1;

            IsSlotExist = true;
            State = resstate;
            _resultID = resultDBid;

            InitMessenger();
        }

        public void Update(bool exist = false, long resultDBid = -1, ResultState resstate = ResultState.NotProcess)
        {
            _resultID = resultDBid;

            IsSlotExist = exist;
            State = resstate;
        }

        private void InitMessenger()
        {
            _messenger = ClassLocator.Default.GetInstance<IMessenger>();
            _messenger.Register<ResultStateNotificationMessage>(this, (r, m) => OnResultStateChanged(m));
        }

        protected override void OnDeactivated()
        {
            _messenger.Unregister<ResultStateNotificationMessage>(this);
            base.OnDeactivated();
        }

        public void Cleanup()
        {
            OnDeactivated();
        }

        private void OnResultStateChanged(ResultStateNotificationMessage msg)
        {
            if (IsSlotExist)
            {
                if (msg.ResultID == _resultID)
                {
                    State = (ResultState)msg.State;
                    OnPropertyChanged(nameof(FillBrush));
                    OnPropertyChanged(nameof(Opacity));
                    OnPropertyChanged(nameof(Dashes));
                }
            }
        }

        public string ID
        {
            get { return (SlotID > 0) ? SlotID.ToString() : "/"; }
        }

        public int SlotID { get; set; }
        public int SlotIndex { get; set; }

        private bool _isSlotNotnull;

        public bool IsSlotExist
        {
            get => _isSlotNotnull;
            set
            {
                if (_isSlotNotnull != value)
                {
                    _isSlotNotnull = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FillBrush));
                    OnPropertyChanged(nameof(Opacity));
                    OnPropertyChanged(nameof(Dashes));
                }
            }
        }

        private ResultState _state;

        public ResultState State
        {
            get => _state; set { if (_state != value) { _state = value; OnPropertyChanged(); } }
        }

        public SolidColorBrush FillBrush
        {
            get
            {
                if (!IsSlotExist)
                    return new SolidColorBrush();

                if (State == ResultState.Error)
                    return (SolidColorBrush)Application.Current.FindResource("WaferErrorColor");

                return (SolidColorBrush)Application.Current.FindResource("WaferBackgroundColor");
            }
        }

        public double Opacity
        {
            get
            {
                if (IsSlotExist)
                {
                    return (State != ResultState.NotProcess) ? 1.0 : 0.8;
                }
                return 0.7;
            }
        }

        public DoubleCollection Dashes
        {
            get
            {
                if (IsSlotExist)
                {
                    return (State != ResultState.NotProcess) ? null : new DoubleCollection { 5, 2.5 };
                }
                return new DoubleCollection { 3, 1.5 };
            }
        }

        public void UpdateWaferSlotVM(LotWaferSlotVM slotvm)
        {
            try
            {
                _resultID = slotvm.Item.Id;
                _isSlotNotnull = true;

                Application.Current?.Dispatcher.Invoke(() =>
                {
                    State = slotvm.State;
                    OnPropertyChanged(nameof(IsSlotExist));
                    OnPropertyChanged(nameof(FillBrush));
                    OnPropertyChanged(nameof(Opacity));
                    OnPropertyChanged(nameof(Dashes));
                });
            }
            catch (Exception ex)
            {
                var notifierVM = ClassLocator.Default.GetInstance<NotifierVM>();
                notifierVM.AddMessage(new Message(MessageLevel.Warning, "Update Wafer Slot VM failure : " + ex.Message));
            }
        }
    }
}
