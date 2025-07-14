using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;

using MvvmDialogs.FrameworkDialogs.OpenFile;

using UnitySC.Shared.Image;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.Shared.DieCutUpUI.Common.ViewModel
{
    public class DieCutUpRecipeEditionVM : ObservableRecipient
    {
        private const double GridColorSaturation = 1.0;
        private const double GridColorLightness = 0.5;

        private readonly IDialogOwnerService _dialogService;

        private AutoRelayCommand _addGridCommand;

        private AutoRelayCommand<GridVM> _deleteGridCommand;
        private AutoRelayCommand<GridVM> _editReticlesCommand;

        private ObservableCollection<GridVM> _grids;

        private BitmapSource _image;

        private string _inputImagePath;

        private int _nextGridIndex;

        private GridVM _selectedGrid;

        private AutoRelayCommand _selectImagePathCommand;

        private AutoRelayCommand<GridVM> _selectReferenceReticleCommand;

        private AutoRelayCommand<GridVM> _showReticlesCommand;

        public DieCutUpRecipeEditionVM(IDialogOwnerService dialogOwnerService)
        {
            _dialogService = dialogOwnerService;
            _grids = new ObservableCollection<GridVM> { CreateGrid() };
            _selectedGrid = _grids.FirstOrDefault();
        }

        public ObservableCollection<GridVM> Grids
        {
            get => _grids;
            set => SetProperty(ref _grids, value);
        }

        public GridVM SelectedGrid
        {
            get => _selectedGrid;
            set
            {
                if (_selectedGrid != null)
                {
                    _selectedGrid.IsSelectingReferenceReticle = false;
                }

                SetProperty(ref _selectedGrid, value);
            }
        }

        public string InputImagePath
        {
            get => _inputImagePath;
            set
            {
                SetProperty(ref _inputImagePath, value);
                if (_inputImagePath != null)
                {
                    var serviceImage = new ServiceImage();
                    serviceImage.LoadFromFile(_inputImagePath);
                    Image = serviceImage.WpfBitmapSource;
                }
            }
        }

        public BitmapSource Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        public AutoRelayCommand AddGridCommand
        {
            get => _addGridCommand ?? (_addGridCommand = new AutoRelayCommand(
                () =>
                {
                    Grids.Add(CreateGrid());
                    SelectedGrid = Grids.LastOrDefault();
                },
                () => { return true; }));
        }

        public AutoRelayCommand<GridVM> DeleteGridCommand
        {
            get =>
                _deleteGridCommand ?? new AutoRelayCommand<GridVM>(
                    grid =>
                    {
                        SelectedGrid = grid;
                        if (grid != null && Grids.Contains(grid))
                        {
                            int index = Grids.IndexOf(grid);
                            int size = Grids.Count();
                            Grids.Remove(grid);
                            SelectedGrid = index > 0 && size > 1 ? Grids[index - 1] : size > 1 ? Grids[index] : null;
                        }
                    },
                    grid => grid != null);
        }

        public AutoRelayCommand<GridVM> EditReticlesCommand
        {
            get => _editReticlesCommand ?? (_editReticlesCommand = new AutoRelayCommand<GridVM>(
                grid =>
                {
                    SelectedGrid = grid;
                    SelectedGrid.IsSelectingReferenceReticle = false;
                },
                grid => grid != SelectedGrid || grid.IsSelectingReferenceReticle));
        }

        public AutoRelayCommand<GridVM> SelectReferenceReticleCommand
        {
            get => _selectReferenceReticleCommand ?? (_selectReferenceReticleCommand = new AutoRelayCommand<GridVM>(
                grid =>
                {
                    SelectedGrid = grid;
                    SelectedGrid.IsSelectingReferenceReticle = true;
                },
                grid => grid != SelectedGrid || !grid.IsSelectingReferenceReticle));
        }

        public AutoRelayCommand<GridVM> ShowReticleCommand
        {
            get => _showReticlesCommand ?? (_showReticlesCommand = new AutoRelayCommand<GridVM>(
                grid =>
                {
                    grid.IsReticleVisible = !grid.IsReticleVisible;
                },
                grid => grid != null));
        }

        public AutoRelayCommand SelectImagePathCommand
        {
            get => _selectImagePathCommand ?? (_selectImagePathCommand = new AutoRelayCommand(
                () =>
                {
                    var settings = new OpenFileDialogSettings { CheckFileExists = true, Filter = "Tif Files |*.tif" };
                    bool? res = _dialogService.ShowOpenFileDialog(settings);
                    if (res == true)
                    {
                        InputImagePath = settings.FileName;
                    }
                },
                () => true));
        }

        private GridVM CreateGrid()
        {
            var res = new GridVM($"Grid {_nextGridIndex + 1}",
                new HslColor(_nextGridIndex, GridColorSaturation, GridColorLightness).ToColor());
            _nextGridIndex += 1;

            // There's no reference reticle => let's start by defining it
            res.IsSelectingReferenceReticle = true;

            return res;
        }
    }
}
