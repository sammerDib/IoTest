using System.Collections.Generic;
using System.Linq;

using Agileo.GUI.Components;
using Agileo.Semi.Gem300.Abstractions.E90;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.SubstrateTracking
{
    public interface IE39ObjectViewModel
    {
        public string Id { get; }

        public LocationState State { get; }
    }

    public class LocationViewModel : Notifier, IE39ObjectViewModel
    {
        public LocationViewModel(string id, LocationState state)
        {
            Id = id;
            State = state;
        }

        public string Id { get; }

        private LocationState _state;

        public LocationState State
        {
            get => _state;
            set => SetAndRaiseIfChanged(ref _state, value);
        }
    }

    public class CarrierViewModel : Notifier, IE39ObjectViewModel
    {
        public CarrierViewModel(string id)
        {
            Id = id;
        }

        public string Id { get; }

        public LocationState State
            => Children.Any(model => model.State == LocationState.Occupied)
                ? LocationState.Occupied
                : LocationState.Unoccupied;

        public List<LocationViewModel> Children { get; } = new();

        public void RaiseStateChanged() => OnPropertyChanged(nameof(State));
    }
}
