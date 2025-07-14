using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Agileo.Common.Localization;
using Agileo.GUI.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Popups;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Rorze.Devices.Robot.MapperRR75x.Configuration;
using UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.Robot.MapperRR75x.Popups;
using UnitySC.GUI.Common.Resources;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Robot;

namespace UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.Robot.MapperRR75x
{
    public class MappingPositionSettingsEditor : NotifyDataError
    {
        #region Properties

        public Dictionary<SampleDimension, SampleSizeMappingPositions> MappingPositions { get; }
        public BusinessPanel Panel { get; }

        public DataTreeSource<MappingBaseModel> DataTreeSource { get; set; }

        public TreeNode<MappingBaseModel> SelectedTreeNode
        {
            get => DataTreeSource.SelectedElement;
            set => DataTreeSource.SelectedElement = value;
        }

        private MappingBaseModel _selectedValue;

        public MappingBaseModel SelectedValue
        {
            get => _selectedValue;
            set
            {
                SetAndRaiseIfChanged(ref _selectedValue, value);
                OnPropertyChanged(nameof(SelectedTreeNode));
            }
        }

        private List<TreeNode<MappingBaseModel>> _allTreeElements;

        public List<TreeNode<MappingBaseModel>> AllTreeElements
        {
            get => _allTreeElements;
            set => SetAndRaiseIfChanged(ref _allTreeElements, value);
        }

        #endregion

        #region Constructor

        static MappingPositionSettingsEditor()
        {
            DataTemplateGenerator.Create(typeof(MappingPositionSettingsEditor), typeof(MappingPositionSettingsEditorView));
        }

        public MappingPositionSettingsEditor(
            Dictionary<SampleDimension, SampleSizeMappingPositions> mappingPositions,
            BusinessPanel panel)
        {
            MappingPositions = mappingPositions;
            Panel = panel;

            DataTreeSource = new DataTreeSource<MappingBaseModel>(item => item.Children);
            PopulateDataTreeSource(MappingPositions);

            Rules.Add(
                new DelegateRule(
                    nameof(MappingPositions),
                    () =>
                    {
                        var errorMessage = ValidateModel();
                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            return LocalizationManager.GetString(errorMessage);
                        }

                        return String.Empty;
                    }));
            ApplyRules();
        }

        #endregion

        #region Commands

        #region Delete Item

        private ICommand _deleteItemCommand;

        public ICommand DeleteItemCommand
            => _deleteItemCommand ??= new DelegateCommand<MappingBaseModel>(
                DeleteItemCommandExecute,
                DeleteItemCommandCanExecute);

        private bool DeleteItemCommandCanExecute(MappingBaseModel model)
        {
            return model != null;
        }

        private void DeleteItemCommandExecute(MappingBaseModel model)
        {
            switch (model)
            {
                case SampleDimensionModel sampleDimensionModel:
                    MappingPositions.Remove(sampleDimensionModel.SampleDimension);
                    break;
                case TransferLocationModel transferLocationModel:
                    MappingPositions[transferLocationModel.SampleDimension]
                        .MappingPositionsPerModule.Remove(transferLocationModel.TransferLocation);
                    break;
            }

            DataTreeSource.Remove(model);
            DisplayFlattenItems();
            OnPropertyChanged(nameof(MappingPositions));
        }

        #endregion

        #region Add Item

        private ICommand _addItemCommand;

        public ICommand AddItemCommand
            => _addItemCommand ??= new DelegateCommand<MappingBaseModel>(
                AddItemCommandExecute,
                AddItemCommandCanExecute);

        private bool AddItemCommandCanExecute(MappingBaseModel model)
        {
            if (model is TransferLocationModel
                {
                    TransferLocation: >= TransferLocation.PreAlignerA
                } transferLocationModel)
            {
                return transferLocationModel.Children.Count == 0;
            }

            return model != null;
        }

