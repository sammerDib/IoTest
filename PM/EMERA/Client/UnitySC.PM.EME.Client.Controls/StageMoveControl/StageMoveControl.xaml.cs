using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using UnitySC.PM.EME.Client.Proxy;
using UnitySC.PM.EME.Client.Proxy.Algo;
using UnitySC.PM.EME.Client.Proxy.Axes;
using UnitySC.PM.EME.Client.Proxy.Camera;
using UnitySC.PM.EME.Client.Proxy.FilterWheel;
using UnitySC.PM.EME.Client.Proxy.KeyboardMouseHook;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Algo.GetZFocus;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.EME.Client.Controls.StageMoveControl
{
    /// <summary>
    /// Interaction logic for StageMoveControl.xaml
    /// </summary>
    public partial class StageMoveControl: UserControl, INotifyPropertyChanged
    {
        private AlgoSupervisor _algoSupervisor;
        private CameraBench _cameraBench;
        private FilterWheelBench _filterWheelBench;
        private Stopwatch _keyDownTimer;
        private bool _isMoving;
        private readonly AxesVM _axes = ClassLocator.Default.GetInstance<AxesVM>();

        private static readonly IEnumerable<ScanRangeType> CachedScanRanges = Enum.GetValues(typeof(ScanRangeType))
            .Cast<ScanRangeType>()
            .Where(e => !e.Equals(ScanRangeType.Configured));

        public StageMoveControl()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;
            ScanRanges = CachedScanRanges;
            ServiceLocator.KeyboardMouseHook.KeyUpDownEvent += KeyboardMouseHook_KeyEvent;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ServiceLocator.KeyboardMouseHook.KeyUpDownEvent -= KeyboardMouseHook_KeyEvent;
            ServiceLocator.KeyboardMouseHook.MouseDownEvent -= KeyboardMouseHook_MouseDownEvent;
        }

        private void KeyboardMouseHook_KeyEvent(object sender, KeyGlobalEventArgs e)
        {
            if (e.CurrentKey == Key.F11)
            {
                IsKeyboardInputMode = true;
                return;
            }

            if (!IsKeyboardInputMode)
                return;

            AxesMoveTypes? curMoveType = null;

            switch (e.CurrentKey)
            {
                case Key.Left:
                    curMoveType = AxesMoveTypes.XMinus;
                    break;

                case Key.Right:
                    curMoveType = AxesMoveTypes.XPlus;
                    break;

                case Key.Up:
                    curMoveType = AxesMoveTypes.YPlus;
                    break;

                case Key.Down:
                    curMoveType = AxesMoveTypes.YMinus;
                    break;

                case Key.PageUp:
                    curMoveType = AxesMoveTypes.ZPlus;
                    break;

                case Key.PageDown:
                    curMoveType = AxesMoveTypes.ZMinus;
                    break;
            }

            if (curMoveType == null)
                return;

            if (e.IsKeyDown)
                OnKeyOrMouseDown(curMoveType.Value);
            else
                OnKeyOrMouseUp();
        }

        private void OnKeyOrMouseDown(AxesMoveTypes moveType)
        {
            if (_isMoving)
            {
                return;
            }

            _isMoving = true;
            
            _keyDownTimer = new Stopwatch();
            _keyDownTimer.Start();
            
            Task.Run(() =>
            {
                while (_keyDownTimer.ElapsedMilliseconds < 500 && _keyDownTimer.IsRunning)
                {
                    Thread.Sleep(10);
                }

                if (_keyDownTimer.IsRunning)
                {
                    _axes.PerformLongMove(moveType);
                }
                else
                {
                    _axes.PerformShortMove(moveType);
                }
            });
        }

        private void OnKeyOrMouseUp()
        {
            if (_keyDownTimer.ElapsedMilliseconds > 500)
            {
                _axes.Stop.Execute(null);
            }
            _keyDownTimer.Stop();
            _isMoving = false;
        }

        private bool _isKeyboardInputMode;

        public bool IsKeyboardInputMode
        {
            get => _isKeyboardInputMode;
            set
            {
                if (_isKeyboardInputMode == value)
                    return;

                _isKeyboardInputMode = value;
                if (_isKeyboardInputMode)
                {
                    ServiceLocator.KeyboardMouseHook.HandleKeyBoard();
                    ServiceLocator.KeyboardMouseHook.StartCaptureMouseDown();
                    ServiceLocator.KeyboardMouseHook.MouseDownEvent += KeyboardMouseHook_MouseDownEvent;
                }
                else
                {
                    ServiceLocator.KeyboardMouseHook.UnhandleKeyBoard();
                    ServiceLocator.KeyboardMouseHook.StopCaptureMouseDown();
                    ServiceLocator.KeyboardMouseHook.MouseDownEvent -= KeyboardMouseHook_MouseDownEvent;
                }
                OnPropertyChanged();
            }
        }

        private void KeyboardMouseHook_MouseDownEvent(object sender, EventArgs e)
        {
            IsKeyboardInputMode = false;
        }

        private void Textbox_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            (sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }

        public WaferDimensionalCharacteristic WaferDimentionalCharac
        {
            get { return (WaferDimensionalCharacteristic)GetValue(WaferDimentionalCharacProperty); }
            set { SetValue(WaferDimentionalCharacProperty, value); }
        }

        public static readonly DependencyProperty WaferDimentionalCharacProperty =
            DependencyProperty.Register(nameof(WaferDimentionalCharac), typeof(WaferDimensionalCharacteristic),
                typeof(StageMoveControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool IsQuickFocusAvailable
        {
            get { return (bool)GetValue(IsQuickFocusAvailableProperty); }
            set { SetValue(IsQuickFocusAvailableProperty, value); }
        }

        public static readonly DependencyProperty IsQuickFocusAvailableProperty =
            DependencyProperty.Register(nameof(IsQuickFocusAvailable), typeof(bool), typeof(StageMoveControl),
                new PropertyMetadata(true));

        public bool IsQuickFocusInProgress
        {
            get { return (bool)GetValue(IsQuickFocusInProgressProperty); }
            set { SetValue(IsQuickFocusInProgressProperty, value); }
        }

        public static readonly DependencyProperty IsQuickFocusInProgressProperty =
            DependencyProperty.Register(nameof(IsQuickFocusInProgress), typeof(bool), typeof(StageMoveControl),
                new PropertyMetadata(false));

        public bool IsAutofocusCameraAvailable
        {
            get { return (bool)GetValue(IsAutofocusCameraAvailableProperty); }
            set { SetValue(IsAutofocusCameraAvailableProperty, value); }
        }

        public static readonly DependencyProperty IsAutofocusCameraAvailableProperty =
            DependencyProperty.Register(nameof(IsAutofocusCameraAvailable), typeof(bool), typeof(StageMoveControl),
                new PropertyMetadata(false));

        public bool IsAutofocusInProgress
        {
            get { return (bool)GetValue(IsAutoFocusInProgressProperty); }
            set { SetValue(IsAutoFocusInProgressProperty, value); }
        }

        public static readonly DependencyProperty IsAutoFocusInProgressProperty =
            DependencyProperty.Register(nameof(IsAutofocusInProgress), typeof(bool), typeof(StageMoveControl),
                new PropertyMetadata(false));

        public static readonly DependencyProperty ScanRangesProperty =
            DependencyProperty.Register(nameof(ScanRanges), typeof(IEnumerable<ScanRangeType>),
                typeof(StageMoveControl),
                new PropertyMetadata(null));

        public IEnumerable<ScanRangeType> ScanRanges
        {
            get { return (IEnumerable<ScanRangeType>)GetValue(ScanRangesProperty); }
            set { SetValue(ScanRangesProperty, value); }
        }

        public static readonly DependencyProperty SelectedRangeProperty =
            DependencyProperty.Register(nameof(SelectedRange), typeof(ScanRangeType), typeof(StageMoveControl),
                new PropertyMetadata(ScanRangeType.Small));

        public ScanRangeType SelectedRange
        {
            get { return (ScanRangeType)GetValue(SelectedRangeProperty); }
            set { SetValue(SelectedRangeProperty, value); }
        }

        private void StartQuickFocusButton_Click(object sender, RoutedEventArgs e)
        {
            if (_algoSupervisor == null)
                _algoSupervisor = ClassLocator.Default.GetInstance<AlgoSupervisor>();
            if (_cameraBench == null)
                _cameraBench = ClassLocator.Default.GetInstance<CameraBench>();
            if (_filterWheelBench == null)
                _filterWheelBench = ClassLocator.Default.GetInstance<FilterWheelBench>();
            _algoSupervisor.GetZFocusChangedEvent += QuickFocusChangedEvent;
            StartAutoFocusButton.IsEnabled = false;
            StopAutoFocusButton.IsEnabled = false;
            StartQuickFocus();
        }

        private void StartQuickFocus()
        {
            if (_filterWheelBench.CurrentFilter == null)
            {
                Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                    ClassLocator.Default.GetInstance<IDialogOwnerService>()
                        .ShowMessageBox("Quick focus failed : Current filter could not be determined", "Quickfocus",
                            MessageBoxButton.OK, MessageBoxImage.Error)));
                QuickFocusTerminated();
                return;
            }

            double currentFilterFocusDistance = _filterWheelBench.CurrentFilter.DistanceOnFocus;
            var getZFocusInput = new GetZFocusInput { TargetDistanceSensor = currentFilterFocusDistance };
            IsQuickFocusInProgress = true;
            _algoSupervisor.StartGetZFocus(getZFocusInput);
        }

        private void QuickFocusChangedEvent(GetZFocusResult getZFocusResult)
        {
            if (!getZFocusResult.Status.IsFinished)
                return;

            if (getZFocusResult.Status.State != FlowState.Success)
            {
                Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                    ClassLocator.Default.GetInstance<IDialogOwnerService>()
                        .ShowMessageBox("Quick focus failed", "Quickfocus", MessageBoxButton.OK,
                            MessageBoxImage.Error)));
            }

            QuickFocusTerminated();
        }

        private void StopQuickFocusButton_Click(object sender, RoutedEventArgs e)
        {
            _algoSupervisor.CancelGetZFocus();
            QuickFocusTerminated();
        }

        private void QuickFocusTerminated()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                IsQuickFocusInProgress = false;
                StartAutoFocusButton.IsEnabled = true;
                StopAutoFocusButton.IsEnabled = true;
            }));
            _algoSupervisor.GetZFocusChangedEvent -= QuickFocusChangedEvent;
        }

        private void StartAutoFocusButton_Click(object sender, RoutedEventArgs e)
        {
            if (_algoSupervisor == null)
                _algoSupervisor = ClassLocator.Default.GetInstance<AlgoSupervisor>();
            if (_cameraBench == null)
                _cameraBench = ClassLocator.Default.GetInstance<CameraBench>();
            _algoSupervisor.AutoFocusCameraChangedEvent += AutoFocusChangedEvent;
            StartQuickFocusButton.IsEnabled = false;
            StopQuickFocusButton.IsEnabled = false;
            StartCameraAutofocus();
        }

        private void StartCameraAutofocus()
        {
            if (!_cameraBench.IsStreaming)
            {
                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Camera must be started",
                    "Camera", MessageBoxButton.OK, MessageBoxImage.Information);
                AutofocusTerminated();
                return;
            }

            var autoFocusInput = new AutoFocusCameraInput(SelectedRange);

            IsAutofocusInProgress = true;
            _algoSupervisor.StartAutoFocusCamera(autoFocusInput);
        }

        private void AutoFocusChangedEvent(AutoFocusCameraResult autoFocusResult)
        {
            if (!autoFocusResult.Status.IsFinished)
                return;

            if (autoFocusResult.Status.State != FlowState.Success)
            {
                Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                    ClassLocator.Default.GetInstance<IDialogOwnerService>()
                        .ShowMessageBox("Autofocus failed", "Autofocus", MessageBoxButton.OK, MessageBoxImage.Error)));
            }

            AutofocusTerminated();
        }

        private void StopAutoFocusButton_Click(object sender, RoutedEventArgs e)
        {
            _algoSupervisor.CancelAutoFocusCamera();
            AutofocusTerminated();
        }

        private void AutofocusTerminated()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                IsAutofocusInProgress = false;
                StartQuickFocusButton.IsEnabled = true;
                StopQuickFocusButton.IsEnabled = true;
            }));
            _algoSupervisor.AutoFocusCameraChangedEvent -= AutoFocusChangedEvent;
        }

        private void OnMoveClickDown(object sender, MouseButtonEventArgs e)
        {
            var moveType = ((StageMoveButton)sender).MoveType;
            OnKeyOrMouseDown(moveType);
        }

        private void OnMoveClickUp(object sender, MouseButtonEventArgs e)
        {
            OnKeyOrMouseUp();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
