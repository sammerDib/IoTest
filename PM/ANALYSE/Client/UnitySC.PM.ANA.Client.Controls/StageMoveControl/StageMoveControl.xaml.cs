using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;

using static UnitySC.PM.ANA.Service.Interface.ObjectiveConfig;

namespace UnitySC.PM.ANA.Client.Controls.StageMoveControl
{
    /// <summary>
    /// Interaction logic for StageMoveControl.xaml
    /// </summary>
    public partial class StageMoveControl : UserControl
    {
        public StageMoveControl()
        {
            InitializeComponent();
            DataContext = this;
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }
            SelectedReference = ServiceLocator.ChuckSupervisor.ChuckVM?.AnaChuckConfiguration?.ReferencesList?.FirstOrDefault();
        }

        private void Textbox_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            (sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }

        private void ListBoxRefs_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (RefsList.SelectedItem != null)
            {
                PopupReferences.IsOpen = false;
            }
        }

        private void ListBoxPos_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (RefsList.SelectedItem != null)
            {
                PopupPositions.IsOpen = false;
            }
        }

        private bool _isLiseAtofocusInProgress;

        public OpticalReferenceDefinition SelectedReference
        {
            get { return (OpticalReferenceDefinition)GetValue(SelectedReferenceProperty); }
            set { SetValue(SelectedReferenceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedReference. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedReferenceProperty = DependencyProperty.Register(
            nameof(SelectedReference),
            typeof(OpticalReferenceDefinition),
            typeof(StageMoveControl),
            new PropertyMetadata(null, OnSelectedReferenceChanged));

        private static void OnSelectedReferenceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // We don't move if it is the first set
            if (!(e.OldValue is null) && (e.NewValue is OpticalReferenceDefinition selectedOpicalRef))
            {
                ServiceLocator.AxesSupervisor.AxesVM.GotoRefPos.Execute(selectedOpicalRef);
            }
        }

        public SpecificPositions DefaultSpecificPosition
        {
            get { return (SpecificPositions)GetValue(DefaultSpecificPositionProperty); }
            set { SetValue(DefaultSpecificPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefaultSpecificPosition. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultSpecificPositionProperty = DependencyProperty.Register(
            nameof(DefaultSpecificPosition),
            typeof(SpecificPositions),
            typeof(StageMoveControl));

        public List<SpecificPositions> AvailablePositions
        {
            get { return (List<SpecificPositions>)GetValue(AvailablePositionsProperty); }
            set { SetValue(AvailablePositionsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AvailablePositions. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AvailablePositionsProperty = DependencyProperty.Register(
            nameof(AvailablePositions),
            typeof(List<SpecificPositions>),
            typeof(StageMoveControl),
            new PropertyMetadata(new List<SpecificPositions>(), OnAvailablePositionsIdChanged));

        private static void OnAvailablePositionsIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StageMoveControl).SetValue(SelectedPositionProperty, (d as StageMoveControl).DefaultSpecificPosition);
        }

        public SpecificPositions SelectedPosition
        {
            get { return (SpecificPositions)GetValue(SelectedPositionProperty); }
            set { SetValue(SelectedPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedPosition. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedPositionProperty = DependencyProperty.Register(
            nameof(SelectedPosition),
            typeof(SpecificPositions),
            typeof(StageMoveControl),
            new PropertyMetadata(SpecificPositions.PositionChuckCenter, OnSelectedPositionChanged));

        private static void OnSelectedPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // We don't move if it is the first set or during control load
            if (!(e.OldValue is null) && (e.NewValue is SpecificPositions selectedPositionRef) && (e.OldValue != e.NewValue))
            {
                var control = d as StageMoveControl;
                if (control.IsLoaded && control.AvailablePositions != null && control.AvailablePositions.Count > 0)
                {
                    ServiceLocator.AxesSupervisor.AxesVM.GotoSpecificPosition.Execute(selectedPositionRef);
                }
            }
        }

        public WaferDimensionalCharacteristic WaferDimentionalCharac
        {
            get { return (WaferDimensionalCharacteristic)GetValue(WaferDimentionalCharacProperty); }
            set { SetValue(WaferDimentionalCharacProperty, value); }
        }

        public static readonly DependencyProperty WaferDimentionalCharacProperty =
            DependencyProperty.Register(nameof(WaferDimentionalCharac), typeof(WaferDimensionalCharacteristic), typeof(StageMoveControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public DieDimensionalCharacteristic DieDimentionalCharac
        {
            get { return (DieDimensionalCharacteristic)GetValue(DieDimentionalCharacProperty); }
            set { SetValue(DieDimentionalCharacProperty, value); }
        }

        public static readonly DependencyProperty DieDimentionalCharacProperty =
            DependencyProperty.Register(nameof(DieDimentionalCharac), typeof(DieDimensionalCharacteristic), typeof(StageMoveControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public WaferMapResult WaferMap
        {
            get { return (WaferMapResult)GetValue(WaferMapProperty); }
            set { SetValue(WaferMapProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WaferMap.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WaferMapProperty =
            DependencyProperty.Register(nameof(WaferMap), typeof(WaferMapResult), typeof(StageMoveControl), new PropertyMetadata(null));

        public bool IsAutofocusAvailable
        {
            get { return (bool)GetValue(IsAutofocusAvailableProperty); }
            set { SetValue(IsAutofocusAvailableProperty, value); }
        }

        public static readonly DependencyProperty IsAutofocusAvailableProperty =
            DependencyProperty.Register(nameof(IsAutofocusAvailable), typeof(bool), typeof(StageMoveControl), new PropertyMetadata(true));

        public bool IsDieNavigation
        {
            get { return (bool)GetValue(IsDieNavigationProperty); }
            set { SetValue(IsDieNavigationProperty, value); }
        }

        public static readonly DependencyProperty IsDieNavigationProperty =
            DependencyProperty.Register(nameof(IsDieNavigation), typeof(bool), typeof(StageMoveControl), new PropertyMetadata(false));

        public List<Point> MeasurePoints
        {
            get { return (List<Point>)GetValue(MeasurePointsProperty); }
            set { SetValue(MeasurePointsProperty, value); }
        }

        public static readonly DependencyProperty MeasurePointsProperty =
            DependencyProperty.Register(nameof(MeasurePoints), typeof(List<Point>), typeof(StageMoveControl), new FrameworkPropertyMetadata(null));

        public List<Point> MeasurePointsOnDie
        {
            get { return (List<Point>)GetValue(MeasurePointsOnDieProperty); }
            set { SetValue(MeasurePointsOnDieProperty, value); }
        }

        public static readonly DependencyProperty MeasurePointsOnDieProperty =
            DependencyProperty.Register(nameof(MeasurePointsOnDie), typeof(List<Point>), typeof(StageMoveControl), new FrameworkPropertyMetadata(null));

        public Brush MeasurePointsBrush
        {
            get { return (Brush)GetValue(MeasurePointsBrushProperty); }
            set { SetValue(MeasurePointsBrushProperty, value); }
        }

        public static readonly DependencyProperty MeasurePointsBrushProperty =
            DependencyProperty.Register(nameof(MeasurePointsBrush), typeof(Brush), typeof(StageMoveControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.DarkRed), FrameworkPropertyMetadataOptions.AffectsRender));

        public bool CanMoveStage
        {
            get { return (bool)GetValue(CanMoveStageProperty); }
            set { SetValue(CanMoveStageProperty, value); }
        }

        public static readonly DependencyProperty CanMoveStageProperty =
            DependencyProperty.Register(nameof(CanMoveStage), typeof(bool), typeof(StageMoveControl), new PropertyMetadata(true));

        public bool DisplayClampStatus
        {
            get { return (bool)GetValue(DisplayClampStatusProperty); }
            set { SetValue(DisplayClampStatusProperty, value); }
        }

        public static readonly DependencyProperty DisplayClampStatusProperty =
            DependencyProperty.Register(nameof(DisplayClampStatus), typeof(bool), typeof(StageMoveControl), new PropertyMetadata(false));

        public bool CanControlClamp
        {
            get { return (bool)GetValue(CanControlClampProperty); }
            set { SetValue(CanControlClampProperty, value); }
        }

        public static readonly DependencyProperty CanControlClampProperty =
            DependencyProperty.Register(nameof(CanControlClamp), typeof(bool), typeof(StageMoveControl), new PropertyMetadata(false));

        public bool IsAutofocusInProgress
        {
            get { return (bool)GetValue(IsAutoFocusInProgressProperty); }
            set { SetValue(IsAutoFocusInProgressProperty, value); }
        }

        public static readonly DependencyProperty IsAutoFocusInProgressProperty =
            DependencyProperty.Register(nameof(IsAutofocusInProgress), typeof(bool), typeof(StageMoveControl), new PropertyMetadata(false));

        private void ButtonSelectCurrentDie_Click(object sender, RoutedEventArgs e)
        {
            var dieSelectionWindow = new DieSelectionWindow(WaferMap, WaferDimentionalCharac);
            dieSelectionWindow.SelectedDie = ServiceLocator.AxesSupervisor.AxesVM.CurrentDieIndex;

            if (dieSelectionWindow.ShowDialog() == true)
            {

                ServiceLocator.AxesSupervisor.AxesVM.MoveToDiePosition(new DieIndex(dieSelectionWindow.SelectedDieColumn, dieSelectionWindow.SelectedDieRow), ServiceLocator.AxesSupervisor.AxesVM.PositionOnDie.X, ServiceLocator.AxesSupervisor.AxesVM.PositionOnDie.Y);
            }
        }

        private void StartAutoFocusButton_Click(object sender, RoutedEventArgs e)
        {
            ServiceLocator.AlgosSupervisor.AutoFocusChangedEvent += AutoFocusChangedEvent;

            ServiceLocator.AxesSupervisor.AxesVM.IsLocked = true;
            ServiceLocator.LightsSupervisor.LightsAreLocked = true;

            var currentObjective = ServiceLocator.CamerasSupervisor.Objective;
            if (currentObjective.ObjType == ObjectiveType.INT || currentObjective.ObjType == ObjectiveType.VIS)
            {
                StartCameraAutofocus();
            }
            else
            {
                StartLiseAutofocus();
            }
        }

        private void StartLiseAutofocus()
        {
            AutoFocusSettings settings = new AutoFocusSettings()
            {
                Type = AutoFocusType.Lise,
                LiseGain = 1.8,
                ProbeId = ServiceLocator.ProbesSupervisor.ProbeLiseUp.DeviceID,
                LiseAutoFocusContext = new TopObjectiveContext(ServiceLocator.CamerasSupervisor.Objective.DeviceID)
            };
            XYPosition currentPosition = ServiceLocator.AxesSupervisor.GetCurrentPosition()?.Result.ToXYPosition();
            XYPositionContext context = new XYPositionContext(currentPosition);

            AutofocusInput autoFocusInput = new AutofocusInput(settings, context);
            IsAutofocusInProgress = true;
            _isLiseAtofocusInProgress = true;
            ServiceLocator.AlgosSupervisor.StartAutoFocus(autoFocusInput);
        }

        private void StartCameraAutofocus()
        {
            AutoFocusSettings settings = new AutoFocusSettings()
            {
                Type = AutoFocusType.Camera,

                CameraId = ServiceLocator.CamerasSupervisor.GetMainCamera().Configuration.DeviceID,
                CameraScanRange = ScanRangeType.Medium,
                ImageAutoFocusContext = ServiceLocator.ContextSupervisor.GetTopImageAcquisitionContext()?.Result
            };
            XYPosition currentPosition = ServiceLocator.AxesSupervisor.GetCurrentPosition()?.Result.ToXYPosition();
            XYPositionContext context = new XYPositionContext(currentPosition);

            AutofocusInput autoFocusInput = new AutofocusInput(settings, context);
            _isLiseAtofocusInProgress = false;
            IsAutofocusInProgress = true;
            ServiceLocator.AlgosSupervisor.StartAutoFocus(autoFocusInput);
        }

        private void AutoFocusChangedEvent(AutofocusResult autoFocusResult)
        {
            if (autoFocusResult.Status.IsFinished)
            {
                if (autoFocusResult.Status.State == FlowState.Success)
                {
                    AutofocusTerminated();
                }
                else
                {
                    if (_isLiseAtofocusInProgress)
                    {
                        // if the lise autofocus failed we execute a camera autofocus
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            StartCameraAutofocus();
                        }));
                    }
                    else
                    {
                        MessageBox.Show("Autofocus failed", "Autofocus", MessageBoxButton.OK, MessageBoxImage.Error);
                        AutofocusTerminated();
                    }
                }
            }
        }

        private void AutofocusTerminated()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                ServiceLocator.AxesSupervisor.AxesVM.IsLocked = false;
                ServiceLocator.LightsSupervisor.LightsAreLocked = false;
                _isLiseAtofocusInProgress = false;
                IsAutofocusInProgress = false;
            }));
            ServiceLocator.AlgosSupervisor.AutoFocusChangedEvent -= AutoFocusChangedEvent;
        }

        private void StopAutoFocusButton_Click(object sender, RoutedEventArgs e)
        {
            ServiceLocator.AlgosSupervisor.CancelAutoFocus();
            AutofocusTerminated();
        }
    }
}
