using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Client.Recipe.ViewModel.Navigation;
using UnitySC.PM.EME.Client.Shared;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.PM.EME.Service.Interface.Recipe.Execution;
using UnitySC.Shared.Image;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using Application = System.Windows.Application;
using Message = UnitySC.Shared.Tools.Service.Message;

namespace UnitySC.PM.EME.Client.Recipe.ViewModel
{
    public class RecipeExecutionViewModel : RecipeWizardStepBaseViewModel
    {
        private readonly EmeClientConfiguration _configuration =
            ClassLocator.Default.GetInstance<EmeClientConfiguration>();
        private readonly Mapper _mapper = ClassLocator.Default.GetInstance<Mapper>();
        private AutoRelayCommand<Tuple<BitmapSource, string>> _openImageInExplorer;
        
        private readonly IEMERecipeService _recipeSupervisor;

        private int _totalImages;

        public RecipeExecutionViewModel(IMessenger messenger, IEMERecipeService recipeSupervisor, EMERecipeVM editedRecipe)
        {
            _messenger = messenger;
            _editedRecipe = editedRecipe;
            _messenger.Register<RecipeExecutionMessage>(this, (_, m) => UpdateStatusAndHandleErrors(m));
            _messenger.Register<ServiceImageWithStatistics>(this, (_, image) => Image = image);
            Name = "Recipe Execution";
            IsEnabled = true;
            ThumbnailsWithPaths = new ObservableCollection<Tuple<BitmapSource, string>>();
            _recipeSupervisor = recipeSupervisor;
        }

        public AutoRelayCommand<Tuple<BitmapSource, string>> OpenImageInExplorer =>
            _openImageInExplorer ?? (_openImageInExplorer = new AutoRelayCommand<Tuple<BitmapSource, string>>(
                imageWithPath =>
                {
                    if (File.Exists(imageWithPath.Item2))
                    {
                        Process.Start("explorer.exe", $"/select,\"{imageWithPath.Item2}\"");
                    }
                },
                imageWithPath => true));

        public int TotalImages
        {
            get => _totalImages;
            set => SetProperty(ref _totalImages, value);
        }


        private int _imageIndex;

        public int ImageIndex
        {
            get => _imageIndex;
            set => SetProperty(ref _imageIndex, value);
        }


        private bool _isRecipeExecuting;

        public bool IsRecipeExecuting
        {
            get => _isRecipeExecuting;
            set => SetProperty(ref _isRecipeExecuting, value);
        }


        private RelayCommand _executeRecipeCommand;

        public ICommand ExecuteRecipeCommand
        {
            get
            {
                return _executeRecipeCommand ?? (_executeRecipeCommand = new RelayCommand(ExecuteRecipe));
            }
        }

        private void ExecuteRecipe()
        {
            IsRecipeExecuting = true;
            var recipe = GetEMERecipe();
            string customPath = EnableCustomPath ? CustomAcquisitionsFolder : String.Empty;
            _recipeSupervisor.StartRecipe(recipe, customPath);
        }


        private AsyncRelayCommand _cancelRecipeCommand;

        public ICommand CancelRecipeCommand
        {
            get
            {
                return _cancelRecipeCommand ?? (_cancelRecipeCommand = new AsyncRelayCommand(CancelRecipe));
            }
        }

        private async Task CancelRecipe()
        {
            await Task.Run(() => _recipeSupervisor.StopRecipe());
        }


        private ExecutionStatus _executionStatus = ExecutionStatus.NotExecuted;
        private readonly IMessenger _messenger;
        private readonly EMERecipeVM _editedRecipe;

        public ExecutionStatus ExecutionStatus
        {
            get => _executionStatus; set => SetProperty(ref _executionStatus, value);
        }

        private void UpdateStatusAndHandleErrors(RecipeExecutionMessage m)
        {
            ExecutionStatus = m.Status;

            switch (m.Status)
            {
                case ExecutionStatus.NotExecuted:
                    break;
                case ExecutionStatus.Started:
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ThumbnailsWithPaths.Clear();
                    }));
                    break;
                case ExecutionStatus.Executing:
                    TotalImages = m.TotalImages;
                    ImageIndex = m.ImageIndex;
                    break;
                case ExecutionStatus.Finished:
                    IsValidated = true;
                    IsRecipeExecuting = false;
                    break;
                case ExecutionStatus.Failed:
                    _messenger.Send(new Message(MessageLevel.Error, m.ErrorMessage));
                    IsRecipeExecuting = false;
                    break;
                case ExecutionStatus.Canceled:
                    IsRecipeExecuting = false;
                    break;
                default:
                    _messenger.Send(new Message(MessageLevel.Error, "Unknown recipe execution status received"));
                    break;
            }

            if (m.Thumbnail != null)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => 
                {
                    var newImageWithPath = new Tuple<BitmapSource, string>(m.Thumbnail.ConvertToWpfBitmapSource(), m.ImageFilePath);
                    ThumbnailsWithPaths.Add(newImageWithPath);
                }));
            }
                
        }


        private ObservableCollection<Tuple<BitmapSource, string>> _thumbnailsWithPaths;

        public ObservableCollection<Tuple<BitmapSource, string>> ThumbnailsWithPaths
        {
            get => _thumbnailsWithPaths;
            set => SetProperty(ref _thumbnailsWithPaths, value);
        }


        public bool EnableCycling => _configuration.EnableCycling;

        private bool _isCycling;

        public bool IsCycling
        {
            get => _isCycling;
            set => SetProperty(ref _isCycling, value);
        }


        private RelayCommand _startCyclingCommand;

        public ICommand StartCyclingCommand
        {
            get
            {
                return _startCyclingCommand ?? (_startCyclingCommand = new RelayCommand(StartCycling));
            }
        }

        private void StartCycling()
        {
            IsCycling = true;
            var recipe = GetEMERecipe();
            string customPath = EnableCustomPath ? CustomAcquisitionsFolder : String.Empty;
            _recipeSupervisor.StartCycling(recipe, customPath);            
        }


        private ICommand _stopCyclingCommand;

        public ICommand StopCyclingCommand
        {
            get
            {
                if (_stopCyclingCommand == null)
                    _stopCyclingCommand = new RelayCommand(StopCycling);

                return _stopCyclingCommand;
            }
        }

        private void StopCycling()
        {
            _recipeSupervisor.StopCycling();
            IsCycling = false;
        }
        
        public EMERecipe GetEMERecipe()
        {
            var recipe = _mapper.AutoMap.Map<EMERecipe>(_editedRecipe);            
            return recipe;
        }


        private bool _enableCustomPath;

        public bool EnableCustomPath
        {
            get => _enableCustomPath;
            set => SetProperty(ref _enableCustomPath, value);
        }


        private string _customAcquisitionFolder;

        public string CustomAcquisitionsFolder
        {
            get => _customAcquisitionFolder;
            set => SetProperty(ref _customAcquisitionFolder, value);
        }


        private AutoRelayCommand _browseCustomAcquisitionsFolder;

        public AutoRelayCommand BrowseCustomAcquisitionsFolder
        {
            get
            {
                return _browseCustomAcquisitionsFolder ?? (_browseCustomAcquisitionsFolder = new AutoRelayCommand(
                    () =>
                    {
                        var folderBrowserDialog = new FolderBrowserDialog { SelectedPath = Path.GetTempPath() };

                        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                        {
                            CustomAcquisitionsFolder = folderBrowserDialog.SelectedPath;
                        }
                    }
                ));
            }
        }

        private ServiceImage _image;

        public ServiceImage Image { get => _image; private set => SetProperty(ref _image, value); }
    }
}
