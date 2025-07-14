using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.MeasuresSettings
{
    public class ResultCorrectionSettingsVM: ObservableObject
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
                            Offset = 0.Micrometers();
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

        private Length _offset = 0.Micrometers();

        public Length Offset
        {
            get => _offset; set { if (_offset != value) { _offset = value; OnPropertyChanged(); } }
        }

        private double _coef = 1.0;

        public double Coef
        {
            get => _coef; set { if (_coef != value) { _coef = value; OnPropertyChanged(); } }
        }

        public ResultCorrectionSettings GetResultCorrectionSetting()
        {
            return new ResultCorrectionSettings() { Offset = Offset, Coef = Coef }; 
        }
    }
}
