using System;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.Controls.WizardNavigationControl;

namespace UnitySC.PM.ANA.Client.Modules.TestMeasure.ViewModel
{
    public class TrenchMeasureVM : ObservableObject, IWizardNavigationItem, IDisposable
    {
        public string Name { get; set; } = "Trench";
        public bool IsEnabled { get; set; } = true;
        public bool IsMeasure { get; set; } = false;
        public bool IsValidated { get; set; } = false;

        private readonly TestMeasureVM _testMeasureVM;

        public TrenchMeasureVM(TestMeasureVM testMeasureVM)
        {
            _testMeasureVM = testMeasureVM;
        }

        public void Dispose()
        {
        }
    }
}
