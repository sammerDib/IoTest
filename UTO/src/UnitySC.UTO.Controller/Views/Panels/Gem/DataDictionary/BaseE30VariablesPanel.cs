using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Timers;

using Agileo.Common.Localization;
using Agileo.GUI.Commands;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.Semi.Communication.Abstractions.E5;
using Agileo.Semi.Gem.Abstractions.E30;

using UnitySC.GUI.Common.Vendor;
using UnitySC.GUI.Common.Vendor.Extensions;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;
using UnitySC.GUI.Common.Vendor.UIComponents.GuiExtended;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.DataDictionary
{
    public abstract class BaseE30VariablesPanel : NotifyDataErrorPanel
    {
        #region Fields

        private bool _preventUpdateValue;

        private static readonly List<BaseE30VariablesPanel> AutoRefreshPanels = new();

        #endregion

        #region Properties

        public bool EnableAutoRefreshFeature { get; }

        public InvisibleBusinessPanelCommand CanEditValueCommand { get; }

        public BusinessPanelToggleCommand AutoRefreshCommand { get; }

        public DataTableSource<E30VariableViewModel> DataVariables { get; } = new();

        private E30VariableViewModel _selectedVariable;

        public E30VariableViewModel SelectedVariable
        {
            get => _selectedVariable;
            set
            {
                SetAndRaiseIfChanged(ref _selectedVariable, value);
                OnSelectedVariableChanged();
            }
        }

        private bool _selectedVariableCanBeEdited;

        public bool SelectedVariableCanBeEdited
        {
            get => _selectedVariableCanBeEdited;
            set => SetAndRaiseIfChanged(ref _selectedVariableCanBeEdited, value);
        }

        private static bool _detailsOpen;

        public bool DetailsOpen
        {
            get => _detailsOpen;
            set => SetAndRaiseIfChanged(ref _detailsOpen, value);
        }

        public static double RefreshDelay { get; set; } = 1000;

        #endregion

        static BaseE30VariablesPanel()
        {
            DataTemplateGenerator.Create(typeof(BaseE30VariablesPanel), typeof(E30VariablesView));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(DataDictionaryPanelsResources)));
        }

        protected BaseE30VariablesPanel(bool enableAutoRefreshFeature, string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
            EnableAutoRefreshFeature = enableAutoRefreshFeature;

            CanEditValueCommand = new InvisibleBusinessPanelCommand(
                nameof(DataDictionaryPanelsResources.GEMPANELS_CAN_EDIT_VALUES),
                new DelegateCommand(() => { }),
                PathIcon.Edit);
            Commands.Add(CanEditValueCommand);

            if (EnableAutoRefreshFeature)
            {
                AutoRefreshPanels.Add(this);

                AutoRefreshCommand = new BusinessPanelCheckToggleCommand(
                    nameof(DataDictionaryPanelsResources.GEMPANELS_AUTOREFRESH),
                    new DelegateCommand(DisableAutoRefresh),
                    new DelegateCommand(EnableAutoRefresh),
                    PathIcon.Actives);

                Commands.Add(AutoRefreshCommand);
                Commands.Add(
                    new BusinessPanelCommand(
                        nameof(DataDictionaryPanelsResources.GEMPANELS_REFRESH),
                        new DelegateCommand(() => UpdateVariables()),
                        PathIcon.Refresh));
            }

            Commands.Add(
                new BusinessPanelCommand(
                    nameof(DataDictionaryPanelsResources.GEMPANELS_CONFIGURE),
                    new DelegateCommand(ConfigureCommandExecute),
                    PathIcon.Setup));

            Rules.Add(new DelegateRule(nameof(SelectedValue), ValidateSelectedValue));

            DataVariables.Search.AddSearchDefinition(
                nameof(DataDictionaryPanelsResources.GEMPANELS_ID),
                variable => variable.Variable.ID.ToString(),
                true);
            DataVariables.Search.AddSearchDefinition(
                nameof(DataDictionaryPanelsResources.GEMPANELS_NAME),
                variable => variable.Variable.Name,
                true);

            DataVariables.Sort.AddSortDefinition(
                nameof(E30VariableViewModel.Value),
                variable => variable.Value.ToString());
            DataVariables.Sort.SetCurrentSorting(
                $"{nameof(E30VariableViewModel.Variable)}.{nameof(E30Variable.ID)}",
                ListSortDirection.Ascending);

            DataVariables.Filter.AddRangeFilter(
                nameof(DataDictionaryPanelsResources.GEMPANELS_ID),
                variable => variable.Variable.ID,
                () => DataVariables);
            DataVariables.Filter.AddEnumFilter(
                nameof(DataDictionaryPanelsResources.GEMPANELS_FORMAT),
                variable => variable.Variable.Format);
        }

        private void ConfigureCommandExecute()
        {
            DataTemplateGenerator.Create(
                typeof(E30VariablesPanelConfigurationPopupViewModel),
                typeof(E30VariablesPanelConfigurationPopupView));
            var popupViewModel =
                new E30VariablesPanelConfigurationPopupViewModel(TimeSpan.FromMilliseconds(RefreshDelay).TotalSeconds);
            Popups.Show(
                new Popup(nameof(DataDictionaryPanelsResources.GEMPANELS_CONFIGURATION))
                {
                    Content = popupViewModel,
                    Commands =
                    {
                        new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)),
                        new PopupCommand(
                            nameof(Agileo.GUI.Properties.Resources.S_OK),
                            new DelegateCommand(
                                () =>
                                {
                                    RefreshDelay = TimeSpan.FromSeconds(popupViewModel.Delay).TotalMilliseconds;
                                    var e30Panels =
                                        GUI.Common.App.Instance.UserInterface.Navigation.RootMenu
                                            .GetChildrenAsFlattenedCollection()
                                            .OfType<BaseE30VariablesPanel>();
                                    foreach (var e30Panel in e30Panels)
                                    {
                                        e30Panel.OnRefreshDelayChanged();
                                    }
                                }))
                    }
                });
        }

        #region Abstract Methods

        protected abstract IEnumerable<E30Variable> LoadVariables();

        #endregion

        #region Protected Methods

        protected E30VariableViewModel GetVariableViewModel(int id)
            => DataVariables.SingleOrDefault(model => model.Variable.ID == id);

        protected virtual void OnSelectedVariableChanged()
        {
            if (SelectedVariable != null && SelectedVariable.Variable.Format != DataItemFormat.LST)
            {
                _editingValue = SelectedVariable.Value;
                SelectedVariableCanBeEdited = true;
            }
            else
            {
                _editingValue = null;
                SelectedVariableCanBeEdited = false;
            }

            UpdateSelectedValue();
        }

        protected void UpdateVariables(bool preventUpdateValue = false)
        {
            if (App.ControllerInstance.GemController.IsSetupDone)
            {
                DispatcherHelper.DoInUiThreadAsynchronously(
                    () =>
                    {
                        _preventUpdateValue = preventUpdateValue;
                        try
                        {
                            var selectedEc = SelectedVariable;
                            DataVariables.UpdateCollection();
                            if (selectedEc != null)
                            {
                                SelectedVariable = DataVariables.SourceView.SingleOrDefault(
                                    variable => variable.Variable.ID == selectedEc.Variable.ID);
                            }
                        }
                        finally
                        {
                            _preventUpdateValue = false;
                        }
                    });
            }
        }

        #endregion

        #region Private Methods

        private void UpdateSelectedValue()
        {
            if (_preventUpdateValue)
            {
                return;
            }

            SelectedValue = _editingValue == null ? string.Empty : _editingValue.GetValueAsString(string.Empty);
        }

        #endregion

        #region Validation Rules

        private string ValidateSelectedValue()
        {
            try
            {
                if (!SelectedVariableCanBeEdited)
                {
                    return null;
                }

                _editingValue = SelectedVariable.Variable.NewDataItemFromValueString(SelectedValue);

                // Check value range in line with Min and Max
                // There is no public API in E30Variable but GEM.Managers.Variables.EquipmentConstant do this in SetValue with
                // internal CheckValueRange method in GEM.Common.ValueObject
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return null;
        }

        #endregion

        #region Edition

        private DataItem _editingValue;

        private string _selectedValue;

        public string SelectedValue
        {
            get => _selectedValue;
            set => SetAndRaiseIfChanged(ref _selectedValue, value);
        }

        #endregion

        #region Commands

        private DelegateCommand _applyValueCommand;

        public DelegateCommand ApplyValueCommand
            => _applyValueCommand
               ?? (_applyValueCommand = new DelegateCommand(ApplyValueCommandExecute, ApplyValueCommandCanExecute));

        private bool ApplyValueCommandCanExecute()
            => SelectedVariable != null
               && SelectedVariable.Variable.Format != DataItemFormat.LST
               && _editingValue != null
               && !HasErrors;

        private void ApplyValueCommandExecute()
        {
            try
            {
                InternalSetValue(_editingValue);
            }
            catch (Exception e)
            {
                Popups.Show(
                    new Popup($"Error while changing the value of {SelectedVariable.Variable.Name}", e.Message)
                    {
                        SeverityLevel = MessageLevel.Error,
                        Commands = { new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_OK)) }
                    });
            }

            UpdateSelectedValue();
        }

        protected virtual void InternalSetValue(DataItem newValue)
        {
            SelectedVariable.Variable.SetValue(newValue);
            UpdateVariables();
        }

        private static Timer _refreshTimer;

        private static void EnableAutoRefresh()
        {
            if (_refreshTimer == null)
            {
                _refreshTimer = new Timer(RefreshDelay);
                _refreshTimer.Elapsed += RefreshTimerElapsed;
            }

            _refreshTimer.Start();

            foreach (var e30Panel in AutoRefreshPanels)
            {
                e30Panel.AutoRefreshCommand.IsChecked = true;
            }
        }

        private static void DisableAutoRefresh()
        {
            _refreshTimer?.Stop();

            foreach (var e30Panel in AutoRefreshPanels)
            {
                e30Panel.AutoRefreshCommand.IsChecked = false;
            }
        }

        private static void RefreshTimerElapsed(object sender = null, ElapsedEventArgs e = null)
        {
            foreach (var e30Panel in AutoRefreshPanels)
            {
                if (AgilControllerApplication.Current.UserInterface.Navigation.SelectedBusinessPanel != e30Panel)
                {
                    continue;
                }

                e30Panel.UpdateVariables(true);
            }
        }

        protected virtual void OnRefreshDelayChanged()
        {
            if (_refreshTimer != null)
            {
                _refreshTimer.Interval = RefreshDelay;
            }
        }

        #endregion

        #region Overrides of BusinessPanel

        public override void OnShow()
        {
            base.OnShow();

            if (EnableAutoRefreshFeature && AutoRefreshCommand?.IsChecked == true)
            {
                EnableAutoRefresh();
            }
        }

        public override void OnHide()
        {
            _refreshTimer?.Stop();
            base.OnHide();
        }

        #endregion

        #region Overrides of IdentifiableElement

        public override void OnSetup()
        {
            base.OnSetup();
            var viewModels = LoadVariables().Select(variable => new E30VariableViewModel(variable));
            DataVariables.AddRange(viewModels);
        }

        #endregion
    }
}
