using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

using Agileo.DataMonitoring;
using Agileo.DataMonitoring.DataSource.Device;
using Agileo.DataMonitoring.DataSource.MessageDataBus;
using Agileo.DataMonitoring.DataWriter.Chart;
using Agileo.DataMonitoring.DataWriter.File;
using Agileo.DataMonitoring.DataWriter.File.StorageStrategy;
using Agileo.DataMonitoring.DataWriter.File.WriteEventTrigger;
using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;

using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.Library;

using UnitsNet;
using UnitsNet.Units;

using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.Editors.DataSourceEditor;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.Editors.DataWriterEditor;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.Library
{
    public class DataCollectionDetailsViewModel : Notifier
    {
        #region Properties

        private readonly List<string> _validationErrors = new();

        private DataCollectionLibraryPanel OwnerPanel { get; }

        public DataCollectionPlan DataCollectionPlan { get; set; }

        public bool IsEditing { get; set; }

        public bool IsNew { get; }

        private bool _canBeSaved;

        public string DcpName
        {
            get => DataCollectionPlan.Name;
            set
            {
                if (DataCollectionPlan.Name == value)
                {
                    return;
                }

                DataCollectionPlan.Name = value;
                OnPropertyChanged(nameof(DcpName));
                _ = ValidateDcpName();
            }
        }

        public string DcpDescription
        {
            get => DataCollectionPlan.Description;
            set
            {
                if (DataCollectionPlan.Description == value)
                {
                    return;
                }

                DataCollectionPlan.Description = value;
                OnPropertyChanged(nameof(DcpDescription));
            }
        }

        public double DcpFrequency
        {
            get => Convert.ToDouble(DataCollectionPlan.Frequency);
            set
            {
                try
                {
                    DataCollectionPlan.Frequency = new Frequency(value, FrequencyUnit.Hertz);
                }
                catch
                {
                    //ignored since the model will not be updated.
                }

                OnPropertyChanged(nameof(DcpFrequency));
                OnPropertyChanged(nameof(SelectedDcpPeriod));
            }
        }

        public string SelectedDcpPeriod
            => (1 / DataCollectionPlan?.Frequency.Hertz ?? 0).ToString("F");

        public List<MdbTagDataSource> DcpSourcesMdb
            => DcpSources?.OfType<MdbTagDataSource>().ToList();

        public List<DeviceDataSource> DcpSourcesDevices
            => DcpSources?.OfType<DeviceDataSource>().ToList();

        public ReadOnlyCollection<IDataSource> DcpSources => DataCollectionPlan.DataSources;

        public List<IDataWriter> DcpWriters => DataCollectionPlan?.DataWriters?.ToList();

        private IDataSource _selectedDcpSource;

        public IDataSource SelectedDcpSource
        {
            get => _selectedDcpSource;
            set
            {
                if (_selectedDcpSource == value)
                {
                    return;
                }

                _selectedDcpSource = value;
                OnPropertyChanged(nameof(SelectedDcpSource));
            }
        }

        private IDataWriter _selectedDcpWriter;

        public IDataWriter SelectedDcpWriter
        {
            get => _selectedDcpWriter;
            set
            {
                if (_selectedDcpWriter == value)
                {
                    return;
                }

                _selectedDcpWriter = value;
                OnPropertyChanged(nameof(SelectedDcpWriter));
            }
        }

        #endregion

        #region Constructor

        public DataCollectionDetailsViewModel(
            DataCollectionLibraryPanel ownerPanel,
            DataCollectionPlan dataCollectionPlan,
            Agileo.EquipmentModeling.Equipment equipment,
            bool isEditing,
            bool isNew = false)
        {
            OwnerPanel = ownerPanel;
            DataCollectionPlan = dataCollectionPlan;
            IsEditing = isEditing;
            IsNew = isNew;
            _equipment = equipment;

            if (IsNew)
            {
                DcpFrequency = 10;
            }
        }

        #endregion

        #region Commands

        #region EditWriter

        private ICommand _editDataWriterCommand;

        public ICommand EditDataWriterCommand
            => _editDataWriterCommand ??= new DelegateCommand(
                EditDataWriterCommandExecuteMethod,
                () => SelectedDcpWriter is FileDataWriter);

        private void EditDataWriterCommandExecuteMethod()
        {
            try
            {
                ShowDataWriterEditor(SelectedDcpWriter.GetType());
            }
            catch
            {
                OwnerPanel.Messages.Show(
                    new UserMessage(
                        MessageLevel.Error,
                        nameof(DataCollectionLibraryResources.DCP_EDITOR_SAVING_ERROR))
                    {
                        CanUserCloseMessage = true
                    });
            }
        }

        #endregion EditWriter

        #region AddCommand

        private ICommand _addCommand;

        public ICommand AddCommand
            => _addCommand ??= new DelegateCommand<string>(AddCommandExecute, _ => true);

        private void AddCommandExecute(string itemToAdd)
        {
            Popup popup;

            switch (itemToAdd)
            {
                case "Source":
                    var sourcePopup = new EquipmentDataSourceEditor(_equipment, DataCollectionPlan);
                    popup = new Popup(
                        new LocalizableText(
                            nameof(DataCollectionLibraryResources
                                .DCP_EDITOR_SOURCE_TYPE_SELECTION_TITLE)),
                        new InvariantText(""))
                    { Content = sourcePopup };
                    popup.Commands.Add(
                        new PopupCommand(
                            nameof(Agileo.GUI.Properties.Resources.S_CANCEL),
                            PopupResult.Cancel));
                    popup.Commands.Add(
                        new PopupCommand(
                            nameof(Agileo.GUI.Properties.Resources.S_OK),
                            new DelegateCommand(
                                () => AddDeviceSource(
                                    sourcePopup.SelectedDevice,
                                    sourcePopup.SelectedStatus),
                                () => sourcePopup.SelectedStatus != null)));
                    OwnerPanel.Popups.Show(popup);
                    break;

                case "Writer":
                    var writerPopup = new DataWriterTypeSelectionPopup();
                    popup = new Popup(
                        nameof(DataCollectionLibraryResources
                            .DCP_EDITOR_WRITER_TYPE_SELECTION_TITLE),
                        nameof(DataCollectionLibraryResources.DCP_EDITOR_WRITER_TYPE_SELECTION))
                    {
                        Content = writerPopup
                    };
                    popup.Commands.Add(
                        new PopupCommand(
                            nameof(Agileo.GUI.Properties.Resources.S_CANCEL),
                            PopupResult.Cancel));
                    popup.Commands.Add(
                        new PopupCommand(
                            nameof(Agileo.GUI.Properties.Resources.S_OK),
                            new DelegateCommand(
                                () => ShowDataWriterEditor(writerPopup.SelectedWriterType),
                                () => writerPopup.SelectedWriterType != null)));
                    OwnerPanel.Popups.Show(popup);
                    break;
            }
        }

        private void ShowDataWriterEditor(Type writerType)
        {
            if (writerType == null)
            {
                throw new ArgumentNullException(nameof(writerType));
            }

            if (writerType.IsSubclassOf(typeof(FileDataWriter)))
            {
                writerType = typeof(FileDataWriter);
            }

            switch (writerType.Name)
            {
                case nameof(FileDataWriter):
                    var fileWriterEditor =
                        new FileDataWriterEditor(SelectedDcpWriter as FileDataWriter);
                    var popup = new Popup(
                        nameof(DataCollectionLibraryResources.DCP_EDITOR_WRITER_EDITOR_TITLE),
                        string.Empty) {Content = fileWriterEditor};
                    popup.Commands.Add(
                        new PopupCommand(
                            nameof(Agileo.GUI.Properties.Resources.S_CANCEL),
                            PopupResult.Cancel));
                    popup.Commands.Add(
                        new PopupCommand(
                            nameof(Agileo.GUI.Properties.Resources.S_OK),
                            new DelegateCommand(
                                () => AddOrReplaceFileWriter(fileWriterEditor),
                                () => fileWriterEditor.OkCommandCanExecute())));
                    OwnerPanel.Popups.Show(popup);
                    break;

                case nameof(ChartDataWriter):
                    DataCollectionPlan.AddDataWriter(new ChartDataWriter());
                    OnPropertyChanged(nameof(DcpWriters));

                    OwnerPanel.Logger.Info("A chart data writer has been added to the Data Collection Plan '{dcpName}'.", DataCollectionPlan.Name);
                    break;
            }
        }

        private void AddOrReplaceFileWriter(FileDataWriterEditor fileWriterEditor)
        {
            if (SelectedDcpWriter == null)
            {
                AddFileWriter(fileWriterEditor);
            }
            else
            {
                ReplaceFileWriter(fileWriterEditor);
            }

            OnPropertyChanged(nameof(DcpWriters));
        }

        private void AddFileWriter(FileDataWriterEditor fileDataWriterEditor)
        {
            var extension = fileDataWriterEditor.SelectedFileExtension.Value;

            var storageStrategy = Activator.CreateInstance(
                fileDataWriterEditor.SelectedFileStorageStrategy,
                fileDataWriterEditor.FileName,
                fileDataWriterEditor.StorageFolderPath);

            var writeTrigger = fileDataWriterEditor.SelectedFileWriteStrategy.Name
                               == nameof(CyclicFileWriteEventTrigger)
                ? Activator.CreateInstance(
                    fileDataWriterEditor.SelectedFileWriteStrategy,
                    fileDataWriterEditor.FileWritingFrequency)
                : Activator.CreateInstance(fileDataWriterEditor.SelectedFileWriteStrategy);

            var writerInstance = Activator.CreateInstance(extension, storageStrategy, writeTrigger);

            DataCollectionPlan.AddDataWriter(writerInstance as IDataWriter);

            OwnerPanel.Logger.Info("A file writer with the file named '{fwName}' has been added to the Data Collection Plan '{dcpName}'.", fileDataWriterEditor.FileName,DataCollectionPlan.Name);
        }

        private void ReplaceFileWriter(FileDataWriterEditor fileDataWriterEditor)
        {
            _ = DataCollectionPlan.RemoveDataWriter(SelectedDcpWriter);
            AddFileWriter(fileDataWriterEditor);
        }

        private void AddDeviceSource(Device device, DeviceStatus deviceStatus)
        {
            DataCollectionPlan.AddDataSource(new DeviceDataSource(device, deviceStatus));
            if (DataCollectionPlan.ExecutionCount > 0)
            {
                UpdateFileWriteStrategies();
            }

            OnPropertyChanged(nameof(DcpSourcesDevices));

            OwnerPanel.Logger.Info("A source device named '{sourceDeviceName}' has been added to the Data Collection Plan '{dcpName}'.", device.Name, DataCollectionPlan.Name);
        }

        private void UpdateFileWriteStrategies()
        {
            // check if writer with SingleFileStorageStrategy exists
            if (DataCollectionPlan.DataWriters.OfType<FileDataWriter>()
                .Any(writer => writer.FileStorageStrategy is SingleFileStorageStrategy))
            {
                OwnerPanel.Messages.Show(
                    new UserMessage(
                        MessageLevel.Warning,
                        nameof(DataCollectionLibraryResources
                            .DCP_EDITOR_SOURCE_LIST_HAS_BEEN_MODIFIED)) {SecondsDuration = 15});
            }
        }

        #endregion

        #region RemoveCommand

        private ICommand _removeCommand;
        private readonly Agileo.EquipmentModeling.Equipment _equipment;

        public ICommand RemoveCommand
            => _removeCommand ??= new DelegateCommand<string>(
                RemoveCommandExecute,
                RemoveCommandCanExecute);

        private bool RemoveCommandCanExecute(string itemToRemove)
        {
            return itemToRemove switch
            {
                "Source" => SelectedDcpSource != null && (DataCollectionPlan.IsDynamic || DataCollectionPlan.DataSources.Count > 1),
                "Writer" => SelectedDcpWriter != null && DataCollectionPlan.IsDynamic,
                _ => false
            };
        }

        private void RemoveCommandExecute(string itemToRemove)
        {
            var confirmationPopup = new Popup(
                nameof(DataCollectionLibraryResources.POPUP_CONFIRMATION),
                new LocalizableText(
                    nameof(DataCollectionLibraryResources.DCP_EDITOR_REMOVE_ELEMENT_CONFIRMATION),
                    itemToRemove.ToLower()))
            { SeverityLevel = MessageLevel.Warning };

            confirmationPopup.Commands.Add(
                new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)));
            confirmationPopup.Commands.Add(
                new PopupCommand(
                    nameof(Agileo.GUI.Properties.Resources.S_OK),
                    new DelegateCommand(
                        () =>
                        {
                            switch (itemToRemove)
                            {
                                case "Source":
                                    DataCollectionPlan.RemoveDataSource(SelectedDcpSource);
                                    OnPropertyChanged(nameof(DcpSources));
                                    OnPropertyChanged(nameof(DcpSourcesMdb));
                                    OnPropertyChanged(nameof(DcpSourcesDevices));
                                    if (DataCollectionPlan.ExecutionCount > 0)
                                    {
                                        UpdateFileWriteStrategies();
                                    }

                                    OwnerPanel.Logger.Info("A DCP source has been removed from the Data Collection Plan '{dcpName}'.", DataCollectionPlan.Name);
                                    break;

                                case "Writer":
                                    DataCollectionPlan.RemoveDataWriter(SelectedDcpWriter);

                                    OwnerPanel.Logger.Info("A DCP writer has been removed from the Data Collection Plan '{dcpName}'.", DataCollectionPlan.Name);
                                    break;
                            }

                            OnPropertyChanged(nameof(DcpWriters));
                        })));

            OwnerPanel.Popups.Show(confirmationPopup);
        }

        #endregion

        #endregion

        #region Methods

        private bool ValidateDcpName()
        {
            if (string.IsNullOrWhiteSpace(DcpName))
            {
                _validationErrors.Add(
                    nameof(DataCollectionLibraryResources.DCP_EDITOR_NAME_CANNOT_BE_EMPTY));
                _canBeSaved = false;
            }
            else if (DcpName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                _validationErrors.Add(
                    nameof(DataCollectionLibraryResources.DCP_EDITOR_NAME_INVALID_CHARACTERS));
                _canBeSaved = false;
            }
            else if (IsNew
                     && App.Instance.DataCollectionPlanLibrarian.Plans.ToList()
                         .Exists(dcp => string.Equals(dcp.Name, DcpName)))
            {
                _validationErrors.Add(
                    nameof(DataCollectionLibraryResources.DCP_EDITOR_NAME_ALREADY_EXISTS));
                _canBeSaved = false;
            }
            else
            {
                _canBeSaved = true;
            }

            return _canBeSaved;
        }

        public bool CanBeSaved()
        {
            if (ValidateDcpName())
            {
                _validationErrors.Clear();
            }

            return (!_validationErrors?.Any() ?? true) && _canBeSaved;
        }

        #endregion
    }
}
