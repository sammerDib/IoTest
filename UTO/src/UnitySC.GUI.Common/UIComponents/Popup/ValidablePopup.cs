using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Popups;

namespace UnitySC.GUI.Common.UIComponents.Popup
{
    public class ValidablePopup : Agileo.GUI.Services.Popups.Popup
    {
        #region Fields

        private readonly PopupCommand _validateCommand;
        private readonly PopupCommand _cancelCommand;

        #endregion

        #region Constructors

        public ValidablePopup(
            PopupCommand validateCommand,
            PopupCommand cancelCommand,
            LocalizableText localizableTitle,
            LocalizableText localizableMessage = null)
            : base(localizableTitle, localizableMessage)
        {
            _validateCommand = validateCommand;
            _cancelCommand = cancelCommand;

            if (_validateCommand != null)
            {
                Commands.Add(_validateCommand);
            }

            if (_cancelCommand != null)
            {
                Commands.Add(_cancelCommand);
            }
        }

        public ValidablePopup(
            PopupCommand validateCommand,
            PopupCommand cancelCommand,
            PopupButtons popupButtons,
            LocalizableText localizableTitle,
            LocalizableText localizableMessage = null)
            : base(popupButtons, localizableTitle, localizableMessage)
        {
            _validateCommand = validateCommand;
            _cancelCommand = cancelCommand;

            if (_validateCommand != null)
            {
                Commands.Add(_validateCommand);
            }

            if (_cancelCommand != null)
            {
                Commands.Add(_cancelCommand);
            }
        }

        public ValidablePopup(
            PopupCommand validateCommand,
            PopupCommand cancelCommand,
            IText title,
            IText message = null)
            : base(title, message)
        {
            _validateCommand = validateCommand;
            _cancelCommand = cancelCommand;

            if (_validateCommand != null)
            {
                Commands.Add(_validateCommand);
            }

            if (_cancelCommand != null)
            {
                Commands.Add(_cancelCommand);
            }
        }

        public ValidablePopup(
            PopupCommand validateCommand,
            PopupCommand cancelCommand,
            PopupButtons popupButtons,
            IText title,
            IText message = null)
            : base(popupButtons, title, message)
        {
            _validateCommand = validateCommand;
            _cancelCommand = cancelCommand;

            if (_validateCommand != null)
            {
                Commands.Add(_validateCommand);
            }

            if (_cancelCommand != null)
            {
                Commands.Add(_cancelCommand);
            }
        }

        #endregion

        #region Public Methods

        public void Validate()
        {
            if (_validateCommand != null && _validateCommand.CanExecute(null))
            {
                _validateCommand.Execute(null);
            }
        }

        public void Cancel()
        {
            if (_cancelCommand != null && _cancelCommand.CanExecute(null))
            {
                _cancelCommand.Execute(null);
            }
        }

        #endregion
    }
}
