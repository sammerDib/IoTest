using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.MeasuresSettings
{
    public class ResultCorrectionAnyUnitSettingsVM: ObservableObject
    {
        private ResultCorrectionType _correctionType = ResultCorrectionType.None;

        public ResultCorrectionType CorrectionType
        {
            get => _correctionType; 
            set 
            { 
                if (_correctionType != value) 
                { 
                    _correctionType = value;
                    switch (_correctionType)
                    {
                        case ResultCorrectionType.None:
                            Offset = 0.0;
                            Coef = 1.0;
                            break;
                        case ResultCorrectionType.Offset:
                            Coef = 1.0;
                            break;
                        case ResultCorrectionType.Linear:
                            break;
                        default:
                            break;
                    }
                    OnPropertyChanged(); 
                } 
            }
        }

        private double _offset = 0.0;

        public double Offset
        {
            get => _offset; set { if (_offset != value) { _offset = value; OnPropertyChanged(); } }
        }

        private string _unit = string.Empty;

        public string Unit
        {
            get => _unit; set { if (_unit != value) { _unit = value; OnPropertyChanged(); } }
        }


        private double _coef = 1.0;

        public double Coef
        {
            get => _coef; set { if (_coef != value) { _coef = value; OnPropertyChanged(); } }
        }
    }
}
