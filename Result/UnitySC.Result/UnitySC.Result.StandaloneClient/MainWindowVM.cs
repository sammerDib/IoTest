using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using MvvmDialogs.FrameworkDialogs.FolderBrowser;

using UnitySC.Result.StandaloneClient.Models;
using UnitySC.Result.StandaloneClient.ViewModel;
using UnitySC.Result.StandaloneClient.ViewModel.Common;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.ResultUI.Common.Components.DataTree;
using UnitySC.Shared.ResultUI.Common.Components.Generic.Filters;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Dialog.FrameworkDialogs.FolderBrowser;

namespace UnitySC.Result.StandaloneClient
{
    public class MainWindowVM : ObservableRecipient
    {
        #region Properties

        private string _windowTitle = "UnitySC Results Standalone Client";

        public string WindowTitle
        {
            get => _windowTitle;
            private set => SetProperty(ref _windowTitle, value);
        }

        public FolderEntryVM FolderEntryVM { get; } = new FolderEntryVM();

        public SettingVM SettingVM { get; } = new SettingVM();

        public DataTreeSource<ExplorerEntry> DataTreeSource { get; }

        private ExplorerEntry _selectedEntry;

        public ExplorerEntry SelectedEntry
        {
            get => _selectedEntry;
            set
            {
                SetProperty(ref _selectedEntry, value);
                UpdateFolderVM(value);
                SyncSelectedCommand?.NotifyCanExecuteChanged();
            }
        }
        
        private bool _isUiBusy;

