using UnitySC.GUI.Common.Vendor.Recipes.Instructions.UserInteraction;

namespace UnitySC.GUI.Common.Vendor.Views.RecipeInstructions.UserInteraction
{
    /// <summary>
    /// Template Model representing the ViewModel (DataContext) of <see cref="UserInteractionEditorView"/>
    /// </summary>
    public class UserInteractionEditor : InstructionEditorViewModel<UserInteractionInstruction>
    {
        public UserInteractionEditor(UserInteractionInstruction model) : base(model)
        {
            _timeout = model.TimeOut ?? 20;
            _useTimeout = model.TimeOut.HasValue;
        }

        private double _timeout;

        public double TimeOut
        {
            get { return _timeout; }
            set
            {
                _timeout = value;
                Model.TimeOut = value;
                OnPropertyChanged(nameof(TimeOut));
            }
        }

        private bool _useTimeout;

        public bool UseTimeout
        {
            get { return _useTimeout; }
            set
            {
                if (value)
                {
                    Model.TimeOut = _timeout;
                }
                else
                {
                    Model.TimeOut = null;
                }
                _useTimeout = value;
                OnPropertyChanged(nameof(UseTimeout));
            }
        }
    }
}
