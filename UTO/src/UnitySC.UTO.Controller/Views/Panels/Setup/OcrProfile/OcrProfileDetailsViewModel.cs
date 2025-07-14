using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;

using UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader;
using UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader.Enums;

namespace UnitySC.UTO.Controller.Views.Panels.Setup.OcrProfile
{
    public class OcrProfileDetailsViewModel : Notifier, IDisposable
    {
        #region Fields

        private List<string> _validationErrors;
        private bool _disposedValue;

        #endregion Fields

        #region Constructor

        public OcrProfileDetailsViewModel()
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException("Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OcrProfileDetailsViewModel" /> class.
        /// </summary>
        public OcrProfileDetailsViewModel(Equipment.Devices.Controller.JobDefinition.OcrProfile profile, bool isEditing, OcrProfilesPanel ownerPanel, bool isNew = false)
        {
            Profile = profile;
            IsEditing = isEditing;
            OwnerPanel = ownerPanel;
            IsNew = isNew;

            if (SubstrateIdReaderFront != null)
            {
                SubstrateIdReaderFront.CommandExecutionStateChanged += SubstrateIdReaderFront_CommandExecutionStateChanged;

                if (!SubstrateIdReaderFront.Recipes.Select(s => s.Name)
                        .Contains(Profile.Parameters.FrontRecipeName))
                {
                    FrontRecipeName = string.Empty;
                }
            }
            else
            {
                FrontRecipeName = string.Empty;
            }

            if(SubstrateIdReaderBack != null)
            {
                SubstrateIdReaderBack.CommandExecutionStateChanged += SubstrateIdReaderBack_CommandExecutionStateChanged;

                if (!SubstrateIdReaderBack.Recipes.Select(s => s.Name)
                        .Contains(Profile.Parameters.BackRecipeName))
                {
                    BackRecipeName = string.Empty;
                }
            }
            else
            {
                BackRecipeName = string.Empty;
            }

            FrontReaderEnabled = ReaderSide is ReaderSide.Front or ReaderSide.Both && SubstrateIdReaderFront != null;
            BackReaderEnabled = ReaderSide is ReaderSide.Back or ReaderSide.Both && SubstrateIdReaderBack != null;

            ValidateProfile();
        }

        #endregion Constructor

        #region Properties

        public SubstrateIdReader SubstrateIdReaderFront
            => App.ControllerInstance.ControllerEquipmentManager.SubstrateIdReaderFront;

        public SubstrateIdReader SubstrateIdReaderBack
            => App.ControllerInstance.ControllerEquipmentManager.SubstrateIdReaderBack;

        public List<string> FrontReaderRecipes => GetFrontReaderRecipe();

        public List<string> BackReaderRecipes => GetBackReaderRecipe();

        private bool _frontReaderEnabled;

        public bool FrontReaderEnabled
        {
            get { return _frontReaderEnabled; }
            set
            {
                SetAndRaiseIfChanged(ref _frontReaderEnabled, value);
                UpdateReaderSide();
            }
        }

        private bool _backReaderEnabled;

        public bool BackReaderEnabled
        {
            get { return _backReaderEnabled; }
            set
            {
                SetAndRaiseIfChanged(ref _backReaderEnabled, value);
                UpdateReaderSide();
            }
        }

        public Equipment.Devices.Controller.JobDefinition.OcrProfile Profile { get; }

        public bool IsEditing { get; private set; }

        public bool IsNew { get; private set; }

        public OcrProfilesPanel OwnerPanel { get; }

        #region Edition Properties

        public string Name
        {
            get { return Profile.Name; }
            set
            {
                if (Profile.Name == value || !IsEditing) return;
                Profile.Name = value;
                OnPropertyChanged();
                ValidateProfile();
            }
        }

        public string Author
        {
            get { return Profile.Author; }
            set
            {
                if (Profile.Author == value || !IsEditing) return;
                Profile.Author = value;
                OnPropertyChanged();
                ValidateProfile();
            }
        }

        public string FrontRecipeName
        {
            get { return Profile.Parameters.FrontRecipeName; }
            set
            {
                if (Profile.Parameters.FrontRecipeName == value || !IsEditing) return;
                Profile.Parameters.FrontRecipeName = value;
                OnPropertyChanged();
                ValidateProfile();
            }
        }

        public string BackRecipeName
        {
            get { return Profile.Parameters.BackRecipeName; }
            set
            {
                if (Profile.Parameters.BackRecipeName == value || !IsEditing) return;
                Profile.Parameters.BackRecipeName = value;
                OnPropertyChanged();
                ValidateProfile();
            }
        }

        public double ScribeAngle
        {
            get { return Profile.Parameters.ScribeAngle; }
            set
            {
                if (Profile.Parameters.ScribeAngle.Equals(value) || !IsEditing) return;
                Profile.Parameters.ScribeAngle = value;
                OnPropertyChanged();
                ValidateProfile();
            }
        }

        public ReaderSide ReaderSide
        {
            get { return Profile.Parameters.ReaderSide; }
            set
            {
                if (Profile.Parameters.ReaderSide == value || !IsEditing) return;
                Profile.Parameters.ReaderSide = value;
                OnPropertyChanged();
                ValidateProfile();
            }
        }
        #endregion

        #endregion Properties

        #region Private

        private void UpdateReaderSide()
        {
            if (FrontReaderEnabled && !BackReaderEnabled)
            {
                ReaderSide = ReaderSide.Front;
            }
            else if (!FrontReaderEnabled && BackReaderEnabled)
            {
                ReaderSide = ReaderSide.Back;
            }
            else if (FrontReaderEnabled && BackReaderEnabled)
            {
                ReaderSide = ReaderSide.Both;
            }
        }

        private List<string> GetFrontReaderRecipe()
        {
            if (SubstrateIdReaderFront != null)
            {
                return SubstrateIdReaderFront.Recipes
                    .Select(r => r.Name)
                    .ToList();
            }

            return new List<string>();
        }

        private List<string> GetBackReaderRecipe()
        {
            if (SubstrateIdReaderBack != null)
            {
                return SubstrateIdReaderBack.Recipes
                    .Select(r => r.Name)
                    .ToList();
            }

            return new List<string>();
        }
        #endregion

        #region Public

        public void EditingComplete()
        {
            IsNew = false;
            IsEditing = false;
        }

        public bool CanBeSaved()
        {
            return !_validationErrors?.Any() ?? true;
        }

        #endregion

        #region Command

        #region RefreshFrontReaderRecipes

        private DelegateCommand _refreshFrontReaderRecipesCommand;

        public ICommand RefreshFrontReaderRecipesCommand => _refreshFrontReaderRecipesCommand ??= new DelegateCommand(RefreshFrontReaderRecipesCommandExecute, RefreshFrontReaderRecipesCommandCanExecute);

        private bool RefreshFrontReaderRecipesCommandCanExecute()
        {
            return SubstrateIdReaderFront != null
                   && SubstrateIdReaderFront.CanExecute(nameof(ISubstrateIdReader.RequestRecipes), out _);
        }

        private void RefreshFrontReaderRecipesCommandExecute()
        {
            SubstrateIdReaderFront.RequestRecipesAsync();
        }
        #endregion

        #region RefreshBackReaderRecipes

        private DelegateCommand _refreshBackReaderRecipesCommand;

        public ICommand RefreshBackReaderRecipesCommand => _refreshBackReaderRecipesCommand ??= new DelegateCommand(RefreshBackReaderRecipesCommandExecute, RefreshBackReaderRecipesCommandCanExecute);

        private bool RefreshBackReaderRecipesCommandCanExecute()
        {
            return SubstrateIdReaderBack != null
                   && SubstrateIdReaderBack.CanExecute(nameof(ISubstrateIdReader.RequestRecipes), out _);
        }

        private void RefreshBackReaderRecipesCommandExecute()
        {
            SubstrateIdReaderBack.RequestRecipesAsync();
        }

        #endregion

        #endregion

        #region Validation

        private void ValidateProfile()
        {
            if(!IsEditing)
                return;

            OwnerPanel.Messages.HideAll();
            _validationErrors = Profile.Validate();

            if (!FrontReaderEnabled && !BackReaderEnabled)
            {
                _validationErrors.Add(OcrProfilePanelResources.OCR_PROFILE_NO_READER_ERROR);
            }

            if (_validationErrors.Count > 0)
            {
                OwnerPanel.Messages.Show(new UserMessage(MessageLevel.Warning, string.Join(Environment.NewLine, _validationErrors)));
            }
        }

        #endregion Validation

        #region EventHandler

        private void SubstrateIdReaderFront_CommandExecutionStateChanged(object sender, CommandExecutionEventArgs e)
        {
            if (e.Execution.Context.Command.Name.Equals(nameof(ISubstrateIdReader.RequestRecipes))
                && e.NewState == ExecutionState.Success)
            {
                OnPropertyChanged(nameof(FrontReaderRecipes));
            }
        }

        private void SubstrateIdReaderBack_CommandExecutionStateChanged(object sender, CommandExecutionEventArgs e)
        {
            if (e.Execution.Context.Command.Name.Equals(nameof(ISubstrateIdReader.RequestRecipes))
                && e.NewState == ExecutionState.Success)
            {
                OnPropertyChanged(nameof(BackReaderRecipes));
            }
        }

        #endregion

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (SubstrateIdReaderFront != null)
                    {
                        SubstrateIdReaderFront.CommandExecutionStateChanged -= SubstrateIdReaderFront_CommandExecutionStateChanged;
                    }

                    if (SubstrateIdReaderBack != null)
                    {
                        SubstrateIdReaderBack.CommandExecutionStateChanged -= SubstrateIdReaderBack_CommandExecutionStateChanged;
                    }
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
