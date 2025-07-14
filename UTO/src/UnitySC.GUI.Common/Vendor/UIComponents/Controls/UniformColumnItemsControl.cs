using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using Agileo.GUI.Commands;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    public class UniformColumnItemsControl : ListView
    {
        private double GetElementWidth(bool navigationIsVisible)
        {
            if (_scrollViewer == null)
            {
                return double.NaN;
            }

            if (_scrollViewer.ActualWidth == 0)
            {
                return 0;
            }

            var separationSize = ColumnsSeparation * (navigationIsVisible ? Columns : Columns - 1);
            return (_scrollViewer.ActualWidth - separationSize) / Columns;
        }

        private double GetElementHeight() => _scrollViewer?.ActualHeight ?? double.NaN;

        static UniformColumnItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(UniformColumnItemsControl),
                new FrameworkPropertyMetadata(typeof(UniformColumnItemsControl)));
        }

        private ScrollViewer _scrollViewer;

        #region Overrides of FrameworkElement

        public UniformColumnItemsControl()
        {
            ScrollToRightCommand = new DelegateCommand(ScrollToRightExecute, ScrollToRightCanExecute);
            ScrollToLeftCommand = new DelegateCommand(ScrollToLeftExecute, ScrollToLeftCanExecute);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _scrollViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
            ItemContainerGenerator.StatusChanged += OnItemContainerGeneratorStatusChanged;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            OnItemContainerGeneratorStatusChanged();
        }

        private void OnItemContainerGeneratorStatusChanged(object sender = null, EventArgs e = null)
        {
            if (ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
            {
                return;
            }

            var isNavigationVisible = Items.Count > Columns;
            NavigationVisibility = isNavigationVisible ? Visibility.Visible : Visibility.Collapsed;

            var itemWidth = GetElementWidth(isNavigationVisible);
            var itemHeight = GetElementHeight();

            for (var i = 0; i < Items.Count; i++)
            {
                if (ItemContainerGenerator.ContainerFromIndex(i) is FrameworkElement element)
                {
                    var leftMargin = !isNavigationVisible && i == 0 ? 0 : ColumnsSeparation;
                    element.Margin = new Thickness(leftMargin, 0, 0, 0);
                    element.Width = itemWidth;
                    element.Height = itemHeight;
                }
            }
        }

        #endregion

        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register(
            nameof(Columns),
            typeof(int),
            typeof(UniformColumnItemsControl),
            new PropertyMetadata(default(int), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UniformColumnItemsControl self)
            {
                self.OnItemContainerGeneratorStatusChanged();
            }
        }

        public int Columns
        {
            get => (int)GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        public static readonly DependencyProperty ColumnsSeparationProperty = DependencyProperty.Register(
            nameof(ColumnsSeparation),
            typeof(double),
            typeof(UniformColumnItemsControl),
            new PropertyMetadata(default(double)));

        public double ColumnsSeparation
        {
            get => (double)GetValue(ColumnsSeparationProperty);
            set => SetValue(ColumnsSeparationProperty, value);
        }

        #region Commands

        public static readonly DependencyPropertyKey ScrollToLeftCommandPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(ScrollToLeftCommand),
                typeof(ICommand),
                typeof(UniformColumnItemsControl),
                new FrameworkPropertyMetadata(default(ICommand), FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty ScrollToLeftCommandProperty =
            ScrollToLeftCommandPropertyKey.DependencyProperty;

        public ICommand ScrollToLeftCommand
        {
            get => (ICommand)GetValue(ScrollToLeftCommandProperty);
            protected set => SetValue(ScrollToLeftCommandPropertyKey, value);
        }

        public static readonly DependencyPropertyKey ScrollToRightCommandPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(ScrollToRightCommand),
                typeof(ICommand),
                typeof(UniformColumnItemsControl),
                new FrameworkPropertyMetadata(default(ICommand), FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty ScrollToRightCommandProperty =
            ScrollToRightCommandPropertyKey.DependencyProperty;

        public ICommand ScrollToRightCommand
        {
            get => (ICommand)GetValue(ScrollToRightCommandProperty);
            protected set => SetValue(ScrollToRightCommandPropertyKey, value);
        }

        private bool ScrollToLeftCanExecute()
        {
            if (_scrollViewer == null)
            {
                return false;
            }

            return _scrollViewer.HorizontalOffset != 0;
        }

        private bool ScrollToRightCanExecute()
        {
            if (_scrollViewer == null)
            {
                return false;
            }

            return Math.Abs(_scrollViewer.HorizontalOffset - _scrollViewer.ScrollableWidth) > 0.01;
        }

        private void ScrollToRightExecute() => _scrollViewer.LineRight();

        private void ScrollToLeftExecute() => _scrollViewer.LineLeft();

        #endregion

        public static readonly DependencyPropertyKey NavigationVisibilityPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(NavigationVisibility),
                typeof(Visibility),
                typeof(UniformColumnItemsControl),
                new FrameworkPropertyMetadata(Visibility.Collapsed, FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty NavigationVisibilityProperty =
            NavigationVisibilityPropertyKey.DependencyProperty;

        public Visibility NavigationVisibility
        {
            get => (Visibility)GetValue(NavigationVisibilityProperty);
            protected set => SetValue(NavigationVisibilityPropertyKey, value);
        }

        public static readonly DependencyProperty IsSelectionEnabledProperty = DependencyProperty.Register(
            nameof(IsSelectionEnabled),
            typeof(bool),
            typeof(UniformColumnItemsControl),
            new PropertyMetadata(default(bool)));

        public bool IsSelectionEnabled
        {
            get { return (bool)GetValue(IsSelectionEnabledProperty); }
            set { SetValue(IsSelectionEnabledProperty, value); }
        }
    }
}
