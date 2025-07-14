using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.Common.Access;
using Agileo.Common.Localization;
using Agileo.Common.Logging;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;
using Agileo.Semi.Communication.Abstractions.E5;
using Agileo.Semi.Gem.Abstractions.E30;

using UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader.Enums;
using UnitySC.Equipment.Devices.Controller.JobDefinition;
using UnitySC.GUI.Common.Resources;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.UIComponents.GuiExtended;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup;
using UnitySC.UTO.Controller.Configuration;
using UnitySC.UTO.Controller.Remote.Constants;

namespace UnitySC.UTO.Controller.Views.Panels.Setup.OcrProfile
{
    public class OcrProfilesPanel : SetupPanel<ControllerConfiguration>
    {
        private const string OcrProfilesTracerName = "OCR Profiles";
        private readonly ILogger _logger;
        private IAccessManager AccessManager => GUI.Common.App.Instance.AccessRights;

        #region Constructors

        static OcrProfilesPanel()
        {
            DataTemplateGenerator.Create(typeof(OcrProfilesPanel), typeof(OcrProfilesPanelView));
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(OcrProfilePanelResources)));
        }

        public OcrProfilesPanel()
            : this(nameof(L10N.BP_OCR_PROFILE), PathIcon.RecipeEditor)
        {
        }

        /// <param name="relativeId"></param>
        /// <param name="icon"></param>
        /// <inheritdoc />
        public OcrProfilesPanel(string relativeId, IIcon icon)
            : base(relativeId, icon)
        {
            _logger = GUI.Common.App.Instance.GetLogger(OcrProfilesTracerName);

            // Invisible commands
            AddProfileCommand = new InvisibleBusinessPanelCommand(
                nameof(OcrProfilePanelResources.OCR_PROFILE_ADD),
                new DelegateCommand(AddOcrProfile, () => !IsEditing),
                PathIcon.Add);
            Commands.Add(AddProfileCommand);

            EditProfileCommand = new InvisibleBusinessPanelCommand(
                nameof(OcrProfilePanelResources.OCR_PROFILE_EDIT),
                new DelegateCommand(DisplayOcrProfileEditor, () => SelectedOcrProfile != null && !IsEditing),
                PathIcon.Edit);
            Commands.Add(EditProfileCommand);

            DeleteProfileCommand = new InvisibleBusinessPanelCommand(
                nameof(OcrProfilePanelResources.OCR_PROFILE_DELETE),
                new DelegateCommand(DeleteOcrProfile, () => SelectedOcrProfile != null && !IsEditing),
                PathIcon.Delete);
            Commands.Add(DeleteProfileCommand);
        }

        public InvisibleBusinessPanelCommand AddProfileCommand { get; }

        public InvisibleBusinessPanelCommand EditProfileCommand { get; }

        public InvisibleBusinessPanelCommand DeleteProfileCommand { get; }

        #endregion Constructors

        #region Properties
        public bool IsEditing => OcrProfileDetailsViewModel is { IsEditing: true };

        public DataTableSource<Equipment.Devices.Controller.JobDefinition.OcrProfile> DataTableSource { get; } = new();

        private bool _detailsIsExpanded;

        public bool DetailsIsExpanded
        {
            get { return _detailsIsExpanded; }
            set { SetAndRaiseIfChanged(ref _detailsIsExpanded, value); }
        }

        private Equipment.Devices.Controller.JobDefinition.OcrProfile _selectedOcrProfile;

        public Equipment.Devices.Controller.JobDefinition.OcrProfile SelectedOcrProfile
        {
            get { return _selectedOcrProfile; }
            set
            {
                if (SetAndRaiseIfChanged(ref _selectedOcrProfile, value))
                {
                    OnSelectedProfileChanged();
                }
            }
        }

        private OcrProfileDetailsViewModel _ocrProfileDetailsViewModel;

        public OcrProfileDetailsViewModel OcrProfileDetailsViewModel
        {
            get { return _ocrProfileDetailsViewModel; }
            set
            {
                _ocrProfileDetailsViewModel?.Dispose();
                _ocrProfileDetailsViewModel = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        #region AddCommand

        private void AddOcrProfile()
        {
            var newOcrProfile = new Equipment.Devices.Controller.JobDefinition.OcrProfile();
            DataTableSource.Add(newOcrProfile);
            ModifiedConfig.OcrProfiles.Add(newOcrProfile);
            SelectOcrProfile(newOcrProfile.Name);
            DisplayOcrProfileEditor(true);
        }

        #endregion

        #region DeleteCommand

        private void DeleteOcrProfile()
        {
            if (!DataTableSource.Contains(SelectedOcrProfile)) return;

            var popup =
                new Popup(new LocalizableText(nameof(OcrProfilePanelResources.OCR_PROFILE_DELETE_CONFIRMATION_TITLE),
                    SelectedOcrProfile.Name))
                {
                    Commands =
                    {
                        new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)),
                        new PopupCommand(nameof(OcrProfilePanelResources.OCR_PROFILE_DELETE), new DelegateCommand(() =>
                        {
                            try
                            {
                                DispatcherHelper.DoInUiThread(delegate
                                {
                                    var ocrProfile = SelectedOcrProfile;

                                    ModifiedConfig.OcrProfiles.Remove(ocrProfile);
                                    DataTableSource.Remove(ocrProfile);

                                    Messages.Show(new UserMessage(MessageLevel.Success,
                                        new LocalizableText(nameof(OcrProfilePanelResources.OCR_PROFILE_DELETED),
                                            ocrProfile.Name)) {SecondsDuration = 5});

                                    _logger.Info(
                                        "OCR Profile '{OcrProfileName}' has been deleted by {UserName}.",
                                        ocrProfile.Name,
                                        AccessManager.CurrentUser?.Name);
                                });
                            }
                            catch (Exception e)
                            {
                                Messages.Show(new UserMessage(MessageLevel.Error,
                                    nameof(OcrProfilePanelResources.OCR_PROFILE_ERROR_DELETION)) {CanUserCloseMessage = true});
                                _logger.Error(e, "An exception occurred while deleting the profile '{OcrProfileName}'", SelectedOcrProfile.Name);
                            }
                        }))
                    }
                };
            Popups.Show(popup);
        }

        #endregion

        #endregion Commands

        #region Private

        private void SelectOcrProfile(string profileName)
        {
            SelectedOcrProfile = ModifiedConfig.OcrProfiles.Single(profile => profile.Name.Equals(profileName));
        }

        private void RefreshOcrProfileList()
        {
            DispatcherHelper.DoInUiThread(() =>
            {
                var isFrontReaderPresent = App.ControllerInstance.ControllerEquipmentManager.SubstrateIdReaderFront != null;
                var isBackReaderPresent = App.ControllerInstance.ControllerEquipmentManager.SubstrateIdReaderBack != null;

                var profiles = ModifiedConfig.OcrProfiles.Where(
                    p => (p.Parameters.ReaderSide == ReaderSide.Front && isFrontReaderPresent)
                         || (p.Parameters.ReaderSide == ReaderSide.Back && isBackReaderPresent)
                         || (p.Parameters.ReaderSide == ReaderSide.Both && isFrontReaderPresent && isBackReaderPresent));
                DataTableSource.Reset(profiles);
                SelectedOcrProfile ??= DataTableSource.FirstOrDefault();
            });
        }

        private void OnSelectedProfileChanged()
        {
            if (!IsEditing)
            {
                DisplayOcrProfileDetails();
            }
        }

        private void DisplayOcrProfileDetails()
        {
            OcrProfileDetailsViewModel = SelectedOcrProfile != null
                ? new OcrProfileDetailsViewModel(SelectedOcrProfile, false, this)
                : null;
            DetailsIsExpanded = true;
        }

        private void DisplayOcrProfileEditor()
        {
            DisplayOcrProfileEditor(false);
        }

        private void DisplayOcrProfileEditor(bool isNew)
        {
            OcrProfileDetailsViewModel = new OcrProfileDetailsViewModel(SelectedOcrProfile, true, this, isNew);
            DetailsIsExpanded = true;
            OnPropertyChanged(nameof(IsEditing));
        }

        private void CompleteEditing()
        {
            OcrProfileDetailsViewModel?.EditingComplete();
            RefreshOcrProfileList();
            DetailsIsExpanded = false;
            OnPropertyChanged(nameof(IsEditing));
        }
        #endregion

        #region Overrides of SetupPanel<ControllerConfiguration>

        protected override bool ConfigurationEqualsLoaded() => ObjectAreEquals(ModifiedConfig?.OcrProfiles, LoadedConfig?.OcrProfiles);

        protected override bool ConfigurationEqualsCurrent() => ObjectAreEquals(ModifiedConfig?.OcrProfiles, CurrentConfig?.OcrProfiles);

        protected override bool ChangesNeedRestart => false;

        public override bool SaveCommandCanExecute()
        {
            return base.SaveCommandCanExecute() && ((OcrProfileDetailsViewModel != null && OcrProfileDetailsViewModel.CanBeSaved()) || OcrProfileDetailsViewModel == null);
        }

        protected override void SaveConfig()
        {
            var current = ((ControllerConfiguration)ConfigManager.Current).OcrProfiles;
            var modified = ((ControllerConfiguration)ConfigManager.Modified).OcrProfiles;
            List<VariableUpdate> variables;
            string ceName;

            if (SelectedOcrProfile != null)
            {
                SelectedOcrProfile.ModificationDate = DateTime.Now;

                ceName = OcrProfileDetailsViewModel.IsNew ? CEIDs.CustomEvents.OcrProfileCreated : CEIDs.CustomEvents.OcrProfileUpdated;

                //Added/Edited Profiles
                variables = new List<VariableUpdate>()
                {
                    new (DVs.OcrProfileName, DataItemFormat.ASC, SelectedOcrProfile.Name),
                    new (DVs.OcrFrontRecipeName, DataItemFormat.ASC, SelectedOcrProfile.Parameters.FrontRecipeName),
                    new (DVs.OcrBackRecipeName, DataItemFormat.ASC, SelectedOcrProfile.Parameters.BackRecipeName),
                    new (DVs.OcrUsed, DataItemFormat.UI1, (byte)SelectedOcrProfile.Parameters.ReaderSide),
                    new (DVs.OcrScribeAngle, DataItemFormat.FP4, SelectedOcrProfile.Parameters.ScribeAngle),
                };

                App.ControllerInstance.GemController.E30Std.DataServices.SendEvent(ceName, variables);
            }

            //Removed Profiles
            foreach (var removedProfile in current.FindAll(p => !modified.Exists(pr=>pr.Name.Equals(p.Name))))
            {
                ceName = CEIDs.CustomEvents.OcrProfileDeleted;
                variables = new List<VariableUpdate>()
                {
                    new (DVs.OcrProfileName, DataItemFormat.ASC, removedProfile.Name),
                    new (DVs.OcrFrontRecipeName, DataItemFormat.ASC, removedProfile.Parameters.FrontRecipeName),
                    new (DVs.OcrBackRecipeName, DataItemFormat.ASC, removedProfile.Parameters.BackRecipeName),
                    new (DVs.OcrUsed, DataItemFormat.UI1, (byte)removedProfile.Parameters.ReaderSide),
                    new (DVs.OcrScribeAngle, DataItemFormat.FP4, removedProfile.Parameters.ScribeAngle),
                };

                App.ControllerInstance.GemController.E30Std.DataServices.SendEvent(ceName, variables);
            }

            base.SaveConfig();
            CompleteEditing();
        }

        public override bool UndoCommandCanExecute()
        {
            return base.UndoCommandCanExecute() || IsEditing;
        }

        protected override void UndoChanges()
        {
            base.UndoChanges();
            CompleteEditing();
        }

        public override void OnSetup()
        {
            base.OnSetup();

            RefreshOcrProfileList();
        }
        #endregion
    }
}
