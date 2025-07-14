using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

using CommunityToolkit.Mvvm.Messaging;

using MvvmValidation;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using Dto = UnitySC.DataAccess.Dto;

namespace ADCConfiguration.ViewModel.Users
{
    public class EditUserViewModel : ViewModelWithMenuBase, INavigationViewModel
    {
        private ICollectionView _usersView;
        private ObservableCollection<EditUserDetailViewModel> _users;


        private EditUserDetailViewModel _selectedUser;
        public EditUserDetailViewModel SelectedUser
        {
            get => _selectedUser;
            set { _selectedUser = value; OnPropertyChanged(); }
        }

        private string _filter;
        public string Filter
        {
            get => _filter;
            set
            {
                if (_filter != value)
                {
                    _filter = value;
                    OnPropertyChanged();
                    _usersView.Refresh();
                }
            }
        }

        public ICollectionView Users
        {
            get { return _usersView; }
            set
            {
                _usersView = value;
                OnPropertyChanged();
            }
        }

        public EditUserViewModel()
        {
            Validator.AddRule("Login", () => RuleResult.Assert(_users != null && !_users.GroupBy(x => x.Login).Where(g => g.Count() > 1).Any(), "User login must be unique"));
            MenuName = "User management";
            CommandMenuItems.Add(new MenuItemViewModel()
            {
                Name = "Refresh",
                Description = "Refresh",
                ExecuteCommand = RefreshCommand,
                ImageResourceKey = "Refresh",
                IconText = "Refresh"
            });

            CommandMenuItems.Add(new MenuItemViewModel()
            {
                Name = "Add",
                Description = "Add user",
                ExecuteCommand = AddCommand,
                ImageResourceKey = "Add",
                IconText = "Add"
            });

            CommandMenuItems.Add(new MenuItemViewModel()
            {
                Name = "Save",
                Description = "Save",
                ExecuteCommand = SaveCommand,
                ImageResourceKey = "SaveADCImage",
                IconText = "Save"
            });

            CommandMenuItems.Add(new MenuItemViewModel()
            {
                Name = "Remove Selected User",
                Description = "remove the current selected user",
                ExecuteCommand = DeleteCommand,
                Color = MenuColorEnum.Red,
                ImageResourceKey = "DeleteADCImage",
                IconText = "Delete"
            });
        }

