using UnitySC.PM.DMT.CommonUI.Proxy;
using UnitySC.PM.DMT.CommonUI.ViewModel.Measure;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.DMT.CommonUI.ViewModel.ExposureSettings
{
    internal class ManualExposureSettingsVMForHighAngleDarkFieldVM : ManualExposureSettingsVM
    {
        public new HighAngleDarkFieldVM Measure => (HighAngleDarkFieldVM)base.Measure;

        public ManualExposureSettingsVMForHighAngleDarkFieldVM(string title, MeasureVM measure, CameraSupervisor cameraSupervisor, ScreenSupervisor screenSupervisor,
            CalibrationSupervisor calibrationSupervisor, AlgorithmsSupervisor algorithmsSupervisor, IDialogOwnerService dialogService,
            Mapper mapper, MainRecipeEditionVM mainRecipeEditionVM)
            : base(title, measure, cameraSupervisor, screenSupervisor, calibrationSupervisor, algorithmsSupervisor, dialogService, mapper, mainRecipeEditionVM)
        {
            SelectedScreenColorIndex = 2;
            CanChangeScreenDisplay = false;
        }
    }
}
