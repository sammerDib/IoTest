using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;

using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

using ICSharpCode.AvalonEdit.Highlighting;

using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.Alarms;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.Command;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.LogViewer;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.Status;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.MaterialLocations;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device
{
    public sealed class DeviceViewModel : Notifier, IDisposable
    {
        #region Fields

        private CommandExecution _currentCommandExecution;

        #endregion

        public DeviceViewModel(Agileo.EquipmentModeling.Device model, LogViewerViewModel logViewer)
        {
            Model = model;

            SetupStatusExplorers();
            Model.StatusValueChanged += OnStatusValueChanged;

            SetupCommandExplorers();
            Model.CommandExecutionStateChanged += OnCommandExecutionStateChanged;

            LoggerViewModel = logViewer;

            CreateHighlightingRules();

            var alarmCenter = Model.GetEquipment().AlarmCenter;

            AlarmsViewModel = new DeviceAlarmsViewModel(alarmCenter, Model.Class, Model.InstanceId);
            AlarmsOccurrencesViewModel = new DeviceAlarmOccurrencesViewModel(alarmCenter, Model.Class, Model.InstanceId);

            if (Model is IMaterialLocationContainer locationContainer)
            {
                MaterialLocationsViewModel = new MaterialLocationsViewModel(locationContainer);
                Statuses.Add(MaterialLocationsViewModel);
            }
        }

        #region Properties

        public Agileo.EquipmentModeling.Device Model { get; }

        public MaterialLocationsViewModel MaterialLocationsViewModel { get; }

        public ObservableCollection<DeviceCommandCategoryViewModel> Commands { get; } = new();

        public ObservableCollection<NamedViewModel> Statuses { get; } = new();

        public LogViewerViewModel LoggerViewModel { get; }

        public DeviceAlarmsViewModel AlarmsViewModel { get; }

        public DeviceAlarmOccurrencesViewModel AlarmsOccurrencesViewModel { get; }

        public UserMessageDisplayer Messages { get; } = new();

        private DeviceCommandViewModel SelectedCommand => SelectedCommandExplorer?.SelectedDeviceCommandViewModel;

        private DeviceCommandContextParametersViewModel ParametersViewModel => SelectedCommand?.CommandContext;

        #region Notified Properties

        private DeviceCommandCategoryViewModel _selectedCommandExplorer;
        public DeviceCommandCategoryViewModel SelectedCommandExplorer
        {
            get => _selectedCommandExplorer;
            set => SetAndRaiseIfChanged(ref _selectedCommandExplorer, value);
        }

        private object _selectedLeftSideViewModel;

        public object SelectedLeftSideViewModel
        {
            get => _selectedLeftSideViewModel;
            set => SetAndRaiseIfChanged(ref _selectedLeftSideViewModel, value);
        }

        #endregion

        #endregion

        #region Private methods

        private void SetupStatusExplorers()
        {
            foreach (var group in Model.DeviceType.AllStatuses().GroupBy(x => x.Category))
            {
                string groupName = GetStatusGroupName(group.Key);
                Statuses.Add(new DeviceStatusCategoryViewModel(Model, groupName, group.ToList()));
            }
            SelectedLeftSideViewModel = Statuses.FirstOrDefault();
        }

        private static string GetStatusGroupName(string category) => !string.IsNullOrEmpty(category) ? category : "Status";

        private void SetupCommandExplorers()
        {
            var sendCommand = new DelegateCommand(SendExecute, SendCanExecute);
            var abortCommand = new DelegateCommand(AbortExecute, AbortCanExecute);

            foreach (var group in Model.DeviceType.AllCommands().GroupBy(x => x.Category))
            {
                var groupName = "Commands";
                if (!string.IsNullOrEmpty(group.Key))
                {
                    groupName = group.Key;
                }

                Commands.Add(new DeviceCommandCategoryViewModel(Model, groupName, group, sendCommand, abortCommand));
            }
            SelectedCommandExplorer = Commands.FirstOrDefault();
        }

        private void CreateHighlightingRules()
        {
            foreach (var s in Model.DeviceType.AllStatuses())
            {
                CreateRule(s);
            }

            foreach (var c in Model.DeviceType.AllCommands())
            {
                CreateRule(c);
            }
        }

        private void CreateRule(DeviceCommand command) => BuildHighlightingRule(command.Name, Brushes.SelectionLocationDestinationArrowBrush.Color, FontStyles.Normal);

        private void CreateRule(DeviceStatus status) => BuildHighlightingRule(status.Name, Brushes.SeverityInformationBrush.Color, FontStyles.Normal);

        private void BuildHighlightingRule(string regexPattern, System.Windows.Media.Color color, FontStyle fontStyle)
        {
            LoggerViewModel.SyntaxHighlighting.MainRuleSet.Rules.Add(new HighlightingRule
            {
                Regex = new Regex(regexPattern),
                Color = new HighlightingColor
                {
                    Foreground = new SimpleHighlightingBrush(color),
                    FontStyle = fontStyle
                }
            });
        }

        private static List<Argument> CommandParametersToArguments(DeviceCommandViewModel commandViewModel)
        {
            return commandViewModel.Model.Parameters.Select(p => new Argument(p.Name, commandViewModel.CommandParameters[p.Name])).ToList();
        }

        #endregion

        // Command simulation

        #region Commands

        private bool SendCanExecute()
        {
            var command = SelectedCommand;

            if (command == null)
            {
                return false;
            }

            command.MessagesViewerViewModel.Clear();

            // If command is in execution, no error is visible
            if (_currentCommandExecution != null && command.Model == _currentCommandExecution.Context.Command)
            {
                return false;
            }

            var context = new CommandContext(Model, command.Model, CommandParametersToArguments(command));

            if (ParametersViewModel.EnableVerification)
            {
                var result = Model.CanExecute(context);
                if (result)
                {
                    return true;
                }

                foreach (var text in context.Errors)
                {
                    command.MessagesViewerViewModel.Errors.Add(text);
                }

                foreach (var text in context.Warnings)
                {
                    command.MessagesViewerViewModel.Warnings.Add(text);
                }

                foreach (var text in context.Informations)
                {
                    command.MessagesViewerViewModel.Infos.Add(text);
                }

                return false;
            }

            return true;

        }

        private void SendExecute()
        {
            var command = SelectedCommand;

            if (command == null)
            {
                return;
            }

            var timeout = ParametersViewModel.UseTimeout ? ParametersViewModel.TimeOut : 0;
            var execution = new CommandExecution(Model, command.Model.Name, CommandParametersToArguments(command), timeout)
            {
                BypassSecurities = !ParametersViewModel.EnableVerification
            };
            _currentCommandExecution = execution;

            Model.RunAsync(execution);
        }

        private bool AbortCanExecute()
        {
            var command = SelectedCommand;
            // If command visible and is in execution
            return command != null && Model.DeviceType.SupportInterruption(InterruptionKind.Abort)
                                   && _currentCommandExecution != null
                                   && command.Model == _currentCommandExecution.Context.Command;
        }

        private void AbortExecute()
        {
            _currentCommandExecution.Context.Device.Interrupt(InterruptionKind.Abort);
        }

        #endregion

        #region Event handlers

        private void OnCommandExecutionStateChanged(object sender, CommandExecutionEventArgs e)
        {
            var resume = $"Command \"{e.Execution.Context.Command.Name}\" {e.NewState}";

            Application.Current.Dispatcher.Invoke(() =>
            {
                TimeSpan duration;

                switch (e.NewState)
                {
                    case ExecutionState.Failed:

                        duration = e.Execution.EndDateTime - e.Execution.BeginDateTime;
                        _currentCommandExecution = null;
                        Messages.HideAll();
                        Messages.Show(
                            new UserMessage(MessageLevel.Error, new InvariantText(resume + $" (executed in {duration})"))
                            {
                                CanUserCloseMessage = false
                            });
                        break;

                    case ExecutionState.Success:

                        duration = e.Execution.EndDateTime - e.Execution.BeginDateTime;
                        _currentCommandExecution = null;
                        if (e.Execution.Result != null)
                        {
                            resume += " with result ";
                        }

                        Messages.HideAll();
                        Messages.Show(
                            new UserMessage(MessageLevel.Success, new InvariantText(resume + $" (executed in {duration})"))
                            {
                                CanUserCloseMessage = false
                            });
                        break;

                    case ExecutionState.Running:

                        Messages.HideAll();
                        Messages.Show(
                            new UserMessage(MessageLevel.Info, new InvariantText(resume))
                            {
                                SecondsDuration = 5,
                                CanUserCloseMessage = true
                            });
                        break;

                    default:

                        Messages.HideAll();
                        break;
                }

                SelectedCommandExplorer?.UpdateProperties(e.Execution.Context.Command);
            });
        }

        private void OnStatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var groupName = GetStatusGroupName(e.Status.Category);
                var statusExplorer = Statuses.OfType<DeviceStatusCategoryViewModel>().FirstOrDefault(x => x.Name.Equals(groupName));
                statusExplorer?.UpdateProperties(e.Status);
            });
        }

        #endregion
        
        public void Dispose()
        {
            Model.StatusValueChanged -= OnStatusValueChanged;
            Model.CommandExecutionStateChanged -= OnCommandExecutionStateChanged;

            AlarmsViewModel.Dispose();
            AlarmsOccurrencesViewModel.Dispose();

            foreach (var command in Commands)
            {
                command.Dispose();
            }
        }
    }
}
