using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Axes.ViewModel
{
    public enum DeviceType
    {
        Axes,
        Chuck,
        Camera,
        Led,
        Error
    }

    public enum AxisType
    {
        A0,
        A1,
        Error
    }

    public class MainAxesVM : ViewModelBase, INotifyPropertyChanged
    {
        #region Constants

        private const double AXES_MIN_POSITION = -62000;
        private const double AXES_MAX_POSITION = 678850;
        private const double CHUCK_MIN_POSITION = -40000000;
        private const double CHUCK_MAX_POSITION = 40000000;

        //private const double CHUCK_MIN_POSITION = 0;
        //private const double CHUCK_MAX_POSITION = 3157333;
        private const int ERROR_POSITION = -99999999;

        private const double AXES_STEP = 20000;
        private const double CHUCK_STEP = 1000000;
        //private const double CHUCK_STEP = 85333;

        #endregion Constants

        #region Fields

        private AxesSupervisor _axesSupervisor;
        private string _deviceId;
        private string _commandToApply = "";
        private string _controllerResponse;

        //private ObservableCollection<SettingType> _settingTypes;
        private string _axesPosition;

        private string _chuckPosition;
        private string _tabControlIndex;
        private DeviceType _current_Device;
        private AxisType _currentAxis;
        private double _minPosition;
        private double _maxPosition;
        private double _step;

        #endregion Fields

        #region Constructors

        public MainAxesVM()
        {
            if (!IsInDesignMode)
                throw new ApplicationException("This constructor is for design mode only.");
        }

        public MainAxesVM(AxesSupervisor axesSupervisor, string deviceId)
        {
            AxesPosition = ERROR_POSITION.ToString();
            ChuckPosition = ERROR_POSITION.ToString();
            _current_Device = DeviceType.Axes;
            _currentAxis = AxisType.A0;
            TabControlIndex = "0";

            _axesSupervisor = axesSupervisor;
            _deviceId = deviceId;
            // CameraSettingsVM = new CameraSettingsVM();
        }

        #endregion Constructors

        private ViewModelBase _cameraSettingsVM;

        public ViewModelBase CameraSettingsVM
        {
            get => _cameraSettingsVM; set { if (_cameraSettingsVM != value) { _cameraSettingsVM = value; RaisePropertyChanged(); } }
        }

        #region Properties

        public string CommandToApply
        {
            get { return _commandToApply; }
            set { _commandToApply = value; RaisePropertyChanged(); }
        }

        public string ControllerResponse
        { get => _controllerResponse; set { _controllerResponse = value; RaisePropertyChanged(); } }

        //public ObservableCollection<SettingType> SettingTypes { get => _settingTypes; set => _settingTypes = value; }
        public string AxesPosition
        {
            get => _axesPosition;
            set
            {
                if (Double.Parse(value) < AXES_MIN_POSITION)
                    value = AXES_MIN_POSITION.ToString();
                if (Double.Parse(value) > AXES_MAX_POSITION)
                    value = AXES_MAX_POSITION.ToString();

                _axesPosition = value;
                RaisePropertyChanged();
            }
        }

        public string ChuckPosition
        {
            get => _chuckPosition;
            set
            {
                if (Double.Parse(value) < CHUCK_MIN_POSITION)
                    value = CHUCK_MIN_POSITION.ToString();
                if (Double.Parse(value) > CHUCK_MAX_POSITION)
                    value = CHUCK_MAX_POSITION.ToString();

                _chuckPosition = value;
                RaisePropertyChanged();
            }
        }

        public double MinPosition
        { get => _minPosition; set { _minPosition = value; RaisePropertyChanged(); } }
        public double MaxPosition
        { get => _maxPosition; set { _maxPosition = value; RaisePropertyChanged(); } }

        public double Step
        {
            get => _step; set { _step = value; RaisePropertyChanged(); }
        }

        public string TabControlIndex
        {
            get
            {
                switch (_tabControlIndex)
                {
                    case "0":
                        _current_Device = DeviceType.Axes;
                        _currentAxis = AxisType.A0;
                        MinPosition = AXES_MIN_POSITION;
                        MaxPosition = AXES_MAX_POSITION;
                        Step = AXES_STEP;
                        break;

                    case "1":
                        _current_Device = DeviceType.Chuck;
                        _currentAxis = AxisType.A1;
                        MinPosition = CHUCK_MIN_POSITION;
                        MaxPosition = CHUCK_MAX_POSITION;
                        Step = CHUCK_STEP;
                        break;

                    case "2":
                        _current_Device = DeviceType.Camera;
                        _currentAxis = AxisType.Error;
                        break;

                    case "3":
                        _current_Device = DeviceType.Led;
                        _currentAxis = AxisType.Error;
                        break;
                }
                return _tabControlIndex;
            }

            set => _tabControlIndex = value;
        }

        #endregion Properties

        private delegate void Delegate();

        private Brush _screenBrush = Brushes.SlateGray;

        public Brush ScreenBrush
        {
            get => _screenBrush; set { if (_screenBrush != value) { _screenBrush = value; RaisePropertyChanged(); } }
        }

        //=================================================================
        // Commandes
        //=================================================================

        #region Commandes

        private void PrintResponse()
        {
            Thread.Sleep(5000);
            var messages = _axesSupervisor.SendControllerResponse().Result;

            foreach (string message in messages)
            {
                ControllerResponse += System.Environment.NewLine + "RX: " + message;
            }
        }

        private RelayCommand _connectCommand;

        public RelayCommand ConnectCommand
        {
            get
            {
                return _connectCommand ?? (_connectCommand = new RelayCommand(
              () =>
              {
                  var axis = _currentAxis.ToString().ToLower();
                  string initCommand = $"ap init {axis}";
                  ControllerResponse += System.Environment.NewLine + "TX: " + initCommand;
                  _axesSupervisor.SendCommandToAxes(initCommand);

                  Task.Run(() => PrintResponse());

                  _ = _current_Device == DeviceType.Axes ? AxesPosition = "0" : ChuckPosition = "0";
              },
              () => { return true; }));
            }
        }

        private RelayCommand _sendCommand;

        public RelayCommand SendCommand
        {
            get
            {
                return _sendCommand ?? (_sendCommand = new RelayCommand(
              () =>
              {
                  if (!string.IsNullOrEmpty(_commandToApply))
                  {
                      string commandBackup = _commandToApply;
                      ControllerResponse += System.Environment.NewLine + "TX: " + _commandToApply;
                      _axesSupervisor.SendCommandToAxes(_commandToApply);

                      Task.Run(() => PrintResponse()).Wait();

                      var axis = _currentAxis.ToString().ToLower();
                      _commandToApply = $"ap getpos {axis}";
                      ControllerResponse += System.Environment.NewLine + "TX: " + _commandToApply;
                      _axesSupervisor.SendCommandToAxes(_commandToApply);

                      Task.Run(() => PrintResponse()).Wait();
                      CommandToApply = commandBackup;

                      // Code in comment
                      // Have to be change for PositionBase type management

                      //if (_current_Device == DeviceType.Axes)
                      //{
                      //    int currentPosition = _axesSupervisor.Getpo().Result;
                      //    if (currentPosition != ERROR_POSITION) AxesPosition = currentPosition.ToString();
                      //}
                      //else
                      //{
                      //    int currentPosition = _axesSupervisor.GetChuckPosition().Result;
                      //    if (currentPosition != ERROR_POSITION) ChuckPosition = currentPosition.ToString();
                      //}
                  }
              },
              () => { return true; }));
            }
        }

        private RelayCommand _moveCommand;

        public RelayCommand MoveCommand
        {
            get
            {
                return _moveCommand ?? (_moveCommand = new RelayCommand(
              () =>
              {
                  string position = _current_Device == DeviceType.Axes ? position = AxesPosition : position = ChuckPosition;
                  var axis = _currentAxis.ToString().ToLower();

                  _commandToApply = $"ap moveabs {axis} {position}";
                  ControllerResponse += System.Environment.NewLine + "TX: " + _commandToApply;
                  _axesSupervisor.SendCommandToAxes(_commandToApply);

                  Task.Run(() => PrintResponse()).Wait();

                  _commandToApply = $"ap getpos {axis}";
                  ControllerResponse += System.Environment.NewLine + "TX: " + _commandToApply;
                  _axesSupervisor.SendCommandToAxes(_commandToApply);
                  Task.Run(() => PrintResponse()).Wait();

                  // Code in comment
                  // Have to be change for PositionBase type management

                  //if (_current_Device == DeviceType.Axes)
                  //{
                  //    int currentPosition = _axesSupervisor.GetAxesPosition().Result;
                  //    if (currentPosition != ERROR_POSITION) AxesPosition = currentPosition.ToString();
                  //}
                  //else
                  //{
                  //    int currentPosition = _axesSupervisor.GetChuckPosition().Result;
                  //    if (currentPosition != ERROR_POSITION) ChuckPosition = currentPosition.ToString();
                  //}
              },
              () => { return true; }));
            }
        }

        private RelayCommand _getPositionCommand;

        public RelayCommand GetPositionCommand
        {
            get
            {
                return _getPositionCommand ?? (_getPositionCommand = new RelayCommand(
              () =>
              {
                  var axis = _currentAxis.ToString().ToLower();
                  _commandToApply = $"ap getpos {axis}";
                  ControllerResponse += System.Environment.NewLine + "TX: " + _commandToApply;
                  _axesSupervisor.SendCommandToAxes(_commandToApply);

                  Task.Run(() => PrintResponse()).Wait();

                  // Code in comment
                  // Have to be change for PositionBase type management

                  //if (_current_Device == DeviceType.Axes)
                  //{
                  //    int currentPosition = _axesSupervisor.GetAxesPosition().Result;
                  //    if (currentPosition != ERROR_POSITION) AxesPosition = currentPosition.ToString();
                  //}
                  //else
                  //{
                  //    int currentPosition = _axesSupervisor.GetChuckPosition().Result;
                  //    if (currentPosition != ERROR_POSITION) ChuckPosition = currentPosition.ToString();
                  //}
              },
              () => { return true; }));
            }
        }

        private void SubstractPosition(double v, DeviceType device)
        {
            if (device == DeviceType.Axes)
            {
                double newValue = Double.Parse(_axesPosition) - v;
                AxesPosition = newValue.ToString();
            }
            else
            {
                double newValue = Double.Parse(_chuckPosition) - v;
                ChuckPosition = newValue.ToString();
            }
        }

        private void AddPosition(double v, DeviceType device)
        {
            if (device == DeviceType.Axes)
            {
                double newValue = Double.Parse(_axesPosition) + v;
                AxesPosition = newValue.ToString();
            }
            else
            {
                double newValue = Double.Parse(_chuckPosition) + v;
                ChuckPosition = newValue.ToString();
            }
        }

        private RelayCommand _substractPositionCommand;

        public RelayCommand SubstractPositionCommand
        {
            get
            {
                return _substractPositionCommand ?? (_substractPositionCommand = new RelayCommand(
              () => SubstractPosition(Step, _current_Device),
              () => { return true; }));
            }
        }

        private RelayCommand _substractPositionBoostedCommand;

        public RelayCommand SubstractPositionBoostedCommand
        {
            get
            {
                return _substractPositionBoostedCommand ?? (_substractPositionBoostedCommand = new RelayCommand(
              () => SubstractPosition(Step * 3, _current_Device),
              () => { return true; }));
            }
        }

        private RelayCommand _addPositionCommand;

        public RelayCommand AddPositionCommand
        {
            get
            {
                return _addPositionCommand ?? (_addPositionCommand = new RelayCommand(
              () => AddPosition(Step, _current_Device),
              () => { return true; }));
            }
        }

        private RelayCommand _addPositionBoostedCommand;

        public RelayCommand AddPositionBoostedCommand
        {
            get
            {
                return _addPositionBoostedCommand ?? (_addPositionBoostedCommand = new RelayCommand(
              () => AddPosition(Step * 3, _current_Device),
              () => { return true; }));
            }
        }

        private RelayCommand _clearLogsCommand;

        public RelayCommand ClearLogsCommand
        {
            get
            {
                return _clearLogsCommand ?? (_clearLogsCommand = new RelayCommand(
              () => ControllerResponse = string.Empty,
              () => { return true; }));
            }
        }

        #endregion Commandes

        //=================================================================
        // Utilitaires
        //=================================================================
    }
}
