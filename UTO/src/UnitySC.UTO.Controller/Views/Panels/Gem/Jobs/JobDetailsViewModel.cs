using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.Common.Localization;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;

using UnitySC.GUI.Common.Vendor;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

using E40Status = Agileo.Semi.Gem300.Abstractions.E40.Status;
using E94Status = Agileo.Semi.Gem300.Abstractions.E94.Status;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.Jobs
{
    public abstract class JobDetailsViewModel : NotifyDataError
    {
        #region Fields

        private bool _isInCreation;
        private bool _isInEdition;
        private bool _isPauseEventListOpened;
        private bool _isSaveCommandVisible;
        private bool _isCancelCommandVisible;

        private List<Error> _errors = new();
        private int _index;

        private DelegateCommand _saveJobCommand;
        private DelegateCommand _cancelJobCommand;

        #endregion

        #region Properties

        #region Global properties

        public List<Error> Errors
        {
            get => _errors;
            set => SetAndRaiseIfChanged(ref _errors, value);
        }

        public int Index
        {
            get => _index;
            set => SetAndRaiseIfChanged(ref _index, value);
        }

        #endregion

        #region Data Tree properties

        public List<JobDetailsViewModel> Children { get; set; } = new();

        #endregion

        #region Display Properties

        public bool IsCarrierListOpened { get; set; }

        public bool IsErrorListOpened { get; set; }

        public bool IsPauseEventListOpened
        {
            get => _isPauseEventListOpened;
            set => SetAndRaiseIfChanged(ref _isPauseEventListOpened, value);
        }

        public bool IsInGlobalEdition => IsInCreation || IsInEdition;

        public bool IsSaveCommandVisible
        {
            get => _isSaveCommandVisible;
            set => SetAndRaiseIfChanged(ref _isSaveCommandVisible, value);
        }

        public bool IsCancelCommandVisible
        {
            get => _isCancelCommandVisible;
            set => SetAndRaiseIfChanged(ref _isCancelCommandVisible, value);
        }

        #endregion

        #region Creation Properties

        public bool IsInCreation
        {
            get => _isInCreation;
            set
            {
                if (!SetAndRaiseIfChanged(ref _isInCreation, value))
                {
                    return;
                }

                OnPropertyChanged(nameof(IsInGlobalEdition));
            }
        }

        #endregion

        #region Edition properties

        public bool IsInEdition
        {
            get => _isInEdition;
            set
            {
                if (!SetAndRaiseIfChanged(ref _isInEdition, value))
                {
                    return;
                }

                OnPropertyChanged(nameof(IsInGlobalEdition));
            }
        }

        #endregion

        #endregion

        #region Events

        internal event EventHandler<JobDetailsChangedEventArgs> JobChanged;

        #endregion

        #region Commands

        #region Save

        public DelegateCommand SaveJobCommand
            => _saveJobCommand ??= new DelegateCommand(SaveCommandExecute, SaveCommandCanExecute);

        internal abstract void SaveCommandExecute();

        internal abstract bool SaveCommandCanExecute();

        #endregion

        #region Cancel

        public DelegateCommand CancelJobCommand
            => _cancelJobCommand ??= new DelegateCommand(
                CancelCommandExecute,
                CancelCommandCanExecute);

        internal abstract void CancelCommandExecute();

        internal abstract bool CancelCommandCanExecute();

        #endregion

        #endregion

        #region Methods

        internal void ChangeEditionModeCommandVisibility(bool isVisible, bool closeExpander)
        {
            IsSaveCommandVisible = isVisible;
            IsCancelCommandVisible = isVisible;

            JobChanged?.Invoke(this, new JobDetailsChangedEventArgs(this, closeExpander));
        }

        #region Send user message

        internal void SendSuccessFailureMessage(E40Status status, string commandName)
        {
            var errors = string.Join(Environment.NewLine, status.Errors.Select(e => e.Text));
            SendMessage(status.IsFailure, errors, commandName);
        }

        internal void SendSuccessFailureMessage(E94Status status, string commandName)
        {
            var errors = string.Join(Environment.NewLine, status.Errors.Select(e => e.Text));
            SendMessage(status.IsFailure, errors, commandName);
        }

        private static void SendMessage(bool isFailure, string errors, string commandName)
        {
            if (isFailure)
            {
                var message = new UserMessage(
                    MessageLevel.Error,
                    new LocalizableText(
                        nameof(GemGeneralRessources.GEM_FAILURE),
                        commandName,
                        errors))
                { CanUserCloseMessage = true, SecondsDuration = 5 };
                AgilControllerApplication.Current.UserInterface.Navigation.SelectedBusinessPanel
                    ?.Messages.Show(message);
            }
            else
            {
                var message = new UserMessage(
                    MessageLevel.Success,
                    new LocalizableText(nameof(GemGeneralRessources.GEM_SUCCESS), commandName))
                {
                    CanUserCloseMessage = true,
                    SecondsDuration = 5
                };
                AgilControllerApplication.Current.UserInterface.Navigation.SelectedBusinessPanel
                    ?.Messages.Show(message);
            }
        }

        
        public static string ValidateEditingId(List<string> existingIds, string idToCheck)
        {
            if (string.IsNullOrEmpty(idToCheck))
            {
                return LocalizationManager.GetString(nameof(GemGeneralRessources.GEM_ERROR_EMPTY));
            }

            if (existingIds.Any(id => id == idToCheck))
            {
                return LocalizationManager.GetString(nameof(GemGeneralRessources.GEM_ERROR_ALREADY_USED),
                    idToCheck);
            }

            return null;
        }

        #endregion

        #endregion
    }
}
