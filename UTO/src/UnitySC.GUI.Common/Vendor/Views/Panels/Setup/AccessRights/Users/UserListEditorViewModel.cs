using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using Agileo.Common.Access;
using Agileo.Common.Access.Users;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Popups;

using UnitySC.GUI.Common.Vendor.UIComponents.GuiExtended;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Setup.AccessRights.Users
{
    public class UserListEditorViewModel : Notifier, IDisposable
    {
        #region Fields

        private readonly BusinessPanel _ownerPanel;
        private readonly IAccessManager _accessManager;
        private readonly List<User> _removedUsers = new();

        private readonly BusinessPanelCommand _addPanelCommand;
        private readonly BusinessPanelCommand _deletePanelCommand;
        private readonly BusinessPanelCommand _editPanelCommand;

        private bool _editSelectedUserCanExecute;
        private bool _deleteSelectedUserCanExecute;
        
        #endregion

        public UserListEditorViewModel(BusinessPanel ownerPanel, IAccessManager accessManager)
        {
            _ownerPanel = ownerPanel;
            _accessManager = accessManager;

            _addPanelCommand = new InvisibleBusinessPanelCommand(nameof(AccessRightsResources.ACCESSRIGHTSPANEL_ADD_USER), AddCommand);
            _deletePanelCommand = new InvisibleBusinessPanelCommand(nameof(AccessRightsResources.ACCESSRIGHTSPANEL_DELETE_USER), DeleteCommand);
            _editPanelCommand = new InvisibleBusinessPanelCommand(nameof(AccessRightsResources.ACCESSRIGHTSPANEL_EDIT_USER), EditCommand);

            _ownerPanel.Commands.Add(_addPanelCommand);
            _ownerPanel.Commands.Add(_deletePanelCommand);
            _ownerPanel.Commands.Add(_editPanelCommand);

            if (IsInDesignMode)
            {
                Users = new ObservableCollection<UserViewModel>
                {
                    new(new User("Administrator", AccessLevel.Level5, string.Empty)),
                    new(new User("Supervisor", AccessLevel.Level7, string.Empty)),
                    new(new User("Operator", AccessLevel.Visibility, string.Empty)),
                    new(new User("User", AccessLevel.Level2, string.Empty))
                };
            }
        }

        #region Properties
        
        public ObservableCollection<UserViewModel> Users { get; } = new();

        private UserViewModel _selectedUser;

        public UserViewModel SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (SetAndRaiseIfChanged(ref _selectedUser, value))
                {
                    RefreshCommandsCanExecute();
                }
            }
        }

        #endregion

        #region Private Methods

        private void ShowUserEditor(UserViewModel userToEdit)
        {
            // Build user list for internal userEditor comparisons
            var userList = new List<UserViewModel>(Users);
            if (userToEdit != null)
            {
                userList.Remove(userToEdit);
            }

            LocalizableText title;
            string applyTextCommand;

            if (userToEdit != null)
            {
                title = new LocalizableText(nameof(AccessRightsResources.ACCESSRIGHTSPANEL_USER_EDIT));
                applyTextCommand = nameof(AccessRightsResources.ACCESSRIGHTSPANEL_APPLY);
            }
            else
            {
                title = new LocalizableText(nameof(AccessRightsResources.ACCESSRIGHTSPANEL_USER_CREATION));
                applyTextCommand = nameof(AccessRightsResources.ACCESSRIGHTSPANEL_CREATE);
            }

            var userEditor = new UserEditorViewModel(_accessManager, userToEdit, userList);

            var popup = new Popup(title)
            {
                Content = userEditor,
                Commands =
                {
                    new PopupCommand(nameof(AccessRightsResources.ACCESSRIGHTSPANEL_CANCEL))
                }
            };

            popup.Commands.Add(new PopupCommand(applyTextCommand, new DelegateCommand(() => ApplyModifications(userEditor, popup), userEditor.UserIsValid))
            {
                AutoCloseOnExecute = false
            });

            _ownerPanel.Popups.Show(popup);
        }

        private void ApplyModifications(UserEditorViewModel userEditor, Popup popup)
        {
            userEditor.ApplyRules();

            if (userEditor.HasErrors)
            {
                return;
            }

            popup.Close();

            var userViewModel = userEditor.UserViewModel;
            if (userViewModel != null)
            {
                // Edit existing user
                userViewModel.AccessLevel = userEditor.NewUserLevel;
                userViewModel.Name = userEditor.Name;

                if (userEditor.IsNewPasswordEditionEnabled)
                {
                    userViewModel.Password = userEditor.NewPassword;
                }
            }
            else
            {
                // New user creation
                Users.Add(
                    new UserViewModel(
                        userEditor.Name,
                        userEditor.NewPassword,
                        userEditor.NewUserLevel));
            }
        }

        private void RefreshCommandsCanExecute()
        {
            if (SelectedUser == null)
            {
                _editSelectedUserCanExecute = false;
                _deleteSelectedUserCanExecute = false;
                return;
            }

            // Prevent self deletion
            if (SelectedUser.User != null && SelectedUser.User.Name.Equals(_accessManager.CurrentUser.Name))
            {
                _editSelectedUserCanExecute = true;
                _deleteSelectedUserCanExecute = false;
                return;
            }

            // Prevent deletion & edition of higher level users
            if (SelectedUser.AccessLevel > _accessManager.CurrentUser.AccessLevel)
            {
                _editSelectedUserCanExecute = false;
                _deleteSelectedUserCanExecute = false;
                return;
            }

            _editSelectedUserCanExecute = true;
            _deleteSelectedUserCanExecute = true;
        }

        private void LoadUsers()
        {
            App.Instance.Dispatcher.Invoke(
                () =>
                {
                    Users.Clear();

                    foreach (var user in _accessManager.AccessRightsManager.UserManager.Users)
                    {
                        Users.Add(new UserViewModel(user));
                    }

                    _removedUsers.Clear();
                });
        }

        #endregion

        #region Public methods

        public bool HasModified() => _removedUsers.Count > 0 || Users.Any(x => x.HasChanged);

        public bool Save()
        {
            bool anyChange = false;

            foreach (var user in _removedUsers)
            {
                _accessManager.AccessRightsManager.UserManager.Remove(user);
                anyChange = true;
            }

            _removedUsers.Clear();

            foreach (var userViewModel in Users)
            {
                if (!userViewModel.HasChanged)
                {
                    continue;
                }

                var user = userViewModel.User;

                // New user case
                if (user == null)
                {
                    _accessManager.AccessRightsManager.UserManager.CreateUser(
                        userViewModel.Name,
                        userViewModel.Password,
                        userViewModel.AccessLevel);
                }
                else
                {
                    if (!user.Name.Equals(userViewModel.Name))
                    {
                        _accessManager.AccessRightsManager.UserManager.ChangeName(user, userViewModel.Name);
                    }

                    if (!user.Password.Equals(userViewModel.Password))
                    {
                        _accessManager.AccessRightsManager.UserManager.ChangePassword(user, userViewModel.Password);
                    }

                    if (user.AccessLevel != userViewModel.AccessLevel)
                    {
                        user.AccessLevel = userViewModel.AccessLevel;
                    }
                }

                anyChange = true;
            }

            LoadUsers();

            return anyChange;
        }

        public void OnSetup()
        {
            LoadUsers();
        }

        public void UndoChanges()
        {
            LoadUsers();
        }

        #endregion

        #region Event handlers

        private void OnLoggedUserChanged(User user = null, UserEventArgs e = null) => RefreshCommandsCanExecute();

        #endregion

        #region Add command

        private ICommand _addCommand;

        public ICommand AddCommand => _addCommand ??= new DelegateCommand(AddCommandExecute, AddCommandCanExecute);

        private bool AddCommandCanExecute() => IsInDesignMode || _addPanelCommand.IsEnabled;

        private void AddCommandExecute() => ShowUserEditor(null);

        #endregion

        #region Delete command

        private ICommand _deleteCommand;

        public ICommand DeleteCommand => _deleteCommand ??= new DelegateCommand(DeleteCommandExecute, DeleteCommandCanExecute);

        private bool DeleteCommandCanExecute() => IsInDesignMode || (_deleteSelectedUserCanExecute && _deletePanelCommand.IsEnabled);

        private void DeleteCommandExecute()
        {
            var popupContent = new LocalizableText(nameof(AccessRightsResources.ACCESSRIGHTSPANEL_USER_DELETION_POPUP_CONTENT), SelectedUser.Name);
            var popup = new Popup(nameof(AccessRightsResources.ACCESSRIGHTSPANEL_USER_DELETION_POPUP_TITLE), popupContent)
            {
                SeverityLevel = MessageLevel.Warning,
                Commands =
                {
                    new PopupCommand(nameof(AccessRightsResources.ACCESSRIGHTSPANEL_CANCEL)),
                    new PopupCommand(nameof(AccessRightsResources.ACCESSRIGHTSPANEL_DELETE), new DelegateCommand(
                        () =>
                        {
                            // Add to list to be removed on save
                            if (SelectedUser.User != null)
                            {
                                _removedUsers.Add(SelectedUser.User);
                            }

                            // Remove from visual list
                            Users.Remove(SelectedUser);
                        }))
                }
            };

            _ownerPanel.Popups.Show(popup);
        }

        #endregion

        #region Edit command

        private ICommand _editCommand;

        public ICommand EditCommand => _editCommand ??= new DelegateCommand(EditCommandExecute, EditCommandCanExecute);

        private bool EditCommandCanExecute() => IsInDesignMode || (_editSelectedUserCanExecute && _editPanelCommand.IsEnabled);

        private void EditCommandExecute() => ShowUserEditor(SelectedUser);

        #endregion
        
        #region AccessManager registration

        private bool _isRegistered;

        public void OnShow()
        {
            _accessManager.UserLogoff += OnLoggedUserChanged;
            _accessManager.UserLogon += OnLoggedUserChanged;
            _isRegistered = true;

            RefreshCommandsCanExecute();

        }

        private void CancelRegistration()
        {
            if (_isRegistered)
            {
                _accessManager.UserLogoff -= OnLoggedUserChanged;
                _accessManager.UserLogon -= OnLoggedUserChanged;
                _isRegistered = false;
            }
        }

        public void OnHide()
        {
            CancelRegistration();
        }
        

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            CancelRegistration();
        }

        #endregion
    }
}
