using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Client.Proxy.Axes;
using UnitySC.PM.EME.Client.Proxy.Chuck;
using UnitySC.PM.EME.Service.Interface.Axes;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.ClientProxy.Referential;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.UI.ViewModels;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Client.Modules.TestHardware.ViewModel
{
    public class PhotoLumMotionAxesViewModel : TabViewModelBase
    {
        private readonly ReferentialSupervisor _referentialSupervisor;
        private readonly IEmeraMotionAxesService _motionAxesSupervisor;
        private ReferentialTag _selectedReferentialTag;
        private AxesVM _axesVM;
        private Position _positionTarget;
        private Position _currentPosition;
        private AsyncRelayCommand<double> _moveX;
        private AsyncRelayCommand<double> _moveY;
        private AsyncRelayCommand<double> _moveZ;
        private XYZPosition _xMinLimitInReferential;
        private XYZPosition _xMaxLimitInReferential;
        private XYZPosition _yMinLimitInReferential;
        private XYZPosition _yMaxLimitInReferential;
        private XYZPosition _zMinLimitInReferential;
        private XYZPosition _zMaxLimitInReferential;
        private bool _isXLimitReached;
        private bool _isYLimitReached;
        private bool _isZLimitReached;
        public PhotoLumMotionAxesViewModel(IMessenger messenger)
        {
            _referentialSupervisor = ClassLocator.Default.GetInstance<ReferentialSupervisor>();
            _motionAxesSupervisor = ClassLocator.Default.GetInstance<IEmeraMotionAxesService>();
            ReferentialTags = new ObservableCollection<ReferentialTag> { ReferentialTag.Motor, ReferentialTag.Wafer };
            SelectedReferentialTag = ReferentialTags.FirstOrDefault();
            InitializeLimits();
            StartPositionUpdate();
            messenger.Register<ServiceImageWithStatistics>(this, (_, image) => Image = image);
        }

        private void AxesVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_axesVM.Position))
            {
                StartPositionUpdate();
            }
        }

        private void StartPositionUpdate()
        {
            Task.Run(() =>
            {
                {
                    var currentPos = _motionAxesSupervisor.GetCurrentPosition()?.Result;
                    var positionReferential = _referentialSupervisor.ConvertTo(currentPos, SelectedReferentialTag)
                        ?.Result.ToXYZPosition();

                    if (positionReferential != null)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            UpdatePosition(positionReferential);
                        });
                    }
                }
            });
        }

        private void UpdatePosition(XYZPosition positionReferential)
        {
            var referential = GetReferentialForTag(SelectedReferentialTag);

            CurrentPosition = new Position(referential, positionReferential.X, positionReferential.Y,
                positionReferential.Z);
            PositionTarget = new Position(referential, positionReferential.X, positionReferential.Y,
                positionReferential.Z);

            IsXLimitReached = IsLimitReached(CurrentPosition.X, _xMinLimitInReferential.X, _xMaxLimitInReferential.X);
            IsYLimitReached = IsLimitReached(CurrentPosition.Y, _yMinLimitInReferential.Y, _yMaxLimitInReferential.Y);
            IsZLimitReached = IsLimitReached(CurrentPosition.Z, _zMinLimitInReferential.Z, _zMaxLimitInReferential.Z);

            OnPropertyChanged(nameof(CurrentPosition));
            OnPropertyChanged(nameof(PositionTarget));
        }
        private void InitializeLimits()
        {
            _xMinLimitInReferential = _referentialSupervisor.ConvertTo(new XYZPosition(new MotorReferential(), AxesVM.XAxisConfig.PositionMin.Millimeters, double.NaN, double.NaN), SelectedReferentialTag)?.Result.ToXYZPosition();
            _xMaxLimitInReferential = _referentialSupervisor.ConvertTo(new XYZPosition(new MotorReferential(), AxesVM.XAxisConfig.PositionMax.Millimeters, double.NaN, double.NaN), SelectedReferentialTag)?.Result.ToXYZPosition();
            _yMinLimitInReferential = _referentialSupervisor.ConvertTo(new XYZPosition(new MotorReferential(), double.NaN, AxesVM.YAxisConfig.PositionMin.Millimeters, double.NaN), SelectedReferentialTag)?.Result.ToXYZPosition();
            _yMaxLimitInReferential = _referentialSupervisor.ConvertTo(new XYZPosition(new MotorReferential(), double.NaN, AxesVM.YAxisConfig.PositionMax.Millimeters, double.NaN), SelectedReferentialTag)?.Result.ToXYZPosition();
            _zMinLimitInReferential = _referentialSupervisor.ConvertTo(new XYZPosition(new MotorReferential(), double.NaN, double.NaN, AxesVM.ZAxisConfig.PositionMin.Millimeters), SelectedReferentialTag)?.Result.ToXYZPosition();
            _zMaxLimitInReferential = _referentialSupervisor.ConvertTo(new XYZPosition(new MotorReferential(), double.NaN, double.NaN, AxesVM.ZAxisConfig.PositionMax.Millimeters), SelectedReferentialTag)?.Result.ToXYZPosition();
        }
        private bool IsLimitReached(double currentValue, double minLimit, double maxLimit, double tolerance = 0.0001)
        {
            return Math.Abs(currentValue - minLimit) < tolerance || Math.Abs(currentValue - maxLimit) < tolerance;
        }
        private ReferentialBase GetReferentialForTag(ReferentialTag tag)
        {
            return tag == ReferentialTag.Motor ? new MotorReferential() : new WaferReferential() as ReferentialBase;
        }
        public AxesVM AxesVM
        {
            get
            {
                if (_axesVM == null)
                {
                    _axesVM = ClassLocator.Default.GetInstance<AxesVM>();
                }

                return _axesVM;
            }
        }

        private ChuckVM _chuckVM;

        public ChuckVM ChuckVM
        {
            get
            {
                if (_chuckVM == null)
                {
                    _chuckVM = ClassLocator.Default.GetInstance<ChuckVM>();
                }

                return _chuckVM;
            }
        }

        public Position PositionTarget
        {
            get => _positionTarget;
            set => SetProperty(ref _positionTarget, value);
        }

        public Position CurrentPosition
        {
            get => _currentPosition;
            set => SetProperty(ref _currentPosition, value);
        }
        public bool IsXLimitReached
        {
            get => _isXLimitReached;
            set => SetProperty(ref _isXLimitReached, value);
        }
        public bool IsYLimitReached
        {
            get => _isYLimitReached;
            set => SetProperty(ref _isYLimitReached, value);
        }
        public bool IsZLimitReached
        {
            get => _isZLimitReached;
            set => SetProperty(ref _isZLimitReached, value);
        }
        public ObservableCollection<ReferentialTag> ReferentialTags { get; set; }

        public ReferentialTag SelectedReferentialTag
        {
            get => _selectedReferentialTag;
            set
            {
                if (SetProperty(ref _selectedReferentialTag, value))
                {
                    Task.Run(() =>
                    {
                        InitializeLimits();
                        var currentPos = _motionAxesSupervisor.GetCurrentPosition()?.Result;
                        var positionReferential = _referentialSupervisor
                            .ConvertTo(currentPos, SelectedReferentialTag)?.Result.ToXYZPosition();

                        if (positionReferential != null)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                UpdatePosition(positionReferential);
                            });
                        }
                    });
                }
            }
        }

        public AsyncRelayCommand<double> MoveX =>
            _moveX ?? (_moveX = new AsyncRelayCommand<double>(
                async (positionX) =>
                {
                    var newPositionX = Math.Max(_xMinLimitInReferential.X, positionX);
                    newPositionX = Math.Min(_xMaxLimitInReferential.X, newPositionX);
                    PositionTarget.X = newPositionX;
                    await MoveAxes();
                },
                (positionX) => { return true; }));

        public AsyncRelayCommand<double> MoveY =>
            _moveY ?? (_moveY = new AsyncRelayCommand<double>(
                async (positionY) =>
                {
                    var newPositionY = Math.Max(_yMinLimitInReferential.Y, positionY);
                    newPositionY = Math.Min(_yMaxLimitInReferential.Y, newPositionY);
                    PositionTarget.Y = newPositionY;
                    await MoveAxes();
                },
                (positionY) => { return true; }));

        public AsyncRelayCommand<double> MoveZ =>
            _moveZ ?? (_moveZ = new AsyncRelayCommand<double>(
                async (positionZ) =>
                {
                    var newPositionZ = Math.Max(_zMinLimitInReferential.Z, positionZ);
                    newPositionZ = Math.Min(_zMaxLimitInReferential.Z, newPositionZ);
                    PositionTarget.Z = newPositionZ;
                    await MoveAxes();
                },
                (positionZ) => { return true; }));

        private async Task MoveAxes()
        {
            var targetPos = PositionTarget.ToXyzPosition();
            if (targetPos != null)
            {
                _axesVM.DoMoveAxes(targetPos);
            }
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            AxesVM.PropertyChanged += AxesVM_PropertyChanged;
            StartPositionUpdate();
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            AxesVM.PropertyChanged -= AxesVM_PropertyChanged;
        }
        
        private ServiceImage _image;
        public ServiceImage Image { get => _image; private set => SetProperty(ref _image, value); }
    }
}
