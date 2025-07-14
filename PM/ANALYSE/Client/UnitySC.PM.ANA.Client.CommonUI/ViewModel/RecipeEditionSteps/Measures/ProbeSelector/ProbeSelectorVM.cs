using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.ProbeSelector
{
    public class ProbeSelectorVM : ObservableObject
    {
        private bool _isEditing = false;

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                if (_isEditing != value)
                {
                    _isEditing = value;

                    if (_isEditing)
                    {
                        UpdateCurrentProbeInSupervisor();
                        if (!(_selectedProbe is null))
                            _selectedProbe.IsEditing = true;
                    }
                    else
                    {
                        _selectedProbe.UnsetAsCurrentProbe(); // Used to update gain
                        _selectedProbe.IsEditing = false;
                    }

                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<SelectableProbeVM> _probes;

        public ObservableCollection<SelectableProbeVM> Probes
        {
            get
            {
                if (_probes is null)
                    _probes = new ObservableCollection<SelectableProbeVM>();
                return _probes;
            }
            set
            {
                if (_probes == value)
                {
                    return;
                }
                _probes = value;
                OnPropertyChanged();
            }
        }

        private SelectableProbeVM _selectedProbe;

        public SelectableProbeVM SelectedProbe
        {
            get => _selectedProbe;
            set
            {
                if (_selectedProbe != value)
                {
                    try
                    {
                        if (_selectedProbe != null)
                            _selectedProbe.UnsetAsCurrentProbe();
                    }
                    catch
                    {
                        // Nothing to do but could happens because in thickness measure,
                        // selected probe does not correspond to physical probe currently used
                    }
                    if (_selectedProbe != null) _selectedProbe.IsEditing = false;
                    _selectedProbe = value;
                    

                    _selectedProbeId = _selectedProbe.ProbeMaterial.ProbeId;
                    if (IsEditing)
                    {
                        _selectedProbe.IsEditing = true;
                        UpdateCurrentProbeInSupervisor();
                    }
                        

                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SelectedProbeId));
                }
            }
        }

        private string _selectedProbeId = null;

        public string SelectedProbeId
        {
            get => _selectedProbeId;
            set
            {
                if (_selectedProbeId != value)
                {
                    _selectedProbeId = value;

                    if ((_selectedProbe != null) && (_selectedProbe.ProbeMaterial.ProbeId != _selectedProbeId))
                    {
                        _selectedProbe = Probes.FirstOrDefault(p => p.ProbeMaterial.ProbeId == _selectedProbeId);
                        OnPropertyChanged(nameof(SelectedProbe));
                    }
                    OnPropertyChanged();
                }
            }
        }

        private void UpdateCurrentProbeInSupervisor()
        {
            if (_selectedProbe is null)
                return;

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                ServiceLocator.ProbesSupervisor.SetCurrentProbe(_selectedProbe.ProbeMaterial.ProbeId);
                SelectedProbe.SetAsCurrentProbe();
            }));
        }



        internal void AddProbe(ProbeMaterialBase compatibleProbe)
        {
            if (!Probes.Any(p => p.ProbeMaterial.ProbeId == compatibleProbe.ProbeId))
            {
                var probeConfig = GetProbeConfig(compatibleProbe);

                SelectableProbeVM newSelectableProbeVM = null;

                switch (probeConfig)
                {
                    case ProbeLiseConfig _:
                        newSelectableProbeVM = new SelectableLiseVM() { Name = probeConfig.Name, ProbeMaterial = compatibleProbe };
                        break;

                    case ProbeDualLiseConfig _:
                        newSelectableProbeVM = new SelectableDualLiseVM() { Name = probeConfig.Name, ProbeMaterial = compatibleProbe };
                        break;

                    case ProbeLiseHFConfig _:
                        newSelectableProbeVM = new SelectableLiseHFVM() { Name = probeConfig.Name, ProbeMaterial = compatibleProbe };
                        break;



                    default:
                        throw new Exception("Not Supported probe material type");
                }


                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    Probes.Add(newSelectableProbeVM);
                }));


                if (SelectedProbe is null)
                    SelectedProbe = Probes.FirstOrDefault();
            }
        }



        private ProbeConfigBase GetProbeConfig(ProbeMaterialBase probe)
        {
            var probeConfig = ServiceLocator.ProbesSupervisor.GetProbesConfig()?.Result.FirstOrDefault(p => p.DeviceID == probe.ProbeId);
            return (ProbeConfigBase)probeConfig;
        }

        internal void SetProbeSettings(ProbeSettings probeSettings)
        {
            if (SelectedProbe is null)
                return;
            SelectedProbe.SetProbeSettings(probeSettings);
        }

        internal ProbeSettings GetSelectedProbeSettings()
        {
            if (SelectedProbe is null)
                return null;
            return SelectedProbe.GetProbeSettings();
        }

        #region RelayCommands

        private AutoRelayCommand _startProbeCalibration;

        public AutoRelayCommand StartProbeCalibration
        {
            get
            {
                return _startProbeCalibration ?? (_startProbeCalibration = new AutoRelayCommand(
                    () =>
                    {
                        SelectedProbe.StartCalibration();
                    },
                    () => { return true; }
                ));
            }
        }


        private AutoRelayCommand _cancelProbeCalibration;

        public AutoRelayCommand CancelProbeCalibration
        {
            get
            {
                return _cancelProbeCalibration ?? (_cancelProbeCalibration = new AutoRelayCommand(
                    () =>
                    {
                        SelectedProbe.CancelCalibration();
                    },
                    () => { return true; }
                ));
            }
        }

        #endregion


    }
}
