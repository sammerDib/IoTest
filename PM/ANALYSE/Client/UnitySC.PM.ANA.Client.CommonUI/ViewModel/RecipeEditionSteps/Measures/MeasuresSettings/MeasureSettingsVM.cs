using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Format.Metro;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.MeasuresSettings
{
    public abstract class MeasureSettingsVM : ObservableObject, IDisposable
    {
        public abstract void Dispose();
        public abstract MeasureSettingsBase GetSettingsWithoutPoints();
 
        public abstract Task LoadSettingsAsync(MeasureSettingsBase measure);
        public abstract Task PrepareToDisplayAsync();
        public abstract void Hide();
        public abstract void DisplaySettingsTab();
        public abstract void HideSettingsTab();
        public abstract void DisplayTestResult(MeasurePointResult result, string resultFolderPath, DieIndex dieIndex);
        public abstract bool AreSettingsValid(ObservableCollection<MeasurePointVM> measurePoints, bool forTestOnPoint=false);

        internal virtual List<MeasurePointVM> GetRealMeasurePoints(ObservableCollection<MeasurePointVM> points)
        {
            return points.ToList();
        }

        internal virtual void EnsureMeasurePointsZPositions(ObservableCollection<MeasurePointVM> measurePoints)
        {
            // By default do nothing
        }

        public string ValidationErrorMessage { get; internal set; }
        private bool _isModified = false;

        // GraphBandBegin and GranphBandEnd are used to display a band on the Graph that usualy correspond to the measure target with the tolerance
        private double _graphBandBegin = 0;

        public double GraphBandBegin
        {
            get => _graphBandBegin; set { if (_graphBandBegin != value) { _graphBandBegin = value; OnPropertyChanged(); } }
        }

        private double _graphBandEnd = 0;

        public double GraphBandEnd
        {
            get => _graphBandEnd; set { if (_graphBandEnd != value) { _graphBandEnd = value; OnPropertyChanged(); } }
        }

        public bool IsModified
        {
            get => _isModified; set { if (_isModified != value) { _isModified = value; OnPropertyChanged(); } }
        }

        private bool _isLoading = false;

        public bool IsLoading
        {
            get => _isLoading; set { if (_isLoading != value) { _isLoading = value; OnPropertyChanged(); } }
        }

        public virtual bool ArePositionsOnDie { get; } = false;

        public virtual bool IsMeasureWithSubMeasurePoints { get; } = false;

        #region PatternRec objectives

        public virtual ObjectiveConfig GetObjectiveUsedByMeasure()
        {
            return null;
        }

        public virtual void SetObjectiveUsedByMeasure()
        {
            // Enforce objective selection to refresh view
            // Because the view may have been updated in measure points tab by changing pattern rec objective
            var objectiveUsedByMeasure = GetObjectiveUsedByMeasure();
            if (objectiveUsedByMeasure != null)
            {
                ServiceLocator.CamerasSupervisor.Objective = objectiveUsedByMeasure;
            }
        }

        #endregion
    }
}
