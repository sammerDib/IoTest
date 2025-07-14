using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using Optional.Collections;

using UnitySC.PM.EME.Client.Proxy.Camera;
using UnitySC.PM.EME.Client.Proxy.FilterWheel;
using UnitySC.PM.EME.Client.Proxy.Light;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Client.Recipe.ViewModel
{
    public class AcquisitionViewModel : ObservableObject
    {        
        public CameraBench Camera { get; }

        private AcquisitionViewModel(CameraBench camera, FilterWheelBench filterWheelBench, LightBench lightBench, 
            ILogger logger, IMessenger messenger)
        {
            Camera = camera;
            Item = new Acquisition();
            
            Lights = lightBench.Lights;
            CurrentLight = Lights.FirstOrDefault();
            
            Filters = filterWheelBench.GetFilters();
            if (Filters == null)
            {
                logger?.Error("Error on LoadRecipeData. Filters are not available.");
                messenger?.Send(new Message(MessageLevel.Error, "Filters are not available"));
                Filters = new List<Filter>();
                return;
            }
            CurrentFilter = Filters.FirstOrDefault();
        }

        public static AcquisitionViewModel LoadAcquisitionAndCreate(CameraBench camera, FilterWheelBench filterWheelBench, LightBench lightBench, Acquisition acquisition, ILogger logger, IMessenger messenger)
        {
            var viewModel =
                new AcquisitionViewModel(camera, filterWheelBench, lightBench, logger, messenger) { Item = acquisition };

            var selectedFilter = viewModel.Filters.FirstOrNone(filter => filter.Type == acquisition.Filter);
            viewModel.CurrentFilter = selectedFilter.ValueOr(new Filter("Unknown", EMEFilter.Unknown, -1));

            var selectedLight = lightBench.Lights.FirstOrDefault(x => x.DeviceID == acquisition.LightDeviceId);
            if (selectedLight != null)
            {
                viewModel.CurrentLight = selectedLight;
            }
            
            return viewModel;
        }

        public static AcquisitionViewModel Create(CameraBench camera, FilterWheelBench filterWheelBench, LightBench lightBench, ILogger logger, IMessenger messenger, string name)
        {
            var viewModel = new AcquisitionViewModel(camera, filterWheelBench, lightBench, logger, messenger)
            {
                ExposureTime = 100,
                Name = name
            }; 
            return viewModel;
        }

        private Acquisition _item;

        public Acquisition Item
        {
            get => _item;
            set => SetProperty(ref _item, value);
        }

        private bool _inEdition;

        public bool InEdition
        {
            get => _inEdition;
            set => SetProperty(ref _inEdition, value);
        }

        public string Name
        {
            get => Item.Name;
            set
            {
                Item.Name = value;
                OnPropertyChanged();
            }
        }

        public double ExposureTime
        {
            get => Item.ExposureTime;
            set
            {
                Item.ExposureTime = value;
                OnPropertyChanged();
            }
        }

        private List<Filter> _filters;

        public List<Filter> Filters
        {
            get => _filters;
            set => SetProperty(ref _filters, value);
        }

        private Filter _currentFilter;

        public Filter CurrentFilter
        {
            get => _currentFilter;
            set
            {
                if (SetProperty(ref _currentFilter, value))
                {
                    Item.Filter = _currentFilter.Type;
                }
            }
        }
        private ObservableCollection<LightVM> _lights;
        public ObservableCollection<LightVM> Lights
        {
            get => _lights;
            set => SetProperty(ref _lights, value);
        }
        private LightVM _currentLight;
        public LightVM CurrentLight
        {
            get { return _currentLight; }
            set
            {              
                SetProperty(ref _currentLight, value);
                Item.LightDeviceId = _currentLight?.DeviceID;               
            }
        }
    }
}
