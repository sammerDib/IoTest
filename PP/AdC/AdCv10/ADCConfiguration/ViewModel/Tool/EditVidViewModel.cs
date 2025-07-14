using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

using Microsoft.Win32; // pour open et save dialog ?

using MvvmValidation;

using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using Dto = UnitySC.DataAccess.Dto;

namespace ADCConfiguration.ViewModel.Tool
{
    public class EditVidViewModel : ViewModelWithMenuBase, INavigationViewModel
    {
        private ObservableCollection<VidViewModel> _vids;

        private VidViewModel _selectedVid;
        public VidViewModel SelectedVid
        {
            get => _selectedVid;
            set
            {
                if (_selectedVid != value)
                {
                    _selectedVid = value;
                    OnPropertyChanged();
                    DeleteCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public EditVidViewModel()
        {
            Validator.AddRule("VidId", () => RuleResult.Assert(!_vids.GroupBy(x => x.Id).Where(g => g.Count() > 1).Any(), "Vid Id must be unique"));
            Validator.AddRule("VidLabel", () => RuleResult.Assert(!_vids.GroupBy(x => x.Label).Where(g => g.Count() > 1).Any(), "Vid Label must be unique"));

            MenuName = "Edit vids";
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
                Name = "Save",
                Description = "Save",
                ExecuteCommand = SaveCommand,
                ImageResourceKey = "SaveADCImage",
                IconText = "Save"
            });
            //Init();
        }

        private ICollectionView _vidsView;
        public ICollectionView Vids
        {
            get { return _vidsView; }
            set
            {
                _vidsView = value;
                OnPropertyChanged();
            }
        }

        public void Init()
        {
            IsBusy = true;


            Task.Factory.StartNew(() =>
            {
                try
                {
                    var dbToolServiceForVid = ClassLocator.Default.GetInstance<DbToolServiceProxy>();
                    _vids = new ObservableCollection<VidViewModel>(dbToolServiceForVid.GetAllVid().Select(x => new VidViewModel(x, Validator)));
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Vids = CollectionViewSource.GetDefaultView(_vids);
                        _vidsView.Filter = VidFilter;
                        Validator.Reset();
                    });
                }
                catch (Exception ex)
                {
                    Services.Services.Instance.LogService.LogError("Refresh Vids", ex);
                    System.Windows.Application.Current.Dispatcher.Invoke((() => { AdcTools.ExceptionMessageBox.Show("Refresh vids error: ", ex); }));
                }
                finally
                {
                    IsBusy = false;
                }
            });
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
                    _vidsView.Refresh();
                }
            }
        }

        private bool VidFilter(object obj)
        {
            if (!string.IsNullOrEmpty(_filter))
            {
                VidViewModel vidViewModel = obj as VidViewModel;
                return vidViewModel.Label.ToLower().Contains(_filter.ToLower()) || vidViewModel.Id.ToString().Contains(_filter);
            }
            else
                return true;
        }

        public void Save()
        {
            IsBusy = true;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var dbToolServiceForVid = ClassLocator.Default.GetInstance<DbToolServiceProxy>();
                    dbToolServiceForVid.SetAllVid(_vids.Select(x => x.DtoVid).ToList(), Services.Services.Instance.AuthentificationService.CurrentUser.Id);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        foreach (var vid in _vids)
                        {
                            vid.HasChanged = false;
                        }
                    });
                }
                catch (Exception ex)
                {
                    Services.Services.Instance.LogService.LogError("Save vids", ex);
                    System.Windows.Application.Current.Dispatcher.Invoke((() => { AdcTools.ExceptionMessageBox.Show("Save vids error: ", ex); }));
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
                SaveFileDialog saveFileDlg = new SaveFileDialog();

                saveFileDlg.Filter = "Vid Files Comma (*.csv) | *.csv;| Vid Files Semi-colon(*.csv) | *.csv";

                if (saveFileDlg.ShowDialog() != true)
                    return;

                char separator = saveFileDlg.FilterIndex == 1 ? ',' : ';';

                using (var file = File.CreateText(saveFileDlg.FileName))
                {
                    foreach (var vid in _vids)
                    {
                        file.WriteLine(string.Format("{0}{1}{2}", vid.Id, separator, vid.Label));
                    }
                }
            }
            catch (Exception ex)
            {
                Services.Services.Instance.LogService.LogError("Export Vids", ex);
                AdcTools.ExceptionMessageBox.Show("Export vids error: ", ex);
            }
        }
        private void Add()
        {
            VidViewModel newVid = new VidViewModel(
                new Dto.Vid()
                {
                    Id = (_vids.Any() ? _vids.Max(x => x.Id) : 0) + 1,
                    Label = "New Vid"
                }, Validator);
            _vids.Add(newVid);
            newVid.HasChanged = true;
            Filter = null;
            SelectedVid = newVid;
            Validator.ValidateAll();
        }

        private void Delete()
        {
            if (SelectedVid != null)
                _vids.Remove(SelectedVid);
            Validator.ValidateAll();
        }

        private void Import()
        {
            try
            {
                OpenFileDialog openFileDlg = new OpenFileDialog();
                openFileDlg.Filter = "Vid Files Comma (*.csv) | *.csv;| Vid Files Semi-colon(*.csv) | *.csv";

                if (openFileDlg.ShowDialog() != true)
                    return;

                char separator = openFileDlg.FilterIndex == 1 ? ',' : ';';

                _vids.Clear();
                Filter = null;

                foreach (string line in File.ReadAllLines(openFileDlg.FileName))
                {
                    string[] values = line.Split(separator);
                    _vids.Add(new VidViewModel(new Dto.Vid() { Id = Convert.ToInt32(values[0]), Label = values[1] }, Validator));
                }

                Validator.ValidateAll();
            }
            catch (Exception ex)
            {
                Services.Services.Instance.LogService.LogError("Import Vids", ex);
                AdcTools.ExceptionMessageBox.Show("Import vids error: ", ex);
                Init();
            }
        }

        public void Refresh()
        {
            Init();
        }

        public bool MustBeSave => _vids.Any(x => x.HasChanged);

        #region Commands


        private AutoRelayCommand _importCommand;
        public AutoRelayCommand ImportCommand
        {
            get
            {
                return _importCommand ?? (_importCommand = new AutoRelayCommand(
              () =>
              {
                  Import();
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _exportCommand;
        public AutoRelayCommand ExportCommand
        {
            get
            {
                return _exportCommand ?? (_exportCommand = new AutoRelayCommand(
              () =>
              {
                  Export();
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
              () => { return SelectedVid != null; }));
            }
        }

        private AutoRelayCommand _refreshCommand = null;
        public AutoRelayCommand RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new AutoRelayCommand(
              () =>
              {
                  if (_vids.Any(x => x.HasChanged))
                  {
                      if (MessageBox.Show("Some vids are not saved and will be lost." + Environment.NewLine + "Do you want to refresh anyway ?", "Some vids are not saved", MessageBoxButton.YesNo) == MessageBoxResult.No)
                          return;
                  }

                  Init();
                  Services.Services.Instance.LogService.LogDebug("Refresh vids");
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
              () => { return !HasErrors; }));
            }
        }

        #endregion
    }

    public class VidViewModel : ViewModelWithMenuBase
    {
        private Dto.Vid _vid;
        private ValidationHelper _parentValidator;

        public Dto.Vid DtoVid
        {
            get { return _vid; }
        }

        public VidViewModel(Dto.Vid vid, ValidationHelper parentValidator)
        {
            _vid = vid;
            _parentValidator = parentValidator;
        }

        public int Id
        {
            get => _vid.Id;
            set
            {
                if (_vid.Id != value)
                {
                    _vid.Id = value;
                    OnPropertyChanged();
                    _parentValidator.Validate("VidId");
                }
            }
        }

        public string Label
        {
            get => _vid.Label;
            set
            {
                if (_vid.Label != value)
                {
                    _vid.Label = value;
                    OnPropertyChanged();
                    _parentValidator.Validate("VidLabel");
                }
            }
        }
    }
}
