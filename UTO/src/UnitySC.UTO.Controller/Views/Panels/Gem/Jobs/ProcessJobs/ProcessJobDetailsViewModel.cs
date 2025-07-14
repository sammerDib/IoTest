using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.Common.Localization;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Popups;
using Agileo.Semi.Gem300.Abstractions.E40;
using Agileo.Semi.Gem300.Abstractions.E87;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;
using UnitySC.UTO.Controller.Remote;
using UnitySC.UTO.Controller.Views.Panels.Gem.Equipment;
using UnitySC.UTO.Controller.Views.Panels.Gem.Popups;

using Status = Agileo.Semi.Gem300.Abstractions.E40.Status;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.Jobs.ProcessJobs
{
    public class ProcessJobDetailsViewModel : JobDetailsViewModel
    {
        #region Members

        private IProcessJob _processJob;

        private readonly PopupDisplayer _popupDisplay;

        #endregion

        #region Constructor

        public ProcessJobDetailsViewModel(
            IProcessJob processJob,
            PopupDisplayer popupDisplay,
            bool isInEdition,
            bool isInCreation)
        : this (popupDisplay, isInEdition, isInCreation)
        {
            ProcessJob = processJob;

            var pj = GemController.E40Std.ProcessJobs.FirstOrDefault(
                pj => pj.ObjID.Equals(processJob.ObjID));
            Index = GemController.E40Std.ProcessJobs.IndexOf(pj);
            
            if (ProcessJob.MaterialType == MaterialType.Carriers)
            {
                foreach (var material in ProcessJob.CarrierIDSlotsAssociation)
                {
                    var e87Carrier = GemController.E87Std.GetCarrierById(material.CarrierID);
                    CarriersDetails.Add(
                        new CorrespondingCarrierSlotMapViewModel(material, e87Carrier));
                }
            }
        }

        public ProcessJobDetailsViewModel(
            PopupDisplayer popupDisplay,
            bool isInEdition,
            bool isInCreation)
        {

            IsInEdition = isInEdition;
            IsInCreation = isInCreation;

            _popupDisplay = popupDisplay;

            CarriersDetails = new List<CorrespondingCarrierSlotMapViewModel>();

            IsCarrierListOpened = CarriersDetails.Count <= 3;
            IsErrorListOpened = Errors.Count <= 3;

            if (IsInGlobalEdition)
            {
                Rules.Add(new DelegateRule(nameof(EditingObjId), ValidateEditingId));
                Rules.Add(new DelegateRule(nameof(EditingRecipeId), ValidateEditingRecipeId));
                Rules.Add(
                    new DelegateRule(nameof(EditingCarriersDetails), ValidateEditingCarriers));
                ApplyRules();
            }
        }

        public ProcessJobDetailsViewModel()
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        #endregion

        #region Properties

        private static GemController GemController => App.ControllerInstance.GemController;

        public IProcessJob ProcessJob
        {
            get => _processJob;
            set => SetAndRaiseIfChanged(ref _processJob, value);
        }

        public List<CorrespondingCarrierSlotMapViewModel> CarriersDetails { get; }

        public List<string> AvailableRecipe => App.UtoInstance.GemController.GetPPList().ToList();

        #region Edition properties

        private string _editingObjId;

        public string EditingObjId
        {
            get => _editingObjId;
            set => SetAndRaiseIfChanged(ref _editingObjId, value);
        }

        private ProcessStart _editingProcessStart;

        public ProcessStart EditingProcessStart
        {
            get => _editingProcessStart;
            set => SetAndRaiseIfChanged(ref _editingProcessStart, value);
        }

        private ObservableCollection<string> _editingPauseEvent = new();

        public ObservableCollection<string> EditingPauseEvent
        {
            get => _editingPauseEvent;
            set => SetAndRaiseIfChanged(ref _editingPauseEvent, value);
        }

        private string _editingRecipeId;

        public string EditingRecipeId
        {
            get => _editingRecipeId;
            set
            {
                if (SetAndRaiseIfChanged(ref _editingRecipeId, value))
                {
                    OnPropertyChanged(nameof(IsEditingRecipeIdNull));
                }
            }
        }

        public bool IsEditingRecipeIdNull => string.IsNullOrEmpty(EditingRecipeId);

        public RecipeMethod EditingRecipeMethod => RecipeMethod.RecipeOnly;

        private List<RecipeVariable> _editingRecipeVariables = new();

        public List<RecipeVariable> EditingRecipeVariables
        {
            get => _editingRecipeVariables;
            set => SetAndRaiseIfChanged(ref _editingRecipeVariables, value);
        }

        private MaterialType _editingMaterialType = MaterialType.Carriers;

        public MaterialType EditingMaterialType
        {
            get => _editingMaterialType;
            set => SetAndRaiseIfChanged(ref _editingMaterialType, value);
        }

        private ObservableCollection<CorrespondingCarrierSlotMapViewModel> _editingCarriersDetails =
            new();

        public ObservableCollection<CorrespondingCarrierSlotMapViewModel> EditingCarriersDetails
        {
            get => _editingCarriersDetails;
            set
            {
                if (SetAndRaiseIfChanged(ref _editingCarriersDetails, value))
                {
                    OnPropertyChanged(nameof(IsEditingCarriersEmpty));
                }
            }
        }

        public bool IsEditingCarriersEmpty => EditingCarriersDetails.Count == 0;

        #endregion

        #endregion

        #region Public method

        public Status Save()
        {
            if (IsInCreation)
            {
                var recipe = new Recipe { ID = EditingRecipeId, Method = EditingRecipeMethod };
                var result = GemController.E40Std.StandardServices.CreateEnh(
                    EditingObjId,
                    EditingCarriersDetails.Select(c => c.MaterialNameListElement).ToList(),
                    recipe,
                    EditingProcessStart,
                    EditingPauseEvent);

                if (result.Status.IsSuccess)
                {
                    ProcessJob = GemController.E40Std.GetProcessJob(EditingObjId);
                }

                return result.Status;
            }

            if (IsInEdition)
            {
                var res = GemController.E40Std.StandardServices
                    .SetStartMethod(new List<string> { ProcessJob.ObjID }, EditingProcessStart);
                return res.Status;
            }

            return Status.Unsuccessful();
        }

        private void AddCarrier(string carrierId)
        {
            //retrieve carrier info from E87
            var e87Carrier = GemController.E87Std.GetCarrierById(carrierId);
            if (e87Carrier != null)
            {
                var slotIDs = new List<byte>();
                for (byte i = 0; i < e87Carrier.SlotMap.Count; i++)
                {
                    if (e87Carrier.SlotMap[i] != SlotState.CorrectlyOccupied)
                    {
                        continue;
                    }
                    //full slot map by default
                    slotIDs.Add((byte)(i + 1));
                }
                var carrier = new MaterialNameListElement(carrierId, slotIDs);
                EditingCarriersDetails.Add(
                    new CorrespondingCarrierSlotMapViewModel(carrier, e87Carrier));
                OnPropertyChanged(nameof(EditingCarriersDetails));
            }
        }

        public void SetEditingCarriersDetails(List<string> carrierIdList)
        {
            EditingCarriersDetails.Clear();
            foreach (var carrierId in carrierIdList)
            {
                AddCarrier(carrierId);
            }

            OnPropertyChanged(nameof(EditingCarriersDetails));
        }

        public void DeleteCarrierSubstrate(
            CorrespondingCarrierSlotMapViewModel correspondingE40CarrierSlot)
        {
            if (!IsInCreation && ProcessJob?.MaterialType != MaterialType.Carriers)
            {
                return;
            }

            EditingCarriersDetails.Remove(correspondingE40CarrierSlot);
            OnPropertyChanged(nameof(EditingCarriersDetails));
        }

        #endregion

        #region Private method

        private string ValidateEditingId()
        {
            var list = GemController.E40Std.ProcessJobs.Select(id => id.ObjID).ToList();
            if (IsInCreation && ProcessJob != null)
            {
                list.Remove(ProcessJob.ObjID);
            }
            return ValidateEditingId(list, EditingObjId);
        }

        private string ValidateEditingRecipeId()
        {
            return string.IsNullOrEmpty(EditingRecipeId)
                ? LocalizationManager.GetString(nameof(GemGeneralRessources.GEM_ERROR_EMPTY))
                : null;
        }

        private string ValidateEditingCarriers()
        {
            return EditingCarriersDetails.Count == 0
                ? LocalizationManager.GetString(
                    nameof(ProcessJobsResources.PJ_ERROR_CARRIER_EMPTY))
                : null;
        }

        #endregion

        #region Commands

        #region Save

        internal override void SaveCommandExecute()
        {
            var status = Save();

            if (status.IsSuccess)
            {
                IsInEdition = false;
                IsInCreation = false;
              
                GemController.E40Std.StandardServices.SetStartMethod(
                    new List<string>() { ProcessJob.ObjID },
                    EditingProcessStart);

                EditingProcessStart = ProcessStart.ManualStart;

                ChangeEditionModeCommandVisibility(false, true);
            }

            SendSuccessFailureMessage(status, GemGeneralRessources.GEM_SAVE);
        }

        internal override bool SaveCommandCanExecute()
        {
            return !HasErrors && (IsInEdition || IsInCreation);
        }

        #endregion

        #region Cancel

        internal override void CancelCommandExecute()
        {
            IsInEdition = false;
            IsInCreation = false;

            ChangeEditionModeCommandVisibility(false, true);
        }

        internal override bool CancelCommandCanExecute()
        {
            return true;
        }

        #endregion

        #region Add pause events

        private DelegateCommand _addPauseEventsCommand;

        public DelegateCommand AddPauseEventsCommand
            => _addPauseEventsCommand ??= new DelegateCommand(
                AddPauseEventsCommandExecute,
                AddPauseEventsCommandCanExecute);

        private void AddPauseEventsCommandExecute()
        {
            // Open add pause event popup
            var availableEvents = GemController.E30Std.DataServices
                .GetEvents()
                .Select(e => e.Name)
                .Except(EditingPauseEvent)
                .ToList();

            var pausePopup = new AddDataPopupViewModel(availableEvents);
            var popup = new Popup(
                new LocalizableText(nameof(GemGeneralRessources.GEM_PAUSEEVENT_ADD)),
                new LocalizableText(nameof(GemGeneralRessources.GEM_PAUSEEVENT_SELECTION)))
            {
                Content = pausePopup
            };

            popup.Commands.Add(new PopupCommand(nameof(GemGeneralRessources.GEM_CANCEL)));
            popup.Commands.Add(
                new PopupCommand(
                    nameof(GemGeneralRessources.GEM_ADD),
                    new DelegateCommand(
                        () =>
                        {
                            EditingPauseEvent.Clear();
                            foreach (var data in pausePopup.SelectedData)
                            {
                                EditingPauseEvent.Add(data);
                            }

                            IsPauseEventListOpened = true;
                        },
                        () => pausePopup.SelectedData != null)));

            _popupDisplay?.Show(popup);
        }

        private bool AddPauseEventsCommandCanExecute()
        {
            if (GemController.E30Std == null)
                return false;

            return IsInGlobalEdition
                   && GemController.E30Std.DataServices.GetEvents().Count
                   != EditingPauseEvent.Count;
        }

        #endregion

        #region Delete pause event

        private DelegateCommand<string> _deletePauseEventsCommand;

        public DelegateCommand<string> DeletePauseEventsCommand
            => _deletePauseEventsCommand ??= new DelegateCommand<string>(
                DeletePauseEventsCommandExecute,
                DeletePauseEventsCommandCanExecute);

        private void DeletePauseEventsCommandExecute(string pauseEvent)
            => EditingPauseEvent.Remove(pauseEvent);

        private bool DeletePauseEventsCommandCanExecute(string pauseEvent) => true;

        #endregion

        #region Add carrier or substrate

        private DelegateCommand _addCarrierSubstrateCommand;

        public DelegateCommand AddCarrierCommand
            => _addCarrierSubstrateCommand ??= new DelegateCommand(
                AddCarrierSubstrateCommandExecute);

        private void AddCarrierSubstrateCommandExecute()
        {
            if (EditingMaterialType != MaterialType.Carriers)
            {
                return;
            }

            var carriersNotInProcessJob = GemController.E87Std.Carriers
                .Where(
                    c => EditingCarriersDetails.All(
                        m => m.MaterialNameListElement.CarrierID != c.ObjID))
                .Select(c => c.ObjID)
                .ToList();

            var carrierPopup = new AddDataPopupViewModel(carriersNotInProcessJob, true);
            var popup = new Popup(
                new LocalizableText(nameof(ProcessJobsResources.PJ_CARRIER_ADD)),
                nameof(ProcessJobsResources.PJ_CARRIER_ADD_SELECTION)) { Content = carrierPopup };
            popup.Commands.Add(new PopupCommand(nameof(GemGeneralRessources.GEM_CANCEL)));
            popup.Commands.Add(
                new PopupCommand(
                    nameof(GemGeneralRessources.GEM_ADD),
                    new DelegateCommand(
                        () => SetEditingCarriersDetails(carrierPopup.SelectedData),
                        () => carrierPopup.SelectedData != null)));

            _popupDisplay?.Show(popup);

            // to open add substrate popup when they will be implemented in the standard
        }

        #endregion

        #region Delete carrier or substrate

        private DelegateCommand<CorrespondingCarrierSlotMapViewModel>
            _deleteCarrierSubstrateCommand;

        public DelegateCommand<CorrespondingCarrierSlotMapViewModel> DeleteCarrierSubstrateCommand
            => _deleteCarrierSubstrateCommand ??=
                new DelegateCommand<CorrespondingCarrierSlotMapViewModel>(
                    DeleteCarrierSubstrateCommandExecute,
                    DeleteCarrierSubstrateCommandCanExecute);

        private void DeleteCarrierSubstrateCommandExecute(
            CorrespondingCarrierSlotMapViewModel e40CarrierInfo)
        {
            DeleteCarrierSubstrate(e40CarrierInfo);
        }

        private bool DeleteCarrierSubstrateCommandCanExecute(
            CorrespondingCarrierSlotMapViewModel e40CarrierInfo)
        {
            return e40CarrierInfo != null;
        }

        #endregion Delete carrier or substrate

        #region Edit Carrier SlotMap

        private DelegateCommand<CorrespondingCarrierSlotMapViewModel>
            _editCarrierSlotMapCommand;

        public DelegateCommand<CorrespondingCarrierSlotMapViewModel> EditCarrierSlotMapCommand
            => _editCarrierSlotMapCommand ??=
                new DelegateCommand<CorrespondingCarrierSlotMapViewModel>(
                    EditCarrierSlotMapCommandExecute);

        private void EditCarrierSlotMapCommandExecute(
            CorrespondingCarrierSlotMapViewModel e40CarrierInfo)
        {
            var slotMapSelection = new SlotMapSelectionViewModel(e40CarrierInfo, IsInEdition);
            var popup = new Popup(
                new LocalizableText(nameof(GemGeneralRessources.GEM_SLOTMAP)),
                nameof(GemGeneralRessources.GEM_SLOTMAP_EDITION))
            {
                Content = slotMapSelection
            };
            popup.Commands.Add(new PopupCommand(nameof(GemGeneralRessources.GEM_CANCEL)));
            popup.Commands.Add(
                new PopupCommand(
                    nameof(GemGeneralRessources.GEM_MODIFY),
                    new DelegateCommand(
                        () =>
                        {
                            e40CarrierInfo.MaterialNameListElement.SlotIds.Clear();
                            
                            for (var i = 0; i < slotMapSelection.SelectedSlots.Count; i++)
                            {
                                e40CarrierInfo.MaterialNameListElement.SlotIds.Add((byte)slotMapSelection.SelectedSlots[i].Index);
                            }
                        }, () => IsInCreation)));
            _popupDisplay.Show(popup);
        }

        #endregion Edit Carrier SlotMap

        #endregion
    }
}
