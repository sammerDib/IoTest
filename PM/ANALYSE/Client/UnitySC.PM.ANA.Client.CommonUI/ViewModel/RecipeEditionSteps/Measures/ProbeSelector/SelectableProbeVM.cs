using System;
using System.Collections.Generic;
using System.Windows.Documents;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.ProbeSelector
{
    public class SelectableProbeVM : ObservableObject, IDisposable
    {
        private bool _isEditing;

        public bool IsEditing
        {
            get => _isEditing; set { if (_isEditing != value) { _isEditing = value; OnPropertyChanged(); } }
        }

        private string _name;

        public string Name
        {
            get => _name; set { if (_name != value) { _name = value; OnPropertyChanged(); } }
        }

        private ProbeMaterialBase _probeMaterial = null;

        public ProbeMaterialBase ProbeMaterial
        {
            get => _probeMaterial; set { if (_probeMaterial != value) { _probeMaterial = value; UpdateMaterial(); OnPropertyChanged(); } }
        }

        private ObjectiveConfig _selectedObjective;

        public ObjectiveConfig SelectedObjective
        {
            get => _selectedObjective;
            set
            {
                if (_selectedObjective != value)
                {
                    _selectedObjective = value;
                    if (IsEditing)
                    {
                        ServiceLocator.CamerasSupervisor.Objective = _selectedObjective;
                    }

                    OnPropertyChanged();
                }
            }
        }

        protected virtual void UpdateMaterial()
        {
        }

        public virtual void SetProbeSettings(ProbeSettings probeSettings)
        {

        }

        public virtual ProbeSettings GetProbeSettings()
        {
            return null;
        }

        public virtual void SetAsCurrentProbe()
        {
        }

        public virtual void UnsetAsCurrentProbe()
        {
        }

        public virtual List<ObjectiveConfig> GetTopObjectives()
        {
            return null;
        }

        private bool _isCalibrationRequiredForSignal = false;

        public bool IsCalibrationRequiredForSignal
        {
            get => _isCalibrationRequiredForSignal; set { if (_isCalibrationRequiredForSignal != value) { _isCalibrationRequiredForSignal = value; OnPropertyChanged(); } }
        }

        private bool _isCalibrationInProgress = false;

        public bool IsCalibrationInProgress
        {
            get => _isCalibrationInProgress; set { if (_isCalibrationInProgress != value) { _isCalibrationInProgress = value; OnPropertyChanged(); } }
        }

        public virtual void StartCalibration()
        {
            return;
        }

        public virtual void CancelCalibration()
        {
            return;
        }

        public virtual void Dispose()
        {
        }
    }
}
