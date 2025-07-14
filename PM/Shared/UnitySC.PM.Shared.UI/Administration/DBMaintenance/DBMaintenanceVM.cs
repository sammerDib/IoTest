using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using UnitySC.PM.Shared.UI.Main;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.Shared.UI.Administration.DBMaintenance
{
    public class DBMaintenanceVM : ObservableObject, IMenuContentViewModel
    {
        #region Properties

        private DBMaintenanceSupervisor _dBMaintenanceSupervisor;

        public ObservableCollection<AvailableDBBackupVM> AvailableDBBackups { get; set; }

        private AvailableDBBackupVM _selectedBackup;

        public AvailableDBBackupVM PropertyName
        {
            get => _selectedBackup;
            set => SetProperty(ref _selectedBackup, value);
        }

        public bool IsEnabled => true;

        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private string _busyTitle = "";

        public string BusyTitle
        {
            get => _busyTitle;
            set => SetProperty(ref _busyTitle, value);
        }

        private string _busyMessage = "";

        public string BusyMessage
        {
            get => _busyMessage;
            set => SetProperty(ref _busyMessage, value);
        }

        private double _busyProgress = 0;

        public double BusyProgress
        {
            get => _busyProgress;
            set => SetProperty(ref _busyProgress, value);
        }

        #endregion

        public DBMaintenanceVM()
        {
            _dBMaintenanceSupervisor = ClassLocator.Default.GetInstance<DBMaintenanceSupervisor>();
            _dBMaintenanceSupervisor.DbMaintenanceProgressChangedEvent += DbMaintenanceSupervisor_DbOperationProgressChangedEventAsync;
            AvailableDBBackups = new ObservableCollection<AvailableDBBackupVM>();
        }

        #region Commands

        private RelayCommand _backupDB;

        public RelayCommand BackupDB
        {
            get
            {
                return _backupDB ?? (_backupDB = new RelayCommand(
                    async () =>
                    {
                        IsBusy = true;
                        BusyProgress = 0;
                        BusyTitle = "Backing up Database.";
                        BusyMessage = "This can take a few minutes...";
                        bool resBackup = false;
                        await Task.Run(() => resBackup = _dBMaintenanceSupervisor.BackupDB());
                        IsBusy = false;
                        if (resBackup)
                        {
                            ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox($"The database backup has been created successfully",
                                "Database backup",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                        }
                        else
                        {
                            ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox($"The database backup failed",
                                "Database backup",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        }

                        UpdateAvailableBackups();
                    },
                    () => { return true; }
                ));
            }
        }

        private RelayCommand<AvailableDBBackupVM> _restoreDB;

        public RelayCommand<AvailableDBBackupVM> RestoreDB
        {
            get
            {
                return _restoreDB ?? (_restoreDB = new RelayCommand<AvailableDBBackupVM>(
                    async (dBBackup) =>
                    {
                        var res = ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox(
                            $"Do you want to restore the database {dBBackup.DBName} from the backup created the {dBBackup.Date} ?",
                            "Database restore",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question,
                            MessageBoxResult.No);
                        if (res == MessageBoxResult.Yes)
                        {
                            IsBusy = true;
                            BusyProgress = 0;
                            BusyTitle = "Restoring Database.";
                            BusyMessage = "This can take a few minutes...";
                            bool resRestore = false;
                            await Task.Run(() => resRestore = _dBMaintenanceSupervisor.RestoreDB(dBBackup.FullPath));
                            IsBusy = false;
                            if (resRestore)
                            {
                                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox($"The database has been restored successfully",
                                    "Database restore",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                            }
                            else
                            {
                                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox($"The database restore failed",
                                    "Database restore",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                            }
                        }
                    },
                    (dBBackup) => { return true; }
                ));
            }
        }

        private AsyncRelayCommand _repairDB;

        public AsyncRelayCommand RepairDB
        {
            get
            {
                return _repairDB ?? (_repairDB = new AsyncRelayCommand(
                    async () =>
                    {
                        IsBusy = true;
                        BusyProgress = 0;
                        BusyTitle = "Repairing the Database.";
                        BusyMessage = "This can take a few minutes...";
                        bool resRepair = false;
                        await Task.Run(() => resRepair = _dBMaintenanceSupervisor.RepairDB(ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser.Id));
                        IsBusy = false;
                        if (resRepair)
                        {
                            ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox($"The database repair succeeded",
                                "Database repair",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                        }
                        else
                        {
                            ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox($"The database repair failed",
                                "Database repair",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        }
                    },
                    () => { return true; }
                ));
            }
        }

        #endregion

        public bool CanClose()
        {
            return true;
        }

        public void Refresh()
        {
            UpdateAvailableBackups();
        }

        private void UpdateAvailableBackups()
        {
            AvailableDBBackups.Clear();
            var backupsList = _dBMaintenanceSupervisor.GetBackupsList();
            if (backupsList != null)
            {
                foreach (var backup in backupsList)
                {
                    var newBackup = new AvailableDBBackupVM();

                    string pattern = @"DB_(?<dbName>\w+)_(?<backupCreationDate>\d{8}_\d{6})";

                    Regex regex = new Regex(pattern);
                    Match match = regex.Match(Path.GetFileNameWithoutExtension(backup));
                    bool isBackupValid = false;
                    if (match.Success)
                    {
                        DateTime backupDateTime;
                        newBackup.DBName = match.Groups["dbName"].Value;
                        if (DateTime.TryParseExact(match.Groups["backupCreationDate"].Value, "yyyyMMdd_HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out backupDateTime))
                        {
                            isBackupValid = true;
                        }
                        newBackup.Date = backupDateTime;
                        newBackup.FullPath = backup;
                    }
                    
                    if (isBackupValid)
                    {
                        // Find the index where the new item should be inserted based on its date
                        int index = 0;
                        while (index < AvailableDBBackups.Count && AvailableDBBackups[index].Date > newBackup.Date)
                        {
                            index++;
                        }

                        // Insert the new item at the calculated index
                        AvailableDBBackups.Insert(index, newBackup);
                    }
                }
            }
        }

        private async void DbMaintenanceSupervisor_DbOperationProgressChangedEventAsync(DbOperationProgressMessage progressMessage)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                BusyMessage = progressMessage.Message;
                BusyProgress += progressMessage.Progress;
            }));
        }
    }
}
