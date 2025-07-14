using System.Collections.ObjectModel;
using System.Linq;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Client.Controls.Camera;
using UnitySC.PM.EME.Client.Proxy.Camera;
using UnitySC.PM.EME.Client.Proxy.Light;
using UnitySC.PM.Shared.UI.ViewModels;
using UnitySC.Shared.Tools.Collection;

namespace UnitySC.PM.EME.Client.Modules.TestHardware.ViewModel
{
    public class TestLightViewModel : TabViewModelBase
    {
        public TestLightViewModel(LightBench lightBench, CameraBench cameraBench, IMessenger messenger)
        {
            Lights = lightBench.Lights;
            if (!Lights.IsNullOrEmpty())
            {
                SelectedLight = Lights.FirstOrDefault();
            }

            CameraViewModel = new StandardCameraViewModel(cameraBench, messenger);
        }

        private ObservableCollection<LightVM> _lights;

        public ObservableCollection<LightVM> Lights
        {
            get => _lights;
            set => SetProperty(ref _lights, value);
        }

        private LightVM _selectedLight;

        public LightVM SelectedLight
        {
            get => _selectedLight;
            set => SetProperty(ref _selectedLight, value);
        }

        private StandardCameraViewModel _cameraViewModel;

        public StandardCameraViewModel CameraViewModel
        {
            get => _cameraViewModel;
            private set => SetProperty(ref _cameraViewModel, value);
        }
        public void Close()
        {
            CameraViewModel?.Dispose();
        }
        internal void Refresh()
        {
            CameraViewModel?.Refresh();
        }
    }
}
