using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using Agileo.GUI;
using Agileo.GUI.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Components.Tools;
using Agileo.GUI.Services.Icons;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.Toolbox
{
    public class ToolboxPanel : BusinessPanel
    {
        public ToolboxPanel()
            : this("ToolBox", IconFactory.PathGeometryFromRessourceKey("ToolBoxIcon"))
        {
        }

        public ToolboxPanel(string id, IIcon icon = null)
            : base(id, icon)
        {
        }

        #region Fields
        
        public IEnumerable<BusinessPanel> BusinessPanels
            => AgilControllerApplication.Current.UserInterface.BusinessPanels.Where(
                panel => panel.FlattenToolReferences.Any());

        #endregion Fields

        #region Properties

        private Tool _selectedTool;

        public Tool SelectedTool
        {
            get => _selectedTool;
            set => SetAndRaiseIfChanged(ref _selectedTool, value);
        }

        private BusinessPanel _selectedBusinessPanel;

        public BusinessPanel SelectedBusinessPanel
        {
            get => _selectedBusinessPanel;
            set
            {
                if (SetAndRaiseIfChanged(ref _selectedBusinessPanel, value))
                {
                    AvailableToolsForBusinessPanel = _selectedBusinessPanel != null
                        ? _selectedBusinessPanel.FlattenToolReferences
                            .Select(reference => reference.Tool)
                            .ToList()
                        : new List<Tool>();
                    SelectedToolForBusinessPanel = ApplyToAllToolReferences
                        ? null
                        : AvailableToolsForBusinessPanel.FirstOrDefault();
                }
            }
        }

        public bool ApplyToAllToolReferences
        {
            get => SelectedToolForBusinessPanel == null;
            set
            {
                SelectedToolForBusinessPanel = value
                    ? null
                    : AvailableToolsForBusinessPanel.FirstOrDefault();
                OnPropertyChanged();
            }
        }

        private List<Tool> _availableToolsForBusinessPanel = new();

        public List<Tool> AvailableToolsForBusinessPanel
        {
            get => _availableToolsForBusinessPanel;
            set => SetAndRaiseIfChanged(ref _availableToolsForBusinessPanel, value);
        }

        private Tool _selectedToolForBusinessPanel;

        public Tool SelectedToolForBusinessPanel
        {
            get => _selectedToolForBusinessPanel;
            set
            {
                SetAndRaiseIfChanged(ref _selectedToolForBusinessPanel, value);
                OnPropertyChanged(nameof(ToolReferenceVisibility));
                OnPropertyChanged(nameof(ToolReferenceIsEnabled));
            }
        }

        public Visibility? ToolReferenceVisibility
        {
            get
            {
                if (SelectedBusinessPanel == null)
                    return null;

                if (SelectedToolForBusinessPanel != null)
                {
                    var toolReference = SelectedBusinessPanel.FlattenToolReferences.FirstOrDefault(
                        reference => ReferenceEquals(reference.Tool, SelectedToolForBusinessPanel));
                    return toolReference?.Visibility;
                }

                var visibilities = SelectedBusinessPanel.FlattenToolReferences
                    .Select(reference => reference.Visibility)
                    .ToList();
                if (visibilities.Count > 0
                    && visibilities.All(visibility => visibility == visibilities.First()))
                {
                    return visibilities.First();
                }

                return null;
            }
            set
            {
                if (!value.HasValue || SelectedBusinessPanel == null)
                    return;

                if (SelectedToolForBusinessPanel != null)
                {
                    foreach (var toolReference in SelectedBusinessPanel.FlattenToolReferences.Where(
                                 toolReference => ReferenceEquals(
                                     toolReference.Tool,
                                     SelectedToolForBusinessPanel)))
                    {
                        toolReference.Visibility = value.Value;
                    }
                }
                else
                {
                    AgilControllerApplication.Current.UserInterface.ToolManager
                        .SetAllToolReferencesVisibility(SelectedBusinessPanel, value.Value);
                }

                OnPropertyChanged();
            }
        }

        public bool? ToolReferenceIsEnabled
        {
            get
            {
                if (SelectedBusinessPanel == null)
                    return null;

                if (SelectedToolForBusinessPanel != null)
                {
                    var toolReference = SelectedBusinessPanel.FlattenToolReferences.FirstOrDefault(
                        reference => ReferenceEquals(reference.Tool, SelectedToolForBusinessPanel));
                    return toolReference?.IsEnabled;
                }

                var enabledCollection = SelectedBusinessPanel.FlattenToolReferences
                    .Select(reference => reference.IsEnabled)
                    .ToList();
                if (enabledCollection.Count > 0
                    && enabledCollection.All(b => b == enabledCollection.First()))
                {
                    return enabledCollection.First();
                }

                return null;
            }
            set
            {
                if (!value.HasValue || SelectedBusinessPanel == null)
                    return;

                if (SelectedToolForBusinessPanel != null)
                {
                    foreach (var toolReference in SelectedBusinessPanel.FlattenToolReferences.Where(
                                 toolReference => ReferenceEquals(
                                     toolReference.Tool,
                                     SelectedToolForBusinessPanel)))
                    {
                        toolReference.IsEnabled = value.Value;
                    }
                }
                else
                {
                    AgilControllerApplication.Current.UserInterface.ToolManager
                        .SetAllToolReferencesAccessibility(SelectedBusinessPanel, value.Value);
                }

                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Commands

        #region OpenToolCommand

        private ICommand _openToolCommand;

        public ICommand OpenToolCommand
            => _openToolCommand ??= new DelegateCommand(
                OpenToolCommandExecute,
                OpenToolCommandCanExecute);

        private bool OpenToolCommandCanExecute()
        {
            return SelectedTool is { IsOpen: false };
        }

        private void OpenToolCommandExecute()
        {
            SelectedTool.IsOpen = true;
        }

        #endregion OpenToolCommand

        #region CloseToolCommand

        private ICommand _closeToolCommand;

        public ICommand CloseToolCommand
            => _closeToolCommand ??= new DelegateCommand(
                CloseToolCommandExecute,
                CloseToolCommandCanExecute);

        private bool CloseToolCommandCanExecute()
        {
            return SelectedTool is { IsOpen: true };
        }

        private void CloseToolCommandExecute()
        {
            SelectedTool.IsOpen = false;
        }

        #endregion CloseToolCommand

        #region CloseAllToolsCommand

        private ICommand _closeAllToolsCommand;

        public ICommand CloseAllToolsCommand
            => _closeAllToolsCommand ??= new DelegateCommand(
                CloseAllToolFromToolboxCommandExecute,
                CloseAllToolFromToolboxCommandCanExecute);

        private bool CloseAllToolFromToolboxCommandCanExecute()
        {
            return SelectedBusinessPanel != null;
        }

        private void CloseAllToolFromToolboxCommandExecute()
        {
            AgilControllerApplication.Current.UserInterface.ToolManager.CloseAllTools(SelectedBusinessPanel);
        }

        #endregion CloseAllToolsCommand

        #endregion Commands
    }
}
