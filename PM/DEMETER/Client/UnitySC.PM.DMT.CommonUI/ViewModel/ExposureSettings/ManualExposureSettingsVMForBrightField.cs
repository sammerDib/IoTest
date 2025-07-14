using System.Collections.Generic;
using System.Windows.Media;

using UnitySC.PM.DMT.CommonUI.Proxy;
using UnitySC.PM.DMT.CommonUI.ViewModel.Measure;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.DMT.CommonUI.ViewModel.ExposureSettings
{
    internal class ManualExposureSettingsVMForBrightField : ManualExposureSettingsVM
    {
        public new BrightFieldVM Measure => (BrightFieldVM)base.Measure;
        public IEnumerable<Color> AvailableColors => Measure.AvailableColors;

        public ManualExposureSettingsVMForBrightField(string title, MeasureVM measure, CameraSupervisor cameraSupervisor, ScreenSupervisor screenSupervisor,
            CalibrationSupervisor calibrationSupervisor, AlgorithmsSupervisor algorithmsSupervisor, IDialogOwnerService dialogService,
            Mapper mapper, MainRecipeEditionVM mainRecipeEditionVM)
            : base(title, measure, cameraSupervisor, screenSupervisor, calibrationSupervisor, algorithmsSupervisor, dialogService, mapper, mainRecipeEditionVM)
        {
            SelectedScreenColorIndex = 0;
        }
    }
}
