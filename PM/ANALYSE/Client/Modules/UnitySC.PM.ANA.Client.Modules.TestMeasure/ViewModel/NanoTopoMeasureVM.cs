using System;
using System.Collections.Generic;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.PM.ANA.Client.Modules.TestMeasure.ViewModel
{
    public class NanoTopoMeasureVM : ObservableObject, IWizardNavigationItem, IDisposable
    {
        public string Name { get; set; } = "Nano Topo";
        public bool IsEnabled { get; set; } = true;
        public bool IsMeasure { get; set; } = false;
        public bool IsValidated { get; set; } = false;
        private NanoTopoMeasureTools _tools;
        private string _lightId;
        private TestMeasureVM _testMeasureVM;

        public IEnumerable<NanoTopoAcquisitionResolution> Resolutions { get; private set; }
        public IEnumerable<string> Algos { get; private set; }


        public NanoTopoMeasureVM(TestMeasureVM testMeasureVM)
        {
            _testMeasureVM = testMeasureVM;
            Resolutions = Enum.GetValues(typeof(NanoTopoAcquisitionResolution)).Cast<NanoTopoAcquisitionResolution>();
            _tools = ((NanoTopoMeasureTools)ServiceLocator.MeasureSupervisor.GetMeasureTools(new NanoTopoSettings())?.Result);
            _lightId = ServiceLocator.MeasureSupervisor.GetMeasureLightIds(MeasureType.NanoTopo)?.Result.FirstOrDefault();
            Algos = _tools?.OrderedAlgoNames;

            SelectedResolution = Resolutions.First();
            SelectedAlgo = Algos?.First();
        }

        private NanoTopoAcquisitionResolution _selectedResolution;

        public NanoTopoAcquisitionResolution SelectedResolution
        {
            get => _selectedResolution;
            set
            {
                if (_selectedResolution != value) { _selectedResolution = value; OnPropertyChanged(); }
            }
        }

        private string _selectedAlgo;

        public string SelectedAlgo
        {
            get => _selectedAlgo; set { if (_selectedAlgo != value) { _selectedAlgo = value; OnPropertyChanged(); } }
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        private AutoRelayCommand _startPSICommand;

        public AutoRelayCommand StartPSICommand
        {
            get
            {
                return _startPSICommand ?? (_startPSICommand = new AutoRelayCommand(
              () =>
              {
                  var context = ServiceLocator.ContextSupervisor.GetTopImageAcquisitionContext()?.Result;
                  var nanoTopoSettings = new NanoTopoSettings()
                  {
                      Name = "Nano Topo Test",
                      IsActive = true,
                      MeasurePoints = new List<int> { 0, 1 },
                      NbOfRepeat = 1,
                      Resolution = SelectedResolution,
                      AlgoName = SelectedAlgo,
                      CameraId = ServiceLocator.CamerasSupervisor.Camera.Configuration.DeviceID,
                      ObjectiveId = ServiceLocator.CamerasSupervisor.Objective.DeviceID,
                      LightId = _lightId,
                      MeasureContext = context
                  };
                  _testMeasureVM.StartMeasure(nanoTopoSettings);
              },
              () => { return true; }));
            }
        }
    }
}
