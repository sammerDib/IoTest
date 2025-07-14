using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Agileo.Common.Localization;
using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;
using Agileo.Semi.Gem.Abstractions.E30;
using Agileo.Semi.Gem300.Abstractions.E40;
using Agileo.Semi.Gem300.Abstractions.E87;
using Agileo.Semi.Gem300.Abstractions.E94;

using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager;
using UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Devices.Controller;
using UnitySC.Equipment.Devices.Controller.Activities.WaferFlow.Enum;
using UnitySC.Equipment.Devices.Controller.JobDefinition;
using UnitySC.GUI.Common.Equipment.LoadPort;
using UnitySC.GUI.Common.UIComponents.Commands;
using UnitySC.GUI.Common.UIComponents.Components.Equipment;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Commands;
using UnitySC.GUI.Common.Vendor.UIComponents.GuiExtended;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.GUI.Common.Vendor.Views.Panels.Recipes;
using UnitySC.GUI.Common.Views.Panels.Production.Equipment.Views.ProcessModules;
using UnitySC.UTO.Controller.Counters;
using UnitySC.UTO.Controller.JobQueuer;
using UnitySC.UTO.Controller.Remote.Constants;
using UnitySC.UTO.Controller.Remote.Helpers;
using UnitySC.UTO.Controller.Remote.Observers;
using UnitySC.UTO.Controller.Views.Panels.EquipmentHandling;
using UnitySC.UTO.Controller.Views.Panels.Gem.Equipment.LoadPorts;
using UnitySC.UTO.Controller.Views.Panels.Gem.Jobs.ProcessJobs;
using UnitySC.UTO.Controller.Views.Panels.Production.Equipment.Popup;
using UnitySC.UTO.Controller.Views.Panels.Production.Equipment.Views;

using Action = Agileo.Semi.Gem300.Abstractions.E94.Action;
using Carrier = UnitySC.Equipment.Abstractions.Material.Carrier;
using LoadPort = UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort;

namespace UnitySC.UTO.Controller.Views.Panels.Production.Equipment
{
    public class EquipmentPanel : BusinessPanel
    {
        #region Fields

        private static IE30Standard E30Std => App.ControllerInstance.GemController.E30Std;
        private static IE87Standard E87Std => App.ControllerInstance.GemController.E87Std;
        private static IE40Standard E40Std => App.ControllerInstance.GemController.E40Std;
        private static IE94Standard E94Std => App.ControllerInstance.GemController.E94Std;
        private static JobQueueManager JobQueueManager => App.ControllerInstance.JobQueueManager;

        #endregion

        #region Constructors

