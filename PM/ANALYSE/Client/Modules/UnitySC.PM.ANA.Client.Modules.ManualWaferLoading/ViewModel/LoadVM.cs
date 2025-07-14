using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.PM.ANA.Client.Proxy.Axes;
using UnitySC.PM.ANA.Client.Proxy.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Client.Modules.ManualWaferLoading.ViewModel
{
    public class LoadVM : ObservableObject
    {
        #region Enum
        public enum Destination
        {
            GoToManualLoad,
            MoveTop,
            MoveBottom,
            MoveLeft,
            MoveRight,
            MoveCenter
        }
        #endregion // Enum

        #region Fields
        private ChuckSupervisor _chuckSupervisor;
        private AxesSupervisor _axesSupervisor;
        private double _coordinate;
        #endregion // Fields

        #region Constructor
        public LoadVM(ManualWaferLoadingVM manualWaferLoading)
        {
            _chuckSupervisor = ClassLocator.Default.GetInstance<ChuckSupervisor>();
            _axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();
            IsControlEnabled = false;
        }
        #endregion // Constructor

        #region Properties

        private bool _isControlEnabled;    
        public bool IsControlEnabled
        {
            get
            {
                return _isControlEnabled;
            }
            set
            {
                if (_isControlEnabled != value)
                {
                    _isControlEnabled = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion // Properties

        #region Methods
        protected void MoveToDestination(Destination destination)
        {
            Length waferDiameter = _chuckSupervisor.ChuckVM.SelectedWaferCategory.DimentionalCharacteristic.Diameter;
            double coordinate = waferDiameter.Millimeters * 0.5;

            var chuckCenterOffset = (_axesSupervisor.GetChuckCenterPosition(waferDiameter)?.Result
                                  ?? new XYPosition(new StageReferential(), 0.0, 0.0))
                                  as XYPosition;

            double XOffset_um = chuckCenterOffset.X * 1000.0;
            double YOffset_um = chuckCenterOffset.Y * 1000.0;

            switch (destination)
            {
                case (Destination.GoToManualLoad):
                    _axesSupervisor.GoToManualLoad(waferDiameter, AxisSpeed.Fast);
                    break;

                case (Destination.MoveTop):
                    _coordinate = coordinate;
                    _axesSupervisor.GotoPosition(new XYPosition(new StageReferential(), XOffset_um, YOffset_um + _coordinate), AxisSpeed.Fast);
                    break;

                case (Destination.MoveBottom):
                    _coordinate = -coordinate;
                    _axesSupervisor.GotoPosition(new XYPosition(new StageReferential(), XOffset_um, YOffset_um + _coordinate), AxisSpeed.Fast);
                    break;

                case (Destination.MoveLeft):
                    _coordinate = -coordinate;
                    _axesSupervisor.GotoPosition(new XYPosition(new StageReferential(), XOffset_um + _coordinate, YOffset_um), AxisSpeed.Fast);
                    break;

                case (Destination.MoveRight):
                    _coordinate = +coordinate;
                    _axesSupervisor.GotoPosition(new XYPosition(new StageReferential(), XOffset_um +  _coordinate, YOffset_um), AxisSpeed.Fast);
                    break;

                case (Destination.MoveCenter):
                    _axesSupervisor.GotoPosition(new XYPosition(new StageReferential(), XOffset_um , YOffset_um), AxisSpeed.Fast);
                    break;
            }
        }

        protected async Task ClampMoveThenRelease(Destination moveDestination)
        {
            await Task.Run(() =>
            {
                _chuckSupervisor.ClampWafer(_chuckSupervisor.ChuckVM.SelectedWaferCategory.DimentionalCharacteristic);
                MoveToDestination(moveDestination);
                _axesSupervisor.WaitMotionEnd(30000);
                _chuckSupervisor.ReleaseWafer(_chuckSupervisor.ChuckVM.SelectedWaferCategory.DimentionalCharacteristic);
            });
        }

        private AutoRelayCommand _goToManualLoad;

        public AutoRelayCommand GoToManualLoad
        {
            get
            {
                return _goToManualLoad ??
                    (_goToManualLoad = new AutoRelayCommand(async () => {
                        IsControlEnabled = true;
                        await ClampMoveThenRelease(Destination.GoToManualLoad); }));
            }
        }

        private AutoRelayCommand _moveLeft;

        public AutoRelayCommand MoveLeft
        {
            get
            {
                return _moveLeft ??
                    (_moveLeft = new AutoRelayCommand(async () => { await ClampMoveThenRelease(Destination.MoveLeft); }));
            }
        }

        private AutoRelayCommand _moveRight;

        public AutoRelayCommand MoveRight
        {
            get
            {
                return _moveRight ??
                    (_moveRight = new AutoRelayCommand(async () => {  await ClampMoveThenRelease(Destination.MoveRight); }));
            }
        }

        private AutoRelayCommand _moveTop;

        public AutoRelayCommand MoveTop
        {
            get
            {
                return _moveTop ??
                    (_moveTop = new AutoRelayCommand(async () => { await ClampMoveThenRelease(Destination.MoveTop); }));
            }
        }

        private AutoRelayCommand _moveBottom;

        public AutoRelayCommand MoveBottom
        {
            get
            {
                return _moveBottom ?? 
                    (_moveBottom = new AutoRelayCommand(async () => { await ClampMoveThenRelease(Destination.MoveBottom); }));
            }
        }

        private AutoRelayCommand _moveCenter;

        public AutoRelayCommand MoveCenter
        {
            get
            {
                return _moveCenter ??
                    (_moveCenter = new AutoRelayCommand(async () => { await ClampMoveThenRelease(Destination.MoveCenter); }));
            }
        }

        private AutoRelayCommand _loadTerminated;

        public AutoRelayCommand LoadTerminated
        {
            get
            {
                return _loadTerminated ??
                    (_loadTerminated = new AutoRelayCommand(
                        () => {
                            _chuckSupervisor.ClampWafer(_chuckSupervisor.ChuckVM.SelectedWaferCategory.DimentionalCharacteristic);
                            MoveToDestination(Destination.MoveCenter);
                        }
                    ));
            }
        }
        #endregion // Methods
    }
}
