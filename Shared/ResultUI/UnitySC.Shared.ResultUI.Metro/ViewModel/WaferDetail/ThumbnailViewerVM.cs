using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail
{
    public abstract class ThumbnailViewerVM : ObservableObject, IDisposable
    {
        #region Fields

        private readonly PointSelectorBase _pointSelector;

        #endregion

        #region Properties

        private bool _hasSeveralPoints;

        public bool HasSeveralPoints
        {
            get { return _hasSeveralPoints; }
            set { SetProperty(ref _hasSeveralPoints, value); }
        }

        public string CurrentPointText
        {
            get
            {
                var selectedPoint = _pointSelector.SingleSelectedPoint;
                if (selectedPoint == null) return "-";

                var selectedRepetaPoint = _pointSelector.SelectedRepetaPoint;
                if (selectedRepetaPoint == null) return $"-/{selectedPoint.Datas.Count}";
                
                return $"{CurrentSelectedRepetaIndex + 1}/{selectedPoint.Datas.Count}";
            }
        }

        private int CurrentSelectedRepetaIndex => _pointSelector.SingleSelectedPoint != null && _pointSelector.SelectedRepetaPoint != null ? 
            _pointSelector.SingleSelectedPoint.Datas.IndexOf(_pointSelector.SelectedRepetaPoint) : -1;

        private string _rootPath;

        public string RootPath
        {
            get { return _rootPath; }
            set
            {
                if (SetProperty(ref _rootPath, value))
                {
                    UpdateThumbnail();
                }
            }
        }
        
        private ImageSource _pointImage;

        public ImageSource PointImage
        {
            get => _pointImage;
            protected set => SetProperty(ref _pointImage, value);
        }

        public bool HasNotImage => PointImage == null;

        #endregion

        protected ThumbnailViewerVM(PointSelectorBase pointSelector)
        {
            _pointSelector = pointSelector;

            _pointSelector.SelectedPointChanged += OnSelectedPointChanged;
            _pointSelector.SelectedRepetaPointChanged += OnSelectedRepetaPointChanged;
        }

        #region Event Handlers
        
        private void OnSelectedPointChanged(object sender, EventArgs e)
        {
            UpdateThumbnailNavigation();
        }

        private void OnSelectedRepetaPointChanged(object sender, EventArgs e)
        {
            UpdateThumbnail();
        }

        #endregion

        #region Private Methods

        private void UpdateThumbnailNavigation()
        {
            var currentPoint = _pointSelector.SingleSelectedPoint;
            HasSeveralPoints = currentPoint != null && currentPoint.Datas.Count > 1;
        }

        private void UpdateThumbnail()
        {
            OnPropertyChanged(nameof(CurrentPointText));

            PointImage = null;

            Clear();
            
            if (_pointSelector.SelectedRepetaPoint is MeasurePointDataResultBase repeta)
            {
                string directory = Path.GetDirectoryName(RootPath);
                string resultImage = GetResultImage(repeta);

                if (!string.IsNullOrWhiteSpace(directory) && !string.IsNullOrWhiteSpace(resultImage))
                {
                    string imagePath = Path.GetFullPath(Path.Combine(directory, resultImage));
                    if (File.Exists(imagePath))
                    {
                        LoadFile(imagePath);
                    }
                }
            }
        }

        protected void RaiseCommandsCanExecute()
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                PreviousThumbnailCommand.NotifyCanExecuteChanged();
                NextThumbnailCommand.NotifyCanExecuteChanged();
                OpenImageViewerCommand.NotifyCanExecuteChanged();
                OnPropertyChanged(nameof(HasNotImage));
            });
        }

        protected abstract void Clear();

        public abstract void LoadFile(string filePath);

        protected abstract string GetResultImage(MeasurePointDataResultBase pointData);

        #endregion

        #region Commands

        private AutoRelayCommand _previousThumbnailCommand;

        public AutoRelayCommand PreviousThumbnailCommand => _previousThumbnailCommand ?? (_previousThumbnailCommand = new AutoRelayCommand(PreviousThumbnailCommandExecute, PreviousThumbnailCommandCanExecute));

        private bool PreviousThumbnailCommandCanExecute()
        {
            if (!HasSeveralPoints) return false;
            return CurrentSelectedRepetaIndex > 0;
        }

        private void PreviousThumbnailCommandExecute()
        {
            int newIndex = CurrentSelectedRepetaIndex - 1;
            _pointSelector.SelectRepetaPoint(this, _pointSelector.SingleSelectedPoint.Datas.ElementAt(newIndex));
        }

        private AutoRelayCommand _nextThumbnailCommand;

        public AutoRelayCommand NextThumbnailCommand => _nextThumbnailCommand ?? (_nextThumbnailCommand = new AutoRelayCommand(NextThumbnailCommandExecute, NextThumbnailCommandCanExecute));

        private bool NextThumbnailCommandCanExecute()
        {
            if (!HasSeveralPoints) return false;

            var selectedPoint = _pointSelector.SingleSelectedPoint;
            if (selectedPoint == null) return false;
            
            return CurrentSelectedRepetaIndex < selectedPoint.Datas.Count - 1;
        }

        private void NextThumbnailCommandExecute()
        {
            int newIndex = CurrentSelectedRepetaIndex + 1;
            _pointSelector.SelectRepetaPoint(this, _pointSelector.SingleSelectedPoint.Datas.ElementAt(newIndex));
        }

        private AutoRelayCommand _openImageViewerCommand;

        public AutoRelayCommand OpenImageViewerCommand => _openImageViewerCommand ?? (_openImageViewerCommand = new AutoRelayCommand(OpenImageViewerCommandExecute, OpenImageViewerCommandCanExecute));

        protected abstract bool OpenImageViewerCommandCanExecute();

        protected abstract void OpenImageViewerCommandExecute();

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
            _pointSelector.SelectedPointChanged -= OnSelectedPointChanged;
            _pointSelector.SelectedRepetaPointChanged -= OnSelectedRepetaPointChanged;
        }

        #endregion
    }
}
