using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    /// <summary>
    /// Show an interaction over the contents of the control
    /// </summary>
    public class InteractionDisplayer : ContentControl
    {
        private const string TemplateStateActive = "Active";
        private const string TemplateStateInactive = "Inactive";

        #region Apply Default Style

        static InteractionDisplayer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(InteractionDisplayer),
                new FrameworkPropertyMetadata(typeof(InteractionDisplayer)));
        }

        #endregion

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // VSM will fail to update the first time, when called from
            // a custom property's 'PropertyChangedCallback' event.
            // Re-call VSM to update state.
            VisualStateManager.GoToState(this, Interaction == null ? TemplateStateInactive : TemplateStateActive,
                false);
        }

        /// <summary>
        /// Get or Set the current Interaction
        /// </summary>
        public static readonly DependencyProperty InteractionProperty = DependencyProperty.Register(
            nameof(Interaction), typeof(object), typeof(InteractionDisplayer),
            new FrameworkPropertyMetadata(default(object), PropertyChangedCallback)
            {
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                BindsTwoWayByDefault = false
            });

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as InteractionDisplayer;
            if (control == null) return;
            var interaction = e.NewValue;
            if (e.NewValue != null) control.LastInteraction = interaction;
            control.IsShowingInteraction = interaction != null;
            VisualStateManager.GoToState(control, interaction == null ? TemplateStateInactive : TemplateStateActive,
                true);
        }

        /// <summary>
        /// Get or Set the current Interaction
        /// </summary>
        public object Interaction
        {
            get { return GetValue(InteractionProperty); }
            set { SetValue(InteractionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="LastInteraction" />Â dependency property key. 
        /// </summary>
        public static readonly DependencyPropertyKey LastInteractionPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(LastInteraction), typeof(object), typeof(InteractionDisplayer), new FrameworkPropertyMetadata(
                default(object),
                FrameworkPropertyMetadataOptions.None));

        /// <summary>
        /// Identifies the <see cref="LastInteraction" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty LastInteractionProperty =
            LastInteractionPropertyKey.DependencyProperty;

        /// <summary>
        /// Represents the last interaction displayed. This property is never null from the moment an interaction has been
        /// displayed.
        /// </summary>
        public object LastInteraction
        {
            get { return GetValue(LastInteractionProperty); }
            protected set { SetValue(LastInteractionPropertyKey, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ShowInteraction" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty ShowInteractionProperty = DependencyProperty.Register(
            nameof(ShowInteraction), typeof(bool), typeof(InteractionDisplayer), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets if the <see cref="Interaction"/> will be display by the control.
        /// </summary>
        public bool ShowInteraction
        {
            get { return (bool)GetValue(ShowInteractionProperty); }
            set { SetValue(ShowInteractionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsShowingInteraction" />Â dependency property key. 
        /// </summary>
        public static readonly DependencyPropertyKey IsShowingInteractionPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(IsShowingInteraction), typeof(bool), typeof(InteractionDisplayer), new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies the <see cref="IsShowingInteraction" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty IsShowingInteractionProperty =
            IsShowingInteractionPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets if the <see cref="Interaction"/> is not null.
        /// </summary>
        public bool IsShowingInteraction
        {
            get { return (bool)GetValue(IsShowingInteractionProperty); }
            protected set { SetValue(IsShowingInteractionPropertyKey, value); }
        }

        /// <summary>
        /// Identifies the <see cref="InteractionTemplate" />Â dependency property. 
        /// </summary>
        public static readonly DependencyProperty InteractionTemplateProperty = DependencyProperty.Register(
            nameof(InteractionTemplate), typeof(DataTemplate), typeof(InteractionDisplayer),
            new FrameworkPropertyMetadata(null));

        /// <summary> 
        /// Gets or sets the data template used to display the <see cref="Interaction"/>. 
        /// </summary>
        public DataTemplate InteractionTemplate
        {
            get { return (DataTemplate)GetValue(InteractionTemplateProperty); }
            set { SetValue(InteractionTemplateProperty, value); }
        }
    }
}
