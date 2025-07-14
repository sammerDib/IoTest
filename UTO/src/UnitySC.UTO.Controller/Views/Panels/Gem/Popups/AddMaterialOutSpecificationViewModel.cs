using System.Collections.Generic;

using Agileo.GUI.Components;

using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.Popups
{
    public class AddMaterialOutSpecificationViewModel : Notifier
    {
        static AddMaterialOutSpecificationViewModel()
        {
            DataTemplateGenerator.Create(typeof(AddMaterialOutSpecificationViewModel), typeof(AddMaterialOutSpecificationView));
        }

        public AddMaterialOutSpecificationViewModel()
        {

        }

        public AddMaterialOutSpecificationViewModel(List<string> availableSource, List<string> availableDestination)
        {
            AvailableSource = availableSource;
            AvailableDestination = availableDestination;
        }


        private List<string> _availableDestination;

        public List<string> AvailableDestination
        {
            get { return _availableDestination; }
            set => SetAndRaiseIfChanged(ref _availableDestination, value);
        }


        private List<string> _availableSource;

        public List<string> AvailableSource
        {
            get { return _availableSource; }
            set
            {
                _availableSource = value;
                OnPropertyChanged(nameof(AvailableSource));
            }
        }


        private string _selectedSource;

        public string SelectedSource
        {
            get { return _selectedSource; }
            set => SetAndRaiseIfChanged(ref _selectedSource, value);
        }

        private string _selectedDestination;

        public string SelectedDestination
        {
            get { return _selectedDestination; }
            set => SetAndRaiseIfChanged(ref _selectedDestination, value);
        }


    }
}
