using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.PM.ANA.Client.Modules.ProbeCapacity.ViewModel;
using UnitySC.PM.ANA.Client.Proxy.Compatibility;
using UnitySC.PM.ANA.Service.Interface.Compatibility.Capability;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client.Modules.ProbeCapacity
{
    public class ProbeCapacityVM : ObservableObject, IMenuContentViewModel
    {
        public ProbeCapacityVM()
        {
            NewCapabilities = Enum.GetValues(typeof(NewCapability)).Cast<NewCapability>();
            SelectedNewCapability = NewCapabilities.First();
            Test = new TestVM(this);
        }

        public TestVM Test { get; private set; }

        public IEnumerable<NewCapability> NewCapabilities { get; private set; }

        private NewCapability _selectedNewCapability;
        public NewCapability SelectedNewCapability
        {
            get => _selectedNewCapability; set { if (_selectedNewCapability != value) { _selectedNewCapability = value; OnPropertyChanged(); } }
        }

        private ProbeVM _selectedProbe;
        public ProbeVM SelectedProbe
        {
            get => _selectedProbe;
            set
            {
                if (_selectedProbe != value)
                {
                    _selectedProbe = value;
                    OnPropertyChanged();
                    AddCapabilityCommand.NotifyCanExecuteChanged();
                    DeleteCapabilityCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private AutoRelayCommand _addCapabilityCommand;
        public AutoRelayCommand AddCapabilityCommand
        {
            get
            {
                return _addCapabilityCommand ?? (_addCapabilityCommand = new AutoRelayCommand(
              () =>
              {
                  CapabilityBase capabiltyBase = null;
                  switch (_selectedNewCapability)
                  {
                      case NewCapability.CrossLayer:
                          capabiltyBase = new CrossLayer();
                          break;
                      case NewCapability.DistanceMeasure:
                          capabiltyBase = new DistanceMeasure();
                          break;
                      case NewCapability.ThicknessMeasure:
                          capabiltyBase = new ThicknessMeasure();
                          break;
                  }
                  SelectedProbe.Capabilities.Add(capabiltyBase);
              },
              () => { return SelectedProbe != null; }));
            }
        }


        private AutoRelayCommand _saveCommand;
        public AutoRelayCommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new AutoRelayCommand(
              () =>
              {
                  var model = VMToModel();
                  ClassLocator.Default.GetInstance<CompatibilitySupervisor>().SaveProbeCompatibility(model);

              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _undoCommand;
        public AutoRelayCommand UndoCommand
        {
            get
            {
                return _undoCommand ?? (_undoCommand = new AutoRelayCommand(
              () =>
              {
                  var msgresult = ClassLocator.Default.GetInstance<UnitySC.Shared.UI.Dialog.IDialogOwnerService>().ShowMessageBox("Do you want to cancel the modifications since the last save ?", "Undo Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

                  if (msgresult == MessageBoxResult.Yes)
                  {
                      Refresh();
                  }
              },
              () => { return true; }));
            }
        }


        private AutoRelayCommand<CapabilityBase> _deleteCapabilityCommand;
        public AutoRelayCommand<CapabilityBase> DeleteCapabilityCommand
        {
            get
            {
                return _deleteCapabilityCommand ?? (_deleteCapabilityCommand = new AutoRelayCommand<CapabilityBase>(
              (capability) =>
              {
                  if (SelectedProbe != null)
                  {
                      var msgresult = ClassLocator.Default.GetInstance<UnitySC.Shared.UI.Dialog.IDialogOwnerService>().ShowMessageBox("Do you really want to definitively remove this capability ?", "Remove item Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

                      if (msgresult == MessageBoxResult.Yes)
                      {
                          SelectedProbe.Capabilities.Remove(capability);
                      }
                  }
              },
              (capability) => { return SelectedProbe != null; }));
            }
        }


        private AutoRelayCommand _addProbeCommand;
        public AutoRelayCommand AddProbeCommand
        {
            get
            {
                return _addProbeCommand ?? (_addProbeCommand = new AutoRelayCommand(
              () =>
              {
                  Probes.Add(new ProbeVM("Probe name"));
                  SelectedProbe = Probes.Last();
              },
              () => { return true; }));
            }
        }


        private AutoRelayCommand<ProbeVM> _deleteProbeCommand;
        public AutoRelayCommand<ProbeVM> DeleteProbeCommand
        {
            get
            {
                return _deleteProbeCommand ?? (_deleteProbeCommand = new AutoRelayCommand<ProbeVM>(
              (probe) =>
              {
                  var msgresult = ClassLocator.Default.GetInstance<UnitySC.Shared.UI.Dialog.IDialogOwnerService>().ShowMessageBox("Do you really want to definitively remove this probe ?", "Remove item Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

                  if (msgresult == MessageBoxResult.Yes)
                  {
                      Probes.Remove(probe);
                      if (Probes.Any())
                          SelectedProbe = Probes.First();
                  }
              },
              (probe) => { return probe != null; }));
            }
        }

        public ObservableCollection<ProbeVM> Probes { get; private set; }

        private void ModelToVM(ProbeCompatibility probeCompatibility)
        {
            Probes = new ObservableCollection<ProbeVM>();
            foreach (var probeM in probeCompatibility.Probes)
            {
                var probeVM = new ProbeVM(probeM.Name);
                foreach (var capability in probeM.Capabilities)
                {
                    probeVM.Capabilities.Add(capability);
                }
                Probes.Add(probeVM);
            }
            OnPropertyChanged(nameof(Probes));
        }

        public ProbeCompatibility VMToModel()
        {
            var model = new ProbeCompatibility();
            model.Probes = new List<Service.Interface.Compatibility.Probe>();
            foreach (var probeVM in Probes)
            {
                var probeM = new Service.Interface.Compatibility.Probe();
                probeM.Name = probeVM.Name;
                probeM.Capabilities = new List<CapabilityBase>();
                foreach (var capability in probeVM.Capabilities)
                {
                    probeM.Capabilities.Add(capability);
                }
                model.Probes.Add(probeM);
            }
            return model;
        }

        public bool IsEnabled => true;

        public bool CanClose()
        {
            return true;
        }

        public void Refresh()
        {
            var probeCompatibility = ClassLocator.Default.GetInstance<CompatibilitySupervisor>().GetProbeCompatibility();
            if (probeCompatibility == null)
                probeCompatibility = new ProbeCompatibility() { Probes = new List<Service.Interface.Compatibility.Probe>() };
            ModelToVM(probeCompatibility);
            if (Probes.Any())
                SelectedProbe = Probes.First();
        }
    }

    public enum NewCapability { CrossLayer, DistanceMeasure, ThicknessMeasure }

}
