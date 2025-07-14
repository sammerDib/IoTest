using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Base.Export;
using UnitySC.Shared.ResultUI.Common.Helpers;
using UnitySC.Shared.ResultUI.Common.Message;
using UnitySC.Shared.ResultUI.Common.ViewModel.Export;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.Shared.ResultUI.Common.ViewModel
{
    public abstract class ResultWaferVM : ObservableRecipient, IDisposable
    {
        private readonly IMessenger _messenger;
        
        #region Properties

        protected IResultDisplay ResultDisplay { get; }

        private IResultDataObject _resdataObj;

        public IResultDataObject ResultDataObj
        {
            get => _resdataObj;
            protected set => SetProperty(ref _resdataObj, value);
        }

        private string _selectedResultFullName;

        public string SelectedResultFullName
        {
            get => _selectedResultFullName;
            private set => SetProperty(ref _selectedResultFullName, value);
        }

        private string _selectedWaferDetaillName;

        public string SelectedWaferDetaillName
        {
            get => _selectedWaferDetaillName;
            private set => SetProperty(ref _selectedWaferDetaillName, value);
        }

        private string _jobRunIterName;

        public string JobRunIterName
        {
            get => _jobRunIterName;
            private set => SetProperty(ref _jobRunIterName, value);
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public abstract string FormatName { get; }

        private string _measureLabelName;

        public string MeasureLabelName
        {
            get => _measureLabelName;
            set => SetProperty(ref _measureLabelName, value);
        }

        public ExportResultVM ExportResultVM { get; }

        private Bitmap _screenshot;

        private Bitmap Screenshot
        {
            get => _screenshot;
            set
            {
                _screenshot?.Dispose();
                _screenshot = value;
            }
        }

        #endregion

        protected ResultWaferVM(IResultDisplay resDisplay)
        {
            _messenger = ClassLocator.Default.GetInstance<IMessenger>();

            ResultDisplay = resDisplay;
           
            _messenger.Register<DisplaySelectedResultFullNameMessage>(this, (r, m) => OnChangeSelectedResultFullName(m));
            _messenger.Register<DisplaySelectedWaferDetaillNameMessage>(this, (r, m) => OnChangeSelectedWaferDetaillName(m));
            _messenger.Register<DisplayJobRunIterNameMessage>(this, (r, m) => OnChangeJobRunIterName(m));

            ExportResultVM = new ExportResultVM();
            ExportResultVM.OnSaveExportCommand += OnSaveExport;
        }

        public virtual void UpdateResData(IResultDataObject resdataObj)
        {
            ResultDataObj = resdataObj;
        }

        #region Messenger Handlers

        public void SetHeaderNames(string resFullName, string waferDetailName, string jobIterName)
        {
            SelectedResultFullName = resFullName;
            SelectedWaferDetaillName = waferDetailName;
            JobRunIterName = jobIterName;
        }

        protected virtual void OnChangeSelectedResultFullName(DisplaySelectedResultFullNameMessage msg)
        {
            SelectedResultFullName = msg.SelectedResultFullName;
        }

        protected virtual void OnChangeSelectedWaferDetaillName(DisplaySelectedWaferDetaillNameMessage msg)
        {
            SelectedWaferDetaillName = msg.SelectedWaferDetaillName;
        }

        protected virtual void OnChangeJobRunIterName(DisplayJobRunIterNameMessage msg)
        {
            JobRunIterName = msg.JobRunIterName;
        }

      

        #endregion

        #region Event Handlers

        private void OnSaveExport()
        {
            if (ExportResultVM != null && !ExportResultVM.IsExporting)
            {
                ExportResultVM.IsExporting = true;

                Task.Run(() =>
                {
                    try
                    {
                        var exportQuery = new ExportQuery
                        {
                            FilePath = ExportResultVM.GetTargetFullPath(),
                            Snapshot = ExportResultVM.ExportSnapshot ? Screenshot : null,
                            SaveAsZip = ExportResultVM.UseZipArchive,
                            SaveResultFile = ExportResultVM.ExportResultData,
                            SaveThumbnails = ExportResultVM.ExportResultThumbnails,
                            AdditionalExports = ExportResultVM.AdditionalEntries.Where(entry => entry.IsChecked).Select(entry => entry.EntryName).ToList()
                        };

                        ResultDisplay.ExportResult.Export(exportQuery, ResultDataObj);

                        Application.Current?.Dispatcher.Invoke(() =>
                        {
                            var notifierVm = ClassLocator.Default.GetInstance<UI.ViewModel.NotifierVM>();
                            notifierVm.AddMessage(new Tools.Service.Message(Tools.Service.MessageLevel.Information, $"{FormatName} Exported in <{ExportResultVM.GetTargetFullPath()}>"));
                        });
                    }
                    catch (Exception ex)
                    {
                        Application.Current?.Dispatcher.Invoke(() =>
                        {
                            var notifierVm = ClassLocator.Default.GetInstance<UI.ViewModel.NotifierVM>();
                            notifierVm.AddMessage(new Tools.Service.Message(Tools.Service.MessageLevel.Error, $"{FormatName} Export failure : {ex.Message}"));
                        });
                    }
                    finally
                    {
                        Screenshot = null;
                        ExportResultVM.IsExporting = false;
                        ExportResultVM.IsStayPopup = false;
                    }

                }).ConfigureAwait(false);
            }
        }

        #endregion

        #region Commands

        private ICommand _openExportPopupCommand;

        public ICommand OpenExportPopupCommand => _openExportPopupCommand ?? (_openExportPopupCommand = new AutoRelayCommand<FrameworkElement>(OpenExportPopupCommandExecute, OpenExportPopupCommandCanExecute));

        protected virtual bool OpenExportPopupCommandCanExecute(FrameworkElement element)
        {
            return true;
        }

        protected virtual void OpenExportPopupCommandExecute(FrameworkElement element)
        {
            if (element != null)
            {
                Screenshot = ScreenshotHelper.Take(element);
            }

            ExportResultVM.GenerateNewTargetPath($"_{FormatName}");
            ExportResultVM.IsStayPopup = true;
        }

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
            _screenshot?.Dispose();

            ExportResultVM.OnSaveExportCommand -= OnSaveExport;

            _messenger.Unregister<DisplaySelectedResultFullNameMessage>(this);
            _messenger.Unregister<DisplaySelectedWaferDetaillNameMessage>(this);
            _messenger.Unregister<DisplayJobRunIterNameMessage>(this);

            OnDeactivated();
        }

        #endregion
    }
}
