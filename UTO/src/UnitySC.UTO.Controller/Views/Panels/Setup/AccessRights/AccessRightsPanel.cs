using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.Common.Access;
using Agileo.Common.Localization;
using Agileo.Common.Tracing;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Components.Tools;
using Agileo.GUI.Services.Icons;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Filters;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup.AccessRights.Rights;
using UnitySC.UTO.Controller.Views.Panels.Setup.AccessRights.Users;

namespace UnitySC.UTO.Controller.Views.Panels.Setup.AccessRights
{
    /// <summary>
    /// AccessRights ViewModel
    /// </summary>
    public class AccessRightsPanel : SetupPanel
    {
        #region Fields

        private readonly IAccessManager _accessManager;
        private readonly UserInterface _userInterface;
        private readonly Dictionary<string, RightViewModel> _modifiedGraphicalElementRights = new();

        #endregion Fields

        #region Constructors

        static AccessRightsPanel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(GUI.Common.Vendor.Views.Panels.Setup.AccessRights.AccessRightsResources)));
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(GUI.Common.Vendor.Views.Panels.Setup.AccessRights.EnumLocalization.AccessLevel)));
        }

        /// <summary>
        /// Default constructor only used by view in design mode
        /// </summary>
        public AccessRightsPanel() : this(new DesignTimeUserInterface(), new AccessManager(), nameof(Agileo.GUI.Properties.Resources.S_SETUP_ACCESSRIGHTS))
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException("Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }

            NavigationTreeSource.Reset(_userInterface.Navigation.RootMenu.Items.Select(item => new MenuItemRightsViewModel(item, _accessManager, OnLevelChanged)));
            NavigationTreeSource.ExpandAll();

            SelectedMenuItem = NavigationTreeSource.GetFlattenElements().Where(node => node.Model.Element is BusinessPanel).ElementAt(2).Model;

            ApplicationCommands.Add(Wrap(new ApplicationCommand("ShutDown", new DelegateCommand(() => { }))
            {
                AccessRights = DesignTimeUserInterface.CreateAccessRights(AccessLevel.Level3)
            }));
            ApplicationCommands.Add(Wrap(new ApplicationCommand("LogOff", new DelegateCommand(() => { }))
            {
                AccessRights = DesignTimeUserInterface.CreateAccessRights(AccessLevel.Visibility)
            }));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessRightsPanel" /> class.
        /// Allows configuration of access rights of GUI elements
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="userInterface">The application user interface</param>
        /// <param name="accessManager">The application accessRights</param>
        /// <param name="icon">The icon.</param>
        public AccessRightsPanel(UserInterface userInterface, IAccessManager accessManager, string id, IIcon icon = null)
            : base(id, icon)
        {
            _userInterface = userInterface ?? throw new ArgumentNullException(nameof(userInterface));
            _accessManager = accessManager ?? throw new ArgumentNullException(nameof(accessManager));

            NavigationTreeSource = new DataTreeSource<MenuItemRightsViewModel>(item => item.SubMenu);
            NavigationTreeSource.Filter.Add(new FilterSwitch<TreeNode<MenuItemRightsViewModel>>(nameof(GUI.Common.Vendor.Views.Panels.Setup.AccessRights.AccessRightsResources.ACCESSRIGHTSPANEL_MODIFIED), node => node.Model.HasModified));
            NavigationTreeSource.Filter.AddEnumFilter(nameof(GUI.Common.Vendor.Views.Panels.Setup.AccessRights.AccessRightsResources.ACCESSRIGHTSPANEL_ACCESSIBILITY), node => node.Model.EnabledRight.Level);
            NavigationTreeSource.Filter.AddEnumFilter(nameof(GUI.Common.Vendor.Views.Panels.Setup.AccessRights.AccessRightsResources.ACCESSRIGHTSPANEL_VISIBILITY), node => node.Model.VisibilityRight.Level);
            NavigationTreeSource.Search.AddSearchDefinition(nameof(GUI.Common.Vendor.Views.Panels.Setup.AccessRights.AccessRightsResources.ACCESSRIGHTSPANEL_NAME), model => model.Element.LocalizedName, true);
            NavigationTreeSource.Search.AddSearchDefinition(nameof(GUI.Common.Vendor.Views.Panels.Setup.AccessRights.AccessRightsResources.ACCESSRIGHTSPANEL_ID), model => model.Element.RelativeId, true);
            NavigationTreeSource.Sort.AddSortDefinition(new LocalizableText(nameof(GUI.Common.Vendor.Views.Panels.Setup.AccessRights.AccessRightsResources.ACCESSRIGHTSPANEL_ORDER)), _ => null);
            NavigationTreeSource.Sort.AddSortDefinition(new LocalizableText(nameof(GUI.Common.Vendor.Views.Panels.Setup.AccessRights.AccessRightsResources.ACCESSRIGHTSPANEL_LEVEL)), node => node.Model.HighestLevel);

            CommandsTreeSource = new DataTreeSource<CommandElementRightsViewModel>(item => item.Commands);

            UserListEditor = new UserListEditorViewModel(this, accessManager);

            _userInterface.AccessRightsApplied += OnAccessRightsApplied;
        }

        #endregion Constructors

        #region Properties

        public DataTreeSource<MenuItemRightsViewModel> NavigationTreeSource { get; }

        public DataTreeSource<CommandElementRightsViewModel> CommandsTreeSource { get; }

        public ObservableCollection<GraphicalElementRightsViewModel<ApplicationCommand>> ApplicationCommands { get; } = new();

        public ObservableCollection<GraphicalElementRightsViewModel<Tool>> Tools { get; } = new();

        public UserListEditorViewModel UserListEditor { get; }

        private MenuItemRightsViewModel _selectedMenuItem;

        /// <summary>
        /// Gets or sets the selected <see cref="MenuItem"/> in graphical tree
        /// </summary>
        public MenuItemRightsViewModel SelectedMenuItem
        {
            get => _selectedMenuItem;
            set
            {
                if (SetAndRaiseIfChanged(ref _selectedMenuItem, value))
                {
                    UpdateCommandsTreeSource();
                }
            }
        }

        public bool SelectedPanelHasCommand => CommandsTreeSource.Nodes.Any();

        #endregion Properties

        #region Event Handlers

        private void OnAccessRightsApplied(object sender, EventArgs e)
        {
            GUI.Common.App.Instance.Dispatcher?.Invoke(
                () =>
                {
                    ApplicationCommands.Clear();
                    foreach (var applicationCommand in _userInterface.ApplicationCommands)
                    {
                        ApplicationCommands.Add(Wrap(applicationCommand));
                    }

                    Tools.Clear();
                    foreach (var tool in _userInterface.ToolManager.Tools)
                    {
                        Tools.Add(Wrap(tool));
                    }

                    var selectedItemId = SelectedMenuItem?.Element?.Id;
                    NavigationTreeSource.Reset(_userInterface.Navigation.RootMenu.Items.Select(item => new MenuItemRightsViewModel(item, _accessManager, OnLevelChanged)));
                    if (selectedItemId != null)
                    {
                        var relatedViewModel = NavigationTreeSource.GetFlattenElements().FirstOrDefault(node => node.Model.Element.Id.Equals(selectedItemId));
                        SelectedMenuItem = relatedViewModel?.Model;
                    }
                });
        }

        #endregion

        #region Private Methods

        private void UpdateCommandsTreeSource()
        {
            if (_selectedMenuItem != null)
            {
                CommandsTreeSource.Reset(_selectedMenuItem.Commands);
                CommandsTreeSource.GetFlattenElements().ForEach(node => node.IsSelectable = false);
            }
            else
            {
                CommandsTreeSource.Reset(Enumerable.Empty<CommandElementRightsViewModel>());
            }

            OnPropertyChanged(nameof(SelectedPanelHasCommand));
        }

        private GraphicalElementRightsViewModel<T> Wrap<T>(T element) where T : GraphicalElement
        {
            return new GraphicalElementRightsViewModel<T>(element, _accessManager, OnLevelChanged);
        }

        private void OnLevelChanged(RightViewModel obj)
        {
            if (obj.HasModified)
            {
                if (!_modifiedGraphicalElementRights.ContainsKey(obj.Path))
                {
                    _modifiedGraphicalElementRights.Add(obj.Path, obj);
                }
            }
            else
            {
                _modifiedGraphicalElementRights.Remove(obj.Path);
            }

            if (NavigationTreeSource.Filter.IsApplied
                || NavigationTreeSource.Sort.CurrentSortDefinition?.PropertyName
                != nameof(GUI.Common.Vendor.Views.Panels.Setup.AccessRights.AccessRightsResources.ACCESSRIGHTSPANEL_NAME))
            {
                NavigationTreeSource.Refresh();
            }
        }

        #endregion

        #region Override of BusinessPanel

        /// <inheritdoc />
        public override void OnHide()
        {
            base.OnHide();
            if (!HasChanges)
            {
                Messages.HideAll();
            }

            UserListEditor.OnHide();
        }

        /// <inheritdoc />
        public override void OnShow()
        {
            base.OnShow();
            UserListEditor.OnShow();
        }

        #endregion

        #region Override of SetupPanel

        /// <inheritdoc />
        public override void OnSetup()
        {
            if (IsInDesignMode)
            {
                return;
            }

            base.OnSetup();
            UserListEditor.OnSetup();
        }

        protected override bool ConfigurationEqualsCurrent()
        {
            return _modifiedGraphicalElementRights.Count == 0 && !UserListEditor.HasModified();
        }

        protected override void SaveConfig()
        {
            // [TLa] Applies changes to instances of RightData.
            // These instances have been provided by the AccessRightsManager and are implicitly linked with it.
            // The call to the Save() method of the AccessRightsManager will therefore take into account the update of these properties.
            // Create a copy of the collection to avoid concurrent modifications
            foreach (var modifiedRight in _modifiedGraphicalElementRights.Values.ToList())
            {
                modifiedRight.Apply();
            }

            _modifiedGraphicalElementRights.Clear();

            UserListEditor.Save();
            _accessManager.AccessRightsManager.Save(true);
            
            // traces
            Logger.Info(
                _accessManager.CurrentAccessRightsToString().AsAttachment(),
                "Access rights file was saved. Details in attachment.");
        }

        protected override void UndoChanges()
        {
            foreach (var modifiedRight in _modifiedGraphicalElementRights.Values.ToList())
            {
                modifiedRight.Level = modifiedRight.InitialLevel;
            }

            _modifiedGraphicalElementRights.Clear();
            UserListEditor.UndoChanges();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _userInterface.AccessRightsApplied -= OnAccessRightsApplied;
                UserListEditor.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
