using System;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.MeasuresSettings;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures
{
    public class PostProcessingOutputVM : ObservableObject
    {
        public PostProcessingOutputVM()
        {
        }

        public PostProcessingOutputVM(string key, double target, string unit = null, ResultCorrectionType correctionType=ResultCorrectionType.None, double tolerance = double.NaN, string name = null, bool isUsed = false )
        {
            Key = key;
            Target = Math.Round(target, 3);
            if (tolerance is double.NaN)
            {
                Tolerance = Target / 10.0;
            }
            Unit = string.IsNullOrEmpty(unit) ? string.Empty : unit;
            Name = string.IsNullOrEmpty(Name) ? $"{Key}" : name;
            IsUsed = isUsed;
            _correction = new ResultCorrectionAnyUnitSettingsVM() { CorrectionType = correctionType, Unit=_unit};
        }

        private string _unit;

        public string Unit
        {
            get => _unit; set { if (_unit != value) { _unit = value; OnPropertyChanged(); } }
        }

        private string _key;

        public string Key
        {
            get => _key; set { if (_key != value) { _key = value; OnPropertyChanged(); } }
        }

        private string _name;

        public string Name
        {
            get => _name; set { if (_name != value) { _name = value; OnPropertyChanged(); } }
        }

        private double _target;

        public double Target
        {
            get => _target; set { if (_target != value) { _target = value; OnPropertyChanged(); } }
        }

        private double _tolerance;

        public double Tolerance
        {
            get => _tolerance; set { if (_tolerance != value) { _tolerance = value; OnPropertyChanged(); } }
        }

        private ResultCorrectionAnyUnitSettingsVM _correction;

        public ResultCorrectionAnyUnitSettingsVM Correction
        {
            get
            {
                if (_correction is null)
                    _correction = new ResultCorrectionAnyUnitSettingsVM();
                return _correction;
            }
            set { if (_correction != value) { _correction = value; OnPropertyChanged(); } }
        }


        private bool _isUsed;

        public bool IsUsed
        {
            get => _isUsed; set { if (_isUsed != value) { _isUsed = value; OnPropertyChanged(); } }
        }
    }
}
