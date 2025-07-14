using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.Result.StandaloneClient.Models;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.ResultUI.Common.Message;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI;
using UnitySC.Shared.UI.ViewModel.Navigation;

namespace UnitySC.Result.StandaloneClient.ViewModel
{
    public class FolderEntryVM : NavigationVM
    {
        public event EventHandler<FileEntry> SelectionChanged;

        #region Properties

        public FolderEntry CurrentFolder { get; private set; }

        public ResultVM ResultVM { get; } = new ResultVM();

        private List<FileEntryVM> _slots = new List<FileEntryVM>();

        public List<FileEntryVM> Slots
        {
            get => _slots;
            private set => SetProperty(ref _slots, value);
        }

        private FileEntryVM _selectedFile;

        public FileEntryVM SelectedFile
        {
            get => _selectedFile;
            set
            {
                if (SetProperty(ref _selectedFile, value))
                {
                    SelectionChanged?.Invoke(this, value?.FileEntry);
                }
                RaiseSelectedFileChanged();
            }
        }

        public string CurrentFileNavigation => SelectedFile != null ? $"{SelectedFile.FileIndex} / {Slots.Count}" : "-";

        #endregion

        #region Overrides of PageNavigationVM

        public override string PageName => "Folder View";

        #endregion

        #region Public Methods

        public void SetSelectedFile(FileEntryVM entryVM)
        {
            SetProperty(ref _selectedFile, entryVM, nameof(SelectedFile));
            RaiseSelectedFileChanged();
        }

        public void OpenFolder(FolderEntry folderEntry, List<FileEntry> fileEntries)
        {
            if (!ReferenceEquals(CurrentFolder, folderEntry))
            {
                CurrentFolder = folderEntry;
                RefreshFileList(fileEntries);
            }

            SetProperty(ref _selectedFile, null, nameof(SelectedFile));
            RaiseSelectedFileChanged();

            Navigate(this);
        }

        public void OpenFile(FileEntry fileEntry)
        {
            var lotWafer = GetViewModelFromFile(fileEntry);
            if (lotWafer == null) return;

            SetProperty(ref _selectedFile, lotWafer, nameof(SelectedFile));
            RaiseSelectedFileChanged();

            OpenSelectedFile();
        }

        public void SelectFile(FolderEntry parentFolder, List<FileEntry> fileEntries, FileEntry fileEntry)
        {
            if (!ReferenceEquals(CurrentFolder, parentFolder))
            {
                CurrentFolder = parentFolder;
                RefreshFileList(fileEntries);
            }

            SetProperty(ref _selectedFile, GetViewModelFromFile(fileEntry), nameof(SelectedFile));
            RaiseSelectedFileChanged();

            if (ReferenceEquals(CurrentPage, ResultVM))
            {
                OpenSelectedFile();
            }
        }

        public void RefreshFileList(List<FileEntry> fileEntries)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var previousSlots = Slots.ToList();

                var slots = new List<FileEntryVM>();
                int index = 0;

                foreach (var entry in fileEntries)
                {
                    if (entry.ResultType == ResultType.NotDefined) continue;

                    var previousSlot = previousSlots.SingleOrDefault(vm => ReferenceEquals(vm.FileEntry, entry));
                    if (previousSlot == null)
                    {
                        entry.LoadThumbnail();
                        slots.Add(new FileEntryVM(entry, index + 1));
                    }
                    else
                    {
                        previousSlot.FileIndex = index + 1;
                        slots.Add(previousSlot);
                    }

                    index++;
                }

