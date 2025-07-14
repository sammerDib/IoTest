using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

using CommunityToolkit.Mvvm.Messaging;

using MvvmDialogs.FrameworkDialogs.MessageBox;
using MvvmDialogs.FrameworkDialogs.OpenFile;

using UnitySC.PM.ANA.Client.Proxy.Axes;
using UnitySC.PM.ANA.Client.Proxy.Axes.Models;
using UnitySC.PM.ANA.Client.Proxy.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.PM.Shared.UI.ViewModels;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.ANA.Client.Modules.TestHardware.ViewModel
{
    public class StageViewModel : TabViewModelBase
    {
        #region Fields

        private const double Epsilon = 0.001;
        private Position _positionTarget;
        private readonly IDialogOwnerService _dialogService;
        private Scenario _scenario;
        private readonly IMessenger _messenger;
        private AutoRelayCommand _resetXY;
        private AutoRelayCommand _resetZTop;
        private AutoRelayCommand _resetZBottom;
        private AutoRelayCommand _startMove;
        private AutoRelayCommand _getCurrentPosition;
        private AutoRelayCommand _openScenario;
        private AutoRelayCommand _switchToScenario;
        private AutoRelayCommand _switchToMovement;
        private AutoRelayCommand _startScenario;
        private AutoRelayCommand _stopScenario;
        private AxesSupervisor _axesSupervisor;
        private ChuckSupervisor _chuckSupervisor;
        private GlobalStatusSupervisor _globalStatusSupervisor;
        private bool _isInScenarioMode = false;

        #endregion Fields

        #region Constructors

        public StageViewModel()
        {
            Title = "Stage";
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                _dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
                Init();
                _messenger = ClassLocator.Default.GetInstance<IMessenger>();
                _messenger.Register<GlobalStatus>(this, (r, m) => GlobalStatusChanged(m));
                _globalStatusSupervisor.OnNewMessage += _globalStatusSupervisor_OnNewMessage;
            }
        }

        private void _globalStatusSupervisor_OnNewMessage(Message message)
        {
            if (Scenario.State != StateScenario.Finished && Scenario.State != StateScenario.Pending)
            {
                switch (message.Level)
                {
                    case MessageLevel.Fatal:
                        Scenario.State = StateScenario.OnFailure;
                        break;

                    case MessageLevel.Error:
                        Scenario.State = StateScenario.LineOnError;
                        break;

                    default:
                        Scenario.State = StateScenario.Running;
                        break;
                }
            }
        }

        private void Init()
        {
            _globalStatusSupervisor = ClassLocator.Default.GetInstance<GlobalStatusSupervisor>();

            PositionTarget.PropertyChanged += PositionTarget_PropertyChanged;
            AxesVM.Position.PropertyChanged += Position_PropertyChanged;
            Scenario.PropertyChanged += Scenario_PropertyChanged;
            AxesVM.Status.PropertyChanged += Status_PropertyChanged;
            AxesVM.PropertyChanged += Axes_PropertyChanged;

            if (_globalStatusSupervisor.GetServerState()?.Result == PMGlobalStates.Free || _globalStatusSupervisor.GetServerState()?.Result == PMGlobalStates.Busy)
            {
                Refresh();
            }
        }

        private void GlobalStatusChanged(GlobalStatus globalStatus)
        {
            // If initialization is terminated
            if (globalStatus.CurrentState == PMGlobalStates.Free || globalStatus.CurrentState == PMGlobalStates.Busy)
            {
                Refresh();
            }
        }

        private void Refresh()
        {
            // We refresh the Axes
            AxesVM.Init();
            AxesVM.IsLocked = false;

            // Refresh of the Position Target
            _positionTarget = (Position)AxesVM.Position.Clone();
            _positionTarget.Speed = AxisSpeed.Normal;

            OnPropertyChanged(nameof(SelectedReference));
        }

        #endregion Constructors

        #region Private methods

        private bool CanStartMove()
        {
            if (PositionTarget.Near(AxesVM.Position, Epsilon) && !PiezoPositionsHasChanged())
                return false;
            if (AxesVM.Status.IsMoving)
                return false;
            if (AxesVM.Status.IsLanded)
                return false;
            return true;
        }

        private bool PiezoPositionsHasChanged()
        {
            var res = false;
            foreach (var target in PiezosTarget)
            {
                var current = AxesVM.Piezos.FirstOrDefault(x => x.Name == target.Name);
                if (current is null)
                    res = true;
                else
                    res = !current.Position.Value.Near(target.Position.Value, Epsilon * 100);
                if (res)
                    break;
            }
            return res;
        }

        private void PositionTarget_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateAllCanExecutes();
        }

        private void Scenario_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateAllCanExecutes();
        }

        private void Status_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateAllCanExecutes();
        }

        private void Position_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateAllCanExecutes();
        }

        private void Axes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
            {
                // Is it the end of a particular move
                if ((e.PropertyName == "IsGoingToParticularPosition") && (!AxesVM.IsGoingToParticularPosition))
                {
                    ResetXY.Execute(null);
                    ResetZTop.Execute(null);
                    ResetZBottom.Execute(null);
                }
            }));
        }

        private void CompleteAxisMove(ScenarioItem positionScenario, out AxisMove axisX, out AxisMove axisY, out AxisMove axisZTop, out AxisMove axisZBottom)
        {
            double? axisSpeedX = positionScenario.SpeedX == null ? Scenario.GlobalSpeedX : positionScenario.SpeedX;
            double? axisSpeedY = positionScenario.SpeedY == null ? Scenario.GlobalSpeedY : positionScenario.SpeedY;
            double? axisSpeedZTop = positionScenario.SpeedZTop == null ? Scenario.GlobalSpeedZTop : positionScenario.SpeedZTop;
            double? axisSpeedZBottom = positionScenario.SpeedZBottom == null ? Scenario.GlobalSpeedZBottom : positionScenario.SpeedZBottom;
            double? axisAccelX = positionScenario.AccelX == null ? Scenario.GlobalAccelX : positionScenario.AccelX;
            double? axisAccelY = positionScenario.AccelY == null ? Scenario.GlobalAccelY : positionScenario.AccelY;
            double? axisAccelZTop = positionScenario.AccelZTop == null ? Scenario.GlobalAccelZTop : positionScenario.AccelZTop;
            double? axisAccelZBottom = positionScenario.AccelZBottom == null ? Scenario.GlobalAccelZBottom : positionScenario.AccelZBottom;
            axisX = new AxisMove(positionScenario.XPos.Value, axisSpeedX.Value, axisAccelX.Value);
            axisY = new AxisMove(positionScenario.YPos.Value, axisSpeedY.Value, axisAccelY.Value);
            axisZTop = new AxisMove(positionScenario.ZTopPos.Value, axisSpeedZTop.Value, axisAccelZTop.Value);
            axisZBottom = new AxisMove(positionScenario.ZBottomPos.Value, axisSpeedZBottom.Value, axisAccelZBottom.Value);
        }

        #endregion Private methods

        #region Public methods

        public AxesVM AxesVM => AxesSupervisor.AxesVM;
        public ChuckVM ChuckVM => ChuckSupervisor.ChuckVM;

        public GlobalStatusSupervisor GlobalStatusSupervisor
        {
            get
            {
                return _globalStatusSupervisor;
            }
        }

        private List<PiezoVM> _piezosTarget;

        public List<PiezoVM> PiezosTarget
        {
            get
            {
                if (_piezosTarget == null)
                {
                    _piezosTarget = new List<PiezoVM>();
                    if (AxesVM != null)
                    {
                        foreach (var piezo in AxesVM.Piezos)
                        {
                            _piezosTarget.Add(new PiezoVM() { Max = new LengthVM(piezo.Max.Length), Min = new LengthVM(piezo.Min.Length), Name = piezo.Name, Position = new LengthVM(piezo.Position.Length) });
                        }
                    }
                }

                return _piezosTarget;
            }
            set
            {
                _piezosTarget = value;
            }
        }

        public AxesSupervisor AxesSupervisor
        {
            get
            {
                if (_axesSupervisor == null)
                {
                    _axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();
                }

                return _axesSupervisor;
            }
        }

        public ChuckSupervisor ChuckSupervisor
        {
            get
            {
                if (_chuckSupervisor == null)
                {
                    _chuckSupervisor = ClassLocator.Default.GetInstance<ChuckSupervisor>();
                }

                return _chuckSupervisor;
            }
        }

        public Position PositionTarget
        {
            get
            {
                if (_positionTarget == null)
                {
                    // The first time, we use the position
                    if (AxesVM != null)
                    {
                        _positionTarget = (Position)AxesVM.Position.Clone();
                    }
                    else
                    {
                        _positionTarget = new Position(new MotorReferential(), 0, 0, 0, 0, AxisSpeed.Normal);
                    }
                }

                return _positionTarget;
            }
            set
            {
                _positionTarget = value;
            }
        }

        public AutoRelayCommand ResetXY
        {
            get
            {
                return _resetXY ?? (_resetXY = new AutoRelayCommand(
                () =>
                {
                    PositionTarget.X = AxesVM.Position.X;
                    PositionTarget.Y = AxesVM.Position.Y;
                },
                () =>
                {
                    return !PositionTarget.Y.Near(AxesVM.Position.Y, Epsilon) || !PositionTarget.X.Near(AxesVM.Position.X, Epsilon);
                }));
            }
        }

        public AutoRelayCommand ResetZTop
        {
            get
            {
                return _resetZTop ?? (_resetZTop = new AutoRelayCommand(
                () =>
                {
                    PositionTarget.ZTop = AxesVM.Position.ZTop;
                },
                () => { return !PositionTarget.ZTop.Near(AxesVM.Position.ZTop, Epsilon); }));
            }
        }

        public bool IsInScenarioMode
        {
            get
            {
                return _isInScenarioMode;
            }

            set
            {
                if (_isInScenarioMode == value)
                {
                    return;
                }

                _isInScenarioMode = value;
                OnPropertyChanged(nameof(IsInScenarioMode));
            }
        }

        private OpticalReferenceDefinition _selectedReference;

        public OpticalReferenceDefinition SelectedReference
        {
            get
            {
                if (_selectedReference == null)
                    _selectedReference = ChuckVM?.AnaChuckConfiguration.ReferencesList?.FirstOrDefault();
                return _selectedReference;
            }

            set
            {
                if (_selectedReference == value)
                {
                    return;
                }

                _selectedReference = value;
                OnPropertyChanged(nameof(SelectedReference));
            }
        }

        public AutoRelayCommand ResetZBottom
        {
            get
            {
                return _resetZBottom ?? (_resetZBottom = new AutoRelayCommand(
                () =>
                {
                    PositionTarget.ZBottom = AxesVM.Position.ZBottom;
                },
                () => { return !PositionTarget.ZBottom.Near(AxesVM.Position.ZBottom, Epsilon); }));
            }
        }

        public AutoRelayCommand StartMove
        {
            get
            {
                return _startMove ?? (_startMove = new AutoRelayCommand(
                () =>
                {
                    Response<bool> response = null;
                    try
                    {
                        var destination = new AnaPosition(
                           new MotorReferential(), // hardware test uses raw mode
                           PositionTarget.X.Near(AxesVM.Position.X, Epsilon) ? double.NaN : PositionTarget.X,
                           PositionTarget.Y.Near(AxesVM.Position.Y, Epsilon) ? double.NaN : PositionTarget.Y,
                           PositionTarget.ZTop.Near(AxesVM.Position.ZTop, Epsilon) ? double.NaN : PositionTarget.ZTop,
                           PositionTarget.ZBottom.Near(AxesVM.Position.ZBottom, Epsilon) ? double.NaN : PositionTarget.ZBottom,
                           PiezosTarget.Select(x => new ZPiezoPosition(new MotorReferential(), x.Name, x.Position.Length)).ToList());

                        response = AxesSupervisor.GotoPosition(destination, AxesVM.SelectedAxisSpeed);
                    }
                    catch (Exception e)

                    {
                        AxesVM.AddMessagesOrExceptionToErrorsList(response, e);
                    }
                },

                () => { return CanStartMove(); }));
            }
        }

        public AutoRelayCommand GetCurrentPosition
        {
            get
            {
                return _getCurrentPosition ?? (_getCurrentPosition = new AutoRelayCommand(
          () =>
          {
              Response<PositionBase> response = null;
              try
              {
                  response = _axesSupervisor.GetCurrentPosition();
                  if (response.Result is AnaPosition position)
                  {
                      AxesVM.Position.X = position.X;
                      AxesVM.Position.Y = position.Y;
                      string piezos = AxesVM.Piezos != null ? string.Join(" ", AxesVM.Piezos) : string.Empty;
                      string message = $"The current position ({DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}) is \n X -> {AxesVM.Position.X:F3} mm \n Y -> {AxesVM.Position.Y:F3} mm \n ZTop -> {AxesVM.Position.ZTop:F3} mm \n ZBottom -> {AxesVM.Position.ZBottom:F3} mm \n Piezos -> {piezos}";
                      _dialogService.ShowMessageBox(message, "Current position", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
                  }
                  else
                  {
                      string message = $"Stage position ({DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}) is not of expected type";
                      _dialogService.ShowMessageBox(message, "Current position", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
                  }
              }
              catch (Exception e)
              {
                  AxesVM.AddMessagesOrExceptionToErrorsList(response, e);
              }
          },
          () => { return true; }));
            }
        }

        public AutoRelayCommand OpenScenario
        {
            get
            {
                return _openScenario ?? (_openScenario = new AutoRelayCommand(
                () =>
                {
                    var settings = new OpenFileDialogSettings
                    {
                        Title = "Open Scenario",
                        Filter = "xml files (*.xml)|*.xml",
                    };

                    bool? result = _dialogService.ShowOpenFileDialog(settings);
                    if (result == true)
                    {
                        try
                        {
                            Scenario.FileName = settings.FileName;
                            var serializer = new XmlSerializer(typeof(Scenario));
                            using (var fileStream = new FileStream(Scenario.FileName, FileMode.Open))
                            {
                                var scenario = (Scenario)serializer.Deserialize(fileStream);
                                Scenario.NumberOfLines = scenario.ScenarioItemList.Length;
                                Scenario.ScenarioItemList = scenario.ScenarioItemList;
                                Scenario.Name = scenario.Name;
                                Scenario.NumberOfLinesTreated = 0;
                                Scenario.State = StateScenario.Pending;
                                Scenario.GlobalSpeedX = scenario.GlobalSpeedX;
                                Scenario.GlobalSpeedY = scenario.GlobalSpeedY;
                                Scenario.GlobalSpeedZTop = scenario.GlobalSpeedZTop;
                                Scenario.GlobalSpeedZBottom = scenario.GlobalSpeedZBottom;
                                Scenario.GlobalAccelX = scenario.GlobalAccelX;
                                Scenario.GlobalAccelY = scenario.GlobalAccelY;
                                Scenario.GlobalAccelZTop = scenario.GlobalAccelZTop;
                                Scenario.GlobalAccelZBottom = scenario.GlobalAccelZBottom;
                                foreach (var positionScenario in Scenario.ScenarioItemList)
                                {
                                    if (!positionScenario.XPos.HasValue || !positionScenario.YPos.HasValue || !positionScenario.ZTopPos.HasValue || !positionScenario.ZBottomPos.HasValue)
                                        throw new Exception("Check position for each movements items. An error is present");
                                }
                            }
                            UpdateAllCanExecutes();
                        }
                        catch (Exception e)
                        {
                            _dialogService.ShowMessageBox(new MessageBoxSettings()
                            {
                                Caption = "Error",
                                Button = MessageBoxButton.OK,
                                Icon = MessageBoxImage.Error,
                                MessageBoxText = e.Message
                            });
                        }
                    }
                },
                () => { return Scenario.State != StateScenario.Running; }));
            }
        }

        public AutoRelayCommand SwitchToScenario
        {
            get
            {
                return _switchToScenario ?? (_switchToScenario = new AutoRelayCommand(
                () =>
                {
                    IsInScenarioMode = true;
                    Scenario.FileName = "";
                    Scenario.Name = "";
                    Scenario.NumberOfCycles = 1;
                    Scenario.NumberOfLines = 0;
                    Scenario.NumberOfLinesTreated = 0;
                    Scenario.State = StateScenario.Pending;
                },
                () => { return Scenario.State != StateScenario.Running; }));
            }
        }

        public AutoRelayCommand SwitchToMovments
        {
            get
            {
                return _switchToMovement ?? (_switchToMovement = new AutoRelayCommand(
                () =>
                {
                    IsInScenarioMode = false;
                    Refresh();
                },
                () => { return Scenario.State != StateScenario.Running; }));
            }
        }

        public Scenario Scenario
        {
            get
            {
                if (_scenario == null)
                {
                    _scenario = new Scenario();
                }

                return _scenario;
            }
            set
            {
                _scenario = value;
            }
        }

        public AutoRelayCommand StartScenario
        {
            get
            {
                return _startScenario ?? (_startScenario = new AutoRelayCommand(
                () =>
                {
                    var message = string.Empty;
                    Task.Run(() =>
                    {
                        Scenario.State = StateScenario.Running;
                        UpdateAllCanExecutes();
                        var runningScenario = System.Diagnostics.Stopwatch.StartNew();
                        for (int i = 1; i <= Scenario.NumberOfCycles; i++)
                        {
                            foreach (var positionScenario in Scenario.ScenarioItemList)
                            {
                                if (Scenario.State == StateScenario.Running)
                                {
                                    AxisMove AxisX, AxisY, AxisZTop, AxisZBottom;
                                    CompleteAxisMove(positionScenario, out AxisX, out AxisY, out AxisZTop, out AxisZBottom);
                                    _axesSupervisor.GotoPointCustomSpeedAccel(AxisX, AxisY, AxisZTop, AxisZBottom);
                                    Scenario.NumberOfLinesTreated++;
                                    if (Scenario.State != StateScenario.LineOnError)
                                    {
                                        _axesSupervisor.WaitMotionEnd(30_000);
                                    }
                                    else
                                    {
                                        Scenario.State = StateScenario.Running;
                                    }
                                }
                            }
                        }
                        Scenario.State = StateScenario.Finished;
                        runningScenario.Stop();
                        message = $"Execution time : {TimeSpan.FromSeconds(runningScenario.Elapsed.TotalSeconds).ToString("hh'h 'mm'm 'ss's 'fff'ms'")}";

                        Application.Current?.Dispatcher.BeginInvoke(new Action(() => _dialogService.ShowMessageBox(message, "Scenario", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None)));
                    });
                },
                () => { return Scenario.NumberOfLines > 0 && !(Scenario.State == StateScenario.Running || Scenario.State == StateScenario.OnFailure) && !AxesVM.Status.IsLanded; }));
            }
        }

        public AutoRelayCommand StopScenario
        {
            get
            {
                return _stopScenario ?? (_stopScenario = new AutoRelayCommand(
                () =>
                {
                    Scenario.NumberOfLinesTreated = 0;
                    Scenario.State = StateScenario.Finished;
                    _axesSupervisor.StopAllMoves();
                    UpdateAllCanExecutes();
                },
                () => { return Scenario.State == StateScenario.Running; }));
            }
        }

        #endregion Public methods
    }
}
