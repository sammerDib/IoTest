using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Agileo.Common.Localization;
using Agileo.GUI.Commands;
using Agileo.GUI.Services.Popups;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Brooks.Devices.Robot.BrooksRobot.Configuration;
using UnitySC.EFEM.Brooks.GUI.Views.Panels.Setup.DeviceSettings.Robot.BrooksRobot.Popups;
using UnitySC.GUI.Common.Resources;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.EFEM.Brooks.GUI.Views.Panels.Setup.DeviceSettings.Robot.BrooksRobot
{
    public class BrooksPositionSettingsEditor : NotifyDataError
    {
        #region Properties

        public Dictionary<SampleDimension, SampleSizeStoppingPositions> StoppingPositions { get; }
        public BrooksRobotSettingsPanel Panel { get; }
        public DataTreeSource<BaseModel> DataTreeSource { get; set; }

        public TreeNode<BaseModel> SelectedTreeNode
        {
            get => DataTreeSource.SelectedElement;
            set => DataTreeSource.SelectedElement = value;
        }

        private BaseModel _selectedValue;

        public BaseModel SelectedValue
        {
            get => _selectedValue;
            set
            {
                SetAndRaiseIfChanged(ref _selectedValue, value);
                OnPropertyChanged(nameof(SelectedTreeNode));
            }
        }

        private List<TreeNode<BaseModel>> _allTreeElements;

        public List<TreeNode<BaseModel>> AllTreeElements
        {
            get => _allTreeElements;
            set => SetAndRaiseIfChanged(ref _allTreeElements, value);
        }

        #endregion

        #region Constructor

        static BrooksPositionSettingsEditor()
        {
            DataTemplateGenerator.Create(typeof(BrooksPositionSettingsEditor), typeof(BrooksPositionSettingsEditorView));
        }

        public BrooksPositionSettingsEditor(
            Dictionary<SampleDimension, SampleSizeStoppingPositions> stoppingPositions,
            BrooksRobotSettingsPanel panel)
        {
            StoppingPositions = stoppingPositions;
            Panel = panel;

            DataTreeSource = new DataTreeSource<BaseModel>(item => item.Children);
            PopulateDataTreeSource(StoppingPositions);

            Rules.Add(
                new DelegateRule(
                    nameof(StoppingPositions),
                    () =>
                    {
                        var errorMessage = ValidateModel();
                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            return LocalizationManager.GetString(errorMessage);
                        }

                        return string.Empty;
                    }));
            ApplyRules();
        }

        #endregion

        #region Commands

        #region Delete Item

        private ICommand _deleteItemCommand;

        public ICommand DeleteItemCommand
            => _deleteItemCommand ??= new DelegateCommand<BaseModel>(
                DeleteItemCommandExecute,
                DeleteItemCommandCanExecute);

        private bool DeleteItemCommandCanExecute(BaseModel model)
        {
            return model != null;
        }

        private void DeleteItemCommandExecute(BaseModel model)
        {
            switch (model)
            {
                case SampleDimensionModel sampleDimensionModel:
                    StoppingPositions.Remove(sampleDimensionModel.SampleDimension);
                    break;
                case TransferLocationModel transferLocationModel:
                    StoppingPositions[transferLocationModel.SampleDimension]
                        .StoppingPositionsPerModule.Remove(transferLocationModel.TransferLocation);
                    break;
            }

            DataTreeSource.Remove(model);
            DisplayFlattenItems();
            OnPropertyChanged(nameof(StoppingPositions));
        }

        #endregion

        #region Add Item

        private ICommand _addItemCommand;

        public ICommand AddItemCommand
            => _addItemCommand ??= new DelegateCommand<BaseModel>(
                AddItemCommandExecute,
                AddItemCommandCanExecute);

        private bool AddItemCommandCanExecute(BaseModel model)
        {
            return model != null;
        }

        private void AddItemCommandExecute(BaseModel parent)
        {
            var node = DataTreeSource.GetTreeNode(parent);
            switch (node.Model)
            {
                case SampleDimensionModel sampleDimensionModel:
                    //popup TransferLocationModel
                    var transferLocations = node.Nodes.Select(m => m.Model)
                        .Cast<TransferLocationModel>()
                        .Select(s => s.TransferLocation)
                        .ToList();

                    var transferLocationPopupModel =
                        new AddTransferLocationPopupViewModel(transferLocations);
                    var transferLocationPopup =
                        new Popup(
                            nameof(BrooksRobotSettingsResource.ROBOT_ADD_TRANSFER_LOCATION))
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
                                    StoppingPositions[sampleDimensionModel.SampleDimension]
                                        .StoppingPositionsPerModule.Add(
                                            transferLocationPopupModel.NewTransferLocation, new ModuleStoppingPosition());

                                    var newTransferLocationModel = new TransferLocationModel(
                                        transferLocationPopupModel.NewTransferLocation,
                                        sampleDimensionModel.SampleDimension);
                                    DataTreeSource.Add(newTransferLocationModel, parent);

                                    var newStoppingPositionModel = new StoppingPositionModel(
                                        transferLocationPopupModel.XPosition,
                                        transferLocationPopupModel.ThetaPosition,
                                        newTransferLocationModel.TransferLocation,
                                        newTransferLocationModel.SampleDimension);

                                    DataTreeSource.Add(newStoppingPositionModel, newTransferLocationModel);

                                    OnPropertyChanged(nameof(StoppingPositions));
                                },
                                () => transferLocationPopupModel.ValidateTransferLocation())));

                    Panel.Popups.Show(transferLocationPopup);
                    break;
                default:
                    //popup sample dimension
                    var sampleDimensions = node.Nodes.Select(m => m.Model)
                        .Cast<SampleDimensionModel>()
                        .Select(s => s.SampleDimension)
                        .ToList();

                    var sampleDimensionViewModel =
                        new AddSampleDimensionPopupViewModel(sampleDimensions);
                    var sampleDimensionPopup =
                        new Popup(nameof(BrooksRobotSettingsResource.ROBOT_ADD_SAMPLE_DIMENSION))
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
                                    StoppingPositions.Add(
                                        sampleDimensionViewModel.NewSampleDimension,
                                        new SampleSizeStoppingPositions());

                                    var newSampleDimension = new SampleDimensionModel(
                                        sampleDimensionViewModel.NewSampleDimension);
                                    DataTreeSource.Add(newSampleDimension, parent);
                                    OnPropertyChanged(nameof(StoppingPositions));
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
            => _editItemCommand ??= new DelegateCommand<BaseModel>(EditItemCommandExecute);

        private void EditItemCommandExecute(BaseModel parent)
        {
            var node = DataTreeSource.GetTreeNode(parent);
            var stoppingPositionNode = (StoppingPositionModel)node.Model;

            var stoppingPositionViewModel = new EditPositionPopupViewModel(stoppingPositionNode.XPosition, stoppingPositionNode.ThetaPosition);
            var stoppingPositionPopup =
                new Popup(nameof(BrooksRobotSettingsResource.ROBOT_EDIT_STOPPING_POSITION))
                {
                    Content = stoppingPositionViewModel
                };
            stoppingPositionPopup.Commands.Add(
                new PopupCommand(nameof(L10N.CMD_CANCEL), new DelegateCommand(() => { })));
            stoppingPositionPopup.Commands.Add(
                new PopupCommand(
                    nameof(L10N.CMD_VALIDATE),
                    new DelegateCommand(
                        () =>
                        {
                            StoppingPositions[stoppingPositionNode.SampleDimension]
                                .StoppingPositionsPerModule[stoppingPositionNode.TransferLocation].XPosition = stoppingPositionViewModel.XPosition;
                            StoppingPositions[stoppingPositionNode.SampleDimension]
                                .StoppingPositionsPerModule[stoppingPositionNode.TransferLocation].ThetaPosition = stoppingPositionViewModel.ThetaPosition;

                            stoppingPositionNode.XPosition = stoppingPositionViewModel.XPosition;
                            stoppingPositionNode.ThetaPosition = stoppingPositionViewModel.ThetaPosition;

                            DataTreeSource.Refresh();

                            OnPropertyChanged(nameof(StoppingPositions));
                        },
                        () => stoppingPositionViewModel.ValidateStoppingPosition())));

            Panel.Popups.Show(stoppingPositionPopup);
        }

        #endregion

        #endregion

        #region Private

        private void DisplayFlattenItems()
        {
            AllTreeElements = DataTreeSource.GetFlattenElements();
        }

        private void PopulateDataTreeSource(
            Dictionary<SampleDimension, SampleSizeStoppingPositions> stoppingPositions)
        {
            var elements = new List<BaseModel>();
            var rootElement = new BaseModel();

            foreach (var valuePair in stoppingPositions)
            {
                var sampleDimensionModel = new SampleDimensionModel(valuePair.Key);

                if (valuePair.Value != null)
                {
                    foreach (var stoppingPositionsPerModule in valuePair.Value
                                 .StoppingPositionsPerModule)
                    {
                        var transferLocationModel = new TransferLocationModel(
                            stoppingPositionsPerModule.Key,
                            valuePair.Key);

                        if (stoppingPositionsPerModule.Value != null)
                        {
                            var stoppingPositionModel = new StoppingPositionModel(
                                stoppingPositionsPerModule.Value.XPosition,
                                stoppingPositionsPerModule.Value.ThetaPosition,
                                stoppingPositionsPerModule.Key,
                                valuePair.Key);

                            transferLocationModel.Children.Add(stoppingPositionModel);
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
            if (StoppingPositions.Count == 0)
            {
                return nameof(BrooksRobotSettingsResource.STOPPING_POSITIONS_ERROR_EMPTY);
            }

            foreach (var stoppingPositionsPerModule in StoppingPositions.Select(
                         s => s.Value.StoppingPositionsPerModule))
            {
                if (stoppingPositionsPerModule.Count == 0)
                {
                    return nameof(BrooksRobotSettingsResource
                        .STOPPING_POSITIONS_ERROR_TRANSFER_LOCATION_EMPTY);
                }

                foreach (var moduleStoppingPositions in stoppingPositionsPerModule)
                {
                    if (moduleStoppingPositions.Value == null)
                    {
                        return nameof(BrooksRobotSettingsResource
                            .STOPPING_POSITIONS_ERROR_STOPPING_POSITION_EMPTY);
                    }
                }
            }

            return string.Empty;
        }

        #endregion
    }

    public class BaseModel
    {
        public SampleDimension SampleDimension { get; set; }

        public TransferLocation TransferLocation { get; set; }

        public double XPosition { get; set; }

        public double ThetaPosition { get; set; }

        public List<BaseModel> Children { get; set; } = new();
    }

    public class SampleDimensionModel : BaseModel
    {
        public SampleDimensionModel(SampleDimension sampleDimension)
        {
            SampleDimension = sampleDimension;
        }
    }

    public class TransferLocationModel : BaseModel
    {
        public TransferLocationModel(
            TransferLocation transferLocation,
            SampleDimension sampleDimension)
        {
            TransferLocation = transferLocation;
            SampleDimension = sampleDimension;
        }
    }

    public class StoppingPositionModel : BaseModel
    {
        public StoppingPositionModel(
            double xPosition,
            double thetaPosition,
            TransferLocation transferLocation,
            SampleDimension sampleDimension)
        {
            XPosition = xPosition;
            ThetaPosition = thetaPosition;
            TransferLocation = transferLocation;
            SampleDimension = sampleDimension;
        }
    }
}
