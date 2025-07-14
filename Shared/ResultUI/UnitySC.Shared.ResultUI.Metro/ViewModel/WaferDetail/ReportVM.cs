using System;
using System.IO;
using System.Linq;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail
{
    public abstract class ReportVM : ObservableObject, IDisposable
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
                    UpdateReport();
                }
            }
        }

        private string _reportLabel;

        public string ReportLabel
        {
            get => _reportLabel;
            protected set => SetProperty(ref _reportLabel, value);
        }

        private string _reportPath;

        public string ReportPath
        {
            get => _reportPath;
            protected set => SetProperty(ref _reportPath, value);
        }

        public bool HasNoReport => string.IsNullOrEmpty(_reportPath);

        #endregion

        protected ReportVM(PointSelectorBase pointSelector)
        {
            _pointSelector = pointSelector;

            _pointSelector.SelectedPointChanged += OnSelectedPointChanged;
            _pointSelector.SelectedRepetaPointChanged += OnSelectedRepetaPointChanged;
        }

        #region Event Handlers
        
        private void OnSelectedPointChanged(object sender, EventArgs e)
        {
            UpdateReportNavigation();
        }

        private void OnSelectedRepetaPointChanged(object sender, EventArgs e)
        {
            UpdateReport();
        }

        #endregion

        #region Private Methods

        private void UpdateReportNavigation()
        {
            var currentPoint = _pointSelector.SingleSelectedPoint;
            HasSeveralPoints = currentPoint != null && currentPoint.Datas.Count > 1;
        }

        private void UpdateReport()
        {
            OnPropertyChanged(nameof(CurrentPointText));

            ReportPath = null;
            ReportLabel = null;

            if (_pointSelector.SelectedRepetaPoint is MeasurePointDataResultBase repeta)
            {
                string directory = Path.GetDirectoryName(RootPath);
                string resultreport = GetResultReport(repeta);

                if (!string.IsNullOrWhiteSpace(directory) && !string.IsNullOrWhiteSpace(resultreport))
                {
                    ReportLabel = Path.GetFileName(resultreport);
                    string imagePath = Path.GetFullPath(Path.Combine(directory, resultreport));
                    if (File.Exists(imagePath))
                    {
                        ReportPath = imagePath;
                    }
                }
            }
            
            RaiseCommandsCanExecute();
        }

        protected void RaiseCommandsCanExecute()
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                PreviousReportCommand.NotifyCanExecuteChanged();
                NextReportCommand.NotifyCanExecuteChanged();
                OpenReportCommand.NotifyCanExecuteChanged();
                OnPropertyChanged(nameof(HasNoReport));
            });
        }

        protected abstract string GetResultReport(MeasurePointDataResultBase pointData);

        #endregion

        #region Commands

        private AutoRelayCommand _previousReportCommand;

        public AutoRelayCommand PreviousReportCommand => _previousReportCommand ?? (_previousReportCommand = new AutoRelayCommand(PreviousReportCommandExecute, PreviousReportCommandCanExecute));

        private bool PreviousReportCommandCanExecute()
        {
            if (!HasSeveralPoints) return false;
            return CurrentSelectedRepetaIndex > 0;
        }

        private void PreviousReportCommandExecute()
        {
            int newIndex = CurrentSelectedRepetaIndex - 1;
            _pointSelector.SelectRepetaPoint(this, _pointSelector.SingleSelectedPoint.Datas.ElementAt(newIndex));
        }

        private AutoRelayCommand _nextReportCommand;

        public AutoRelayCommand NextReportCommand => _nextReportCommand ?? (_nextReportCommand = new AutoRelayCommand(NextReportCommandExecute, NextReportCommandCanExecute));

        private bool NextReportCommandCanExecute()
        {
            if (!HasSeveralPoints) return false;

            var selectedPoint = _pointSelector.SingleSelectedPoint;
            if (selectedPoint == null) return false;
            
            return CurrentSelectedRepetaIndex < selectedPoint.Datas.Count - 1;
        }

        private void NextReportCommandExecute()
        {
            int newIndex = CurrentSelectedRepetaIndex + 1;
            _pointSelector.SelectRepetaPoint(this, _pointSelector.SingleSelectedPoint.Datas.ElementAt(newIndex));
        }

        private AutoRelayCommand _openReportCommand;

        public AutoRelayCommand OpenReportCommand => _openReportCommand ?? (_openReportCommand = new AutoRelayCommand(OpenReportCommandExecute, OpenReportCommandCanExecute));

        protected  bool OpenReportCommandCanExecute() => ReportPath != null;

        protected  void OpenReportCommandExecute()
        {
            System.Diagnostics.Process.Start(ReportPath);
        }

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