        public bool IsUiBusy
        {
            get => _isUiBusy;
            set
            {
                if (value)
                {
                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        SetProperty(ref _isUiBusy, true);
                        OpenFolderCommand.NotifyCanExecuteChanged();
                        OpenFileCommand.NotifyCanExecuteChanged();
                    });
                }
                else
                {
                    Application.Current?.Dispatcher?.BeginInvoke(DispatcherPriority.ApplicationIdle, (Action)(() =>
                    {
                        SetProperty(ref _isUiBusy, false);
                        OpenFolderCommand.NotifyCanExecuteChanged();
                        OpenFileCommand.NotifyCanExecuteChanged();
                    }));
                }
            }
        }

        private bool _explorerIsExpanded;

        public bool ExplorerIsExpanded
        {
            get => _explorerIsExpanded;
            set => SetProperty(ref _explorerIsExpanded, value);
        }

        #endregion

        public MainWindowVM()
        {
            FolderEntryVM.SelectionChanged += FolderVM_SelectionChanged;

            DataTreeSource = new DataTreeSource<ExplorerEntry>(entry =>
            {
                if (entry is FolderEntry folder)
                {
                    folder.BuildChildren();
                    return folder.Children;
                }

                return null;
            });

            // Search
            DataTreeSource.Search.AddSearchDefinition(nameof(FileEntry.FileName), item => item is FileEntry file ? file.FileName : string.Empty, true);

            // Sort
            DataTreeSource.Sort.AddSortDefinition("File Name", node => node.Model.Path);
            DataTreeSource.Sort.AddSortDefinition("Result Type", node => node.Model is FileEntry file ? file.ResultType : ResultType.NotDefined);

            // Constant filter to hide empty folders
            DataTreeSource.Filter.Add(new FilterSwitch<TreeNode<ExplorerEntry>>("Results files only", node => node.Model is FileEntry file && file.ResultType != ResultType.NotDefined && file.ResultType != ResultType.Empty)
            {
                IsConstant = true,
                IsEnabled = true
            });

            DataTreeSource.Filter.Add(new FilterCollection<TreeNode<ExplorerEntry>, string>("Result Type",
                () => DataTreeSource.GetFlattenElements().Select(node => node.Model).OfType<FileEntry>().Select(entry => entry.TypeName).Where(x => !x.IsNullOrEmpty()).Distinct(),
                node => node.Model is FileEntry file ? file.TypeName : string.Empty));

            DataTreeSource.Filter.ApplyFiltersCommandExecute();
            DataTreeSource.DisplayedElementsChanged += DataTreeSourceOnDisplayedElementsChanged;
        }

        #region Methods

        #region Event Handlers

        private bool _preventRaiseCollectionChanged;

        /// <summary>
        /// Raised on Sort / Search / Filter is applied
        /// </summary>
        private void DataTreeSourceOnDisplayedElementsChanged(object sender, EventArgs e)
        {
            if (_preventRaiseCollectionChanged) return;

            if (SelectedEntry is FileEntry file)
            {
                if (!ReferenceEquals(file, FolderEntryVM.SelectedFile?.FileEntry))
                {
                    FolderEntryVM.SetSelectedFile(null);
                }

                var parentNode = DataTreeSource.GetTreeNode(file)?.Parent;
                if (parentNode == null)
                {
                    FolderEntryVM.RefreshFileList(new List<FileEntry>());
                    return;
                }
                FolderEntryVM.RefreshFileList(parentNode.VisibleTreeElements.Select(node => node.Model).OfType<FileEntry>().ToList());
            }
            else if (SelectedEntry is FolderEntry folder)
            {
                var folderNode = DataTreeSource.GetTreeNode(folder);
                if (folderNode == null)
                {
                    FolderEntryVM.RefreshFileList(new List<FileEntry>());
                    return;
                }
                FolderEntryVM.RefreshFileList(folderNode.VisibleTreeElements.Select(node => node.Model).OfType<FileEntry>().ToList());
            }
        }

        private void FolderVM_SelectionChanged(object sender, FileEntry e)
        {
            if (e != null)
            {
                SetProperty(ref _selectedEntry, e, nameof(SelectedEntry));
            }
            else if (FolderEntryVM.CurrentFolder != null)
            {
                SetProperty(ref _selectedEntry, FolderEntryVM.CurrentFolder, nameof(SelectedEntry));
            }

            SyncSelectedCommand?.NotifyCanExecuteChanged();
        }

        #endregion

        #region Public Methods

        public void Init(string argumentPath = "")
        {
            IsUiBusy = true;

            if (argumentPath.IsNullOrEmpty())
            {
                OpenDefaultFolder();
            }
            else
            {
                try
                {
                    var fileAttributes = File.GetAttributes(argumentPath);
                    if (fileAttributes.HasFlag(FileAttributes.Directory))
                    {
                        ResetDataTree(argumentPath, true);
                    }
                    else
                    {
                        string parentPath = Directory.GetParent(argumentPath)?.FullName ?? string.Empty;

                        ResetDataTree(parentPath, false);
                        SelectedEntry = DataTreeSource.GetFlattenElements().FirstOrDefault(node => node.Model is FileEntry file && file.Path == argumentPath)?.Model;
                        FolderEntryVM.OpenSelectedFile();
                    }
                }
                catch (Exception e)
                {
                    OpenDefaultFolder();
                    App.Instance?.NotifierVM?.AddMessage(new Message(MessageLevel.Error, "Error when opening the folder/file in args. Error : " + e.Message));
                }
            }

            IsUiBusy = false;
        }

        private void OpenDefaultFolder()
        {
            string directory = Settings.AppPath;
            if (Settings.DefaultResultDirectory.IsNullOrEmpty() || !Directory.Exists(Settings.DefaultResultDirectory))
            {
                App.Instance?.NotifierVM?.AddMessage(new Message(MessageLevel.Error, "App.config default directory path is incorrect or empty. This path is opened : " + directory));
            }
            else
            {
                directory = Settings.DefaultResultDirectory;
            }

            ResetDataTree(directory, true);
        }

        #endregion

        #region Private Methods

        private void UpdateFolderVM(ExplorerEntry value)
        {
            IsUiBusy = true;

            if (value is FileEntry file)
            {
                if (!ReferenceEquals(file, FolderEntryVM.SelectedFile?.FileEntry))
                {
                    FolderEntryVM.SetSelectedFile(null);
                }

                var parentNode = DataTreeSource.GetTreeNode(file)?.Parent;
                var parent = parentNode?.Model;
                if (parent is FolderEntry folderParent)
                {
                    FolderEntryVM.SelectFile(folderParent, parentNode.VisibleTreeElements.Select(node => node.Model).OfType<FileEntry>().ToList(), file);
                    WindowTitle = $"UnitySC Results Standalone Client - <{folderParent.Path}>";
                }
                else
                {
                    App.Instance?.NotifierVM?.AddMessage(new Message(MessageLevel.Warning, $"Parent folder of file {file.Path} not found."));
                    FolderEntryVM.SelectFile(new FolderEntry(string.Empty, string.Empty), new List<FileEntry> { file }, file);
                }
            }
            else if (value is FolderEntry folder)
            {
                var folderNode = DataTreeSource.GetTreeNode(folder);
                FolderEntryVM.OpenFolder(folder, folderNode.VisibleTreeElements.Select(node => node.Model).OfType<FileEntry>().ToList());
                WindowTitle = $"UnitySC Results Standalone Client - <{folder.Path}>";
            }
            
            IsUiBusy = false;
        }

        /// <summary>
        /// Rebuild the tree and select the root element
        /// </summary>
        private void ResetDataTree(string folderPath, bool autoSelectRootElement)
        {
            var root = CreateFolderEntry(folderPath);
            DataTreeSource.Reset(new List<ExplorerEntry>
            {
                root
            });

            if (autoSelectRootElement)
            {
                SelectedEntry = root;
                if (DataTreeSource.GetTreeNode(root) is TreeNode<ExplorerEntry> rootNode)
                {
                    rootNode.IsExpanded = true;
                }
            }
        }
        
        private static FolderEntry CreateFolderEntry(string folderPath)
        {
            string folderName = folderPath.Substring(folderPath.LastIndexOf('\\') + 1);
            return new FolderEntry(folderPath, folderName);
        }
        
        #endregion
        
        #endregion

        #region Commands

        private ICommand _closeExpanderCommand;

        public ICommand CloseExpanderCommand => _closeExpanderCommand ?? (_closeExpanderCommand = new AutoRelayCommand(CloseExpanderCommandExecute));

        private void CloseExpanderCommandExecute() => ExplorerIsExpanded = false;

        private AutoRelayCommand _openFolderCommand;

        public AutoRelayCommand OpenFolderCommand => _openFolderCommand ?? (_openFolderCommand = new AutoRelayCommand(OpenFolderCommandExecute, OpenFolderCommandCanExecute));

        private void OpenFolderCommandExecute()
        {
            var folderDialogSettings = new FolderBrowserDialogSettings();
            var folderDialog = new CustomFolderBrowserDialog(folderDialogSettings);

            folderDialog.ShowDialog(Application.Current.MainWindow);

            if (!folderDialogSettings.SelectedPath.IsNullOrEmpty())
            {
                IsUiBusy = true;
                ResetDataTree(folderDialogSettings.SelectedPath, true);
                IsUiBusy = false;
            }
        }

        private bool OpenFolderCommandCanExecute() => !IsUiBusy;

        private ICommand _refreshFolderCommand;

        public ICommand RefreshFolderCommand => _refreshFolderCommand ?? (_refreshFolderCommand = new AutoRelayCommand(RefreshFolderCommandExecute));

        private void RefreshFolderCommandExecute()
        {
            var rootElement = DataTreeSource.Nodes.FirstOrDefault();
            string currentPath = SelectedEntry?.Path;

            if (rootElement?.Model is FolderEntry rootFolder)
            {
                ResetDataTree(rootFolder.Path, false);
            }

            if (currentPath != null)
            {
                var associatedNode = DataTreeSource.GetFlattenElements().FirstOrDefault(node => node.Model.Path == currentPath);
                if (associatedNode != null)
                {
                    SelectedEntry = associatedNode.Model;
                    SyncSelectedCommandExecute();

                    if (associatedNode.Model is FolderEntry)
                    {
                        associatedNode.IsExpanded = true;
                    }
                }
            }
            else
            {
                var newRootElement = DataTreeSource.Nodes.FirstOrDefault();
                if (newRootElement != null)
                {
                    SelectedEntry = newRootElement.Model;
                    newRootElement.IsExpanded = true;
                }
            }
        }

        private AutoRelayCommand _openFileCommand;

        public AutoRelayCommand OpenFileCommand => _openFileCommand ?? (_openFileCommand = new AutoRelayCommand(OpenFileCommandExecute, OpenFileCommandCanExecute));

        private void OpenFileCommandExecute()
        {
            ExplorerIsExpanded = false;
            
            IsUiBusy = true;
            Task.Run(() =>
            {
                FolderEntryVM.OpenSelectedFile();
                IsUiBusy = false;
            });
        }

        private bool OpenFileCommandCanExecute() => !IsUiBusy;

        private AutoRelayCommand _syncSelectedCommand;

        public AutoRelayCommand SyncSelectedCommand => _syncSelectedCommand ?? (_syncSelectedCommand = new AutoRelayCommand(SyncSelectedCommandExecute, SyncSelectedCommandCanExecute));

        private bool SyncSelectedCommandCanExecute()
        {
            return SelectedEntry != null;
        }

        private void SyncSelectedCommandExecute()
        {
            _preventRaiseCollectionChanged = true;
            DataTreeSource.SyncWithSelected();
            _preventRaiseCollectionChanged = false;
        }

        private ICommand _collapseAllCommand;

        public ICommand CollapseAllCommand => _collapseAllCommand ?? (_collapseAllCommand = new AutoRelayCommand(CollapseAllCommandExecute));

        private void CollapseAllCommandExecute()
        {
            _preventRaiseCollectionChanged = true;
            DataTreeSource.CollapseAll();
            _preventRaiseCollectionChanged = false;
        }

        private ICommand _expandAllCommand;

        public ICommand ExpandAllCommand => _expandAllCommand ?? (_expandAllCommand = new AutoRelayCommand(ExpandAllCommandExecute));

        private void ExpandAllCommandExecute()
        {
            _preventRaiseCollectionChanged = true;
            DataTreeSource.ExpandAll();
            _preventRaiseCollectionChanged = false;
        }

        private ICommand _openSettingsPanelCommand;

        public ICommand OpenSettingsPanelCommand => _openSettingsPanelCommand ?? (_openSettingsPanelCommand = new AutoRelayCommand(OpenSettingsPanelCommandExecute));

        private void OpenSettingsPanelCommandExecute()
        {
            FolderEntryVM.Navigate(SettingVM);
            ExplorerIsExpanded = false;
        }

        #endregion
    }
}
