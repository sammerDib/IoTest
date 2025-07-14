using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.ResultUI.Common.ViewModel.Defect;

namespace UnitySC.Shared.ResultUI.Common.ViewModel
{
    public class ThumbnailsDefectsVM : ObservableRecipient
    {
        #region Properties

        // Thumbnails matching defect category to be displayed
        private ObservableCollection<Thumbnail> _activeThumbnails;
        public ObservableCollection<Thumbnail> ActiveThumbnails
        {
            get { return _activeThumbnails; }
            set
            {
                _activeThumbnails = value;
                OnPropertyChanged();
            }
        }

        // Thumbnail matching selected defect
        private Thumbnail _thumbnailSelectedDefect;
        public Thumbnail ThumbnailSelectedDefect
        {
            get => _thumbnailSelectedDefect;
            set
            {
                if (_thumbnailSelectedDefect != value)
                {
                    _thumbnailSelectedDefect = value;
                    OnPropertyChanged();
                    if (_thumbnailSelectedDefect != null)
                    {
                        _selectedDefect.Change(value.DefectId);
                    }
                }
            }
        }

        private DefectVMBase _selectedDefect;
        public DefectVMBase SelectedDefect
        {
            get => _selectedDefect;
            set
            {
                if (_selectedDefect != value)
                {
                    _selectedDefect = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public int TotalThumbnails { get; set; }

        #endregion Properties

        public ThumbnailsDefectsVM()
        {
            _activeThumbnails = new ObservableCollection<Thumbnail>();

            _filteredThumbnails = CollectionViewSource.GetDefaultView(ActiveThumbnails);
            _filteredThumbnails.Filter = FiltrerImage;

            Progress = new Progress<int>(newProgress => ComputationProgress = newProgress);
        }

        #region Progress Bar
        public IProgress<int> Progress;

        private int _computationProgress = 0;
        public int ComputationProgress
        {
            get { return _computationProgress; }
            private set => SetProperty(ref _computationProgress, value);
        }
        #endregion

        #region Filtering
        private ICollectionView _filteredThumbnails;
        public ICollectionView FilteredThumbnails
        {
            get { return _filteredThumbnails; }
            set
            {
                _filteredThumbnails = value;
                OnPropertyChanged();
            }
        }

        private List<string> _selectedCategories = new List<string>();
        public List<string> SelectedCategories
        {
            get { return _selectedCategories; }
            set 
            {
                SetProperty(ref _selectedCategories, value);

                FilteredThumbnails.Refresh();
            }
        }

        private bool FiltrerImage(object obj)
        {
            Thumbnail th = obj as Thumbnail;

            return (SelectedCategories.Contains(th.LabelCategory));
        }
        #endregion


        public void UpdateSelectedThumbnail()
        {
            ThumbnailSelectedDefect = SelectedDefect?.DefectClusterImages.DefaultIfEmpty(null).FirstOrDefault();
        }

        public void Dispose()
        {
            ActiveThumbnails.Clear();

            SelectedDefect.DefectClusterImages.Clear();
            SelectedDefect.Features.Clear();
        }

        // display % for each image
        public void UpdateProgressBar(int value)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                Progress.Report(value);
            });
        }
    }
}