                SetFileList(slots);
            });
        }

        private readonly object _lockResultUpdate = new object();

        public void OpenSelectedFile()
        {
            // case wafer not present or in error result cannot be displayed
            if (_selectedFile == null) return;

            //Set cursor to busy mode
            Application.Current.Dispatcher.Invoke(BusyHourglass.SetBusyState);

            IResultDataObject result;
            
            if (_selectedFile.FileEntry.ResultType == ResultType.NotDefined)
            {
                App.Instance.NotifierVM.AddMessage(new Message(MessageLevel.Error, $"Unable to load file {_selectedFile.FileEntry.Path} because result type is undefined."));
                return;
            }

            try
            {
                result = App.Instance.ResultDataFactory.CreateFromFile(
                    _selectedFile.FileEntry.ResultType, _selectedFile.FileIndex, _selectedFile.FileEntry.Path);
            }
            catch (Exception ex)
            {
                App.Instance.NotifierVM.AddMessage(new Message(MessageLevel.Error, $"An error occurred while loading the result located at : {_selectedFile.FileEntry.Path}. Error : " + ex.Message));
                return;
            }

            ClassLocator.Default.GetInstance<IMessenger>().Send(new DisplaySelectedWaferDetaillNameMessage
            {
                SelectedWaferDetaillName = _selectedFile.FileEntry.FileName
            });

            Application.Current.Dispatcher.Invoke(() =>
            {
                lock (_lockResultUpdate)
                {
                    if (App.Instance.ResultWaferDictionary.TryGetValue(result.ResType, out var resultWaferVM))
                    {
                        ResultVM.CurrentResultWaferVM = resultWaferVM;
                        try
                        {
                            resultWaferVM.UpdateResData(result);
                        }
                        catch (Exception ex)
                        {
                            App.Instance.NotifierVM.AddMessage(new Message(MessageLevel.Error, $"An error occurred while opening the result located at : {_selectedFile.FileEntry.Path}. Error : " + ex.Message));
                        }
                    }
                }

                if (!ReferenceEquals(CurrentPage, ResultVM))
                {
                    Navigate(ResultVM);
                }

                SelectionChanged?.Invoke(this, _selectedFile.FileEntry);
            });
        }

        #endregion

        #region Private Methods

        private FileEntryVM GetViewModelFromFile(FileEntry entry)
        {
            return Slots.FirstOrDefault(x => ReferenceEquals(x.FileEntry, entry));
        }

        private void SetFileList(List<FileEntryVM> slots)
        {
            Slots = slots;
            SetProperty(ref _selectedFile, null, nameof(SelectedFile));
            RaiseSelectedFileChanged();
        }

        private void RaiseSelectedFileChanged()
        {
            OnPropertyChanged(nameof(CurrentFileNavigation));
            SelectPreviousFileCommand.NotifyCanExecuteChanged();
            SelectNextFileCommand.NotifyCanExecuteChanged();
        }
        
        #endregion

        #region Commands

        private AutoRelayCommand _openSelectedWaferCommand;

        public AutoRelayCommand OpenSelectedFileCommand => _openSelectedWaferCommand ?? (_openSelectedWaferCommand = new AutoRelayCommand(OpenSelectedFile, OpenSelectedFileCommandCanExecute));

        private bool OpenSelectedFileCommandCanExecute()
        {
            return SelectedFile != null;
        }

        private AutoRelayCommand<FileEntry> _openFileEntryCommand;

        public AutoRelayCommand<FileEntry> OpenFileEntryCommand => _openFileEntryCommand ?? (_openFileEntryCommand = new AutoRelayCommand<FileEntry>(OpenFile, OpenFileEntryCommandCanExecute));

        private bool OpenFileEntryCommandCanExecute(FileEntry arg) => arg != null;

        private AutoRelayCommand _refreshThumbnailsCommand;

        public AutoRelayCommand RefreshThumbnailsCommand => _refreshThumbnailsCommand ?? (_refreshThumbnailsCommand = new AutoRelayCommand(RefreshThumbnailsCommandExecute));
        
        private void RefreshThumbnailsCommandExecute()
        {
            foreach (var slot in Slots)
            {
                slot.FileEntry.GenerateThumbnail();
            }
        }

        private AutoRelayCommand _selectPreviousFileCommand;

        public AutoRelayCommand SelectPreviousFileCommand => _selectPreviousFileCommand ?? (_selectPreviousFileCommand = new AutoRelayCommand(SelectPreviousFileCommandExecute, SelectPreviousFileCommandCanExecute));

        private bool SelectPreviousFileCommandCanExecute()
        {
            return SelectedFile != null && Slots.IndexOf(SelectedFile) > 0;
        }

        private void SelectPreviousFileCommandExecute()
        {
            int currentIndex = Slots.IndexOf(SelectedFile);
            currentIndex--;
            var slot = Slots.ElementAt(currentIndex);
            if (slot != null)
            {
                SetProperty(ref _selectedFile, slot, nameof(SelectedFile));
                RaiseSelectedFileChanged();

                if (ReferenceEquals(CurrentPage, ResultVM))
                {
                    OpenSelectedFile();
                }
            }
        }

        private AutoRelayCommand _selectNextFileCommand;

        public AutoRelayCommand SelectNextFileCommand => _selectNextFileCommand ?? (_selectNextFileCommand = new AutoRelayCommand(SelectNextFileCommandExecute, SelectNextFileCommandCanExecute));

        private bool SelectNextFileCommandCanExecute()
        {
            return SelectedFile != null && Slots.IndexOf(SelectedFile) < Slots.Count - 1;
        }

        private void SelectNextFileCommandExecute()
        {
            int currentIndex = Slots.IndexOf(SelectedFile);
            currentIndex++;
            var slot = Slots.ElementAt(currentIndex);
            if (slot != null)
            {
                SetProperty(ref _selectedFile, slot, nameof(SelectedFile));
                RaiseSelectedFileChanged();

                if (ReferenceEquals(CurrentPage, ResultVM))
                {
                    OpenSelectedFile();
                }
            }
        }

        #endregion
    }
}