        private void AddItemCommandExecute(MappingBaseModel parent)
        {
            //Get node
            var node = DataTreeSource.GetTreeNode(parent);
            switch (node.Model)
            {
                //popup TransferLocationModel
                case SampleDimensionModel sampleDimensionModel:

                    //Get locations
                    var transferLocations = node.Nodes.Select(m => m.Model)
                        .Cast<TransferLocationModel>()
                        .Select(s => s.TransferLocation)
                        .ToList();

                    //Create popup view model
                    var transferLocationPopupModel =
                        new AddTransferLocationPopupViewModel(transferLocations);

                    //Create popup
                    var transferLocationPopup =
                        new Popup(
                            nameof(RobotSettingsResources.S_SETUP_ROBOT_ADD_TRANSFER_LOCATION))
                        {
                            Content = transferLocationPopupModel
                        };
                    transferLocationPopup.Commands.Add(
                        new PopupCommand(nameof(L10N.CMD_CANCEL), new DelegateCommand(() => { })));
                    transferLocationPopup.Commands.Add(
                        new PopupCommand(
                            nameof(L10N.CMD_VALIDATE),
                            new DelegateCommand(
                                () =>
                                {
                                    //Update configuration
                                    MappingPositions[sampleDimensionModel.SampleDimension]
                                        .MappingPositionsPerModule.Add(
                                            transferLocationPopupModel.NewTransferLocation,
                                            new ModuleMappingPosition()
                                            {
                                                ArmFirstMappingPosition = transferLocationPopupModel.ArmFirstMappingPosition,
                                                ArmSecondMappingPosition = transferLocationPopupModel.ArmSecondMappingPosition
                                            });

                                    //Update HMI
                                    var newTransferLocationModel = new TransferLocationModel(
                                        transferLocationPopupModel.NewTransferLocation,
                                        sampleDimensionModel.SampleDimension);
                                    DataTreeSource.Add(newTransferLocationModel, parent);

                                    var newMappingPositionModel = new MappingPositionModel(
                                        transferLocationPopupModel.ArmFirstMappingPosition,
                                        transferLocationPopupModel.ArmSecondMappingPosition,
                                        transferLocationPopupModel.NewTransferLocation,
                                        sampleDimensionModel.SampleDimension);
                                    DataTreeSource.Add(newMappingPositionModel, newTransferLocationModel);

                                    OnPropertyChanged(nameof(MappingPositions));
                                },
                                () => transferLocationPopupModel.ValidateTransferLocation())));

                    Panel.Popups.Show(transferLocationPopup);
                    break;

                //popup sample dimension
                default:
               
                    var sampleDimensions = node.Nodes.Select(m => m.Model)
                        .Cast<SampleDimensionModel>()
                        .Select(s => s.SampleDimension)
                        .ToList();

                    var sampleDimensionViewModel =
                        new AddSampleDimensionPopupViewModel(sampleDimensions);
                    var sampleDimensionPopup =
                        new Popup(nameof(RobotSettingsResources.S_SETUP_ROBOT_ADD_SAMPLE_DIMENSION))
                        {
                            Content = sampleDimensionViewModel
                        };
                    sampleDimensionPopup.Commands.Add(
                        new PopupCommand(nameof(L10N.CMD_CANCEL), new DelegateCommand(() => { })));
                    sampleDimensionPopup.Commands.Add(
                        new PopupCommand(
                            nameof(L10N.CMD_VALIDATE),
                            new DelegateCommand(
                                () =>
                                {
                                    MappingPositions.Add(
                                        sampleDimensionViewModel.NewSampleDimension,
                                        new SampleSizeMappingPositions());

                                    var newSampleDimension = new SampleDimensionModel(
                                        sampleDimensionViewModel.NewSampleDimension);
                                    DataTreeSource.Add(newSampleDimension, parent);
                                    OnPropertyChanged(nameof(MappingPositions));
                                },
                                () => sampleDimensionViewModel.ValidateSampleDimension())));

                    Panel.Popups.Show(sampleDimensionPopup);
                    break;
            }
        }

        #endregion

        #region Edit Item

        private ICommand _editItemCommand;

        public ICommand EditItemCommand
            => _editItemCommand ??= new DelegateCommand<MappingBaseModel>(EditItemCommandExecute);

        private void EditItemCommandExecute(MappingBaseModel parent)
        {
            //Get node
            var node = DataTreeSource.GetTreeNode(parent);
            var mappingPositionNode = (MappingPositionModel)node.Model;

            //Create popup view model
            var mappingPositionViewModel = new EditMappingPositionPopupViewModel(
                mappingPositionNode.ArmFirstMappingPosition,
                mappingPositionNode.ArmSecondMappingPosition);

            //Create popup
            var positionPopup =
                new Popup(nameof(RobotSettingsResources.S_SETUP_ROBOT_EDIT_MAPPING_POSITION))
                {
                    Content = mappingPositionViewModel
                };
            positionPopup.Commands.Add(
                new PopupCommand(nameof(L10N.CMD_CANCEL), new DelegateCommand(() => { })));
            positionPopup.Commands.Add(
                new PopupCommand(
                    nameof(L10N.CMD_VALIDATE),
                    new DelegateCommand(
                        () =>
                        {
                           var position = MappingPositions[mappingPositionNode.SampleDimension]
                                .MappingPositionsPerModule[mappingPositionNode.TransferLocation];

                           //Update configuration
                            position.ArmFirstMappingPosition = mappingPositionViewModel.ArmFirstMappingPosition;
                            position.ArmSecondMappingPosition = mappingPositionViewModel.ArmSecondMappingPosition;

                            //update HMI node
                            mappingPositionNode.ArmFirstMappingPosition = mappingPositionViewModel.ArmFirstMappingPosition;
                            mappingPositionNode.ArmSecondMappingPosition = mappingPositionViewModel.ArmSecondMappingPosition;

                            DataTreeSource.Refresh();

                            OnPropertyChanged(nameof(MappingPositions));
                        },
                        () => mappingPositionViewModel.ValidateMappingPosition())));

            //Show Popup
            Panel.Popups.Show(positionPopup);
        }

