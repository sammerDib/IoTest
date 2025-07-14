using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.DMT.Service.Interface.AutoExposure;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Interface.Recipe;
using UnitySC.PM.DMT.Shared.UI.Message;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.DMT.Shared.UI.ViewModel
{
    public class ExposureSettingsWithAutoVM : ExposureSettingsVM, IRecipient<RecipeMessage>
    {
        private readonly ScreenSupervisor _screenSupervisor;
        private readonly AlgorithmsSupervisor _algorithmsSupervisor;
        private readonly IDialogOwnerService _dialogService;

        private AutoRelayCommand _autoExposureCompute;

        private double _previousExposureTime;
        private double _tempExposureTime;

        public ExposureSettingsWithAutoVM(Side side, MeasureType? measureType, CameraSupervisor cameraSupervisor, ScreenSupervisor screenSupervisor,
            AlgorithmsSupervisor algorithmsSupervisor,
            IDialogOwnerService dialogService) : base(side, cameraSupervisor)
        {
            _screenSupervisor = screenSupervisor;
            _algorithmsSupervisor = algorithmsSupervisor;
            _dialogService = dialogService;
            Messenger.RegisterAll(this);
            MeasureType = measureType;
        }

        public MeasureType? MeasureType { get; set; }
        
        public bool IsDoingAutoExposure { get; private set; }

        public AutoRelayCommand AutoExposureCompute =>
            _autoExposureCompute ?? (_autoExposureCompute = new AutoRelayCommand(() =>
                {
                    _tempExposureTime = 0;
                    _previousExposureTime = EditExposureTime;
                    OnAutoExposureStarted(new EventArgs());
                    _screenSupervisor.SetScreenColor(WaferSide, Colors.White, false);
                    IsDoingAutoExposure = true;
                    Task.Run(DoStartAutoExposure);
                },
                () => true));

        protected virtual void DoStartAutoExposure()
        {
            _algorithmsSupervisor.StartAutoExposure(WaferSide, MeasureType.GetValueOrDefault());
        }

        public event EventHandler<EventArgs> AutoExposureStarted;

        protected virtual void OnAutoExposureStarted(EventArgs eventArg)
        {
            AutoExposureStarted?.Invoke(this, eventArg);
        }

        public event EventHandler<EventArgs> AutoExposureTerminated;

        protected virtual void OnAutoExposureTerminated(EventArgs eventArg)
        {
            AutoExposureTerminated?.Invoke(this, eventArg);
        }

        public void Receive(RecipeMessage message)
        {
            if (!IsDoingAutoExposure)
                return;

            if (message.Status is AutoExposureStatus status)
                _tempExposureTime = Math.Round(status.ExposureTimeMs, 4);

            switch (message.Status.State)
            {
                case DMTRecipeState.ExecutionComplete:

                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (_tempExposureTime > 0)
                            EditExposureTime = _tempExposureTime;
                        OnAutoExposureTerminated(new EventArgs());
                        IsDoingAutoExposure = false;
                    }));
                    break;

                case DMTRecipeState.Executing:
                    break;

                default:
                    IsDoingAutoExposure = false;
                    EditExposureTime = _previousExposureTime;
                    OnAutoExposureTerminated(new EventArgs());
                    Application.Current.Dispatcher.Invoke(() =>
                    _dialogService.ShowMessageBox($"The auto exposure failed :\n{message.Status.Message}",
                        "AutoExposure", MessageBoxButton.OK, MessageBoxImage.Error));
                    break;
            }
        }
    }
}
