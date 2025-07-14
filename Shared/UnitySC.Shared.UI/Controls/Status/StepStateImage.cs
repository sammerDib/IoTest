using System.Windows;
using System.Windows.Controls;

namespace UnitySC.Shared.UI.Controls
{
    // This control displays a tri-state image
    // Off
    // In progress
    // On

    public enum StepStates
    {
        NotDone,
        InProgress,
        Done,
        Error
    }

    public class StepStateImage : Control
    {
        public StepStates StepState
        {
            get { return (StepStates)GetValue(StepStateProperty); }
            set { SetValue(StepStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StepState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StepStateProperty =
            DependencyProperty.Register(nameof(StepState), typeof(StepStates), typeof(StepStateImage), new PropertyMetadata(StepStates.NotDone));
    }
}