        #endregion

        #endregion

        #region Private

        private void DisplayFlattenItems()
        {
            AllTreeElements = DataTreeSource.GetFlattenElements();
        }

        private void PopulateDataTreeSource(
            Dictionary<SampleDimension, SampleSizeMappingPositions> mappingPositions)
        {
            var elements = new List<MappingBaseModel>();
            var rootElement = new MappingBaseModel();

            foreach (var valuePair in mappingPositions)
            {
                var sampleDimensionModel = new SampleDimensionModel(valuePair.Key);

                if (valuePair.Value != null)
                {
                    foreach (var mappingPositionsPerModule in valuePair.Value
                                 .MappingPositionsPerModule)
                    {
                        var transferLocationModel = new TransferLocationModel(
                            mappingPositionsPerModule.Key,
                            valuePair.Key);

                        var position = mappingPositionsPerModule.Value;
                        if (position != null)
                        {

                            var mappingPositionModel = new MappingPositionModel(
                                position.ArmFirstMappingPosition,
                                position.ArmSecondMappingPosition,
                                mappingPositionsPerModule.Key,
                                valuePair.Key);

                            transferLocationModel.Children.Add(mappingPositionModel);
                        }

                        sampleDimensionModel.Children.Add(transferLocationModel);
                    }
                }

                rootElement.Children.Add(sampleDimensionModel);
            }

            elements.Add(rootElement);

            DataTreeSource.Reset(elements);
        }

        public string ValidateModel()
        {
            if (MappingPositions.Count == 0)
            {
                return nameof(RobotSettingsResources.STOPPING_POSITIONS_ERROR_EMPTY);
            }

            foreach (var mappingPositionsPerModule in MappingPositions.Select(
                         s => s.Value.MappingPositionsPerModule))
            {
                if (mappingPositionsPerModule.Count == 0)
                {
                    return nameof(RobotSettingsResources
                        .STOPPING_POSITIONS_ERROR_TRANSFER_LOCATION_EMPTY);
                }

                foreach (var moduleMappingPositions in mappingPositionsPerModule)
                {

                    if (moduleMappingPositions.Key >= TransferLocation.LoadPort9)
                    {
                        //TODO erreur location is not a load port
                        return nameof(RobotSettingsResources
                            .STOPPING_POSITIONS_ERROR_INNER_POSITION_EMPTY);
                    }

                    if (moduleMappingPositions.Value == null)
                    {
                        //Todo
                        return nameof(RobotSettingsResources
                            .STOPPING_POSITIONS_ERROR_INNER_POSITION_EMPTY);
                    }
                }
            }

            return string.Empty;
        }

        #endregion
    }

    public class MappingBaseModel
    {
        public SampleDimension SampleDimension { get; set; }

        public TransferLocation TransferLocation { get; set; }

        public uint ArmFirstMappingPosition { get; set; }

        public uint ArmSecondMappingPosition { get; set; }

        public List<MappingBaseModel> Children { get; set; } = new();
    }

    public class SampleDimensionModel : MappingBaseModel
    {
        public SampleDimensionModel(SampleDimension sampleDimension)
        {
            SampleDimension = sampleDimension;
        }
    }

    public class TransferLocationModel : MappingBaseModel
    {
        public TransferLocationModel(
            TransferLocation transferLocation,
            SampleDimension sampleDimension)
        {
            TransferLocation = transferLocation;
            SampleDimension = sampleDimension;
        }
    }

    public class MappingPositionModel : MappingBaseModel
    {
        public MappingPositionModel(
            uint armFirstMappingPosition,
            uint armSecondMappingPosition,
            TransferLocation transferLocation,
            SampleDimension sampleDimension)
        {
            ArmFirstMappingPosition = armFirstMappingPosition;
            ArmSecondMappingPosition = armSecondMappingPosition;
            TransferLocation = transferLocation;
            SampleDimension = sampleDimension;
        }
    }
}
