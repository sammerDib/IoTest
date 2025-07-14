using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using MvvmDialogs.FrameworkDialogs.SaveFile;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ViewModel;

using Dto = UnitySC.DataAccess.Dto;

namespace UnitySC.PM.Shared.UI.Administration.Log
{
    public class LogViewModel : ObservableObject, IMenuContentViewModel
    {
        private ServiceInvoker<IUserService> _userService;
        private ServiceInvoker<ILogService> _logService;
        private ILogger _logger;
        private IDialogOwnerService _dialogOwnerService;
        private int _toolKey;

        public LogViewModel(ILogger<LogViewModel> logger, IDialogOwnerService dialogOwnerService)
        {
            Init();
            _logger = logger;
            _dialogOwnerService = dialogOwnerService;
            Users = new ObservableCollection<FilterItemViewModel<Dto.User>>();
            Areas = new ObservableCollection<FilterItemViewModel<Dto.Log.TableTypeEnum?>>();
            Actions = new ObservableCollection<FilterItemViewModel<Dto.Log.ActionTypeEnum?>>();
        }

        private void Init()
        {
            _userService = new ServiceInvoker<IUserService>("UserService", ClassLocator.Default.GetInstance<SerilogLogger<IUserService>>(), ClassLocator.Default.GetInstance<IMessenger>(), ClientConfiguration.GetDataAccessAddress());
            _logService = new ServiceInvoker<ILogService>("LogService", ClassLocator.Default.GetInstance<SerilogLogger<ILogService>>(), ClassLocator.Default.GetInstance<IMessenger>(), ClientConfiguration.GetDataAccessAddress());
            _toolKey = ClassLocator.Default.GetInstance<GlobalStatusSupervisor>().Configuration.ToolKey;
        }

        public bool IsEnabled => true;

        public void Refresh()
        {
            Init();

            // User
            Users.Clear();
            Users.Add(new FilterItemViewModel<Dto.User>(null)); // All
            foreach (var user in _userService.Invoke(x => x.GetAll(_toolKey)))
            {
                Users.Add(new FilterItemViewModel<Dto.User>(user));
            }
            SelectedUser = Users.First();

            // Area
            Areas.Clear();
            Areas.Add(new FilterItemViewModel<Dto.Log.TableTypeEnum?>(null)); // All
            foreach (Dto.Log.TableTypeEnum table in Enum.GetValues(typeof(Dto.Log.TableTypeEnum)))
            {
                Areas.Add(new FilterItemViewModel<Dto.Log.TableTypeEnum?>(table));
            }
            SelectedArea = Areas.First();

            // Action
            Actions.Clear();
            Actions.Add(new FilterItemViewModel<Dto.Log.ActionTypeEnum?>(null)); // All
            foreach (Dto.Log.ActionTypeEnum action in Enum.GetValues(typeof(Dto.Log.ActionTypeEnum)))
            {
                Actions.Add(new FilterItemViewModel<Dto.Log.ActionTypeEnum?>(action));
            }

            SelectedAction = Actions.First();
            EndDate = DateTime.Now;
            StartDate = EndDate.Value.AddDays(-10);

            ApplyFilter();
        }

        private bool _isBusy = false;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<FilterItemViewModel<Dto.User>> Users { get; private set; }
        public ObservableCollection<FilterItemViewModel<Dto.Log.TableTypeEnum?>> Areas { get; private set; }
        public ObservableCollection<FilterItemViewModel<Dto.Log.ActionTypeEnum?>> Actions { get; private set; }

        private FilterItemViewModel<Dto.User> _selectedUser;

        public FilterItemViewModel<Dto.User> SelectedUser
        {
            get => _selectedUser; set { if (_selectedUser != value) { _selectedUser = value; OnPropertyChanged(); } }
        }

        private FilterItemViewModel<Dto.Log.TableTypeEnum?> _selectedArea;

        public FilterItemViewModel<Dto.Log.TableTypeEnum?> SelectedArea
        {
            get => _selectedArea; set { if (_selectedArea != value) { _selectedArea = value; OnPropertyChanged(); } }
        }

        private FilterItemViewModel<Dto.Log.ActionTypeEnum?> _selectedAction;

