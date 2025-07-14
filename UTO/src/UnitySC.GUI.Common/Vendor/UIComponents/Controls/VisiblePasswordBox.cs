using System;
using System.Windows;
using System.Windows.Controls;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="PasswordBox" /> with visualization feature.
    /// </summary>
    public class VisiblePasswordBox : TextBox
    {
        #region Apply Default Style

        private const string PasswordTemplateName = "PART_Password";

        private PasswordBox _passwordBox;

        static VisiblePasswordBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VisiblePasswordBox),
                new FrameworkPropertyMetadata(typeof(VisiblePasswordBox)));
        }

        #endregion

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _passwordBox = Template.FindName(PasswordTemplateName, this) as PasswordBox;
            if (_passwordBox == null)
            {
                throw new InvalidOperationException($"The control {nameof(VisiblePasswordBox)} must contain a {nameof(PasswordBox)} named '{PasswordTemplateName}' in its associated template");
            }
            _passwordBox.PasswordChanged += PasswordBoxPasswordChanged;
        }

        // Prevent updating the loop between Text and PasswordBox.Password
        private bool _lockUpdatePasswordBox;

        private void PasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!IsShown)
            {
                _lockUpdatePasswordBox = true;
                Text = _passwordBox.Password;
                _lockUpdatePasswordBox = false;
            }
        }

        /// <inheritdoc />
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            VisualizePasswordButtonIsEnabled = !string.IsNullOrEmpty(Text);

            if (!_lockUpdatePasswordBox) _passwordBox.Password = Text;
        }

        /// <summary>
        /// Identifies the <see cref="IsShown" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty IsShownProperty = DependencyProperty.Register(
            nameof(IsShown), typeof(bool), typeof(VisiblePasswordBox), new PropertyMetadata(default(bool)));

        /// <summary>
        /// Gets or sets if the password is currently visible.
        /// </summary>
        public bool IsShown
        {
            get { return (bool)GetValue(IsShownProperty); }
            set { SetValue(IsShownProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="VisualizePasswordButtonIsEnabled" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty VisualizePasswordButtonIsEnabledProperty =
            DependencyProperty.Register(
                nameof(VisualizePasswordButtonIsEnabled), typeof(bool), typeof(VisiblePasswordBox),
                new PropertyMetadata(default(bool)));

        /// <summary>
        /// Gets or sets if the visualization button is enable.
        /// </summary>
        public bool VisualizePasswordButtonIsEnabled
        {
            get { return (bool)GetValue(VisualizePasswordButtonIsEnabledProperty); }
            set { SetValue(VisualizePasswordButtonIsEnabledProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="CanVisualizePassword" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty CanVisualizePasswordProperty = DependencyProperty.Register(
            nameof(CanVisualizePassword), typeof(bool), typeof(VisiblePasswordBox), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets if the password can be visualized.
        /// </summary>
        public bool CanVisualizePassword
        {
            get { return (bool)GetValue(CanVisualizePasswordProperty); }
            set { SetValue(CanVisualizePasswordProperty, value); }
        }
    }
}
