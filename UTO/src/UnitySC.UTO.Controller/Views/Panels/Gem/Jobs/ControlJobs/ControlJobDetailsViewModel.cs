using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.Common.Localization;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Popups;
using Agileo.Semi.Communication.Abstractions.E5;
using Agileo.Semi.Gem.Abstractions.E30;
using Agileo.Semi.Gem300.Abstractions.E40;
using Agileo.Semi.Gem300.Abstractions.E87;
using Agileo.Semi.Gem300.Abstractions.E94;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;
using UnitySC.UTO.Controller.Remote;
using UnitySC.UTO.Controller.Views.Panels.Gem.Equipment;
using UnitySC.UTO.Controller.Views.Panels.Gem.Popups;

using Action = Agileo.Semi.Gem300.Abstractions.E94.Action;
using Carrier = Agileo.Semi.Gem300.Abstractions.E87.Carrier;
using Status = Agileo.Semi.Gem300.Abstractions.E94.Status;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.Jobs.ControlJobs
{
    public class ControlJobDetailsViewModel : JobDetailsViewModel
    {
        #region Fields

        private IControlJob _controlJob;

        private readonly PopupDisplayer _popupDisplay;

        #endregion

        #region private

        private void RefreshCreationCarrierInputSpecsList()
        {
            CreationInputSpecCarriers.Clear();
            var carriers = GetCarriersIdByProcessJobsId(CreationProcessJobs.ToList());
            foreach (var c in carriers)
            {
                var e87Carrier = E87Standard.GetCarrierById(c);
                var mtl = new MaterialNameListElement(c, GenerateSlotIds(e87Carrier));
                CreationInputSpecCarriers.Add(new CorrespondingCarrierSlotMapViewModel(mtl, e87Carrier));
            }
        }

        private List<byte> GenerateSlotIds(Carrier e87Carrier)
        {
            var slotIDs = new List<byte>();
            for (byte i = 0; i < e87Carrier.SlotMap.Count; i++)
            {
                //full slot map by default
                slotIDs.Add((byte)(i + 1));
            }

            return slotIDs;
        }

        #endregion private

        #region Properties
        private static GemController GemController => App.ControllerInstance.GemController;

        private static IE94Standard E94Standard => GemController.E94Std;

        private static IE40Standard E40Standard => GemController.E40Std;

        private static IE30Standard E30Standard => GemController.E30Std;

        private static IE87Standard E87Standard => GemController.E87Std;

        #region Edition properties

        private StartMethod _editingStartMethod;

        public StartMethod EditingStartMethod
        {
            get => _editingStartMethod;
            set => SetAndRaiseIfChanged(ref _editingStartMethod, value);
        }

        #endregion

        #region Creation properties

        private string _creationObjId;

        public string CreationObjId
        {
            get => _creationObjId;
            set => SetAndRaiseIfChanged(ref _creationObjId, value);
        }

        public State CreationState
            => State.QUEUED;

        public StartMethod CreationStartMethod { get; set; }

        public string CreationCollectionPlanId { get; set; }

        public ObservableCollection<string> CreationPauseEvents { get; }

        private ObservableCollection<string> _creationProcessJobs;

        public ObservableCollection<string> CreationProcessJobs
        {
            get => _creationProcessJobs;
            set => SetAndRaiseIfChanged(ref _creationProcessJobs, value);
        }

        private ObservableCollection<CorrespondingCarrierSlotMapViewModel>
            _creationInputSpecCarriers;

        public ObservableCollection<CorrespondingCarrierSlotMapViewModel> CreationInputSpecCarriers
        {
            get => _creationInputSpecCarriers;
            set => SetAndRaiseIfChanged(ref _creationInputSpecCarriers, value);
        }

        private ObservableCollection<CorrespondingMaterialOutSpecViewModel>
            _creationMaterialOutSpecification = new();

        public ObservableCollection<CorrespondingMaterialOutSpecViewModel>
            CreationMaterialOutSpecification
        {
            get => _creationMaterialOutSpecification;
            set => SetAndRaiseIfChanged(ref _creationMaterialOutSpecification, value);
        }

        #endregion

        #region Global properties

        public bool IsPjListOpened { get; set; }

        #endregion

        #region ControlJob properties

        public string CollectionPlanId => _controlJob?.DataCollectionPlan ?? string.Empty;

        public string ObjId => _controlJob?.ObjID ?? string.Empty;

        public State State => _controlJob?.State ?? State.QUEUED;

        public StartMethod StartMethod => _controlJob?.StartMethod ?? StartMethod.Auto;

        public Collection<string> PauseEvents
        {
            get
            {
                if (_controlJob != null && _controlJob.PauseEvents != null)
                {
                    return new Collection<string>(_controlJob.PauseEvents);
                }

                return  new Collection<string>();
            }
        }

        public ProcessOrderManagement ControlOrder
            => _controlJob?.ProcessOrderManagement ?? ProcessOrderManagement.ARRIVAL;

        private List<IProcessJob> _processJobs = new();

        public List<IProcessJob> ProcessJobs
        {
            get => _processJobs;
            set
            {
                _processJobs = value;
                OnPropertyChanged(nameof(ProcessJobs));
            }
        }

        private List<string> _inputSpecCarriers = new();

        public List<string> InputSpecCarriers
        {
            get => _inputSpecCarriers;
            set => SetAndRaiseIfChanged(ref _inputSpecCarriers, value);
        }

        private List<CorrespondingMaterialOutSpecViewModel> _materialOutSpecification = new();

        public List<CorrespondingMaterialOutSpecViewModel> MaterialOutSpecification
        {
            get => _materialOutSpecification;
            set => SetAndRaiseIfChanged(ref _materialOutSpecification, value);
        }

        #endregion

        #endregion

        #region Constructor

        public ControlJobDetailsViewModel(IControlJob cj, PopupDisplayer popupDisplay, bool isInEdition, bool isInCreation)
            : this(popupDisplay, isInEdition, isInCreation)
        {
            _controlJob = cj ?? throw new ArgumentNullException(nameof(cj));
            Index = E94Standard.ControlJobs.IndexOf(cj);
            IsCarrierListOpened = _controlJob.CarrierInputSpecifications.Count <= 3;
            IsPjListOpened = ProcessJobs.Count <= 3;
            IsErrorListOpened = Errors.Count <= 3;

            ProcessJobs = E40Standard.ProcessJobs.Where(
                    pj => (_controlJob?.ProcessingControlSpecifications.Select(u => u.PRJobID)
                            .ToList())
                        .Contains(pj.ObjID))
                .ToList();
            InputSpecCarriers = _controlJob?.CarrierInputSpecifications.ToList();

            foreach (var s in _controlJob?.MaterialOutSpecifications)
            {
                var sourceCarrier = E87Standard.GetCarrierById(s.SourceMap.CarrierID);
                var destinationCarrier = E87Standard.GetCarrierById(s.DestinationMap.CarrierID);

                var carrierSource = new MaterialNameListElement(s.SourceMap.CarrierID,
                    s.SourceMap.SubstrateLocations);
                var carrierDestination = new MaterialNameListElement(
                    s.DestinationMap.CarrierID,
                    s.DestinationMap.SubstrateLocations);

                MaterialOutSpecification.Add(
                    new CorrespondingMaterialOutSpecViewModel
                    {
                        Source =
                            new CorrespondingCarrierSlotMapViewModel (carrierSource, sourceCarrier),
                        Destination =
                            new CorrespondingCarrierSlotMapViewModel (carrierDestination, destinationCarrier)
                    });
            }
        }

        public ControlJobDetailsViewModel(PopupDisplayer popupDisplay, bool isInEdition, bool isInCreation)
        {
            _popupDisplay = popupDisplay;

            CreationInputSpecCarriers =
                new ObservableCollection<CorrespondingCarrierSlotMapViewModel>();
            CreationMaterialOutSpecification =
                new ObservableCollection<CorrespondingMaterialOutSpecViewModel>();
            CreationProcessJobs = new ObservableCollection<string>();
            CreationPauseEvents = new ObservableCollection<string>();

            IsInEdition = isInEdition;
            IsInCreation = isInCreation;

            if (IsInGlobalEdition)
            {
                Rules.Add(new DelegateRule(nameof(CreationObjId), ValidateEditingId));
                Rules.Add(
                    new DelegateRule(nameof(CreationProcessJobs), ValidateEditingProcessJobs));
                ApplyRules();
            }
        }

        public ControlJobDetailsViewModel()
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        #endregion

        #region Methods

        public Status Save()
        {
            if (IsInCreation)
            {
                var processingControlSpecifications =
                    new Collection<ProcessingControlSpecification>(
                        CreationProcessJobs.Select(
                                pjId => new ProcessingControlSpecification {PRJobID = pjId})
                            .ToList());

                var result = Status.Success();
                try
                {
                    var mtlOutSpec = new Collection<MaterialOutSpecification>();
                    foreach (var m in CreationMaterialOutSpecification)
                    {
                        if (m == null) continue;
                        var sourceSlots = m.Source?.MaterialNameListElement?.SlotIds != null ?
                            new Collection<byte>(m.Source?.MaterialNameListElement?.SlotIds) :
                            new ();
                        var destinationSlots =
                            m.Destination?.MaterialNameListElement?.SlotIds != null
                                ? new Collection<byte>(m.Destination?.MaterialNameListElement?.SlotIds) :
                                new();
                        var sourceLocationMap = new LocationMap()
                        {
                            CarrierID = m.Source?.Carrier?.ObjID ?? string.Empty
                        };
                        foreach (var ss in sourceSlots)
                        {
                            sourceLocationMap.SubstrateLocations.Add(ss);
                        }

                        var destinationLocationMap = new LocationMap()
                        {
                            CarrierID = m.Destination?.Carrier?.ObjID ?? string.Empty
                        };
                        foreach (var ds in destinationSlots)
                        {
                            destinationLocationMap.SubstrateLocations.Add(ds);
                        }

                        mtlOutSpec.Add(
                            new MaterialOutSpecification
                            {
                                SourceMap = sourceLocationMap,
                                DestinationMap = destinationLocationMap
                            });
                    }

                    E94Standard.AddControlJob(
                        CreationObjId,
                        new Collection<string>(
                            CreationInputSpecCarriers.Select(c => c.Carrier.ObjID).ToList()),
                        new Collection<MaterialOutSpecification>(mtlOutSpec),
                        processingControlSpecifications,
                        ProcessOrderManagement.ARRIVAL,
                        CreationStartMethod);
                }
                catch (Exception exception)
                {
                    result = Status.Failure(ErrorCode.NoError, exception.Message);
                }

                if (result.IsSuccess)
                {
                    var createdCj = E94Standard.GetControlJob(CreationObjId);
                    if (createdCj != null && !string.IsNullOrEmpty(CreationCollectionPlanId))
                    {
                        createdCj.DataCollectionPlan = CreationCollectionPlanId;
                    }
                }

                return result;
            }

            if (IsInEdition)
            {
                E94Standard.StandardServices.Cancel(ObjId, Action.SaveJobs);
                var result = Status.Success();
                try
                {
                    E94Standard.AddControlJob(
                        ObjId,
                        new Collection<string>(InputSpecCarriers),
                        new Collection<MaterialOutSpecification>(_controlJob.MaterialOutSpecifications),
                        new Collection<ProcessingControlSpecification>(_controlJob.ProcessingControlSpecifications),
                        _controlJob.ProcessOrderManagement,
                        EditingStartMethod);
                }
                catch (Exception exception)
                {
                    result = Status.Failure(ErrorCode.NoError, exception.Message);
                }

                return result;
            }

            return Status.Failure();
        }

        private void AddProcessJob(string processJobId)
        {
            CreationProcessJobs.Add(processJobId);
            OnPropertyChanged(nameof(CreationProcessJobs));
        }

        public void SetCreationProcessJobs(List<string> processJobIdList)
        {
            foreach (var processJobId in processJobIdList)
            {
                AddProcessJob(processJobId);
            }
        }

        public void DeleteProcessJob(string processJobId)
        {
            CreationProcessJobs.Remove(processJobId);
            OnPropertyChanged(nameof(CreationProcessJobs));
        }

        private string ValidateEditingId()
        {
            var list = E94Standard.ControlJobs.Select(id => id.ObjID).ToList();
            if (IsInCreation && _controlJob != null)
            {
                list.Remove(_controlJob.ObjID);
            }

            return ValidateEditingId(list, CreationObjId);
        }

        private string ValidateEditingProcessJobs()
        {
            return CreationProcessJobs.Count == 0
                ? LocalizationManager.GetString(nameof(ControlJobResources.CJ_ERROR_PJ_COUNT))
                : null;
        }

        private List<string> GetAvailableProcessJobs()
        {
            var processJobsInCreation = CreationProcessJobs.ToList();
            var processJobsInOtherControlJobs = E94Standard.ControlJobs
                .SelectMany(cj => cj.ProcessingControlSpecifications.Select(spec => spec.PRJobID))
                .Distinct()
                .ToList();

            return E40Standard.ProcessJobs.Select(pj => pj.ObjID)
                .Except(processJobsInCreation)
                .Except(processJobsInOtherControlJobs)
                .ToList();
        }

        private static List<string> GetCarriersIdByProcessJobsId(IEnumerable<string> processJobsIds)
        {
            var pjs = processJobsIds.Distinct()
                .Select(id => E40Standard.GetProcessJob(id))
                .Where(pj => pj is {MaterialType: MaterialType.Carriers});

            return pjs.SelectMany(pj => pj.CarrierIDSlotsAssociation.Select(m => m.CarrierID))
                .Distinct()
                .ToList();
        }

        public void UpdateControlJob(IControlJob newControlJob)
        {
            _controlJob = newControlJob;
            Index = E94Standard.ControlJobs.IndexOf(newControlJob);
            OnPropertyChanged(null);
        }

        private void AddInputCarrier(string carrierId)
        {
            var carrierGem = E87Standard.GetCarrierById(carrierId);
            var mtl = new MaterialNameListElement(carrierId, GenerateSlotIds(carrierGem));
            CreationInputSpecCarriers.Add(new CorrespondingCarrierSlotMapViewModel(mtl, carrierGem));
            OnPropertyChanged(nameof(CreationInputSpecCarriers));
        }

        public void SetEditingCarriersDetails(List<string> carrierIdList)
        {
            CreationInputSpecCarriers.Clear();
            foreach (var carrierId in carrierIdList)
            {
                AddInputCarrier(carrierId);
            }

            OnPropertyChanged(nameof(CreationInputSpecCarriers));
        }

        public void DeleteCarrierInput(
            CorrespondingCarrierSlotMapViewModel correspondingE40CarrierSlot)
        {
            CreationInputSpecCarriers.Remove(correspondingE40CarrierSlot);
            OnPropertyChanged(nameof(CreationInputSpecCarriers));
        }

        #endregion Methods

        #region Override

        #region Save

        internal override void SaveCommandExecute()
        {
            var results = Save();

            if (results.IsSuccess)
            {
                IsInEdition = false;
                IsInCreation = false;

                ChangeEditionModeCommandVisibility(false, true);
            }

            SendSuccessFailureMessage(results, GemGeneralRessources.GEM_SAVE);
        }

        internal override bool SaveCommandCanExecute()
        {
            if (IsInCreation
                && !string.IsNullOrEmpty(CreationObjId)
                && CreationProcessJobs.Count > 0
                && CreationInputSpecCarriers.Count > 0)
            {
                return true;
            }

            if (IsInEdition)
            {
                return true;
            }

            return false;
        }

        #endregion Save

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

        #endregion Cancel

        #region Add Process job

        private DelegateCommand _addProcessJobCommand;

        public DelegateCommand AddProcessJobCommand
            => _addProcessJobCommand ??= new DelegateCommand(
                AddProcessJobCommandExecute,
                () => true);

        private void AddProcessJobCommandExecute()
        {
            var processJobPopup = new AddDataPopupViewModel(GetAvailableProcessJobs());
            var popup = new Popup(
                new LocalizableText(nameof(ControlJobResources.CJ_PROCESSJOB_ADD)),
                new LocalizableText(nameof(ControlJobResources.CJ_PROCESSJOB_ADD_SELECTION)))
            {
                Content = processJobPopup
            };

            popup.Commands.Add(new PopupCommand(nameof(GemGeneralRessources.GEM_CANCEL)));
            popup.Commands.Add(
                new PopupCommand(
                    nameof(GemGeneralRessources.GEM_ADD),
                    new DelegateCommand(
                        () =>
                        {
                            SetCreationProcessJobs(processJobPopup.SelectedData);

                            RefreshCreationCarrierInputSpecsList();

                            OnPropertyChanged(nameof(Errors));
                        },
                        () => processJobPopup.SelectedData != null)));

            _popupDisplay?.Show(popup);
        }

        #endregion Add Process job

        #region Add pause events

        private DelegateCommand _addPauseEventsCommand;

        public DelegateCommand AddPauseEventsCommand
            => _addPauseEventsCommand ??= new DelegateCommand(
                AddPauseEventsCommandExecute,
                AddPauseEventsCommandCanExecute);

        private void AddPauseEventsCommandExecute()
        {
            // Open add pause event popup
            var availableEvents = E30Standard.DataServices.GetEvents()
                .Select(e => e.Name)
                .Except(CreationPauseEvents)
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
                            CreationPauseEvents.Clear();
                            foreach (var data in pausePopup.SelectedData)
                            {
                                CreationPauseEvents.Add(data);
                            }

                            IsPauseEventListOpened = true;
                        },
                        () => pausePopup.SelectedData != null)));
            _popupDisplay?.Show(popup);
        }

        private bool AddPauseEventsCommandCanExecute()
        {
            return IsInCreation || IsInEdition;
        }

        #endregion Add pause events

        #region Add carrier Input or substrate

        private DelegateCommand _addCarrierInputCommand;

        public DelegateCommand AddCarrierInputCommand
            => _addCarrierInputCommand ??= new DelegateCommand(AddCarrierInputCommandExecute);

        private void AddCarrierInputCommandExecute()
        {
            var carriersAvailable = E87Standard.Carriers.Select(c => c.ObjID)
                .ToList();

            var carrierPopup = new AddDataPopupViewModel(carriersAvailable, true);
            var popup = new Popup(
                new LocalizableText(nameof(ControlJobResources.CJ_CARRIER_INPUT_ADD)),
                nameof(ControlJobResources.CJ_CARRIER_INPUT_ADD_SELECTION))
            {
                Content = carrierPopup
            };
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

        #endregion Add carrier Input or substrate

        #region Delete carrier or substrate

        private DelegateCommand<CorrespondingCarrierSlotMapViewModel> _deleteCarrierInputCommand;

        public DelegateCommand<CorrespondingCarrierSlotMapViewModel> DeleteCarrierInputCommand
            => _deleteCarrierInputCommand ??=
                new DelegateCommand<CorrespondingCarrierSlotMapViewModel>(
                    DeleteCarrierInputSubstrateCommandExecute,
                    DeleteCarrierInputCommandCanExecute);

        private void DeleteCarrierInputSubstrateCommandExecute(
            CorrespondingCarrierSlotMapViewModel e40CarrierInfo)
        {
            DeleteCarrierInput(e40CarrierInfo);
        }

        private bool DeleteCarrierInputCommandCanExecute(
            CorrespondingCarrierSlotMapViewModel e40CarrierInfo)
        {
            return e40CarrierInfo != null;
        }

        #endregion

        #region Add MaterialOutSpec

        private DelegateCommand _addMaterialOutSpecificationCommand;

        public DelegateCommand AddMaterialOutSpecificationCommand
            => _addMaterialOutSpecificationCommand ??= new DelegateCommand(
                AddMaterialOutSpecificationCommandExecute);

        private void AddMaterialOutSpecificationCommandExecute()
        {
            var carriersAvailable = E87Standard.Carriers.Select(c => c.ObjID)
                .ToList();

            var materialOutSpecificationPopup =
                new AddMaterialOutSpecificationViewModel(carriersAvailable, carriersAvailable);

            var popup = new Popup(
                new LocalizableText(nameof(GemGeneralRessources.GEM_MTRL_OUT_SPEC_ADD)),
                new LocalizableText(nameof(GemGeneralRessources.GEM_MTRL_OUT_SPEC_SELECTION)))
            {
                Content = materialOutSpecificationPopup
            };
            popup.Commands.Add(new PopupCommand(nameof(GemGeneralRessources.GEM_CANCEL)));
            popup.Commands.Add(
                new PopupCommand(
                    nameof(GemGeneralRessources.GEM_ADD),
                    new DelegateCommand(
                        () =>
                        {
                            var e87CarrierSource =
                                E87Standard.GetCarrierById(
                                    materialOutSpecificationPopup.SelectedSource);
                            var e87CarrierDestination =
                                E87Standard.GetCarrierById(
                                    materialOutSpecificationPopup.SelectedDestination);
                            var slotMapSource = new SlotMapViewModel();
                            slotMapSource.UpdateSlotMap(e87CarrierSource);
                            var slotMapDestination = new SlotMapViewModel();
                            slotMapDestination.UpdateSlotMap(e87CarrierDestination);

                            var carrierSource = new MaterialNameListElement(materialOutSpecificationPopup.SelectedSource,
                                GenerateSlotIds(e87CarrierSource));
                            var carrierDestination = new MaterialNameListElement(
                                materialOutSpecificationPopup.SelectedDestination,
                                GenerateSlotIds(e87CarrierDestination));

                            CreationMaterialOutSpecification.Add(
                                new CorrespondingMaterialOutSpecViewModel
                                {
                                    Source =
                                        new CorrespondingCarrierSlotMapViewModel(carrierSource, e87CarrierSource),
                                    Destination =
                                        new CorrespondingCarrierSlotMapViewModel(carrierDestination, e87CarrierDestination)
                                });
                        })));
            _popupDisplay?.Show(popup);
        }

        #endregion Add MaterialOutSpec

        #region Delete MaterialOutSpec

        private DelegateCommand<CorrespondingMaterialOutSpecViewModel>
            _deleteMaterialOutSpecificationCommand;

        public DelegateCommand<CorrespondingMaterialOutSpecViewModel>
            DeleteMaterialOutSpecificationCommand
            => _deleteMaterialOutSpecificationCommand ??=
                new DelegateCommand<CorrespondingMaterialOutSpecViewModel>(
                    DeleteMaterialOutSpecificationCommandExecute,
                    DeleteMaterialOutSpecificationCommandCanExecute);

        private void DeleteMaterialOutSpecificationCommandExecute(
            CorrespondingMaterialOutSpecViewModel e40CarrierInfo)
        {
            if (IsInCreation)
            {
                CreationMaterialOutSpecification.Remove(e40CarrierInfo);
            }
            else if (IsInEdition)
            {
                MaterialOutSpecification.Remove(e40CarrierInfo);
            }
        }

        private bool DeleteMaterialOutSpecificationCommandCanExecute(
            CorrespondingMaterialOutSpecViewModel e40CarrierInfo)
        {
            return e40CarrierInfo != null;
        }

        #endregion Delete MaterialOutSpec

        #region Remove creation ProcessJob

        private DelegateCommand<string> _deleteProcessJobCommand;

        public DelegateCommand<string> DeleteProcessJobCommand
            => _deleteProcessJobCommand ??= new DelegateCommand<string>(
                DeleteProcessJobCommandExecute,
                DeleteProcessJobCommandCanExecute);

        private void DeleteProcessJobCommandExecute(string processJobId)
        {
            var processJobToRemove =
                CreationProcessJobs.FirstOrDefault(pjId => pjId == processJobId);

            DeleteProcessJob(processJobToRemove);

            RefreshCreationCarrierInputSpecsList();
        }

        private bool DeleteProcessJobCommandCanExecute(string carrier)
        {
            return true;
        }

        #endregion Remove creation ProcessJob

        #region Delete pause event

        private DelegateCommand<string> _deletePauseEventsCommand;

        public DelegateCommand<string> DeletePauseEventsCommand
            => _deletePauseEventsCommand ??= new DelegateCommand<string>(
                DeletePauseEventsCommandExecute,
                DeletePauseEventsCommandCanExecute);

        private void DeletePauseEventsCommandExecute(string pauseEvent)
        {
            _ = CreationPauseEvents.Remove(pauseEvent);
        }

        private bool DeletePauseEventsCommandCanExecute(string pauseEvent)
        {
            return true;
        }

        #endregion Delete pause event

        #region Edit Carrier SlotMap

        private DelegateCommand<CorrespondingCarrierSlotMapViewModel>
            _editCarrierSlotMapCommand;

        public DelegateCommand<CorrespondingCarrierSlotMapViewModel> EditCarrierSlotMapCommand
            => _editCarrierSlotMapCommand ??=
                new DelegateCommand<CorrespondingCarrierSlotMapViewModel>(
                    EditCarrierSlotMapCommandExecute);

        private void EditCarrierSlotMapCommandExecute(
            CorrespondingCarrierSlotMapViewModel carrierInfo)
        {
            var slotMapSelection = new SlotMapSelectionViewModel(carrierInfo, IsInEdition);
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
                            carrierInfo.MaterialNameListElement.SlotIds.Clear();

                            for (var i = 0; i < slotMapSelection.SelectedSlots.Count; i++)
                            {
                               carrierInfo.MaterialNameListElement.SlotIds.Add((byte)slotMapSelection.SelectedSlots[i].Index);
                            }
                        }, () => IsInCreation)));
            _popupDisplay.Show(popup);
        }

        #endregion Edit Carrier SlotMap

        #endregion
    }
}