        private void Init()
        {
            IsBusy = true;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    // Interogation de la bdd
#warning ** USP **  constant address need refacto to get adresss
                    var address = new ServiceAddress() { Host = "localhost", Port = 2221 }; //ClientConfiguration.GetDataAccessAddress()

                    var userServiceInvoker = new ServiceInvoker<IUserService>("UserService",
                        ClassLocator.Default.GetInstance<SerilogLogger<IUserService>>(),
                        ClassLocator.Default.GetInstance<IMessenger>(), address);
                    var dtoUsers = userServiceInvoker.Invoke(x => x.GetAll(null));

                    // Convertion de DTO vers ViewModel
                    _users = new ObservableCollection<EditUserDetailViewModel>(dtoUsers.Select(x => new EditUserDetailViewModel(x, Validator)));

                    System.Windows.Application.Current.Dispatcher.Invoke((() =>
                    {
                        Users = CollectionViewSource.GetDefaultView(_users);
                        _usersView.Filter = UserFilter;
                        _usersView.SortDescriptions.Add(new SortDescription("IsEnabled", ListSortDirection.Descending));
                        _usersView.SortDescriptions.Add(new SortDescription("Login", ListSortDirection.Ascending));
                        SelectedUser = _users.FirstOrDefault();
                        Validator.ValidateAll();
                    }));
                }
                catch (Exception ex)
                {
                    Services.Services.Instance.LogService.LogError("Refresh user", ex);
                    System.Windows.Application.Current.Dispatcher.Invoke((() => { AdcTools.ExceptionMessageBox.Show("Refresh user: ", ex); }));
                }
                finally
                {
                    IsBusy = false;
                }
            });
        }

        private bool UserFilter(object obj)
        {
            if (!string.IsNullOrEmpty(_filter))
            {
                EditUserDetailViewModel editUser = obj as EditUserDetailViewModel;
                return editUser.Login.ToLower().Contains(Filter.ToLower()) || editUser.FirstName.ToLower().Contains(Filter.ToLower()) || editUser.LastName.ToLower().Contains(Filter.ToLower());
            }
            else
                return true;
        }

        public void Refresh()
        {
            Init();
        }

        private void Add()
        {
            EditUserDetailViewModel newUser = new EditUserDetailViewModel(new Dto.User() { Name /*Login*/ =  "New user" }, Validator);
            newUser.HasChanged = true;
            newUser.PasswordRequired = true;
            _users.Add(newUser);
            Filter = null;
            SelectedUser = newUser;
        }

        private void Save()
        {
            if (_users.Any(x => x.HasErrors))
            {
                AdcTools.AttentionMessageBox.Show("Can not save. Some users are in errors !");
                return;
            }
            IsBusy = true;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    foreach (EditUserDetailViewModel user in _users.Where(x => x.HasChanged && !x.HasErrors))
                    {
                   //     user.DtoUser.Id = _userService.SetUser(user.DtoUser, Services.Services.Instance.AuthentificationService.CurrentUser.Id);
                        user.PasswordRequired = false;
                        user.HasChanged = false;
                    }
                }
                catch (Exception ex)
                {
                    Services.Services.Instance.LogService.LogError("Save user", ex);
                    System.Windows.Application.Current.Dispatcher.Invoke((() => { AdcTools.ExceptionMessageBox.Show("Save user error: ", ex); }));
                    Init();
                }
                finally
                {
                    IsBusy = false;
                }
            });
        }

        public void Delete()
        {
            if (!Services.Services.Instance.PopUpService.ShowConfirme("Confirmation", "Would you realy want to remove this User ?"))
                return;

            if (SelectedUser != null)
            {
                if (SelectedUser.DtoUser.Id != 0)
                {
                    try
                    {
                      //  _userService.RemoveUser(SelectedUser.DtoUser, Services.Services.Instance.AuthentificationService.CurrentUser.Id);
                        _users.Remove(SelectedUser);
                    }
                    catch (Exception ex)
                    {
                        Services.Services.Instance.LogService.LogError("Delete user", ex);
                        System.Windows.Application.Current.Dispatcher.Invoke((() => { AdcTools.AttentionMessageBox.Show("Can not remove user linked to recipe or configuration"); }));
                    }
                }
                else
                {
                    _users.Remove(SelectedUser);
                }

            }
        }

        public bool MustBeSave => _users.Where(x => x.HasChanged).Any();

        #region Commands

        private AutoRelayCommand _refreshCommand = null;
        public AutoRelayCommand RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new AutoRelayCommand(
              () =>
              {
                  if (_users.Any(x => x.HasChanged))
                  {
                      if (MessageBox.Show("Some user are not saved and will be lost." + Environment.NewLine + "Do you want to refresh anyway ?", "Some user are not saved", MessageBoxButton.YesNo) == MessageBoxResult.No)
                          return;
                  }

                  Init();
                  Services.Services.Instance.LogService.LogDebug("Refresh user");
              },
              () => { return true; }));
            }
        }


        private AutoRelayCommand _addCommand;
        public AutoRelayCommand AddCommand
        {
            get
            {
                return _addCommand ?? (_addCommand = new AutoRelayCommand(
              () =>
              {
                  Add();
              },
              () => { return true; }));
            }
        }


        private AutoRelayCommand _saveCommand;
        public AutoRelayCommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new AutoRelayCommand(
              () =>
              {
                  Save();
              },
              () => { return !_users.Any(x => x.HasErrors) && !HasErrors; }));
            }
        }


        private AutoRelayCommand _deleteCommand;
        public AutoRelayCommand DeleteCommand
        {
            get
            {
                return _deleteCommand ?? (_deleteCommand = new AutoRelayCommand(
              () =>
              {
                  Delete();
              },
              () => { return true; }));
            }
        }

        #endregion
    }
}

