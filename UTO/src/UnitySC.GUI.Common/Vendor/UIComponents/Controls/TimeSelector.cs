using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    public class TimeSelector : Control
    {
        static TimeSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeSelector), new FrameworkPropertyMetadata(typeof(TimeSelector)));
        }

        public TimeSelector()
        {
            var date = DateTime.Now;
            SelectedHour = date.Hour;
            SelectedMinute = date.Minute;
            SelectedSecond = date.Second;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            CreateButtons();
        }

        #region Events

        public event MouseButtonEventHandler SecondButtonMouseUp;

        internal void OnSecondButtonMouseUp(MouseButtonEventArgs e)
        {
            SecondButtonMouseUp?.Invoke(this, e);
        }

        public event MouseButtonEventHandler MinuteButtonMouseUp;

        internal void OnMinuteButtonMouseUp(MouseButtonEventArgs e)
        {
            MinuteButtonMouseUp?.Invoke(this, e);
        }

        public event EventHandler SelectedHourChanged;

        internal void OnSelectedHourChanged()
        {
            SelectedHourChanged?.Invoke(this, new EventArgs());
        }

        public event EventHandler SelectedMinuteChanged;

        internal void OnSelectedMinuteChanged()
        {
            SelectedMinuteChanged?.Invoke(this, new EventArgs());
        }

        public event EventHandler SelectedSecondChanged;

        internal void OnSelectedSecondChanged()
        {
            SelectedSecondChanged?.Invoke(this, new EventArgs());
        }

        #endregion

        public static readonly DependencyProperty SelectedHourProperty = DependencyProperty.Register(
            "SelectedHour", typeof(int), typeof(TimeSelector), new PropertyMetadata(default(int), SelectedHourChangedCallback));

        private static void SelectedHourChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timeSelector = (TimeSelector)d;
            timeSelector.InitializeDefaultValues();
            timeSelector.OnSelectedHourChanged();
        }

        public int SelectedHour
        {
            get { return (int)GetValue(SelectedHourProperty); }
            set { SetValue(SelectedHourProperty, value); }
        }

        public static readonly DependencyProperty SelectedMinuteProperty = DependencyProperty.Register(
            "SelectedMinute", typeof(int), typeof(TimeSelector), new PropertyMetadata(default(int), SelectedMinuteChangedCallback));

        private static void SelectedMinuteChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timeSelector = (TimeSelector)d;
            timeSelector.InitializeDefaultValues();
            timeSelector.OnSelectedMinuteChanged();
        }

        public int SelectedMinute
        {
            get { return (int)GetValue(SelectedMinuteProperty); }
            set { SetValue(SelectedMinuteProperty, value); }
        }

        public static readonly DependencyProperty SelectedSecondProperty = DependencyProperty.Register(
            "SelectedSecond", typeof(int), typeof(TimeSelector), new PropertyMetadata(default(int), SelectedSecondChangedCallback));

        private static void SelectedSecondChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timeSelector = (TimeSelector)d;
            timeSelector.InitializeDefaultValues();
            timeSelector.OnSelectedSecondChanged();
        }

        public int SelectedSecond
        {
            get { return (int)GetValue(SelectedSecondProperty); }
            set { SetValue(SelectedSecondProperty, value); }
        }

        public static readonly DependencyProperty PossibleHoursProperty = DependencyProperty.Register(
            "PossibleHours", typeof(IList), typeof(TimeSelector), new PropertyMetadata(default(IList)));

        public IList PossibleHours
        {
            get { return (IList)GetValue(PossibleHoursProperty); }
            set { SetValue(PossibleHoursProperty, value); }
        }

        public static readonly DependencyProperty PossibleMinutesProperty = DependencyProperty.Register(
            "PossibleMinutes", typeof(IList), typeof(TimeSelector), new PropertyMetadata(default(IList)));

        public IList PossibleMinutes
        {
            get { return (IList)GetValue(PossibleMinutesProperty); }
            set { SetValue(PossibleMinutesProperty, value); }
        }

        public static readonly DependencyProperty PossibleSecondsProperty = DependencyProperty.Register(
            "PossibleSeconds", typeof(IList), typeof(TimeSelector), new PropertyMetadata(default(IList)));

        public IList PossibleSeconds
        {
            get { return (IList)GetValue(PossibleSecondsProperty); }
            set { SetValue(PossibleSecondsProperty, value); }
        }

        private void CreateButtons()
        {
            var possibleHours = new List<TimeSelectorButton>();
            for (var hour = 0; hour < 24; ++hour)
            {
                var button = new TimeSelectorButton
                {
                    Content = hour
                };
                button.Click += HourButtonOnClick;
                possibleHours.Add(button);
            }
            PossibleHours = possibleHours;

            var possibleMinutes = new List<TimeSelectorButton>();
            for (var minute = 0; minute < 60; minute += 5)
            {
                var button = new TimeSelectorButton
                {
                    Content = minute
                };
                button.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(MinuteButtonUp), true);
                button.Click += MinuteButtonOnClick;
                possibleMinutes.Add(button);
            }
            PossibleMinutes = possibleMinutes;

            var possibleSeconds = new List<TimeSelectorButton>();
            for (var second = 0; second < 60; second += 5)
            {
                var button = new TimeSelectorButton
                {
                    Content = second
                };
                button.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(SecondButtonUp), true);
                button.Click += SecondButtonOnClick;
                possibleSeconds.Add(button);
            }
            PossibleSeconds = possibleSeconds;

            InitializeDefaultValues();
        }

        private void InitializeDefaultValues()
        {
            if (PossibleMinutes == null || PossibleHours == null) return;

            DeselectAllHours();
            DeselectAllMinutes();
            DeselectAllSeconds();

            var currentHourButton = PossibleHours.OfType<TimeSelectorButton>().SingleOrDefault(b => (int)b.Content == SelectedHour);
            if (currentHourButton != null) currentHourButton.IsSelected = true;

            var currentMinuteButton = PossibleMinutes.OfType<TimeSelectorButton>().SingleOrDefault(b => (int)b.Content == SelectedMinute);
            if (currentMinuteButton != null) currentMinuteButton.IsSelected = true;

            var currentSecondButton = PossibleSeconds.OfType<TimeSelectorButton>().SingleOrDefault(b => (int)b.Content == SelectedSecond);
            if (currentSecondButton != null) currentSecondButton.IsSelected = true;
        }

        private void MinuteButtonUp(object sender, MouseButtonEventArgs e)
        {
            OnMinuteButtonMouseUp(e);
        }

        private void SecondButtonUp(object sender, MouseButtonEventArgs e)
        {
            OnSecondButtonMouseUp(e);
        }

        private void HourButtonOnClick(object sender, RoutedEventArgs e)
        {
            var button = (TimeSelectorButton)sender;
            DeselectAllHours();
            button.IsSelected = true;
            SelectedHour = (int)button.Content;
        }

        private void MinuteButtonOnClick(object sender, RoutedEventArgs e)
        {
            var button = (TimeSelectorButton)sender;
            DeselectAllMinutes();
            button.IsSelected = true;
            SelectedMinute = (int)button.Content;
        }

        private void SecondButtonOnClick(object sender, RoutedEventArgs e)
        {
            var button = (TimeSelectorButton)sender;
            DeselectAllSeconds();
            button.IsSelected = true;
            SelectedSecond = (int)button.Content;
        }

        private void DeselectAllHours()
        {
            foreach (var selectorButton in PossibleHours.OfType<TimeSelectorButton>())
            {
                selectorButton.IsSelected = false;
            }
        }

        private void DeselectAllMinutes()
        {
            foreach (var selectorButton in PossibleMinutes.OfType<TimeSelectorButton>())
            {
                selectorButton.IsSelected = false;
            }
        }

        private void DeselectAllSeconds()
        {
            foreach (var selectorButton in PossibleSeconds.OfType<TimeSelectorButton>())
            {
                selectorButton.IsSelected = false;
            }
        }
    }
}
