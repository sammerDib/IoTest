using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.Efem.Enums;
using UnitySC.GUI.Common.UIComponents.Components.Equipment.Modules.Aligner;
using UnitySC.GUI.Common.UIComponents.Components.Equipment.Modules.LoadPort;
using UnitySC.GUI.Common.UIComponents.Components.Equipment.Modules.ProcessModule;
using UnitySC.GUI.Common.UIComponents.Components.Equipment.Modules.Robot;

namespace UnitySC.GUI.Common.UIComponents.Components.Equipment
{
    public partial class MachineView
    {
        #region Constructor

        public MachineView()
        {
            InitializeComponent();
            this.Loaded += MachineView_Loaded;
        }

        #endregion

        #region Dependency properties

        public static readonly DependencyProperty ModulesProperty = DependencyProperty.Register(
            nameof(Modules),
            typeof(IEnumerable<SelectableMachineModuleViewModel>),
            typeof(MachineView),
            new PropertyMetadata(default(IEnumerable<SelectableMachineModuleViewModel>), ModulesChangedCallback));

        private static void ModulesChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MachineView self)
            {
                self.InitializeCollections();
            }
        }

        public IEnumerable<SelectableMachineModuleViewModel> Modules
        {
            get { return (IEnumerable<SelectableMachineModuleViewModel>)GetValue(ModulesProperty); }
            set { SetValue(ModulesProperty, value); }
        }

        public static readonly DependencyPropertyKey TopModulesPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(TopModules),
                typeof(IEnumerable<SelectableMachineModuleViewModel>),
                typeof(MachineView),
                new FrameworkPropertyMetadata(default(IEnumerable<SelectableMachineModuleViewModel>), FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty TopModulesProperty = TopModulesPropertyKey.DependencyProperty;

        public IEnumerable<SelectableMachineModuleViewModel> TopModules
        {
            get { return (IEnumerable<SelectableMachineModuleViewModel>)GetValue(TopModulesProperty); }
            protected set { SetValue(TopModulesPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey LeftModulesPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(LeftModules),
                typeof(IEnumerable<SelectableMachineModuleViewModel>),
                typeof(MachineView),
                new FrameworkPropertyMetadata(default(IEnumerable<SelectableMachineModuleViewModel>), FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty LeftModulesProperty = LeftModulesPropertyKey.DependencyProperty;

        public IEnumerable<SelectableMachineModuleViewModel> LeftModules
        {
            get { return (IEnumerable<SelectableMachineModuleViewModel>)GetValue(LeftModulesProperty); }
            protected set { SetValue(LeftModulesPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey RightModulesPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(RightModules),
                typeof(IEnumerable<SelectableMachineModuleViewModel>),
                typeof(MachineView),
                new FrameworkPropertyMetadata(default(IEnumerable<SelectableMachineModuleViewModel>), FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty RightModulesProperty = RightModulesPropertyKey.DependencyProperty;

        public IEnumerable<SelectableMachineModuleViewModel> RightModules
        {
            get { return (IEnumerable<SelectableMachineModuleViewModel>)GetValue(RightModulesProperty); }
            protected set { SetValue(RightModulesPropertyKey, value); }
        }

        public static readonly DependencyPropertyKey BottomModulesPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(BottomModules),
                typeof(IEnumerable<SelectableMachineModuleViewModel>),
                typeof(MachineView),
                new FrameworkPropertyMetadata(default(IEnumerable<SelectableMachineModuleViewModel>), FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty BottomModulesProperty = BottomModulesPropertyKey.DependencyProperty;

        public IEnumerable<SelectableMachineModuleViewModel> BottomModules
        {
            get { return (IEnumerable<SelectableMachineModuleViewModel>)GetValue(BottomModulesProperty); }
            protected set { SetValue(BottomModulesPropertyKey, value); }
        }

        [Category("Main")]
        public static readonly DependencyProperty AreLocationsSelectableProperty = DependencyProperty.Register(
            nameof(AreLocationsSelectable),
            typeof(bool),
            typeof(MachineView),
            new PropertyMetadata(default(bool)));

        public bool AreLocationsSelectable
        {
            get => (bool)GetValue(AreLocationsSelectableProperty);
            set => SetValue(AreLocationsSelectableProperty, value);
        }

        public static readonly DependencyProperty InvertProcessModulesProperty = DependencyProperty.Register(
            nameof(InvertProcessModules),
            typeof(bool),
            typeof(MachineView),
            new PropertyMetadata(default(bool), InvertProcessModuleChangedCallback));

        private static void InvertProcessModuleChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MachineView self)
            {
                self.InitializeCollections();
            }
        }

        public bool InvertProcessModules
        {
            get => (bool)GetValue(InvertProcessModulesProperty);
            set => SetValue(InvertProcessModulesProperty, value);
        }

        #endregion

        #region Robot animation

        private void SetRobotHorizontalOffset(double offset)
        {
            if (double.IsNaN(offset))
            {
                return;
            }
            Canvas.SetLeft(RobotModule, offset);
        }

        public static readonly DependencyProperty RobotPositionProperty = DependencyProperty.Register(
            nameof(RobotPosition),
            typeof(TransferLocation?),
            typeof(MachineView),
            new PropertyMetadata(null, OnRobotPositionChangedCallback));

        public TransferLocation? RobotPosition
        {
            get { return (TransferLocation?)GetValue(RobotPositionProperty); }
            set { SetValue(RobotPositionProperty, value); }
        }

        private static void OnRobotPositionChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MachineView machine)
            {
                machine.ApplyRobotPosition();
            }
        }

        private void ApplyRobotPosition()
        {
            var targetElement = GetFrameworkElementFromCurrentRobotPosition();
            var targetRect = GetRectPosition(targetElement);
            var orientation = GetRobotOrientation(targetRect);
            var angle = GetAngle(orientation);
            var offset = GetOffset(targetRect, orientation);

            if (!double.IsNaN(angle))
            {
                RobotModule.SetAngle(angle);
            }

            if (!double.IsNaN(offset))
            {
                SetRobotHorizontalOffset(offset);
            }
        }

        private double GetOffset(Rect? targetRect, RobotOrientation orientation)
        {
            // Represents the margin between the robot and the left or right element.
            const double horizontalMargin = 25;

            var robotCenterOffset = RobotModule.ActualWidth / 2;

            if (targetRect == null)
            {
                // Set Robot at center of Canvas
                return (Canvas.ActualWidth / 2) - robotCenterOffset;
            }

            var rect = targetRect.Value;

            switch (orientation)
            {
                case RobotOrientation.Up or RobotOrientation.Down:
                    {
                        var targetVerticalCenter = (rect.Left + rect.Right) / 2;
                        return targetVerticalCenter - robotCenterOffset;
                    }
                case RobotOrientation.Left:
                    {
                        return rect.Right + horizontalMargin;
                    }
                case RobotOrientation.Right:
                    {
                        return rect.Left - horizontalMargin - (RobotModule.ActualHeight * 2);
                    }
                default:
                    {
                        return rect.Left - RobotModule.ActualWidth - horizontalMargin;
                    }
            }
        }

        private SelectableMachineModuleViewModel GetModuleFromCurrentRobotPosition()
        {
            if (Modules == null) return null;
            return RobotPosition switch
            {
                TransferLocation.LoadPort1 => Modules.FirstOrDefault(m => m is LoadPortModuleViewModel && m.Index == 1),
                TransferLocation.LoadPort2 => Modules.FirstOrDefault(m => m is LoadPortModuleViewModel && m.Index == 2),
                TransferLocation.LoadPort3 => Modules.FirstOrDefault(m => m is LoadPortModuleViewModel && m.Index == 3),
                TransferLocation.LoadPort4 => Modules.FirstOrDefault(m => m is LoadPortModuleViewModel && m.Index == 4),
                TransferLocation.LoadPort5 => Modules.FirstOrDefault(m => m is LoadPortModuleViewModel && m.Index == 5),
                TransferLocation.LoadPort6 => Modules.FirstOrDefault(m => m is LoadPortModuleViewModel && m.Index == 6),
                TransferLocation.LoadPort7 => Modules.FirstOrDefault(m => m is LoadPortModuleViewModel && m.Index == 7),
                TransferLocation.LoadPort8 => Modules.FirstOrDefault(m => m is LoadPortModuleViewModel && m.Index == 8),
                TransferLocation.LoadPort9 => Modules.FirstOrDefault(m => m is LoadPortModuleViewModel && m.Index == 9),
                TransferLocation.LoadLockA => null,
                TransferLocation.LoadLockB => null,
                TransferLocation.LoadLockC => null,
                TransferLocation.LoadLockD => null,
                TransferLocation.PreAlignerA => Modules.FirstOrDefault(m => m is AlignerModuleViewModel && m.Index == 0),
                TransferLocation.PreAlignerB => Modules.FirstOrDefault(m => m is AlignerModuleViewModel && m.Index == 1),
                TransferLocation.PreAlignerC => Modules.FirstOrDefault(m => m is AlignerModuleViewModel && m.Index == 2),
                TransferLocation.PreAlignerD => Modules.FirstOrDefault(m => m is AlignerModuleViewModel && m.Index == 3),
                TransferLocation.ProcessModuleA => Modules.FirstOrDefault(m => m is ProcessModuleViewModel && m.Index == 1),
                TransferLocation.ProcessModuleB => Modules.FirstOrDefault(m => m is ProcessModuleViewModel && m.Index == 2),
                TransferLocation.ProcessModuleC => Modules.FirstOrDefault(m => m is ProcessModuleViewModel && m.Index == 3),
                TransferLocation.ProcessModuleD => Modules.FirstOrDefault(m => m is ProcessModuleViewModel && m.Index == 4),
                TransferLocation.DummyPortA => null,
                TransferLocation.DummyPortB => null,
                TransferLocation.DummyPortC => null,
                TransferLocation.DummyPortD => null,
                TransferLocation.Robot => null,
                null => null,
                _ => null
            };
        }

        /// <summary>
        /// Obtains the graphic element that the robot should stand in front of.
        /// </summary>
        private FrameworkElement GetFrameworkElementFromCurrentRobotPosition()
        {
            var module = GetModuleFromCurrentRobotPosition();
            if (module == null)
            {
                return null;
            }

            if (TopModules.Contains(module))
            {
               return TopModulesItemsControl.ItemContainerGenerator.ContainerFromItem(module) as FrameworkElement;
            }

            if (BottomModules.Contains(module))
            {
                return BottomModulesItemsControl.ItemContainerGenerator.ContainerFromItem(module) as FrameworkElement;
            }

            if (RightModules.Contains(module))
            {
                return RightModulesItemsControl.ItemContainerGenerator.ContainerFromItem(module) as FrameworkElement;
            }

            if (LeftModules.Contains(module))
            {
                return LeftModulesItemsControl.ItemContainerGenerator.ContainerFromItem(module) as FrameworkElement;
            }

            return null;
        }

        private Rect? GetRectPosition(FrameworkElement frameworkElement)
        {
            if (frameworkElement == null)
            {
                return null;
            }

            var container = Canvas;
            Point relativeLocation = frameworkElement.TranslatePoint(new Point(0, 0), container);
            return new Rect(
                relativeLocation,
                new Size(frameworkElement.ActualWidth, frameworkElement.ActualHeight));
        }

        private RobotOrientation GetRobotOrientation(Rect? rectPosition)
        {
            const double minVerticalDelta = 100;

            if (rectPosition == null)
            {
                // Default robot orientation
                return RobotOrientation.Right;
            }

            var canvasCenter = new Point(Canvas.ActualWidth / 2, Canvas.ActualHeight / 2);
            var rectPositionCenter = GetCenter(rectPosition.Value);

            var yDelta = canvasCenter.Y - rectPositionCenter.Y;

            if (RobotModule.DataContext is RobotModuleViewModel robotModuleViewModel)
            {
                var orientation = robotModuleViewModel.GetOrientation();
                if (orientation.HasValue)
                {
                    return orientation.Value;
                }
            }

            // The target element is sufficiently above or below the center of the canvas.
            if (Math.Abs(yDelta) > minVerticalDelta)
            {
                return yDelta > 0 ? RobotOrientation.Up : RobotOrientation.Down;
            }

            var xDelta = canvasCenter.X - rectPositionCenter.X;
            return xDelta > 0 ? RobotOrientation.Left : RobotOrientation.Right;
        }

        private static Point GetCenter(Rect rect)
        {
            return new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
        }

        private static double GetAngle(RobotOrientation orientation)
        {
            return orientation switch
            {
                RobotOrientation.Up => 0,
                RobotOrientation.Right => 90,
                RobotOrientation.Down => 180,
                RobotOrientation.Left => -90,
                _ => 90
            };
        }

        #endregion

        private void InitializeCollections()
        {
            TopModules = InvertProcessModules
                ? Modules?.Where(p => p.Position == DevicePosition.Top).OrderByDescending(p=>p.Order)
                : Modules?.Where(p => p.Position == DevicePosition.Top).OrderBy(p => p.Order);

            LeftModules = Modules?.Where(p => p.Position == DevicePosition.Left).OrderBy(p => p.Order);
            RightModules = Modules?.Where(p => p.Position == DevicePosition.Right).OrderBy(p => p.Order);
            BottomModules = Modules?.Where(p => p.Position == DevicePosition.Bottom).OrderBy(p => p.Order);

            ApplyRobotPosition();
        }

        private void MachineView_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyRobotPosition();
        }
    }
}