        public FilterItemViewModel<Dto.Log.ActionTypeEnum?> SelectedAction
        {
            get => _selectedAction; set { if (_selectedAction != value) { _selectedAction = value; OnPropertyChanged(); } }
        }

        private DateTime? _startDate;

        public DateTime? StartDate
        {
            get => _startDate; set { if (_startDate != value) { _startDate = value; OnPropertyChanged(); } }
        }

        private DateTime? _endDate;

        public DateTime? EndDate
        {
            get => _endDate; set { if (_endDate != value) { _endDate = value; OnPropertyChanged(); } }
        }

        private string _detail;

        public string Detail
        {
            get => _detail; set { if (_detail != value) { _detail = value; OnPropertyChanged(); } }
        }

        private List<Dto.Log> _logs;

        public List<Dto.Log> Logs
        {
            get => _logs; set { if (_logs != value) { _logs = value; OnPropertyChanged(); } }
        }

        private int _bResult;

        public int NbResult
        {
            get => _bResult; set { if (_bResult != value) { _bResult = value; OnPropertyChanged(); ExportCommand.NotifyCanExecuteChanged(); } }
        }

        private void ApplyFilter()
        {
            IsBusy = true;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    int? userId = SelectedUser.WrappedObject?.Id;
                    var logs = _logService.Invoke(x => x.GetLogs(userId, SelectedAction.WrappedObject, SelectedArea.WrappedObject, StartDate, EndDate, Detail));
                    System.Windows.Application.Current.Dispatcher.Invoke((() =>
                    {
                        Logs = logs;
                        NbResult = Logs.Count;
                    }));
                }
                catch (Exception ex)
                {
                    Logs = null;
                    NbResult = 0;
                    _logger.Error(ex, "Log filter");
                    _dialogOwnerService.ShowException(ex, "Apply filter error");
                }
                finally
                {
                    IsBusy = false;
                }
            });
        }

        private void Export()
        {
            try
            {
                var settings = new SaveFileDialogSettings
                {
                    Title = "Save csv",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    Filter = "Logs Files Comma (*.csv) | *.csv;| Logs Files Semi-colon(*.csv) | *.csv",
                    CheckFileExists = false,
                    DefaultExt = "*.csv"
                };

                var rep = _dialogOwnerService.ShowSaveFileDialog(settings);

                if (rep.HasValue && rep.Value)
                {
                    char separator = settings.FilterIndex == 1 ? ',' : ';';

                    using (var file = File.CreateText(settings.FileName))
                    {
                        foreach (var log in _logs)
                        {
                            file.WriteLine(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}", log.Table, separator, log.Action, separator, log.User, separator, log.Date, separator, log.Detail));
                        }
                    }
                    _dialogOwnerService.ShowMessageBox("Export successful");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Export logs");
                _dialogOwnerService.ShowException(ex, "Export logs");
            }
        }

        public bool CanClose() => true;

        #region Commands

        private AutoRelayCommand _refreshCommand = null;

        public AutoRelayCommand RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new AutoRelayCommand(
              () =>
              {
                  Refresh();
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _applyFilterCommand;

        public AutoRelayCommand ApplyFilterCommand
        {
            get
            {
                return _applyFilterCommand ?? (_applyFilterCommand = new AutoRelayCommand(
              () =>
              {
                  ApplyFilter();
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _clearStartDateCommand;

        public AutoRelayCommand ClearStartDateCommand
        {
            get
            {
                return _clearStartDateCommand ?? (_clearStartDateCommand = new AutoRelayCommand(
              () =>
              {
                  StartDate = null;
              },
              () => { return StartDate != null; }));
            }
        }

        private AutoRelayCommand _clearEndDateCommand;

        public AutoRelayCommand ClearEndDateCommand
        {
            get
            {
                return _clearEndDateCommand ?? (_clearEndDateCommand = new AutoRelayCommand(
              () =>
              {
                  EndDate = null;
              },
              () => { return EndDate != null; }));
            }
        }

        private AutoRelayCommand _export;

        public AutoRelayCommand ExportCommand
        {
            get
            {
                return _export ?? (_export = new AutoRelayCommand(
              () =>
              {
                  Export();
              },
              () => { return NbResult != 0; }));
            }
        }

        #endregion Commands
    }
}
