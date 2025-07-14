using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;

using Agileo.Common.Localization;
using Agileo.Common.Logging;
using Agileo.DataMonitoring;
using Agileo.DataMonitoring.DataWriter.File;
using Agileo.DataMonitoring.Events;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;

using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.Library;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Sort;
using UnitySC.GUI.Common.Vendor.UIComponents.GuiExtended;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.Library.Popups;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.Library
{
    /// <summary>
    /// Display data about <see cref="DataCollectionPlanLibrarian" /> and its
    /// <see cref="DataCollectionPlan" />.
    /// </summary>
    public class DataCollectionLibraryPanel : BusinessPanel
    {
        #region Fields

        private const string DataCollectionLibraryTracerName = "Data Collection Library";

        public readonly ILogger Logger;

        private readonly Agileo.EquipmentModeling.Equipment _equipment;

        // Original instance set when opening the DCP editing panel
        private DataCollectionPlan _dcpInEdition;

        #endregion

        #region Properties

        public bool IsTemporaryLibrary { get; }

        public bool IsEditing => DataCollectionDetailsViewModel is {IsEditing: true};

        public DataTableSource<DataCollectionPlan> DataTableSource { get; } = new();

        /// <summary>
        /// Get the <see cref="DataCollectionPlanLibrarian" /> associated with the
        /// <see cref="DataCollectionLibraryPanel" />
        /// </summary>
        private DataCollectionPlanLibrarian DataCollectionPlanLibrarian { get; }

        private DataCollectionPlan _selectedDcp;

        /// <summary>Get or set the selected <see cref="DataCollectionPlan" />.</summary>
        public DataCollectionPlan SelectedDcp
        {
            get => _selectedDcp;
            set
            {
                if (SetAndRaiseIfChanged(ref _selectedDcp, value))
                {
                    DisplayDataCollectionDetails();
                }
            }
        }

        private bool _detailsIsExpanded;

        public bool DetailsIsExpanded
        {
            get => _detailsIsExpanded;
            set => SetAndRaiseIfChanged(ref _detailsIsExpanded, value);
        }

        private DataCollectionDetailsViewModel _dataCollectionDetailsViewModel;

        public DataCollectionDetailsViewModel DataCollectionDetailsViewModel
        {
            get => _dataCollectionDetailsViewModel;
            private set
            {
                _dataCollectionDetailsViewModel = value;
                OnPropertyChanged();
                UpdateCommandButtonVisibility();
            }
        }

        #endregion Properties

        #region Constructors

        static DataCollectionLibraryPanel()
        {
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(DataCollectionLibraryResources)));
        }

        /// <summary>Default constructor only used by view in design instance.</summary>
        public DataCollectionLibraryPanel()
            : this(null, "Design Time Constructor", null)
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }

            DataCollectionDetailsViewModel =
                new DataCollectionDetailsViewModel(null, null, null, false);
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="DataCollectionLibraryPanel" /> class.
        /// </summary>
        /// <param name="equipment"></param>
        /// <param name="relativeId">The relative id.</param>
        /// <param name="dataCollectionPlanLibrarian">
        /// The object responsible to provide data collection plans existing in the system.
        /// </param>
        /// <param name="isTemporaryDcpLibrary">Specify if library instance is temporary dcp collection</param>
        /// <param name="icon">The icon.</param>
        public DataCollectionLibraryPanel(
            Agileo.EquipmentModeling.Equipment equipment,
            string relativeId,
            DataCollectionPlanLibrarian dataCollectionPlanLibrarian,
            bool isTemporaryDcpLibrary = false,
            IIcon icon = null)
            : base(relativeId, icon)
        {
            // Initialize parameters
            Logger = App.Instance.GetLogger(DataCollectionLibraryTracerName);
            _equipment = equipment;
            IsTemporaryLibrary = isTemporaryDcpLibrary;

            DataCollectionPlanLibrarian = dataCollectionPlanLibrarian;

            // Initialize sorter
            DataTableSource.Sort.Add(
                new SortDefinition<DataCollectionPlan>(
                    nameof(DataCollectionPlan.Frequency),
                    dcp => dcp.Frequency.Hertz));
            DataTableSource.Sort.SetCurrentSorting(
                nameof(DataCollectionPlan.Name),
                ListSortDirection.Ascending);

            // Business Panel commands
            Commands.Add(
                new BusinessPanelCommand(
                    nameof(DataCollectionLibraryResources.START_DCP),
                    StartDcpCommand,
                    PathIcon.Play));
            Commands.Add(
                new BusinessPanelCommand(
                    nameof(DataCollectionLibraryResources.STOP_DCP),
                    StopDcpCommand,
                    PathIcon.Stop));

            if (IsTemporaryLibrary)
            {
                AddDcpCommand = new InvisibleBusinessPanelCommand(
                    nameof(DataCollectionLibraryResources.ADD_DCP),
                    new DelegateCommand(() => ShowDcpEditor(true), () => !IsEditing),
                    PathIcon.Add);
                EditDcpCommand = new InvisibleBusinessPanelCommand(
                    nameof(DataCollectionLibraryResources.EDIT_DCP),
                    new DelegateCommand(
                        () => DisplayDataCollectionEditDetails(SelectedDcp, false),
                        () => SelectedDcp?.IsActive == false && !IsEditing),
                    PathIcon.Edit);
                DuplicateDcpCommand = new InvisibleBusinessPanelCommand(
                    nameof(DataCollectionLibraryResources.DUPLICATE_DCP),
                    new DelegateCommand(
                        DuplicateDcpCommandExecute,
                        () => SelectedDcp != null && !IsEditing));
                DeleteDcpCommand = new InvisibleBusinessPanelCommand(
                    nameof(DataCollectionLibraryResources.DELETE_DCP),
                    new DelegateCommand(
                        DeleteDcpCommandExecute,
                        () => SelectedDcp?.IsActive == false && !IsEditing),
                    PathIcon.Delete);

                Commands.Add(AddDcpCommand);
                Commands.Add(EditDcpCommand);
                Commands.Add(DuplicateDcpCommand);
                Commands.Add(DeleteDcpCommand);

                // Edition commands
                SaveDcpCommand = new BusinessPanelCommand(
                    nameof(DataCollectionLibraryResources.DCP_EDITOR_SAVE_DCP),
                    new DelegateCommand(SaveCommandExecute, SaveCommandCanExecute),
                    PathIcon.Save) {IsVisible = false};
                Commands.Add(SaveDcpCommand);

                CancelCommand = new BusinessPanelCommand(
                    nameof(Agileo.GUI.Properties.Resources.S_CANCEL),
                    new DelegateCommand(CancelEditionCommand),
                    PathIcon.Undo) {IsVisible = false};
                Commands.Add(CancelCommand);
            }

            if (DataCollectionPlanLibrarian != null)
            {
                var sourceList = DataCollectionPlanLibrarian.Plans.Where(
                        dataCollectionPlan => dataCollectionPlan.IsDynamic == IsTemporaryLibrary)
                    .ToList();
                DataTableSource.Reset(sourceList);

                DataCollectionPlanLibrarian.DataCollectionPlanAdded +=
                    DataCollectionPlanLibrarian_DataCollectionPlanAdded;
                DataCollectionPlanLibrarian.DataCollectionPlanRemoved +=
                    DataCollectionPlanLibrarian_DataCollectionPlanRemoved;
            }
            else
            {
                DataTableSource.UpdateCollection();
            }
        }

        #endregion Constructors

        #region Overrides

        public override void OnShow()
        {
            base.OnShow();
            UpdateCommandButtonVisibility();
        }

        #endregion

        #region Commands

        #region StartDCPCommand

        private ICommand _startDcpCommand;

        public ICommand StartDcpCommand
            => _startDcpCommand ??= new DelegateCommand(StartDcpExecute, StartDcpCanExecute);

        private bool StartDcpCanExecute()
        {
            return CanStartDcp(SelectedDcp);
        }

        private readonly UserMessage _startDcpCanExecuteMessage =
            new(MessageLevel.Warning, string.Empty);

        private bool CanStartDcp(DataCollectionPlan selectedDcp)
        {
            if (selectedDcp == null
                || selectedDcp.IsActive
                || DataCollectionDetailsViewModel.IsEditing)
            {
                Messages.Hide(_startDcpCanExecuteMessage);
                return false;
            }

            if (selectedDcp.DataSources.Count == 0 || selectedDcp.DataWriters.Count == 0)
            {
                if (!Equals(
                        Messages.DisplayedUserMessage?.Message,
                        _startDcpCanExecuteMessage.Message))
                {
                    _startDcpCanExecuteMessage.Message = new LocalizableText(
                        nameof(DataCollectionLibraryResources.DCP_LIBRARY_CANNOT_START_DCP),
                        selectedDcp.Name);
                    Messages.Show(_startDcpCanExecuteMessage);
                }

                return false;
            }

            Messages.Hide(_startDcpCanExecuteMessage);
            return true;
        }

        private void StartDcpExecute()
        {
            SelectedDcp.Start();
        }

        #endregion StartDCPCommand

        #region StopDCPCommand

        private ICommand _stopDcpCommand;

        public ICommand StopDcpCommand
            => _stopDcpCommand ??= new DelegateCommand(
                StopDcpCommandExecute,
                StopDcpCommandCanExecute);

        private bool StopDcpCommandCanExecute()
        {
            return SelectedDcp?.IsActive == true;
        }

        private void StopDcpCommandExecute()
        {
            Task.Factory.StartNew(
                () =>
                {
                    SelectedDcp.Stop();
                    DisplaySavingResults(SelectedDcp);
                });
        }

        #endregion StopDCPCommand

        #region AddDcpCommand

        public InvisibleBusinessPanelCommand AddDcpCommand { get; }

        #endregion

        #region EditDcpCommand

        public InvisibleBusinessPanelCommand EditDcpCommand { get; }

        #endregion

        #region DuplicateDcpCommand

        public InvisibleBusinessPanelCommand DuplicateDcpCommand { get; }

        private void DuplicateDcpCommandExecute()
        {
            var newDcp = (DataCollectionPlan)SelectedDcp.Clone();
            newDcp.Name = NamingStrategy.GetCloneName(newDcp.Name, DataCollectionPlanLibrarian.Plans.Select(x => x.Name).ToList());
            _ = SaveDataCollectionPlan(newDcp, true);
        }

        #endregion

        #region DeleteDcpCommand

        public InvisibleBusinessPanelCommand DeleteDcpCommand { get; }

        private void DeleteDcpCommandExecute()
        {
            var confirmationPopup = new Popup(
                nameof(DataCollectionLibraryResources.POPUP_CONFIRMATION),
                new LocalizableText(
                    nameof(DataCollectionLibraryResources.DCP_LIBRARY_REMOVE_DCP_CONFIRMATION),
                    SelectedDcp.Name)) {SeverityLevel = MessageLevel.Warning};

            confirmationPopup.Commands.Add(
                new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)));
            confirmationPopup.Commands.Add(
                new PopupCommand(
                    nameof(Agileo.GUI.Properties.Resources.S_YES),
                    new DelegateCommand(
                        () => { DataCollectionPlanLibrarian.Remove(SelectedDcp); })));

            Popups.Show(confirmationPopup);
        }

        #endregion

        #region SaveDcpCommand

        public BusinessPanelCommand SaveDcpCommand { get; }

        private bool SaveCommandCanExecute()
        {
            return DataCollectionDetailsViewModel != null
                   && DataCollectionDetailsViewModel.CanBeSaved();
        }

        private void SaveCommandExecute()
        {
            if (SaveDataCollectionPlan(
                    DataCollectionDetailsViewModel.DataCollectionPlan,
                    DataCollectionDetailsViewModel.IsNew))
            {
                DisplaySavingResults(SelectedDcp);
            }
        }

        #endregion

        #region CancelCommand

        public BusinessPanelCommand CancelCommand { get; }

        private void CancelEditionCommand()
        {
            if (_dcpInEdition != null
                && _dcpInEdition.Equals(DataCollectionDetailsViewModel.DataCollectionPlan))
            {
                CancelDcpEdition();
            }
            else
            {
                var popup =
                    new Popup(
                        nameof(DataCollectionLibraryResources.POPUP_CONFIRMATION),
                        nameof(DataCollectionLibraryResources.DCP_EDITOR_LEAVE_EDITOR_WITHOUT_SAVE))
                    {
                        Commands =
                        {
                            new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)),
                            new PopupCommand(
                                nameof(DataCollectionLibraryResources.DCP_DISCARD),
                                new DelegateCommand(CancelDcpEdition))
                        }
                    };
                Popups.Show(popup);
            }
        }

        #endregion

        #endregion Commands

        #region Private

        private void DisplayDataCollectionDetails()
        {
            DataCollectionDetailsViewModel = SelectedDcp != null
                ? new DataCollectionDetailsViewModel(
                    this,
                    (DataCollectionPlan)SelectedDcp.Clone(),
                    _equipment,
                    false)
                : null;
        }

        private void DisplayDataCollectionEditDetails(DataCollectionPlan plan, bool isNew)
        {
            if (isNew)
            {
                DataCollectionDetailsViewModel = new DataCollectionDetailsViewModel(
                    this,
                    plan,
                    _equipment,
                    true,
                    true);
                return;
            }

            _dcpInEdition = SelectedDcp;
            DataCollectionDetailsViewModel = new DataCollectionDetailsViewModel(
                this,
                (DataCollectionPlan)SelectedDcp.Clone(),
                _equipment,
                true);
        }

        private bool SaveDataCollectionPlan(DataCollectionPlan planToSave, bool isNew = false)
        {
            var savedDcp = (DataCollectionPlan)planToSave.Clone();

            if (!isNew && _dcpInEdition != null)
            {
                //if the dcp was already existing and in edition, we remove the old one form the librarian
                DataCollectionPlanLibrarian.Remove(_dcpInEdition);
                _dcpInEdition = null;
            }

            try
            {
                DataCollectionPlanLibrarian.Add(savedDcp);
                return true;
            }
            catch (Exception e)
            {
                Messages.Show(
                    new UserMessage(
                        MessageLevel.Error,
                        new LocalizableText(
                            nameof(DataCollectionLibraryResources.DCP_EDITOR_SAVING_ERROR)))
                    {
                        CanUserCloseMessage = true
                    });

                Logger.Error(
                    e,
                    "An exception occurred while saving the Data Collection Plan '{dcpName}'",
                    planToSave.Name);
                return false;
            }
        }

        private void CancelDcpEdition()
        {
            Messages.HideAll();

            if (DataCollectionDetailsViewModel.IsNew)
            {
                Messages.Show(
                    new UserMessage(
                        MessageLevel.Error,
                        new LocalizableText(
                            nameof(DataCollectionLibraryResources
                                .DCP_EDITOR_LEAVE_EDITOR_WITHOUT_SAVE)))
                    {
                        CanUserCloseMessage = true
                    });
            }

            DisplayDataCollectionDetails();
            DetailsIsExpanded = false;
        }

        private void DisplaySavingResults(DataCollectionPlan relatedDcp)
        {
            var successMessage = relatedDcp.DataWriters?.OfType<FileDataWriter>().ToList().Count > 0
                ? CreateFileSuccessMessage(relatedDcp)
                : new UserMessage(
                    MessageLevel.Success,
                    new LocalizableText(nameof(DataCollectionLibraryResources.SAVING_SUCCESS_MSG)));

            successMessage.SecondsDuration = 5;
            successMessage.CanUserCloseMessage = true;
            Messages.Show(successMessage);
        }

        private UserMessage CreateFileSuccessMessage(DataCollectionPlan relatedDcp)
        {
            var successMessage = new UserMessage(
                MessageLevel.Success,
                new LocalizableText(
                    nameof(DataCollectionLibraryResources.FILE_SAVING_SUCCESS_MSG)));

            // if stopped dcp has more than one writer, open popup with 'Open Folder' command for each writer.
            // if dcp has only one dcp, open directly output folder
            if (relatedDcp.DataWriters?.OfType<FileDataWriter>().ToList().Count > 1)
            {
                successMessage.Commands.Add(
                    new UserMessageCommand(
                        nameof(DataCollectionLibraryResources.SHOW),
                        new DelegateCommand(
                            delegate
                            {
                                Popups.Show(
                                    new Popup(
                                        PopupButtons.OK,
                                        new LocalizableText(
                                            nameof(DataCollectionLibraryResources.DETAILS)))
                                    {
                                        Content = new FolderPathDetailsPopup(
                                            relatedDcp.DataWriters.OfType<FileDataWriter>())
                                    });
                            })));
            }
            else
            {
                successMessage.Commands.Add(
                    new UserMessageCommand(
                        nameof(DataCollectionLibraryResources.OPEN),
                        new DelegateCommand(
                            () =>
                            {
                                var storageFolderPath = relatedDcp.DataWriters
                                    ?.OfType<FileDataWriter>()
                                    .FirstOrDefault()
                                    ?.FileStorageStrategy.StorageFolderPath;
                                if (storageFolderPath == null)
                                {
                                    return;
                                }

                                try
                                {
                                    var location = Assembly.GetEntryAssembly()?.Location;

                                    if (location != null)
                                    {
                                        Process.Start(Directory.GetParent(
                                                          location)
                                                      + storageFolderPath);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Logger.Error(
                                        e,
                                        "An exception occurred while opening the directory after saving the DCP '{dcpName}'",
                                        relatedDcp.Name);
                                }
                            })));
            }

            return successMessage;
        }

        private void ShowDcpEditor(bool isNewDcpCreation = false)
        {
            var newPlan = new DataCollectionPlan(string.Empty) {IsDynamic = true};

            DisplayDataCollectionEditDetails(newPlan, isNewDcpCreation);
        }

        private void UpdateCommandButtonVisibility()
        {
            if (!IsTemporaryLibrary)
            {
                return;
            }

            if (DataCollectionDetailsViewModel is {IsEditing: true})
            {
                SaveDcpCommand.IsVisible = true;
                CancelCommand.IsVisible = true;
                DetailsIsExpanded = true;
            }
            else
            {
                SaveDcpCommand.IsVisible = false;
                CancelCommand.IsVisible = false;
            }

            OnPropertyChanged(nameof(IsEditing));
        }

        private void DataCollectionPlanLibrarian_DataCollectionPlanAdded(
            object sender,
            DataCollectionPlanAddedEventArgs e)
        {
            var dcp = e.DataCollectionPlan;
            if (dcp == null)
            {
                return;
            }

            if (!dcp.IsDynamic || !IsTemporaryLibrary)
            {
                return;
            }

            DataTableSource.Add(dcp);
            SelectedDcp = dcp;
        }

        private void DataCollectionPlanLibrarian_DataCollectionPlanRemoved(
            object sender,
            DataCollectionPlanRemovedEventArgs e)
        {
            var dcp = e.DataCollectionPlan;
            if (dcp == null)
            {
                return;
            }

            DataTableSource.Remove(dcp);
        }

        #endregion

        #region IDisposable

        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (IsInDesignMode)
                {
                    foreach (var dataCollectionPlan in DataCollectionPlanLibrarian.Plans)
                    {
                        dataCollectionPlan.Dispose();
                    }
                }

                DataCollectionPlanLibrarian.DataCollectionPlanAdded -=
                    DataCollectionPlanLibrarian_DataCollectionPlanAdded;
                DataCollectionPlanLibrarian.DataCollectionPlanRemoved -=
                    DataCollectionPlanLibrarian_DataCollectionPlanRemoved;
            }

            base.Dispose(disposing);
            _disposed = true;
        }

        #endregion IDisposable
    }
}
