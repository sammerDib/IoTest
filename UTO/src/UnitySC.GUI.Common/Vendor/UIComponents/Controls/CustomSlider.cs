using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    public class CustomSlider : Slider
    {
        static CustomSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomSlider), new FrameworkPropertyMetadata(typeof(CustomSlider)));
        }

        private ItemsControl _tickItemsControl;
        private Track _track;

        private IEnumerable<ITickItem> TickItems
        {
            get
            {
                return TickSource?.Cast<ITickItem>() ?? new List<ITickItem>();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _tickItemsControl = GetTemplateChild("PART_TickItemsControl") as ItemsControl;
            if (_tickItemsControl != null)
            {
                _tickItemsControl.ItemContainerGenerator.StatusChanged += ItemContainerGeneratorOnStatusChanged;
            }

            _track = GetTemplateChild("PART_Track") as Track;
        }

        private void ItemContainerGeneratorOnStatusChanged(object sender, EventArgs e)
        {
            if (_tickItemsControl.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                Dispatcher.InvokeAsync(UpdateCanvas, DispatcherPriority.Render);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Dispatcher.InvokeAsync(UpdateCanvas, DispatcherPriority.Render);
            return base.ArrangeOverride(finalSize);
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            Dispatcher.InvokeAsync(UpdateCanvas, DispatcherPriority.Render);
        }

        public static readonly DependencyProperty TickTemplateProperty = DependencyProperty.Register(
            nameof(TickTemplate), typeof(DataTemplate), typeof(CustomSlider), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate TickTemplate
        {
            get
            {
                return (DataTemplate)GetValue(TickTemplateProperty);
            }
            set
            {
                SetValue(TickTemplateProperty, value);
            }
        }

        public static readonly DependencyProperty TickSourceProperty = DependencyProperty.Register(
            nameof(TickSource), typeof(IEnumerable), typeof(CustomSlider), new PropertyMetadata(default(IEnumerable), TickSourceChangedCallback));

        private static void TickSourceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CustomSlider).UpdateTicks();
        }

        private void UpdateTicks()
        {
            if (TickSource == null) return;
            if (Notifier.IsInDesignModeStatic) return;
            var values = TickItems.Select(item => item.Value);
            Ticks = new DoubleCollection(values);
            Minimum = TickItems.Min(item => item.Value);
            Maximum = TickItems.Max(item => item.Value);
            UpdateCanvas();
        }


        private void UpdateCanvas()
        {
            var minValue = Minimum;
            var maxValue = Maximum;

            if (_track == null) return;

            // x value by pixel
            var ratio = (maxValue - minValue) / (ActualWidth - _track.Thumb.ActualWidth);

            foreach (var i in _tickItemsControl.Items)
            {
                var item = i as ITickItem;
                var uiElement = _tickItemsControl.ItemContainerGenerator.ContainerFromItem(item) as ContentPresenter;

                if (item == null || uiElement == null) continue;

                var canvasLeft = (item.Value - minValue) / ratio;
                canvasLeft -= uiElement.ActualWidth / 2 - _track.Thumb.ActualWidth / 2;
                canvasLeft = Math.Round(canvasLeft);
                uiElement.SetValue(Canvas.LeftProperty, canvasLeft);
            }
        }

        public IEnumerable TickSource
        {
            get
            {
                return (IEnumerable)GetValue(TickSourceProperty);
            }
            set
            {
                SetValue(TickSourceProperty, value);
            }
        }

        public static readonly DependencyProperty TickContainerStyleProperty = DependencyProperty.Register(
            nameof(TickContainerStyle), typeof(Style), typeof(CustomSlider), new PropertyMetadata(default(Style)));

        public Style TickContainerStyle
        {
            get
            {
                return (Style)GetValue(TickContainerStyleProperty);
            }
            set
            {
                SetValue(TickContainerStyleProperty, value);
            }
        }

    }

    public interface ITickItem
    {
        double Value { get; }
    }

    public class TextTickItem : ITickItem
    {
        public string Text { get; set; }
        public double Value { get; set; }
    }
}
