using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.Common.Localization;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.Semi.Gem300.Abstractions.E40;
using Agileo.Semi.Gem300.Abstractions.E94;

using UnitySC.GUI.Common.UIComponents.XamlResources.Shared;
using UnitySC.GUI.Common.Vendor;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Sort;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree;
using UnitySC.GUI.Common.Vendor.UIComponents.GuiExtended;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.UTO.Controller.Remote;
using UnitySC.UTO.Controller.Views.Panels.Gem.Equipment.Carriers;
using UnitySC.UTO.Controller.Views.Panels.Gem.Jobs.ControlJobs;
using UnitySC.UTO.Controller.Views.Panels.Gem.Jobs.ProcessJobs;
using UnitySC.UTO.Controller.Views.Panels.Gem.Popups;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.Jobs
{
    public class JobsPanel : BusinessPanel
    {
        #region Fields

        private static CarriersViewerPanel _carriersPanel;

        private JobDetailsViewModel _selectedValue;
        private bool _isExpanderOpened;

        #endregion

        #region Constructors

        static JobsPanel()
        {
            DataTemplateGenerator.Create(typeof(JobsPanel), typeof(JobsPanelView));

            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(GemGeneralRessources)));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(ProcessJobsResources)));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(ControlJobResources)));
        }

        public JobsPanel()
            : this($"{nameof(JobsPanel)} DesignTime Constructor")
        {
        }

        public JobsPanel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
            #region Add commands

            #region Edition commands

            AddCommand = new InvisibleBusinessPanelCommand(
                nameof(GemGeneralRessources.GEM_ADD_JOB),
                new DelegateCommand(AddCommandExecute, AddCommandCanExecute),
                PathIcon.Add);

            EditCjCommand = new InvisibleBusinessPanelCommand(
                nameof(GemGeneralRessources.GEM_EDIT_JOB),
                new DelegateCommand(EditCjCommandExecute, EditCjCommandCanExecute),
                PathIcon.Edit);

            DeleteCommand = new InvisibleBusinessPanelCommand(
                nameof(GemGeneralRessources.GEM_DELETE_JOB),
                new DelegateCommand(DeleteCommandExecute, DeleteCommandCanExecute),
                PathIcon.Delete);

            DuplicatePjCommand = new InvisibleBusinessPanelCommand(
                nameof(ProcessJobsResources.PJ_DUPLICATE_PJ),
                new DelegateCommand(DuplicatePjCommandExecute, DuplicatePjCommandCanExecute),
                PathIcon.Duplicate);

            Commands.Add(AddCommand);
            Commands.Add(EditCjCommand);
            Commands.Add(DeleteCommand);
            Commands.Add(DuplicatePjCommand);

            #endregion

            #region Job management commands

            StartJobCommand = new BusinessPanelCommand(
                nameof(GemGeneralRessources.GEM_PLAY),
                new DelegateCommand(StartJobCommandExecute, StartJobCommandCanExecute),
                PathIcon.Play);
            PauseJobCommand = new BusinessPanelCommand(
                nameof(GemGeneralRessources.GEM_PAUSE),
                new DelegateCommand(PauseJobCommandExecute, PauseJobCommandCanExecute),
                PathIcon.Pause);
            StopJobCommand = new BusinessPanelCommand(
                nameof(GemGeneralRessources.GEM_STOP),
                new DelegateCommand(StopJobCommandExecute, StopJobCommandCanExecute),
                PathIcon.Stop);
            AbortJobCommand = new BusinessPanelCommand(
                nameof(GemGeneralRessources.GEM_ABORT),
                new DelegateCommand(AbortJobCommandExecute, AbortJobCommandCanExecute),
                PathIcon.Abort);
            ResumeJobCommand = new BusinessPanelCommand(
                nameof(GemGeneralRessources.GEM_RESUME),
                new DelegateCommand(ResumeJobCommandExecute, ResumeJobCommandCanExecute),
                PathIcon.Abort);
            HoqCjCommand = new BusinessPanelCommand(
                nameof(ControlJobResources.CJ_HOQ),
                new DelegateCommand(HoqCjCommandExecute, HoqCjCommandCanExecute),
                CustomPathIcon.ControlJobHOQ);
            DeselectCjCommand = new BusinessPanelCommand(
                nameof(ControlJobResources.CJ_DESELECT),
                new DelegateCommand(DeselectCjCommandExecute, DeselectCjCommandCanExecute),
                CustomPathIcon.Deselection);

            Commands.Add(StartJobCommand);
            Commands.Add(PauseJobCommand);
            Commands.Add(StopJobCommand);
            Commands.Add(AbortJobCommand);
            Commands.Add(ResumeJobCommand);
            Commands.Add(HoqCjCommand);
            Commands.Add(DeselectCjCommand);

            #endregion

            #endregion

            PjCjTreeSource = new DataTreeSource<JobDetailsViewModel>(item => item.Children);
            PjCjTreeSource.Sort.Add(
                new SortDefinition<TreeNode<JobDetailsViewModel>>(
                    nameof(JobDetailsViewModel.Index),
                    TreeSourceOrder));

            PjCjTreeSource.Search.AddSearchDefinition(
                nameof(GemGeneralRessources.GEM_ID),
                GetComparableStringFunc,
                true);
        }

        #endregion

        #region Properties

        private static GemController GemController => App.ControllerInstance.GemController;

        public DataTreeSource<JobDetailsViewModel> PjCjTreeSource { get; set; }

        public JobDetailsViewModel SelectedValue
        {
            get => _selectedValue;
            set
            {
                if (_selectedValue != null)
                {
                    _selectedValue.JobChanged -= JobDetailsOnJobChanged;
                }

                SetAndRaiseIfChanged(ref _selectedValue, value);

                if (_selectedValue != null)
                {
                    _selectedValue.JobChanged += JobDetailsOnJobChanged;
                }
            }
        }

        public static ReadOnlyCollection<IProcessJob> ProcessJobs
            => GemController.E40Std.ProcessJobs;

        public static ReadOnlyCollection<IControlJob> ControlJobs
            => GemController.E94Std.ControlJobs;

        public bool IsExpanderOpened
        {
            get => _isExpanderOpened;
            set => SetAndRaiseIfChanged(ref _isExpanderOpened, value);
        }

        #endregion

        #region Command properties

        public BusinessPanelCommand StartJobCommand { get; }
        public BusinessPanelCommand PauseJobCommand { get; }
        public BusinessPanelCommand StopJobCommand { get; }
        public BusinessPanelCommand AbortJobCommand { get; }
        public BusinessPanelCommand ResumeJobCommand { get; }
        public BusinessPanelCommand HoqCjCommand { get; }
        public BusinessPanelCommand DeselectCjCommand { get; }

        public InvisibleBusinessPanelCommand AddCommand { get; }
        public InvisibleBusinessPanelCommand EditCjCommand { get; }
        public InvisibleBusinessPanelCommand DeleteCommand { get; }
        public InvisibleBusinessPanelCommand DuplicatePjCommand { get; }

        #endregion

        #region Methods

        public void UpdateJobsTree()
        {
            DispatcherHelper.DoInUiThread(
                () =>
                {
                    // Add CJ at root
                    var rootElement = ControlJobs.Select(
                            cj =>
                            {
                                var controlJob =
                                    new ControlJobDetailsViewModel(cj, Popups, false, false);

                                // Add PJ that are CJ children
                                controlJob.Children = GetAssociatedProcessJobDetails(controlJob);
                                return controlJob;
                            })
                        .ToList<JobDetailsViewModel>();

                    // Add PJ without parent CJ
                    rootElement.AddRange(GetAloneProcessJobDetails());

                    PjCjTreeSource.Reset(rootElement);
                });
        }

        private List<JobDetailsViewModel> GetAssociatedProcessJobDetails(
            ControlJobDetailsViewModel controlJob)
        {
            return controlJob.ProcessJobs.ConvertAll<JobDetailsViewModel>(
                pj => new ProcessJobDetailsViewModel(pj, Popups, false, false));
        }

        private List<TreeNode<JobDetailsViewModel>> GetAssociatedProcessJobDetailsNode(
            ControlJobDetailsViewModel controlJob)
        {
            return PjCjTreeSource.GetFlattenElements()
                .Where(
                    pjNode => pjNode.Model is ProcessJobDetailsViewModel pj
                              && controlJob.ProcessJobs.Select(job => job.ObjID)
                                  .Contains(pj.ProcessJob.ObjID))
                .ToList();
        }

        internal List<JobDetailsViewModel> GetAloneProcessJobDetails()
        {
            var associatedPjIds = ControlJobs.SelectMany(
                    cj => cj.ProcessingControlSpecifications.Select(spec => spec.PRJobID))
                .Distinct()
                .ToList();

            var allPjIds = ProcessJobs.Select(pj => pj.ObjID).ToList();

            var alonePjIds = allPjIds.Except(associatedPjIds).ToList();

            return ProcessJobs.Join(
                    alonePjIds,
                    pj => pj.ObjID,
                    id => id,
                    (pj, _) => (JobDetailsViewModel)new ProcessJobDetailsViewModel(
                        pj,
                        Popups,
                        false,
                        false))
                .ToList();
        }

        private void UpdateProcessJob(IProcessJob processJob)
        {
            if (processJob == null)
            {
                return;
            }
            var pjNode = PjCjTreeSource.GetFlattenElements()
                .Find(
                    job => job.Model is ProcessJobDetailsViewModel pj
                           && pj.ProcessJob.ObjID.Equals(processJob.ObjID));
            if (pjNode != null)
            {
                ((ProcessJobDetailsViewModel)pjNode.Model).ProcessJob = processJob;
            }
        }

        private void UpdateControlJob(IControlJob controlJob)
        {
            if (controlJob == null)
            {
                return;
            }
            var cjNode = PjCjTreeSource.GetFlattenElements()
                .Find(
                    job => job.Model is ControlJobDetailsViewModel cj
                           && cj.ObjId.Equals(controlJob.ObjID));
            if (cjNode != null)
            {
                ((ControlJobDetailsViewModel)cjNode.Model).UpdateControlJob(controlJob);
            }
        }

        private void ReorderControlJobs()
        {
            foreach (var controlJob in ControlJobs)
            {
                UpdateControlJob(controlJob);
            }

            PjCjTreeSource.Sort.OnSortApplied();
        }

        private static string TreeSourceOrder(TreeNode<JobDetailsViewModel> arg)
        {
            return arg.Model switch
            {
                ProcessJobDetailsViewModel pj => $"pj_{pj.Index}",
                ControlJobDetailsViewModel cj => $"cj_{cj.Index}",
                _ => string.Empty
            };
        }

        private static string GetComparableStringFunc(JobDetailsViewModel arg)
        {
            return arg switch
            {
                ProcessJobDetailsViewModel pj => pj.ProcessJob.ObjID,
                ControlJobDetailsViewModel cj => cj.ObjId,
                _ => string.Empty
            };
        }

        #endregion

        #region Commands

        #region Start job

        private void StartJobCommandExecute()
        {
            switch (SelectedValue)
            {
                case ControlJobDetailsViewModel cj:
                    {
                        var result =
                            GemController.E94Std.StandardServices.Start(cj.ObjId);
                        cj.SendSuccessFailureMessage(result, ControlJobResources.CJ_START);
                        break;
                    }
                case ProcessJobDetailsViewModel pj:
                    {
                        var result = GemController.E40Std.StandardServices.Command(
                            pj.ProcessJob.ObjID,
                            CommandName.STARTPROCESS,
                            new List<CommandParameter>());
                        pj.SendSuccessFailureMessage(result, ProcessJobsResources.PJ_START);
                        break;
                    }
            }
        }

        private bool StartJobCommandCanExecute()
        {
            if (SelectedValue?.IsInGlobalEdition != false)
            {
                return false;
            }

            return SelectedValue switch
            {
                ControlJobDetailsViewModel cj => cj.ProcessJobs.Count > 0
                                                 && cj.State == State.WAITINGFORSTART
                                                 && App.ControllerInstance.ControllerEquipmentManager.IsControllerOk(),
                ProcessJobDetailsViewModel pj => !string.IsNullOrEmpty(pj.ProcessJob.RecipeID)
                                                 && pj.ProcessJob.JobState == JobState.QUEUED_POOLED
                                                 && App.ControllerInstance.ControllerEquipmentManager.IsControllerOk(),
                _ => false
            };
        }

        #endregion

        #region Pause job

        private void PauseJobCommandExecute()
        {
            switch (SelectedValue)
            {
                case ControlJobDetailsViewModel cj:
                    {
                        var result =
                            GemController.E94Std.StandardServices.Pause(cj.ObjId);
                        cj.SendSuccessFailureMessage(result, ControlJobResources.CJ_PAUSE);
                        break;
                    }
                case ProcessJobDetailsViewModel pj:
                    {
                        var result = GemController.E40Std.StandardServices.Command(
                            pj.ProcessJob.ObjID,
                            CommandName.PAUSE,
                            new List<CommandParameter>());
                        pj.SendSuccessFailureMessage(result, GemGeneralRessources.GEM_PAUSE);
                        break;
                    }
            }
        }

        private bool PauseJobCommandCanExecute()
        {
            if (SelectedValue?.IsInGlobalEdition != false)
            {
                return false;
            }

            return SelectedValue switch
            {
                ControlJobDetailsViewModel cj => cj.ProcessJobs.Count > 0
                                                 && cj.State == State.EXECUTING
                                                 && App.ControllerInstance.ControllerEquipmentManager.IsControllerOk(),
                ProcessJobDetailsViewModel pj => !string.IsNullOrEmpty(pj.ProcessJob.RecipeID)
                                                 && pj.ProcessJob.JobState == JobState.PROCESSING
                                                 && App.ControllerInstance.ControllerEquipmentManager.IsControllerOk(),
                _ => false
            };
        }

        #endregion

        #region Stop job

        private void StopJobCommandExecute()
        {
            switch (SelectedValue)
            {
                case ControlJobDetailsViewModel cj:
                    {
                        var result =
                            GemController.E94Std.StandardServices.Stop(
                                cj.ObjId,
                                Action.SaveJobs);
                        cj.SendSuccessFailureMessage(result, ControlJobResources.CJ_STOP);
                        break;
                    }
                case ProcessJobDetailsViewModel pj:
                    {
                        var result = GemController.E40Std.StandardServices.Command(
                            pj.ProcessJob.ObjID,
                            CommandName.STOP,
                            new List<CommandParameter>());
                        pj.SendSuccessFailureMessage(result, GemGeneralRessources.GEM_STOP);
                        break;
                    }
            }
        }

        private bool StopJobCommandCanExecute()
        {
            if (SelectedValue?.IsInGlobalEdition != false)
            {
                return false;
            }

            return SelectedValue switch
            {
                ControlJobDetailsViewModel cj => cj.ProcessJobs.Count > 0
                                                 && (cj.State == State.PAUSED
                                                     || cj.State == State.EXECUTING)
                                                 && App.ControllerInstance.ControllerEquipmentManager.IsControllerOk(),
                ProcessJobDetailsViewModel pj => !string.IsNullOrEmpty(pj.ProcessJob.RecipeID)
                                                 &&(pj.ProcessJob.JobState == JobState.PROCESSING
                                                    || pj.ProcessJob.JobState == JobState.PAUSED)
                                                 && App.ControllerInstance.ControllerEquipmentManager.IsControllerOk(),
                _ => false
            };
        }

        #endregion

        #region Abort job

        private void AbortJobCommandExecute()
        {
            switch (SelectedValue)
            {
                case ControlJobDetailsViewModel cj:
                    {
                        var result =
                            GemController.E94Std.StandardServices.Abort(
                                cj.ObjId,
                                Action.RemoveJobs);
                        cj.SendSuccessFailureMessage(result, ControlJobResources.CJ_ABORT);
                        break;
                    }
                case ProcessJobDetailsViewModel pj:
                    {
                        var result = GemController.E40Std.StandardServices.Command(
                            pj.ProcessJob.ObjID,
                            CommandName.ABORT,
                            new List<CommandParameter>());
                        pj.SendSuccessFailureMessage(result, GemGeneralRessources.GEM_ABORT);
                        break;
                    }
            }
        }

        private bool AbortJobCommandCanExecute()
        {
            if (SelectedValue?.IsInGlobalEdition != false)
            {
                return false;
            }

            return SelectedValue switch
            {
                ControlJobDetailsViewModel cj => cj.ProcessJobs.Count > 0
                                                 && (cj.State == State.PAUSED
                                                 || cj.State == State.EXECUTING)
                                                 && App.ControllerInstance.ControllerEquipmentManager.IsControllerOk(),
                ProcessJobDetailsViewModel pj => !string.IsNullOrEmpty(pj.ProcessJob.RecipeID)
                                                 && (pj.ProcessJob.JobState == JobState.PROCESSING
                                                     || pj.ProcessJob.JobState == JobState.STOPPED
                                                     || pj.ProcessJob.JobState == JobState.STOPPING
                                                     || pj.ProcessJob.JobState == JobState.PAUSING
                                                     || pj.ProcessJob.JobState == JobState.PAUSED)
                                                     && App.ControllerInstance.ControllerEquipmentManager.IsControllerOk(),
                _ => false
            };
        }

        #endregion

        #region Resume job

        private void ResumeJobCommandExecute()
        {
            switch (SelectedValue)
            {
                case ControlJobDetailsViewModel cj:
                    {
                        var result =
                            GemController.E94Std.StandardServices.Resume(cj.ObjId);
                        cj.SendSuccessFailureMessage(result, ControlJobResources.CJ_RESUME);
                        break;
                    }
                case ProcessJobDetailsViewModel pj:
                    {
                        var result = GemController.E40Std.StandardServices.Command(
                            pj.ProcessJob.ObjID,
                            CommandName.RESUME,
                            new List<CommandParameter>());
                        pj.SendSuccessFailureMessage(result, GemGeneralRessources.GEM_RESUME);
                        break;
                    }
            }
        }

        private bool ResumeJobCommandCanExecute()
        {
            if (SelectedValue?.IsInGlobalEdition != false)
            {
                return false;
            }

            return SelectedValue switch
            {
                ControlJobDetailsViewModel cj => cj.ProcessJobs.Count > 0
                                                 && cj.State == State.PAUSED
                                                 && App.ControllerInstance.ControllerEquipmentManager.IsControllerOk(),
                ProcessJobDetailsViewModel pj => !string.IsNullOrEmpty(pj.ProcessJob.RecipeID)
                                                 && pj.ProcessJob.JobState is JobState.STOPPED
                                                     or JobState.ABORTED or JobState.PAUSED
                                                 && App.ControllerInstance.ControllerEquipmentManager.IsControllerOk(),
                _ => false
            };
        }

        #endregion

        #region HOQ CJ

        private void HoqCjCommandExecute()
        {
            if (SelectedValue is not ControlJobDetailsViewModel cj)
            {
                return;
            }

            var result =
                GemController.E94Std.StandardServices.Hoq(cj.ObjId);
            cj.SendSuccessFailureMessage(result, ControlJobResources.CJ_HOQ);
        }

        private bool HoqCjCommandCanExecute()
        {
            if (SelectedValue?.IsInGlobalEdition != false)
            {
                return false;
            }

            if (SelectedValue is ControlJobDetailsViewModel cj)
            {
                return cj.ProcessJobs.Count > 0
                       && cj.State == State.QUEUED
                       && App.ControllerInstance.ControllerEquipmentManager.IsControllerOk();
            }

            return false;
        }

        #endregion

        #region Deselect CJ

        private void DeselectCjCommandExecute()
        {
            if (SelectedValue is not ControlJobDetailsViewModel cj)
            {
                return;
            }

            var result = GemController.E94Std.StandardServices.Deselect(cj.ObjId);
            cj.SendSuccessFailureMessage(result, ControlJobResources.CJ_DESELECT);
        }

        private bool DeselectCjCommandCanExecute()
        {
            if (SelectedValue?.IsInGlobalEdition != false)
            {
                return false;
            }

            if (SelectedValue is ControlJobDetailsViewModel cj)
            {
                return cj.ProcessJobs.Count > 0
                       && cj.State == State.SELECTED
                       && App.ControllerInstance.ControllerEquipmentManager.IsControllerOk();
            }

            return false;
        }

        #endregion

        #region Add

        private void AddCommandExecute()
        {
            var popupSelectAdd = new Popup(
                new LocalizableText(nameof(GemGeneralRessources.GEM_ADD)),
                new LocalizableText(GemGeneralRessources.GEM_ADD_CHOICE))
            {
                SeverityLevel = MessageLevel.Info
            };

            popupSelectAdd.Commands.Add(
                new PopupCommand(
                    nameof(Agileo.GUI.Properties.Resources.S_CANCEL)));

            popupSelectAdd.Commands.Add(
                new PopupCommand(
                    new LocalizableText(ControlJobResources.CJ_CONTROL_JOB),
                    new DelegateCommand(
                        () =>
                        {
                            IsExpanderOpened = true;
                            SelectedValue = new ControlJobDetailsViewModel(Popups, false, true);
                            SelectedValue.ChangeEditionModeCommandVisibility(true, false);
                        },
                        () => ProcessJobs.Count > 0)));

            popupSelectAdd.Commands.Add(
                new PopupCommand(
                    new LocalizableText(ProcessJobsResources.PJ_PROCESS_JOB),
                    new DelegateCommand(
                        () =>
                        {
                            IsExpanderOpened = true;
                            SelectedValue = new ProcessJobDetailsViewModel(
                                Popups,
                                false,
                                true);
                            SelectedValue.ChangeEditionModeCommandVisibility(true, false);
                        })));

            Popups.Show(popupSelectAdd);
        }

        private bool AddCommandCanExecute()
        {
            if (SelectedValue != null)
            {
                return !SelectedValue.IsInCreation && App.ControllerInstance.ControllerEquipmentManager.IsControllerOk();
            }

            return App.ControllerInstance.ControllerEquipmentManager.IsControllerOk();
        }

        #endregion

        #region Delete

        private void DeleteCommandExecute()
        {
            switch (SelectedValue)
            {
                case ControlJobDetailsViewModel cj:
                    var popupSelectDelete = new Popup(
                        new LocalizableText(nameof(GemGeneralRessources.GEM_DELETE)),
                        new LocalizableText(
                            nameof(GemGeneralRessources.GEM_DELETE_CHOICE),
                            cj.ObjId)) { SeverityLevel = MessageLevel.Info };

                    popupSelectDelete.Commands.Add(
                        new PopupCommand(
                            nameof(Agileo.GUI.Properties.Resources.S_CANCEL),
                            icon: PathIcon.Cancel));
                    popupSelectDelete.Commands.Add(
                        new PopupCommand(
                            new LocalizableText(GemGeneralRessources.GEM_DELETE_ONLY),
                            new DelegateCommand(() => DeleteCjOnly(cj))));
                    popupSelectDelete.Commands.Add(
                        new PopupCommand(
                            new LocalizableText(GemGeneralRessources.GEM_DELETE_ALL),
                            new DelegateCommand(() => DeleteCjAndItsPj(cj))));

                    Popups.Show(popupSelectDelete);
                    break;
                case ProcessJobDetailsViewModel pj:
                    DeletePj(pj);
                    break;
            }
        }

        private bool DeleteCommandCanExecute()
        {
            if (SelectedValue?.IsInGlobalEdition != false)
            {
                return false;
            }

            return SelectedValue switch
            {
                ControlJobDetailsViewModel cj => cj.State != State.SELECTED && App.ControllerInstance.ControllerEquipmentManager.IsControllerOk(),
                ProcessJobDetailsViewModel pj => !string.IsNullOrEmpty(pj.ProcessJob.RecipeID)
                                                 && pj.ProcessJob.JobState == JobState.QUEUED_POOLED
                                                 && App.ControllerInstance.ControllerEquipmentManager.IsControllerOk(),
                _ => false
            };
        }

        private static void DeleteCjOnly(ControlJobDetailsViewModel cj)
        {
            var result = GemController.E94Std.StandardServices.Cancel(
                cj.ObjId,Action.SaveJobs);
            cj.SendSuccessFailureMessage(result, GemGeneralRessources.GEM_DELETE);
        }

        private static void DeleteCjAndItsPj(ControlJobDetailsViewModel cj)
        {
            var result = GemController.E94Std.StandardServices.Cancel(
                cj.ObjId,
                Action.RemoveJobs);
            cj.SendSuccessFailureMessage(result, GemGeneralRessources.GEM_DELETE);
        }

        private void DeletePj(ProcessJobDetailsViewModel pj)
        {
            var popup = new Popup(
                new LocalizableText(nameof(GemGeneralRessources.GEM_DELETE)),
                new LocalizableText(
                    nameof(GemGeneralRessources.GEM_DELETE_CONFIRMATION),
                    pj.ProcessJob.ObjID))
            {
                SeverityLevel = MessageLevel.Warning,
                Commands =
                {
                    new PopupCommand(
                        nameof(Agileo.GUI.Properties.Resources.S_CANCEL),
                        icon: PathIcon.Cancel),
                    new PopupCommand(
                        nameof(Agileo.GUI.Properties.Resources.S_OK),
                        new DelegateCommand(
                            () =>
                            {
                                var result =
                                    GemController.E40Std.StandardServices
                                        .Dequeue(new List<string> {pj.ProcessJob.ObjID});
                                pj.SendSuccessFailureMessage(
                                    result.Status,
                                    GemGeneralRessources.GEM_DELETE);
                            }),
                        PathIcon.Checked)
                }
            };
            Popups.Show(popup);
        }

        #endregion

        #region Edit

        private void EditCjCommandExecute()
        {
            if (SelectedValue is not ProcessJobDetailsViewModel pj)
            {
                return;
            }

            IsExpanderOpened = true;
            pj.IsInEdition = true;
            pj.ChangeEditionModeCommandVisibility(true, false);
        }

        private bool EditCjCommandCanExecute()
        {
            if (SelectedValue is ProcessJobDetailsViewModel pj && pj.ProcessJob != null)
            {
                return !string.IsNullOrEmpty(pj.ProcessJob.RecipeID)
                       && !pj.IsInEdition
                       && !pj.IsInCreation
                       && App.ControllerInstance.ControllerEquipmentManager.IsControllerOk();
            }

            return false;
        }

        #endregion

        #region Duplicate

        private void DuplicatePjCommandExecute()
        {
            if (SelectedValue is not ProcessJobDetailsViewModel pj)
            {
                return;
            }

            //Ask for new job name
            var list = GemController.E40Std.ProcessJobs
                .Select(id => id.ObjID)
                .ToList();

            var idPopup = new JobIdPopupViewModel(list);
            var popup = new Popup(
                new LocalizableText(nameof(ProcessJobsResources.PJ_PROCESS_JOB)),
                new LocalizableText(nameof(ProcessJobsResources.PJ_INQUIRE_ID)))
            {
                Content = idPopup
            };

            popup.Commands.Add(new PopupCommand(nameof(GemGeneralRessources.GEM_CANCEL)));
            popup.Commands.Add(
                new PopupCommand(
                    nameof(GemGeneralRessources.GEM_ADD),
                    new DelegateCommand(
                        () =>
                        {
                            var recipe = new Recipe
                            {
                                ID = pj.ProcessJob.RecipeID,
                                Method = pj.ProcessJob.RecipeMethod
                            };

                            var materialNameList = pj.ProcessJob.CarrierIDSlotsAssociation
                                .Select(
                                    carrier => new MaterialNameListElement(
                                        carrier.CarrierID,
                                        carrier.SlotIds))
                                .ToList();
                            var result =
                                GemController.E40Std.StandardServices
                                    .CreateEnh(
                                        idPopup.JobName,
                                        materialNameList,
                                        recipe,
                                        pj.ProcessJob.ProcessStart,
                                        pj.ProcessJob.PauseEvent);
                            pj.SendSuccessFailureMessage(
                                result.Status,
                                GemGeneralRessources.GEM_DUPLICATE);
                        },
                        () => !idPopup.HasErrors)));
            Popups.Show(popup);
        }

        private bool DuplicatePjCommandCanExecute()
        {
            if (SelectedValue is ProcessJobDetailsViewModel pj && pj.ProcessJob != null)
            {
                return !string.IsNullOrEmpty(pj.ProcessJob.RecipeID)
                       && App.ControllerInstance.ControllerEquipmentManager.IsControllerOk();
            }

            return false;
        }

        #endregion

        #region Go to carrier

        public static DelegateCommand<CorrespondingCarrierSlotMapViewModel>
            GoToCarrierCommand { get; } = new(
            GoToCarrierCommandExecute,
            GoToCarrierCommandCanExecute);

        private static void GoToCarrierCommandExecute(
            CorrespondingCarrierSlotMapViewModel correspondingE40Carrier)
            => _carriersPanel.Navigate(correspondingE40Carrier.Carrier.ObjID);

        private static bool GoToCarrierCommandCanExecute(
            CorrespondingCarrierSlotMapViewModel e40CarrierInfo)
        {
            if (_carriersPanel is not { CanNavigateTo: true })
            {
                return false;
            }

            return e40CarrierInfo?.Carrier != null
                   && GemController.E87Std?.GetCarrierById(
                       e40CarrierInfo.Carrier.ObjID)
                   != null;
        }

        public static DelegateCommand<string> GoToCarrierFromIdCommand { get; } = new(
            GoToCarrierFromIdCommandExecute,
            GoToCarrierFromIdCommandCanExecute);

        private static void GoToCarrierFromIdCommandExecute(string carrierId)
            => _carriersPanel.Navigate(carrierId);

        private static bool GoToCarrierFromIdCommandCanExecute(string carrierId)
            => _carriersPanel is { CanNavigateTo: true };

        #endregion

        #endregion

        #region Event Handler

        private void E94StdOnControlJobChanged(object sender, ControlJobStateChangedEventArgs e)
        {
            UpdateControlJob(e.ControlJob);
        }

        private void E94StdOnControlJobDisposed(object sender, ControlJobEventArgs e)
        {
            DispatcherHelper.DoInUiThread(
                () =>
                {
                    var cjNode = PjCjTreeSource.GetFlattenElements()
                        .Find(
                            job => job.Model is ControlJobDetailsViewModel cj
                                   && cj.ObjId.Equals(e.ControlJob.ObjID));

                    if (cjNode == null)
                    {
                        return;
                    }

                    if (cjNode.HasChild)
                    {
                        var pjNode =
                            GetAssociatedProcessJobDetailsNode(
                                (ControlJobDetailsViewModel)cjNode.Model);

                        // Add all the ProcessJobs to the root of the tree
                        PjCjTreeSource.AddRange(pjNode.Select(node => node.Model));
                    }

                    PjCjTreeSource.Remove(cjNode);
                });
        }

        private void E94StdOnControlJobInstantiated(object sender, ControlJobEventArgs e)
        {
            DispatcherHelper.DoInUiThread(
                () =>
                {
                    var controlJob = new ControlJobDetailsViewModel(
                        e.ControlJob,
                        Popups,
                        false,
                        false);

                    var associatedProcessJobDetailsNode =
                        GetAssociatedProcessJobDetailsNode(controlJob);

                    // Remove all associated ProcessJobs from tree
                    PjCjTreeSource.RemoveRange(associatedProcessJobDetailsNode);

                    // Add all the ProcessJobs to the new ControlJob
                    controlJob.Children.AddRange(
                        associatedProcessJobDetailsNode.Select(node => node.Model));

                    // Add the ControlJob to the tree
                    PjCjTreeSource.Add(controlJob);
                });
        }

        private void E94StdOnControlJobMovedToHeadOfQueue(object sender, ControlJobEventArgs e)
        {
            ReorderControlJobs();
        }

        private void E40StdOnProcessJobInstantiated(object sender, ProcessJobEventArgs e)
        {
            DispatcherHelper.DoInUiThread(
                () =>
                {
                    PjCjTreeSource.Add(
                        new ProcessJobDetailsViewModel(e.ProcessJob, Popups, false, false));
                });
        }

        private void E40StdOnProcessJobDisposed(object sender, ProcessJobEventArgs e)
        {
            DispatcherHelper.DoInUiThread(
                () =>
                {
                    var pj = PjCjTreeSource.GetFlattenElements()
                        .Find(
                            job => job.Model is ProcessJobDetailsViewModel pj
                                   && pj.ProcessJob.ObjID.Equals(e.ProcessJob.ObjID));
                    PjCjTreeSource.Remove(pj);
                });
        }

        private void E40StdOnProcessJobChanged(object sender, ProcessJobStateChangedEventArgs e)
        {
            UpdateProcessJob(e.ProcessJob);
        }

        private void JobDetailsOnJobChanged(object sender, JobDetailsChangedEventArgs e)
        {
            switch (e.Job)
            {
                case ProcessJobDetailsViewModel pj:
                    UpdateProcessJob(pj.ProcessJob);
                    break;
                case ControlJobDetailsViewModel cj:
                    var currentCj = ControlJobs.FirstOrDefault(job => job.ObjID.Equals(cj.ObjId));
                    if (currentCj != null)
                    {
                        UpdateControlJob(currentCj);
                    }

                    break;
            }

            if (!e.CloseExpander)
            {
                return;
            }

            IsExpanderOpened = false;
            SelectedValue = null;
        }

        #endregion

        #region Override

        #region Overrides of IdentifiableElement

        public override void OnSetup()
        {
            base.OnSetup();

            UpdateJobsTree();

            GemController.E40Std.ProcessJobChanged +=
                E40StdOnProcessJobChanged;
            GemController.E40Std.ProcessJobDisposed +=
                E40StdOnProcessJobDisposed;
            GemController.E40Std.ProcessJobInstantiated +=
                E40StdOnProcessJobInstantiated;

            GemController.E94Std.ControlJobChanged +=
                E94StdOnControlJobChanged;
            GemController.E94Std.ControlJobDisposed +=
                E94StdOnControlJobDisposed;
            GemController.E94Std.ControlJobInstantiated +=
                E94StdOnControlJobInstantiated;
            GemController.E94Std.ControlJobMovedToHeadOfQueue +=
                E94StdOnControlJobMovedToHeadOfQueue;

            if (_carriersPanel == null)
            {
#pragma warning disable S2696 // Instance members should not write to "static" fields
                _carriersPanel = AgilControllerApplication.Current.UserInterface.BusinessPanels
                    .OfType<CarriersViewerPanel>()
                    .FirstOrDefault();
#pragma warning restore S2696 // Instance members should not write to "static" fields
            }
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            GemController.E40Std.ProcessJobChanged -=
                E40StdOnProcessJobChanged;
            GemController.E40Std.ProcessJobDisposed -=
                E40StdOnProcessJobDisposed;
            GemController.E40Std.ProcessJobInstantiated -=
                E40StdOnProcessJobInstantiated;

            GemController.E94Std.ControlJobChanged -=
                E94StdOnControlJobChanged;
            GemController.E94Std.ControlJobDisposed -=
                E94StdOnControlJobDisposed;
            GemController.E94Std.ControlJobInstantiated -=
                E94StdOnControlJobDisposed;
            GemController.E94Std.ControlJobMovedToHeadOfQueue -=
                E94StdOnControlJobDisposed;

            if (SelectedValue is not null)
            {
                SelectedValue.JobChanged -= JobDetailsOnJobChanged;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
