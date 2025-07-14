using System;
using System.ComponentModel;
using System.Linq;

using Agileo.GUI.Components;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;

using UnitySC.GUI.Common.Vendor.Views.RecipeInstructions;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.Scenarios.Library
{
    public class InstructionValidationPopup : Popup
    {
        private readonly IInstructionEditorViewModel _relatedInstruction;

        public InstructionValidationPopup(LocalizableText title, IInstructionEditorViewModel instruction) : base(title)
        {
            _relatedInstruction = instruction;
            Content = instruction;
        }

        private UserMessage _validationErrorMessage;

        public UserMessage ValidationErrorMessage
        {
            get => _validationErrorMessage;
            set => SetAndRaiseIfChanged(ref _validationErrorMessage, value);
        }

        #region Overrides

        public override void OnShow()
        {
            base.OnShow();
            if (_relatedInstruction?.Model == null)
            {
                return;
            }

            _relatedInstruction.Model.PropertyChanged += Instruction_PropertyChanged;
        }

        public override void OnHide()
        {
            base.OnHide();
            if (_relatedInstruction?.Model == null)
            {
                return;
            }

            _relatedInstruction.Model.PropertyChanged -= Instruction_PropertyChanged;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _relatedInstruction?.Model != null)
            {
                _relatedInstruction.Model.PropertyChanged -= Instruction_PropertyChanged;
            }

            base.Dispose(disposing);
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName.Equals(nameof(ValidationErrorMessage))) return;
            ValidateInstruction();
        }

        #endregion

        private void Instruction_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ValidateInstruction();
        }

        private void ValidateInstruction()
        {
            var errors = _relatedInstruction.Model.Validate().ToList();
            ValidationErrorMessage = errors.Any()
                ? new UserMessage(MessageLevel.Error, string.Join(Environment.NewLine, errors))
                : null;
        }
    }
}
