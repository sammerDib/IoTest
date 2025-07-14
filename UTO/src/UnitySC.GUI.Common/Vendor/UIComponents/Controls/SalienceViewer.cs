using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Agileo.GUI.Services.Saliences;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    /// <summary>
    /// Usefull to display a value associated to a <see cref="SalienceType"/>.
    /// </summary>
    public class SalienceViewer : Control
    {
        #region Apply Default Style

        static SalienceViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SalienceViewer),
                new FrameworkPropertyMetadata(typeof(SalienceViewer)));
        }

        #endregion

        private void UpdateState(bool useTransitions)
        {
            if (Type == SalienceType.None)
            {
                VisualStateManager.GoToState(this, "Inactive", useTransitions);
            }
            else
            {
                if (IsMouseOverOrSelected)
                {
                    VisualStateManager.GoToState(this, "Expanded", useTransitions);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Active", useTransitions);
                }
            }
        }

        /// <summary>
        /// Identifies the <see cref="Type" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(
            nameof(Type), typeof(SalienceType), typeof(SalienceViewer),
            new UIPropertyMetadata(SalienceType.None, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SalienceViewer;
            if (control == null) return;
            control.ApplyBackground();
            control.UpdateState(true);
        }

        /// <summary>
        /// Gets or sets the current <see cref="SalienceType"/>.
        /// </summary>
        public SalienceType Type
        {
            get { return (SalienceType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Count" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty CountProperty = DependencyProperty.Register(
            nameof(Count), typeof(int), typeof(SalienceViewer), new FrameworkPropertyMetadata(0));

        /// <summary>
        /// Gets or sets the current count to display.
        /// </summary>
        public int Count
        {
            get { return (int)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // VSM will fail to update the first time, when called from
            // a custom property's 'PropertyChangedCallback' event.
            //
            // Re-call VSM to update state.
            //
            ApplyBackground();
            UpdateState(false);
        }

        private void ApplyBackground()
        {
            switch (Type)
            {
                case SalienceType.Alarm:
                    Background = AlarmBrush;
                    break;
                case SalienceType.Caution:
                    Background = CautionBrush;
                    break;
                case SalienceType.UserAttention:
                    Background = UserAttentionBrush;
                    break;
                case SalienceType.UnfinishedTask:
                    Background = UnfinishedTaskBrush;
                    break;
                case SalienceType.None:
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        #region Brushes

        /// <summary>
        /// Identifies the <see cref="AlarmBrush" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty AlarmBrushProperty = DependencyProperty.Register(
            nameof(AlarmBrush), typeof(SolidColorBrush), typeof(SalienceViewer),
            new PropertyMetadata(default(SolidColorBrush)));

        /// <summary>
        /// Gets or sets the brush associated to the <see cref="SalienceType.Alarm"/> type. 
        /// </summary>
        public SolidColorBrush AlarmBrush
        {
            get { return (SolidColorBrush)GetValue(AlarmBrushProperty); }
            set { SetValue(AlarmBrushProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="CautionBrush" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty CautionBrushProperty = DependencyProperty.Register(
            nameof(CautionBrush), typeof(SolidColorBrush), typeof(SalienceViewer),
            new PropertyMetadata(default(SolidColorBrush)));

        /// <summary>
        /// Gets or sets the brush associated to the <see cref="SalienceType.Caution"/> type. 
        /// </summary>
        public SolidColorBrush CautionBrush
        {
            get { return (SolidColorBrush)GetValue(CautionBrushProperty); }
            set { SetValue(CautionBrushProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="UserAttentionBrush" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty UserAttentionBrushProperty = DependencyProperty.Register(
            nameof(UserAttentionBrush), typeof(SolidColorBrush), typeof(SalienceViewer),
            new PropertyMetadata(default(SolidColorBrush)));

        /// <summary>
        /// Gets or sets the brush associated to the <see cref="SalienceType.UserAttention"/> type. 
        /// </summary>
        public SolidColorBrush UserAttentionBrush
        {
            get { return (SolidColorBrush)GetValue(UserAttentionBrushProperty); }
            set { SetValue(UserAttentionBrushProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="UnfinishedTaskBrush" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty UnfinishedTaskBrushProperty = DependencyProperty.Register(
            nameof(UnfinishedTaskBrush), typeof(SolidColorBrush), typeof(SalienceViewer),
            new PropertyMetadata(default(SolidColorBrush)));

        /// <summary>
        /// Gets or sets the brush associated to the <see cref="SalienceType.UnfinishedTask"/> type. 
        /// </summary>
        public SolidColorBrush UnfinishedTaskBrush
        {
            get { return (SolidColorBrush)GetValue(UnfinishedTaskBrushProperty); }
            set { SetValue(UnfinishedTaskBrushProperty, value); }
        }

        #endregion

        public static readonly DependencyProperty IsMouseOverOrSelectedProperty = DependencyProperty.Register(
            nameof(IsMouseOverOrSelected), typeof(bool), typeof(SalienceViewer), new PropertyMetadata(default(bool), IsMouseOverOrSelectedChangedCallback));

        private static void IsMouseOverOrSelectedChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SalienceViewer;
            control?.UpdateState(true);
        }

        public bool IsMouseOverOrSelected
        {
            get { return (bool)GetValue(IsMouseOverOrSelectedProperty); }
            set { SetValue(IsMouseOverOrSelectedProperty, value); }
        }

        public static readonly DependencyProperty CollapsedWidthProperty = DependencyProperty.Register(
            nameof(CollapsedWidth), typeof(double), typeof(SalienceViewer), new PropertyMetadata(default(double)));

        public double CollapsedWidth
        {
            get { return (double)GetValue(CollapsedWidthProperty); }
            set { SetValue(CollapsedWidthProperty, value); }
        }
    }
}