        static EquipmentPanel()
        {
            DataTemplateGenerator.Create(typeof(EquipmentPanel), typeof(EquipmentPanelView));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(ProductionEquipmentResources)));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(E84Resources)));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(RecipePanelResources)));
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a design time instance of the
        /// <see
        ///     cref="T:UnitySC.UTO.Controller.Vendor.Views.Panels.Maintenance.Equipment.LoadPortsViewer.LoadPortsCardViewerPanel" />
        /// class.
        /// </summary>
        public EquipmentPanel()
            : this("DesignTime Constructor")
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }

            for (var i = 0; i < 3; i++)
            {
                LoadPortViewers.Add(new LoadPortViewer());
            }
        }

        /// <inheritdoc />
        public EquipmentPanel(string id, IIcon icon = null)
            : base(id, icon)
        {
            AbortRecipeBusinessPanelCommand = new InvisibleBusinessPanelCommand(
                nameof(EquipmentHandlingResources.EQUIPMENT_PM_ABORT_RECIPE),
                new DelegateCommand(() => { }));

            Commands.Add(AbortRecipeBusinessPanelCommand);

            Commands.Add(
                new ContextualBusinessPanelCommand(
                    nameof(EquipmentHandlingResources.EQUIPMENT_INIT),
                    InitCommand,
                    PathIcon.Refresh));
            Commands.Add(
                new ContextualBusinessPanelCommand(
                    nameof(EquipmentHandlingResources.EQUIPMENT_CREATE_JOB),
                    CreateJobCommand,
                    PathIcon.Add));
            Commands.Add(
                new ContextualBusinessPanelCommand(
                    nameof(EquipmentHandlingResources.EQUIPMENT_CANCEL),
                    CancelCommand,
                    PathIcon.Cancel));
            Commands.Add(
                new ContextualBusinessPanelCommand(
                    nameof(EquipmentHandlingResources.EQUIPMENT_ABORT),
                    AbortCommand,
                    PathIcon.Abort));
            Commands.Add(
                new ContextualBusinessPanelCommand(
                    nameof(EquipmentHandlingResources.EQUIPMENT_RESUME),
                    ResumeCommand,
                    PathIcon.Play));
            Commands.Add(
                new ContextualBusinessPanelCommand(
                    nameof(EquipmentHandlingResources.EQUIPMENT_PAUSE),
                    PauseCommand,
                    PathIcon.Pause));
            Commands.Add(
                new ContextualBusinessPanelCommand(
                    nameof(EquipmentHandlingResources.EQUIPMENT_STOP),
                    StopCommand,
                    PathIcon.Stop));

            var resetCommand = new SafeDelegateCommand(ResetCommandExecute, ResetCommandCanExecute);
            ResetCommand = new InvisibleBusinessPanelCommand(
                nameof(EquipmentHandlingResources.EQUIPMENT_RESET),
                resetCommand,
                PathIcon.Restore);
            Commands.Add(ResetCommand);

            JobQueueViewModel = new JobQueueViewModel();
        }

        #endregion

        #region Properties
        public MachineViewModel MachineViewModel { get; set; }

        public InvisibleBusinessPanelCommand AbortRecipeBusinessPanelCommand { get; }

        public List<LoadPortViewer> LoadPortViewers { get; } = new();

        public List<IProductionProcessModuleViewModel> ProcessModuleViewModels { get; } = new();

        public ControllerEquipmentManager EquipmentManager { get; private set; }

        public UnitySC.Equipment.Devices.Controller.Controller Controller { get; private set; }

        public JobQueueViewModel JobQueueViewModel { get; }

        public bool IsJobQueueDisplayedAsATool => LoadPortViewers.Count > 2;

        public UserMessageDisplayer UserMessageDisplayer
            => App.ControllerInstance.MainUserMessageDisplayer;

        public bool InvertProcessModules
            => GUI.Common.App.Instance.Config.EquipmentConfig.InvertPmOnUserInterface;

        public string EquipmentName
            => E30Std.EquipmentConstantsServices.GetValueByWellKnownName(ECs.EqpName)
                .ValueTo<string>();

        public bool ThroughputDisplay
            => App.ControllerInstance.ThroughputDisplay;

        public ulong ProcessedSubstrateCounter
            => App.ControllerInstance.CountersManager.PersistentCounters.ProcessedSubstrateCounter;
        #endregion

        #region Overrides of IdentifiableElement

        /// <inheritdoc />
        public override void OnSetup()
        {
            base.OnSetup();
            
            EquipmentManager = App.ControllerInstance.ControllerEquipmentManager;

            foreach (var processModule in EquipmentManager.Equipment
                         .AllOfType<DriveableProcessModule>())
            {
                var card = App.UtoInstance.DeviceUiManagerService.GetProductionCardViewModel(processModule);
                if (card is IProductionProcessModuleViewModel iProcessModuleViewModel)
                {
                    iProcessModuleViewModel.AbortRecipeCanExecute =
                        () => AbortRecipeBusinessPanelCommand.IsEnabled;
                    ProcessModuleViewModels.Add(iProcessModuleViewModel);
                }
            }

            if (InvertProcessModules)
            {
                ProcessModuleViewModels.Reverse();
            }

            foreach (var loadPort in EquipmentManager.Equipment.AllOfType<LoadPort>())
            {
                LoadPortViewers.Add(
                    new LoadPortViewer(
                        loadPort.InstanceId,
                        E87Std.GetLoadPort(loadPort.InstanceId),
                        loadPort));
            }

            MachineViewModel = new MachineViewModel(EquipmentManager, App.ControllerInstance.DeviceUiManagerService, this);

            OnPropertyChanged(nameof(IsJobQueueDisplayedAsATool));

            Controller = EquipmentManager.Controller as UnitySC.Equipment.Devices.Controller.Controller;

            E30Std.EquipmentConstantsServices.EquipmentConstantChanged += EquipmentConstantsServices_EquipmentConstantChanged;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                LoadPortViewers.Clear();
                ProcessModuleViewModels.Clear();
                MachineViewModel?.Dispose();

                E30Std.EquipmentConstantsServices.EquipmentConstantChanged -= EquipmentConstantsServices_EquipmentConstantChanged;
            }

            base.Dispose(disposing);
        }

        #endregion
        
        #region Commands

        #region Init

        private ContextualSafeDelegateCommandAsync _initCommand;

        private ContextualSafeDelegateCommandAsync InitCommand
            => _initCommand ??= new ContextualSafeDelegateCommandAsync(
                InitCommandExecute,
                InitCommandCanExecute);

        private Task InitCommandExecute()
        {
            return EquipmentManager.Controller.InitializeAsync(
                !GUI.Common.App.Instance.Config.UseWarmInit);
        }

        private Tuple<bool, string> InitCommandCanExecute()
        {
            var context = EquipmentManager.Controller.NewCommandContext(
                nameof(EquipmentManager.Controller.Initialize));
            context.AddArgument("mustForceInit", false);
            var canExecute = EquipmentManager.Controller.CanExecute(context);

            var sb = new StringBuilder();
            foreach (var error in context.Errors)
            {
                sb.AppendLine($"- {error}");
            }

            if (App.ControllerInstance.GemController.IsControlledByHost)
            {
                canExecute = false;
                sb.AppendLine("- GEM Control State must not be Remote");
            }

            return new Tuple<bool, string>(canExecute, sb.ToString());
        }

        #endregion

        #region Create Job

        private ContextualSafeDelegateCommand _createJobCommand;

        private ContextualSafeDelegateCommand CreateJobCommand
            => _createJobCommand ??= new ContextualSafeDelegateCommand(
                CreateJobCommandExecute,
                CreateJobCommandCanExecute);

        private void CreateJobCommandExecute()
        {
            OnClickCommandExecute();

            var popupJob = new StartProcessJobPopupViewModel();
            var popup =
                new Agileo.GUI.Services.Popups.Popup(
                    new LocalizableText(nameof(ProcessJobsResources.PJ_PROCESS_JOB)))
                {
                    IsFullScreen = true, Content = popupJob
                };
            popup.Commands.Add(
                new PopupCommand(
                    nameof(ProductionEquipmentResources.CANCEL),
                    new DelegateCommand(() => { })));
            popup.Commands.Add(
                new PopupCommand(
                    nameof(EquipmentHandlingResources.EQUIPMENT_START_JOB),
                    new DelegateCommand(
                        () => StartProcessJobPopup_StartCommandExecute(popupJob),
                        () =>
                        {
                            if (!popupJob.ValidateJob()
                                || (Controller.State is not (OperatingModes.Idle or OperatingModes.Executing)))
                            {
                                return false;
                            }

                            var wafers = new List<Wafer>();

                            foreach (var viewModel in popupJob.SubstrateSelectionViewers)
                            {
                                Helpers.BuildSubstratesListByCarrier(
                                    viewModel.LoadPort?.Carrier,
                                    viewModel.LpSelectedSlots,
                                    wafers);
                            }

                            var job = new Job(
                                "Job",
                                "controlJob",
                                popupJob.DataFlowTree.SelectedRecipe.Name,
                                wafers,
                                popupJob.OcrReading
                                    ? popupJob.SelectedProfile
                                    : null);

                            return EquipmentManager.Controller.CanExecute(
                                nameof(IController.StartJobExecution),
                                out _,
                                job);
                        })));
            Popups.Show(popup);
        }

        private void StartProcessJobPopup_StartCommandExecute(
            StartProcessJobPopupViewModel popupJob)
        {
            Popups.ShowDuring(
                new BusyIndicator(ProductionEquipmentResources.EQUIPMENT_JOBINPROGRESS),
                () =>
                {
                    var loopMode = false;
                    uint numberOfExecutions = 1;

                    //Get information about cycling mode
                    if (popupJob.CyclingMode)
                    {
                        loopMode = popupJob.LoopMode;
                        numberOfExecutions = loopMode
                            ? 1
                            : popupJob.NumberOfExecutions;
                    }

                    var selectedSlotsByCarrier = new Dictionary<Carrier, List<IndexedSlotState>>();

                    foreach (var viewModel in popupJob.SubstrateSelectionViewers)
                    {
                        if (viewModel.LoadPort is { Carrier: not null })
                        {
                            selectedSlotsByCarrier.Add(
                                viewModel.LoadPort.Carrier,
                                TreatCarrierPickOrder(viewModel.LpSelectedSlots));
                        }
                    }

                    var dataFlowManager = App.ControllerInstance.ControllerEquipmentManager.Controller
                              					.TryGetDevice<AbstractDataFlowManager>();
                    var recipeName = dataFlowManager.GetRecipeName(popupJob.DataFlowTree.SelectedRecipe);
                    
                    App.ControllerInstance.JobQueueManager.AddJob(
                        loopMode,
                        numberOfExecutions,
                        selectedSlotsByCarrier,
                        recipeName,
                        popupJob.OcrReading,
                        popupJob.SelectedProfile);
                });
        }

        private Tuple<bool, string> CreateJobCommandCanExecute()
        {
            var canExecute = true;
            var sb = new StringBuilder();
            if (Controller.State != OperatingModes.Idle
                && Controller.State != OperatingModes.Executing)
            {
                canExecute = false;
                sb.AppendLine("- Controller must be Idle or Executing");
            }

            if (App.ControllerInstance.GemController.IsControlledByHost)
            {
                canExecute = false;
                sb.AppendLine("- GEM Control State must not be Remote");
            }

            return new Tuple<bool, string>(canExecute, sb.ToString());
        }

        #endregion

        #region Cancel

        private ContextualSafeDelegateCommandAsync _cancelCommand;

        private ContextualSafeDelegateCommandAsync CancelCommand
            => _cancelCommand ??= new ContextualSafeDelegateCommandAsync(
                CancelCommandExecute,
                CancelCommandCanExecute);

        private Task CancelCommandExecute()
        {
            if (App.ControllerInstance.IsGem300Supported)
            {
                return Task.Run(
                    () =>
                    {
                        foreach (var selectedJob in JobQueueViewModel.SelectedJobs.ToList())
                        {
                            try
                            {
                                if (E94Std.ControlJobs.FirstOrDefault(
                                        j => j.ObjID == selectedJob.ControlJob.ObjID) is { } cj)
                                {
                                    E94Std.StandardServices.Cancel(cj.ObjID, Action.RemoveJobs);
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            finally
                            {
                                JobQueueManager.RemoveJob(selectedJob);
                            }
                        }
                    });
            }

            return Task.Run(
                () =>
                {
                    foreach (var selectedJob in JobQueueViewModel.SelectedJobs)
                    {
                        JobQueueManager.RemoveJob(selectedJob);
                    }
                });
        }

        private Tuple<bool, string> CancelCommandCanExecute()
        {
            var canExecute = true;
            var sb = new StringBuilder();
            if (Controller.State != OperatingModes.Idle
                && Controller.State != OperatingModes.Executing)
            {
                canExecute = false;
                sb.AppendLine("- Controller must be Idle or Executing");
            }

            if (App.ControllerInstance.GemController.IsControlledByHost)
            {
                canExecute = false;
                sb.AppendLine("- GEM Control State must not be Remote");
            }

            if (JobQueueViewModel.SelectedJobs.Count == 0)
            {
                canExecute = false;
                sb.AppendLine("- Please select a job in the job queue");
            }

            if (JobQueueViewModel.SelectedJobs.Any(job => job.ControlJob == null))
            {
                canExecute = false;
                sb.AppendLine("- At least one selected job has no control job defined");
            }

            if (JobQueueViewModel.SelectedJobs.Any(job => job.ControlJob != null && job.ControlJob.State != State.QUEUED))
            {
                canExecute = false;
                sb.AppendLine("- At least one selected job is not in QUEUED state");
            }

            return new Tuple<bool, string>(canExecute, sb.ToString());
        }

        #endregion

        #region Abort

        private ContextualSafeDelegateCommandAsync _abortCommand;

        private ContextualSafeDelegateCommandAsync AbortCommand
            => _abortCommand ??= new ContextualSafeDelegateCommandAsync(
                AbortCommandExecute,
                AbortCommandCanExecute);

        private Task AbortCommandExecute()
        {
            if (App.ControllerInstance.IsGem300Supported)
            {
                return Task.Run(
                    () =>
                    {
                        var executingCj = E94Std.ControlJobs.Where(
                                cj => cj.State is not State.QUEUED and not State.COMPLETED)
                            .ToList();
                        if (executingCj.Count == 0)
                        {
                            Controller.Interrupt(InterruptionKind.Abort);
                            return;
                        }

                        foreach (var controlJob in executingCj)
                        {
                            E94Std.StandardServices.Abort(controlJob.ObjID, Action.RemoveJobs);
                        }
                    });
            }

            return Controller.InterruptAsync(InterruptionKind.Abort);
        }

        private Tuple<bool, string> AbortCommandCanExecute()
        {
            var canExecute = true;
            var sb = new StringBuilder();
            if (Controller.State != OperatingModes.Initialization
                && Controller.State != OperatingModes.Executing)
            {
                canExecute = false;
                sb.AppendLine("- Controller must be Idle or Executing");
            }

            if (App.ControllerInstance.GemController.IsControlledByHost)
            {
                canExecute = false;
                sb.AppendLine("- GEM Control State must not be Remote");
            }

            return new Tuple<bool, string>(canExecute, sb.ToString());
        }

        #endregion

        #region Pause

        private ContextualSafeDelegateCommandAsync _pauseCommand;

        private ContextualSafeDelegateCommandAsync PauseCommand
            => _pauseCommand ??= new ContextualSafeDelegateCommandAsync(
                PauseCommandExecute,
                PauseCommandCanExecute);

        private Task PauseCommandExecute()
        {
            if (App.ControllerInstance.IsGem300Supported)
            {
                return Task.Run(
                    () =>
                    {
                        foreach (var selectedJob in JobQueueViewModel.SelectedJobs)
                        {
                            var processJob = E40Std.ProcessJobs.FirstOrDefault(
                                pj => pj.ObjID == selectedJob.ProcessJob.ObjID);
                            if (processJob != null)
                            {
                                E40Std.StandardServices.Command(
                                    processJob.ObjID,
                                    CommandName.PAUSE,
                                    new List<CommandParameter>());
                            }
                        }
                    });
            }

            return Task.Run(
                () =>
                {
                    foreach (var selectedJob in JobQueueViewModel.SelectedJobs)
                    {
                        Controller.Pause(selectedJob.ProcessJob.ObjID);
                    }
                });
        }

        private Tuple<bool, string> PauseCommandCanExecute()
        {
            var canExecute = true;
            var sb = new StringBuilder();
            if (Controller.State != OperatingModes.Idle
                && Controller.State != OperatingModes.Executing)
            {
                canExecute = false;
                sb.AppendLine("- Controller must be Idle or Executing");
            }

            if (App.ControllerInstance.GemController.IsControlledByHost)
            {
                canExecute = false;
                sb.AppendLine("- GEM Control State must not be Remote");
            }

            if (JobQueueViewModel.SelectedJobs.Count == 0)
            {
                canExecute = false;
                sb.AppendLine("- Please select a job in the job queue");
            }

            foreach (var job in JobQueueViewModel.SelectedJobs)
            {
               if(Controller.CanExecute(
                    nameof(IController.Pause),
                    out var context,
                    job.ProcessJob?.ObjID))
               {
                   continue;
               }

               canExecute = false;
               foreach (var error in context.Errors)
               {
                    sb.AppendLine($"- {error}");
               }
            }

            return new Tuple<bool, string>(canExecute, sb.ToString());
        }

        #endregion

        #region Resume

        private ContextualSafeDelegateCommandAsync _resumeCommand;

        private ContextualSafeDelegateCommandAsync ResumeCommand
            => _resumeCommand ??= new ContextualSafeDelegateCommandAsync(
                ResumeCommandExecute,
                ResumeCommandCanExecute);

        private Task ResumeCommandExecute()
        {
            if (App.ControllerInstance.IsGem300Supported)
            {
                return Task.Run(
                    () =>
                    {
                        foreach (var selectedJob in JobQueueViewModel.SelectedJobs)
                        {
                            var processJob = E40Std.ProcessJobs.FirstOrDefault(
                                pj => pj.ObjID == selectedJob.ProcessJob.ObjID);
                            if (processJob != null)
                            {
                                E40Std.StandardServices.Command(
                                    processJob.ObjID,
                                    CommandName.RESUME,
                                    new List<CommandParameter>());
                            }
                        }
                    });
            }

            return Task.Run(
                () =>
                {
                    foreach (var selectedJob in JobQueueViewModel.SelectedJobs)
                    {
                        Controller.Resume(selectedJob.ProcessJob.ObjID);
                    }
                });
        }

        private Tuple<bool, string> ResumeCommandCanExecute()
        {
            var canExecute = true;
            var sb = new StringBuilder();

            if (Controller.State != OperatingModes.Idle
                && Controller.State != OperatingModes.Executing)
            {
                canExecute = false;
                sb.AppendLine("- Controller must be Idle or Executing");
            }

            if (App.ControllerInstance.GemController.IsControlledByHost)
            {
                canExecute = false;
                sb.AppendLine("- GEM Control State must not be Remote");
            }

            if (JobQueueViewModel.SelectedJobs.Count == 0)
            {
                canExecute = false;
                sb.AppendLine("- Please select a job in the job queue");
            }

            foreach (var job in JobQueueViewModel.SelectedJobs)
            {
                if (Controller.CanExecute(
                        nameof(IController.Resume),
                        out var context,
                        job.ProcessJob?.ObjID))
                {
                    continue;
                }

                canExecute = false;
                foreach (var error in context.Errors)
                {
                    sb.AppendLine($"- {error}");
                }
            }

            return new Tuple<bool, string>(canExecute, sb.ToString());
        }

        #endregion

        #region Stop

        private ContextualSafeDelegateCommandAsync _stopCommand;

        private ContextualSafeDelegateCommandAsync StopCommand
            => _stopCommand ??= new ContextualSafeDelegateCommandAsync(
                StopCommandExecute,
                StopCommandCanExecute);

        private Task StopCommandExecute()
        {
            if (App.ControllerInstance.IsGem300Supported)
            {
                return Task.Run(
                    () =>
                    {
                        foreach (var selectedJob in JobQueueViewModel.SelectedJobs.Where(j => j.ProcessJob != null))
                        {
                            var processJob = E40Std.ProcessJobs.FirstOrDefault(
                                pj => pj.ObjID == selectedJob.ProcessJob.ObjID);
                            if (processJob != null)
                            {
                                E40Std.StandardServices.Command(
                                    processJob.ObjID,
                                    CommandName.STOP,
                                    new List<CommandParameter>());
                            }
                        }
                    });
            }

            return Task.Run(
                () =>
                {
                    var stopConfig = App.ControllerInstance.GemController.E30Std
                        .EquipmentConstantsServices.GetValueByWellKnownName(ECs.StopConfig)
                        .ValueTo<StopConfig>();

                    foreach (var selectedJob in JobQueueViewModel.SelectedJobs)
                    {
                        Controller.Stop(selectedJob.ProcessJob.ObjID, stopConfig);
                    }
                });
        }

        private Tuple<bool, string> StopCommandCanExecute()
        {
            var stopConfig = App.ControllerInstance.GemController.E30Std.EquipmentConstantsServices
                .GetValueByWellKnownName(ECs.StopConfig)
                .ValueTo<StopConfig>();

            var canExecute = true;
            var sb = new StringBuilder();

            if (Controller.State != OperatingModes.Idle
                && Controller.State != OperatingModes.Executing)
            {
                canExecute = false;
                sb.AppendLine("- Controller must be Idle or Executing");
            }

            if (App.ControllerInstance.GemController.IsControlledByHost)
            {
                canExecute = false;
                sb.AppendLine("- GEM Control State must not be Remote");
            }

            if (JobQueueViewModel.SelectedJobs.Count == 0)
            {
                canExecute = false;
                sb.AppendLine("- Please select a job in the job queue");
            }

            foreach (var job in JobQueueViewModel.SelectedJobs)
            {
                if (Controller.CanExecute(
                        nameof(IController.Stop),
                        out var context,
                        job.ProcessJob?.ObjID,
                        stopConfig))
                {
                    continue;
                }

                canExecute = false;
                foreach (var error in context.Errors)
                {
                    sb.AppendLine($"- {error}");
                }
            }

            return new Tuple<bool, string>(canExecute, sb.ToString());
        }

        #endregion

        #region Reset Counter Command

        public InvisibleBusinessPanelCommand ResetCommand { get; }

        private bool ResetCommandCanExecute()
        {
            return ResetCommand?.IsEnabled ?? false;
        }

        private void ResetCommandExecute()
        {
            App.ControllerInstance.CountersManager.IncrementCounter(CounterDefinition.ProcessedSubstrateCounter, true);
        }

        #endregion

        #region Click Command

        //Used to close job queue tool when click outside the tool

        private SafeDelegateCommand _onClickCommand;

        /// <summary>
        /// Command allowing the automatic closing of tools (triggered by clicking on a P4P Panel)
        /// </summary>
        public SafeDelegateCommand OnClickCommand
            => _onClickCommand ??= new SafeDelegateCommand(OnClickCommandExecute);

        private void OnClickCommandExecute()
        {
            if (FlattenToolReferences == null)
            {
                return;
            }

            foreach (var tool in FlattenToolReferences.Select(tool => tool.Tool))
            {
                if (tool is JobQueueViewModel && tool.IsOpen)
                {
                    tool.IsOpen = false;
                }
            }
        }

        #endregion

        #endregion

        #region Private Methods

        private List<IndexedSlotState> TreatCarrierPickOrder(
            List<IndexedSlotState> originalSlotStates)
        {
            switch (App.ControllerInstance.ControllerConfig.CarrierPickOrder)
            {
                case CarrierPickOrder.BottomToTop:
                    return originalSlotStates.OrderBy(i => i.Index).ToList();
                case CarrierPickOrder.TopToBottom:
                    return originalSlotStates.OrderByDescending(i => i.Index).ToList();
                default:
                    return originalSlotStates;
            }
        }

        #endregion

        #region Public Methods

        public virtual void DisplayUserMessageOnLoadPortViewer(UserMessage message, int loadportId)
        {
            var viewer = LoadPortViewers.FirstOrDefault(x => x.LoadPort.InstanceId == loadportId);
            if (viewer != null)
            {
                viewer.UserMessageDisplayer.Show(message);
            }
        }

        public virtual void HideUserMessageOnLoadPortViewer(UserMessage message, int loadportId)
        {
            var viewer = LoadPortViewers.FirstOrDefault(x => x.LoadPort.InstanceId == loadportId);
            if (viewer != null)
            {
                viewer.UserMessageDisplayer.Hide(message);
            }
        }

        #endregion

        #region Event handler

        private void EquipmentConstantsServices_EquipmentConstantChanged(
            object sender,
            VariableEventArgs e)
        {
            if (e.Variable.Name == ECs.EqpName)
            {
                OnPropertyChanged(nameof(EquipmentName));
            }
        }

        #endregion
    }
}
