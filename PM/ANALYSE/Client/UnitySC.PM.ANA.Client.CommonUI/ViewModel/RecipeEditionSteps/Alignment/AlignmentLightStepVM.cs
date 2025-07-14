using System;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Controls;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class AlignmentLightStepVM : AlignmentStepBaseVM, IDisposable
    {
        private TaskCompletionSource<bool> _taskExecuteAuto;

        public AlignmentLightStepVM(RecipeAlignmentVM recipeAlignmentVM) : base(recipeAlignmentVM)
        {
            ServiceLocator.AlgosSupervisor.AutoLightChangedEvent += AlignmentLightVM_AutoLightChangedEvent;
        }

        private double _resultLightPower = double.NaN;

        public double ResultLightPower
        {
            get => _resultLightPower; set { if (_resultLightPower != value) { _resultLightPower = value; OnPropertyChanged(); } }
        }

        public async Task<bool> ExecuteAutoAsync()
        {
            StepState = StepStates.InProgress;
            ResultLightPower = double.NaN;
            _taskExecuteAuto = new TaskCompletionSource<bool>();
            try
            {
                string mainCameraId = ServiceLocator.CamerasSupervisor.GetMainCamera().Configuration.DeviceID;
                string mainLightId = ServiceLocator.LightsSupervisor.GetMainLight().DeviceID;
                var autoLightInput = new AutolightInput(mainCameraId, mainLightId, 30, new ScanRangeWithStep(1, 100, 0.1));
                autoLightInput.InitialContext = new ObjectiveContext(ObjectiveToUse.DeviceID);

                ClassLocator.Default.GetInstance<AlgosSupervisor>().StartAutoLight(autoLightInput);
            }
            catch (Exception)
            {
                StepState = StepStates.Error;
                ErrorMessage = "Failed to start the Auto Light";
                return false;
            }

            return await _taskExecuteAuto.Task;
        }

        //private static CameraVM GetCameraForAutoLight()
        //{
        //    // TODO We must retrieve the main camera
        //    return ClassLocator.Default.GetInstance<CamerasSupervisor>().Cameras.FirstOrDefault();
        //}

        //private static LightVM GetLightForAutoLight()
        //{
        //    var camera = GetCameraForAutoLight();
        //    // TODO We must retrieve the main visible Light
        //    return camera.Lights.FirstOrDefault();
        //}

        private void AlignmentLightVM_AutoLightChangedEvent(AutolightResult autoLightResult)
        {
            if (_taskExecuteAuto is null)
                return;

            if (_taskExecuteAuto.Task.Status == TaskStatus.RanToCompletion)
                return;
            StepState = GetStepStateFromFlowState(autoLightResult.Status.State);

            ErrorMessage = (autoLightResult.Status.State == FlowState.Error) ? autoLightResult.Status.Message : string.Empty;

            if (autoLightResult.Status.State == FlowState.Success)
            {
                Score = (int)(autoLightResult.QualityScore * 100);
                ResultLightPower = autoLightResult.LightPower;

                _taskExecuteAuto.TrySetResult(true);
                return;
            }
            if (autoLightResult.Status.State == FlowState.Error)
                _taskExecuteAuto.TrySetResult(false);
        }

        public void StopExecutionAuto()
        {
            if ((IsInAutomaticMode) && (StepState == StepStates.InProgress))
            {
                Task.Run(() => ClassLocator.Default.GetInstance<AlgosSupervisor>().CancelAutoLight());

                _taskExecuteAuto.TrySetResult(false);
                StepState = StepStates.Error;
                ErrorMessage = "The Auto-Light has been canceled";
            }
        }

        public void Dispose()
        {
            ClassLocator.Default.GetInstance<AlgosSupervisor>().AutoLightChangedEvent -= AlignmentLightVM_AutoLightChangedEvent;
        }

        protected override Task SubmitManualSettings()
        {
            try
            {
                ResultLightPower = ServiceLocator.LightsSupervisor.GetMainLight().Intensity;
            }
            catch (Exception)
            {
                Logger.Error("Failed to set the light intensity");
                ErrorMessage = "Failed to set the light intensity";
                StepState = StepStates.Error;
            }
            RecipeAlignment.IsModified = true;
            StepState = StepStates.Done;
            return Task.CompletedTask;
        }
    }
}
