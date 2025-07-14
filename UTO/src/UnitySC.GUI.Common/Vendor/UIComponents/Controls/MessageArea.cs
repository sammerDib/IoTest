using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    /// <inheritdoc />
    /// <summary>
    /// Control used to display a message associated with a <see cref="T:Agileo.GUI.Enums.MessageLevel" />
    /// </summary>
    public class MessageArea : Control
    {
        #region Apply Default Style

        static MessageArea()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageArea),
                new FrameworkPropertyMetadata(typeof(MessageArea)));
        }

        #endregion

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
            VisualStateManager.GoToState(this, Message == null ? "Inactive" : "Active", true);
        }

        /// <summary>
        /// Identifies the <see cref="Message" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            nameof(Message), typeof(UserMessage), typeof(MessageArea),
            new PropertyMetadata(default(UserMessage), MessageChangeCallback));

        /// <summary>
        /// Gets or sets the current display <see cref="UserMessage"/> instance. 
        /// </summary>
        public UserMessage Message
        {
            get { return (UserMessage)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        private static void MessageChangeCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MessageArea;
            if (control == null) return;
            control.ApplyBackground();
            VisualStateManager.GoToState(control, e.NewValue == null ? "Inactive" : "Active", true);
        }

        private void ApplyBackground()
        {
            if (Message == null) return;
            switch (Message.Level)
            {
                case MessageLevel.NotAssigned:
                    Background = NotAssignedBackground;
                    Foreground = NotAssignedForeground;
                    break;
                case MessageLevel.Warning:
                    Background = WarningBackground;
                    Foreground = WarningForeground;
                    break;
                case MessageLevel.Error:
                    Background = ErrorBackground;
                    Foreground = ErrorForeground;
                    break;
                case MessageLevel.Info:
                    Background = InfoBackground;
                    Foreground = InfoForeground;
                    break;
                case MessageLevel.Success:
                    Background = SuccessBackground;
                    Foreground = SuccessForeground;
                    break;
                case MessageLevel.Various:
                    Background = VariousBackground;
                    Foreground = VariousForeground;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        #region Foregrounds

        /// <summary>
        /// Identifies the <see cref="NotAssignedForeground" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty NotAssignedForegroundProperty = DependencyProperty.Register(
            nameof(NotAssignedForeground), typeof(SolidColorBrush), typeof(MessageArea),
            new PropertyMetadata(default(SolidColorBrush)));

        /// <summary>
        /// Gets or sets the foreground associated to the <see cref="MessageLevel.NotAssigned"/> level. 
        /// </summary>
        public SolidColorBrush NotAssignedForeground
        {
            get { return (SolidColorBrush)GetValue(NotAssignedForegroundProperty); }
            set { SetValue(NotAssignedForegroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="WarningForeground" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty WarningForegroundProperty = DependencyProperty.Register(
            nameof(WarningForeground), typeof(SolidColorBrush), typeof(MessageArea),
            new PropertyMetadata(default(SolidColorBrush)));

        /// <summary>
        /// Gets or sets the foreground associated to the <see cref="MessageLevel.Warning"/> level. 
        /// </summary>
        public SolidColorBrush WarningForeground
        {
            get { return (SolidColorBrush)GetValue(WarningForegroundProperty); }
            set { SetValue(WarningForegroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ErrorForeground" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty ErrorForegroundProperty = DependencyProperty.Register(
            nameof(ErrorForeground), typeof(SolidColorBrush), typeof(MessageArea),
            new PropertyMetadata(default(SolidColorBrush)));

        /// <summary>
        /// Gets or sets the foreground associated to the <see cref="MessageLevel.Error"/> level. 
        /// </summary>
        public SolidColorBrush ErrorForeground
        {
            get { return (SolidColorBrush)GetValue(ErrorForegroundProperty); }
            set { SetValue(ErrorForegroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="InfoForeground" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty InfoForegroundProperty = DependencyProperty.Register(
            nameof(InfoForeground), typeof(SolidColorBrush), typeof(MessageArea),
            new PropertyMetadata(default(SolidColorBrush)));

        /// <summary>
        /// Gets or sets the foreground associated to the <see cref="MessageLevel.Info"/> level. 
        /// </summary>
        public SolidColorBrush InfoForeground
        {
            get { return (SolidColorBrush)GetValue(InfoForegroundProperty); }
            set { SetValue(InfoForegroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="SuccessForeground" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty SuccessForegroundProperty = DependencyProperty.Register(
            nameof(SuccessForeground), typeof(SolidColorBrush), typeof(MessageArea),
            new PropertyMetadata(default(SolidColorBrush)));

        /// <summary>
        /// Gets or sets the foreground associated to the <see cref="MessageLevel.Success"/> level. 
        /// </summary>
        public SolidColorBrush SuccessForeground
        {
            get { return (SolidColorBrush)GetValue(SuccessForegroundProperty); }
            set { SetValue(SuccessForegroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="VariousForeground" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty VariousForegroundProperty = DependencyProperty.Register(
            nameof(VariousForeground), typeof(SolidColorBrush), typeof(MessageArea),
            new PropertyMetadata(default(SolidColorBrush)));

        /// <summary>
        /// Gets or sets the foreground associated to the <see cref="MessageLevel.Various"/> level. 
        /// </summary>
        public SolidColorBrush VariousForeground
        {
            get { return (SolidColorBrush)GetValue(VariousForegroundProperty); }
            set { SetValue(VariousForegroundProperty, value); }
        }

        #endregion

        #region Backgrounds

        /// <summary>
        /// Identifies the <see cref="NotAssignedBackground" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty NotAssignedBackgroundProperty = DependencyProperty.Register(
            nameof(NotAssignedBackground), typeof(SolidColorBrush), typeof(MessageArea),
            new PropertyMetadata(default(SolidColorBrush)));

        /// <summary>
        /// Gets or sets the background associated to the <see cref="MessageLevel.NotAssigned"/> level. 
        /// </summary>
        public SolidColorBrush NotAssignedBackground
        {
            get { return (SolidColorBrush)GetValue(NotAssignedBackgroundProperty); }
            set { SetValue(NotAssignedBackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="WarningBackground" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty WarningBackgroundProperty = DependencyProperty.Register(
            nameof(WarningBackground), typeof(SolidColorBrush), typeof(MessageArea),
            new PropertyMetadata(default(SolidColorBrush)));

        /// <summary>
        /// Gets or sets the background associated to the <see cref="MessageLevel.Warning"/> level. 
        /// </summary>
        public SolidColorBrush WarningBackground
        {
            get { return (SolidColorBrush)GetValue(WarningBackgroundProperty); }
            set { SetValue(WarningBackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ErrorBackground" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty ErrorBackgroundProperty = DependencyProperty.Register(
            nameof(ErrorBackground), typeof(SolidColorBrush), typeof(MessageArea),
            new PropertyMetadata(default(SolidColorBrush)));

        /// <summary>
        /// Gets or sets the background associated to the <see cref="MessageLevel.Error"/> level. 
        /// </summary>
        public SolidColorBrush ErrorBackground
        {
            get { return (SolidColorBrush)GetValue(ErrorBackgroundProperty); }
            set { SetValue(ErrorBackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="InfoBackground" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty InfoBackgroundProperty = DependencyProperty.Register(
            nameof(InfoBackground), typeof(SolidColorBrush), typeof(MessageArea),
            new PropertyMetadata(default(SolidColorBrush)));

        /// <summary>
        /// Gets or sets the background associated to the <see cref="MessageLevel.Info"/> level. 
        /// </summary>
        public SolidColorBrush InfoBackground
        {
            get { return (SolidColorBrush)GetValue(InfoBackgroundProperty); }
            set { SetValue(InfoBackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="SuccessBackground" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty SuccessBackgroundProperty = DependencyProperty.Register(
            nameof(SuccessBackground), typeof(SolidColorBrush), typeof(MessageArea),
            new PropertyMetadata(default(SolidColorBrush)));

        /// <summary>
        /// Gets or sets the background associated to the <see cref="MessageLevel.Success"/> level. 
        /// </summary>
        public SolidColorBrush SuccessBackground
        {
            get { return (SolidColorBrush)GetValue(SuccessBackgroundProperty); }
            set { SetValue(SuccessBackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="VariousBackground" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty VariousBackgroundProperty = DependencyProperty.Register(
            nameof(VariousBackground), typeof(SolidColorBrush), typeof(MessageArea),
            new PropertyMetadata(default(SolidColorBrush)));

        /// <summary>
        /// Gets or sets the background associated to the <see cref="MessageLevel.Various"/> level. 
        /// </summary>
        public SolidColorBrush VariousBackground
        {
            get { return (SolidColorBrush)GetValue(VariousBackgroundProperty); }
            set { SetValue(VariousBackgroundProperty, value); }
        }

        #endregion
    }
}
